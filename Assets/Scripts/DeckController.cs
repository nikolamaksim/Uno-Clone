using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckController : MonoBehaviour
{
    public RectTransform[] players;
    public RectTransform stackingDeck;
    public RectTransform draggingPanel;
    [SerializeField] GameController gameController;
    [SerializeField] SceneController sceneController;
    int beginningCardCount = 7;
    bool isDealingCard = false;
    public bool debugMode;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //shuffleDeck();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            //StartCoroutine(redealCards());
        }
    }

    public void shuffleDeck()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            int randomIndex = Random.Range(0, transform.childCount);
            Transform tempCard = transform.GetChild(i);
            transform.GetChild(randomIndex).SetSiblingIndex(tempCard.GetSiblingIndex());
            tempCard.SetSiblingIndex(randomIndex);
        }
    }

    public IEnumerator redealCards()
    {
        for (int i = 0; i < beginningCardCount; i++)
        {
            for (int p = 0; p < gameController.playerCount; p++)
            {
                Transform topCard = transform.GetChild(transform.childCount-1);
                topCard.SetParent(gameController.playingPlayers[p]);
                if (p == 0 || debugMode)
                {
                    topCard.GetComponent<CardMovement>().openCard(true);
                }
                topCard.localRotation = Quaternion.Euler(0, 0, 0);
                float offset = -(beginningCardCount - 1) / 2f;
                float gap = players[p].rect.width / (float)beginningCardCount;
                topCard.LeanMoveLocal(Vector3.right * gap * (offset + i), 1f).setEaseOutQuint();
                yield return 0;
            }
        }
        transform.GetChild(transform.childCount-1).SetParent(stackingDeck);
        stackingDeck.GetChild(0).GetComponent<CardMovement>().openCard(false);
        stackingDeck.GetChild(0).LeanMoveLocal(Vector3.zero, 1f).setEaseOutQuint();
        yield return new WaitForSeconds(1f);
        if (stackingDeck.GetChild(0).GetComponent<Card>().type == "color picker" || stackingDeck.GetChild(0).GetComponent<Card>().type == "+4")
        {
            sceneController.openColorPicker();
        }
    }

    public void dealCard(bool clicked = false)
    {
        if (!isDealingCard && (!clicked || clicked && gameController.isRealPlayer() && !gameController.isReporting))
        {
            StartCoroutine(dealCardCoroutine(gameController.punishment == 0 ? 1 : gameController.punishment)); // check if player can stack when only 1 card dealt
        }
    }

    public IEnumerator dealCardCoroutine(int count, int suspended = -1)
    {
        isDealingCard = true;
        if (transform.childCount < count)
        {
            int temp = stackingDeck.childCount - 1;
            for (int c = 0; c < temp; c++)
            {
                Transform card = stackingDeck.GetChild(0);
                card.GetComponent<CardMovement>().closeCard();
                card.SetParent(transform);
                card.localPosition = Vector3.zero;
                if (card.GetComponent<Card>().type == "+4" || card.GetComponent<Card>().type == "color picker")
                {
                    card.Find("Color").GetComponent<Image>().color = Color.black;
                    card.GetComponent<Card>().color = Color.black;
                }
            }
            shuffleDeck();
        }
        for (int i = 0; i < count; i++)
        {    
            Transform newCard = transform.GetChild(transform.childCount-1);
            if (gameController.isRealPlayer() || debugMode)
            {
                newCard.GetComponent<CardMovement>().openCard(true);
            }
            newCard.SetParent(gameController.playingPlayers[gameController.playerTurn]);
            updatePlayerHand(gameController.playerTurn);
            yield return new WaitForSeconds(0.2f);
        }
        if (suspended == -1)
        {
            gameController.punishment = 0;
            gameController.nextPlayer(); // check if player can stack dealt card
        }
        else
        {
            gameController.playerTurn = suspended;
            gameController.isReporting = false;
        }
        isDealingCard = false;
    }

    public void updatePlayerHand(int player)
    {
        for (int c = 0; c < gameController.playingPlayers[player].childCount; c++)
        {
            gameController.playingPlayers[player].GetChild(c).localRotation = Quaternion.Euler(0, 0, 0);
            float offset = -(gameController.playingPlayers[player].childCount - 1) / 2f;
            float gap = gameController.playingPlayers[player].rect.width / (float)gameController.playingPlayers[player].childCount;
            gameController.playingPlayers[player].GetChild(c).LeanMoveLocal(Vector3.right * gap * (offset + c), 0.25f).setEaseOutQuint();
        }
    }
}
