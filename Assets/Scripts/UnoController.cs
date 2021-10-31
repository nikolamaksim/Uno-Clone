using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnoController : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [SerializeField] DeckController deckController;
    [SerializeField] GameObject reportButton;
    [SerializeField] GameObject unoButton;
    bool canReport;

    public void toggleReport(bool state, bool isSaid = false)
    {
        canReport = state && !isSaid;
        if (reportButton != null)
        {
            reportButton.SetActive(canReport);
        }
        else
        {
            unoButton.SetActive(canReport);
        }

        if (canReport)
        {
            for (int p = 0; p < gameController.playingPlayers.Count; p++)
            {
                CrudeAI ai = gameController.playingPlayers[p].GetComponent<CrudeAI>();
                if (ai != null)
                {
                    gameController.playingPlayers[p].GetComponent<CrudeAI>().isReportDecided = false;
                    gameController.reportStartTime = Time.time;
                }
            }
        }
    }

    public void report()
    {
        if (canReport)
        {
            gameController.isReporting = true; // suspend all players while reporting ??
            toggleReport(false);
            int temp = gameController.playerTurn;
            for (int p = 0; p < gameController.playingPlayers.Count; p++)
            {
                if (gameController.playingPlayers[p].name == name)
                {
                    gameController.playerTurn = p;
                    break;
                }
            }
            StartCoroutine(deckController.dealCardCoroutine(2, temp));
        }
    }

    public void sayUno()
    {
        toggleReport(false);
    }

    
}
