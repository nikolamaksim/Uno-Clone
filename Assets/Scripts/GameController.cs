using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField] DeckController deckController;
    [SerializeField] SceneController sceneController;
    [SerializeField] CardSpawner cardSpawner;
    [SerializeField] RectTransform directionPreview;
    [SerializeField] TextMeshProUGUI punismentCounter;
    public List<RectTransform> playingPlayers;
    [Range(2, 4)] public int playerCount;
    public int playerTurn;
    public int punishment;
    public int turnDirection = 1;
    public bool isChallenging = false;
    public bool isReporting = false;
    Color lastChallengeColor = Color.clear;
    int lastPlayedPlayer;
    public float startTime;
    public float reportStartTime;

    void Start()
    {
        joinPlayers();
        highlightPlayer();
    }

    void joinPlayers()
    {
        if (playerCount == 2)
        {
            playingPlayers.Add(deckController.players[0]);
            playingPlayers.Add(deckController.players[2]);
        }
        else
        {
            for (int p = 0; p < playerCount; p++)
            {
                playingPlayers.Add(deckController.players[p]);
            }
        }
    }

    void highlightPlayer()
    {
        for (int p = 0; p < playingPlayers.Count; p++)
        {
            if (p == playerTurn)
            {
                playingPlayers[p].GetComponent<Image>().color = new Color(1, 0, 0, 0.4f);
            }
            else
            {
                playingPlayers[p].GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
            }
        }
    }

    void updateTurnDirection()
    {
        if (turnDirection == 1)
        {
            directionPreview.LeanRotateY(0f, 0.5f).setEaseOutQuint();
        }
        else
        {
            directionPreview.LeanRotateY(180f, 0.5f).setEaseOutQuint();
        }
    }

    public void nextPlayer(bool muted = false)
    {
        punismentCounter.text = "Punisment: " + punishment.ToString();
        updateTurnDirection();
        bool kicked = playingPlayers[playerTurn].childCount == 0;
        int lastPlayer = playerTurn;

        if (!isRealPlayer())
        {
            playingPlayers[lastPlayer].GetComponent<UnoController>().toggleReport(playingPlayers[lastPlayer].childCount == 1, playingPlayers[lastPlayer].GetComponent<CrudeAI>().isSaidUno);
        }
        else
        {
            playingPlayers[lastPlayer].GetComponent<UnoController>().toggleReport(playingPlayers[lastPlayer].childCount == 1);
        }

        playerTurn += muted ? turnDirection * 2 : turnDirection;
        if (playerTurn >= playingPlayers.Count)
        {
            playerTurn %= playingPlayers.Count;
        }
        else if (playerTurn < 0)
        {
            playerTurn += playingPlayers.Count;
        }

        if (kicked)
        {
            RectTransform currentPlayer = playingPlayers[playerTurn];
            playingPlayers.RemoveAt(lastPlayer);
            for (int p = 0; p < playingPlayers.Count; p++)
            {
                if (playingPlayers[p] == currentPlayer)
                {
                    playerTurn = p;
                    break;
                }
            }
        }

        playingPlayers[playerTurn].GetComponent<UnoController>().toggleReport(false);

        if (!isRealPlayer())
        {
            startTime = Time.time;
            playingPlayers[playerTurn].GetComponent<CrudeAI>().isDecided = false;
        }

        highlightPlayer();
    }

    public void pickColor(Button button)
    {
        if (button == null)
        {
            if (isChallenging)
            {    
                int randomIndex = Random.Range(0, 4);
                lastChallengeColor = cardSpawner.colors[randomIndex];
            }
            else
            {
                List<Color> myColors = new List<Color>();
                for (int c = 0; c < playingPlayers[playerTurn].childCount; c++)
                {
                    Color thisColor = playingPlayers[playerTurn].GetChild(c).GetComponent<Card>().color;
                    if (!myColors.Contains(thisColor) && thisColor != Color.black)
                    {
                        myColors.Add(thisColor);
                    }
                }
                int randomIndex = Random.Range(0, myColors.Count);
                lastChallengeColor = myColors[randomIndex];
            }
        }
        else
        {
            lastChallengeColor = button.colors.normalColor;
        }
        lastPlayedPlayer = playerTurn;
        deckController.stackingDeck.GetChild(deckController.stackingDeck.childCount-1).Find("Color").GetComponent<Image>().color = lastChallengeColor;
        deckController.stackingDeck.GetChild(deckController.stackingDeck.childCount-1).GetComponent<Card>().color = lastChallengeColor;
        nextPlayer();
        sceneController.closeColorPicker();
        if (isChallenging && isRealPlayer())
        {
            sceneController.openChallenge();
        }
    }

    public void challenge(bool accept)
    {
        if (!accept)
        {
            deckController.dealCard();
        }
        else
        {    
            bool isLie = false;
            for (int c = 0; c < playingPlayers[lastPlayedPlayer].childCount; c++)
            {
                if (playingPlayers[lastPlayedPlayer].GetChild(c).GetComponent<Card>().color == lastChallengeColor)
                {
                    playerTurn = lastPlayedPlayer;
                    deckController.dealCard();
                    isLie = true;
                    break;
                }
            }
            if (!isLie)
            {
                punishment += 2;
                deckController.dealCard();
            }
        }
        isChallenging = false;
        sceneController.closeChallenge();
    }

    public bool isRealPlayer()
    {
        return playingPlayers[playerTurn].GetComponent<CrudeAI>() == null;
    }

}
