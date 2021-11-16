using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrudeAI : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [SerializeField] DeckController deckController;
    [SerializeField, Range(0, 100)] int chanceOfAcceptChallenge;
    [SerializeField, Range(0, 100)] int chanceOfPutCard;
    [SerializeField, Range(0, 100)] int chanceOfSayUno;
    [SerializeField, Range(0, 100)] int chanceOfReport;
    BehaviourTree behaviourTree;
    public bool isDecided;
    public bool isSaidUno;
    public bool isReportDecided;

    void Start()
    {
        behaviourTree = new BehaviourTree("Bot Tree");

        Selector selectMove = new Selector("Select Move");
            Sequence challenge = new Sequence("(+4) Challenge");
                Leaf isChallenging = new Leaf("Is Challenging?", checkChallenge);
                Selector challengeAnswer = new Selector("Challenge Answer");
                    Leaf acceptChallenge = new Leaf("Accept Challenge", acceptChal);
                    Leaf declineChallenge = new Leaf("Decline Challenge", declineChal);
            Leaf putPunisher = new Leaf("Put Punisher", tryToPutPunisher);
            Leaf putCard = new Leaf("Put Card", tryToPutCard);
            Leaf dealCard = new Leaf("Deal Card", tryToDealCard);


                    challengeAnswer.addChild(acceptChallenge);
                    challengeAnswer.addChild(declineChallenge);
                challenge.addChild(isChallenging);
                challenge.addChild(challengeAnswer);
            selectMove.addChild(challenge);

            selectMove.addChild(putPunisher);

            selectMove.addChild(putCard);

            selectMove.addChild(dealCard);
        
        behaviourTree.addChild(selectMove);

        behaviourTree.PrintTree();
    }

    public Node.Status checkChallenge()
    {
        return gameController.isChallenging ? Node.Status.SUCCESS : Node.Status.FAILURE;
    }

    public Node.Status acceptChal()
    {
        int randomInt = Random.Range(1, 101);
        if (randomInt <= chanceOfAcceptChallenge)
        {
            tryToSayUno();
            isDecided = true;
            //Debug.Log(name + ": Challenge Accepted!");
            gameController.challenge(true);
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }

    public Node.Status declineChal()
    {
        tryToSayUno();
        isDecided = true;
        gameController.challenge(false);
        //Debug.Log(name + ": Challenge Declined!");
        return Node.Status.SUCCESS;
    }


    public Node.Status tryToPutPunisher()
    {
        if (gameController.punishment != 0)
        {
            int randomInt = Random.Range(1, 101);
            if (randomInt <= chanceOfPutCard)
            {
                List<Transform> validCards = new List<Transform>();
                Card currentCard;
                for (int c = 0; c < transform.childCount; c++)
                {
                    currentCard = transform.GetChild(c).GetComponent<Card>();
                    if (currentCard.type == "+2" || currentCard.type == "+4")
                    {
                        validCards.Add(currentCard.transform);
                    }
                }
                if (validCards.Count != 0)
                {
                    int randomIndex = Random.Range(0, validCards.Count);
                    validCards[randomIndex].GetComponent<CardMovement>().stackCard_AI();
                    //Debug.Log(name + ": Put Punisher");
                    tryToSayUno();
                    isDecided = true;
                    return Node.Status.SUCCESS;
                }
            }
        }
        return Node.Status.FAILURE;
    }


    public Node.Status tryToPutCard()
    {
        if (gameController.punishment == 0)
        {    
            int randomInt = Random.Range(1, 101);
            if (randomInt <= chanceOfPutCard)
            {
                List<Transform> validCards = new List<Transform>();
                Card currentCard;
                Card topCard = deckController.stackingDeck.GetChild(deckController.stackingDeck.childCount-1).GetComponent<Card>();
                for (int c = 0; c < transform.childCount; c++)
                {
                    currentCard = transform.GetChild(c).GetComponent<Card>();
                    if (currentCard.color == Color.black || currentCard.color == topCard.color || currentCard.type == topCard.type)
                    {
                        validCards.Add(currentCard.transform);
                    }
                }
                if (validCards.Count != 0)
                {
                    int randomIndex = Random.Range(0, validCards.Count);
                    validCards[randomIndex].GetComponent<CardMovement>().stackCard_AI();
                    //Debug.Log(name + ": Put any Card");
                    tryToSayUno();
                    isDecided = true;
                    return Node.Status.SUCCESS;
                }
            }
        }
        //Debug.Log(name + ": failed to put any card");
        return Node.Status.FAILURE;
    }


    public Node.Status tryToDealCard()
    {
        tryToSayUno();
        isDecided = true;
        deckController.dealCard();
        //Debug.Log(name + ": Cards Dealed!");
        return Node.Status.SUCCESS;
    }

    bool isMyTurn()
    {
        return gameController.playingPlayers[gameController.playerTurn].name == name;
    }

    void tryToSayUno()
    {
        isSaidUno = Random.Range(1, 101) <= chanceOfSayUno;
    }


    void Update()
    {
        if (isMyTurn() && !isDecided && !gameController.isReporting)
        {
            float thinkDuration = Time.time - gameController.startTime;
            if (thinkDuration >= 1f)
            {
                behaviourTree.Process();
            }
        }

        if (!isReportDecided && Time.time - gameController.reportStartTime >= 3f)
        {
            int randomInt = Random.Range(1, 101);
            if (randomInt <= chanceOfReport)
            {
                for (int p = 0; p < gameController.playingPlayers.Count; p++)
                {
                    if (gameController.playingPlayers[p].name != name)
                    {
                        gameController.playingPlayers[p].GetComponent<UnoController>().report();
                    }
                }
            }
            isReportDecided = true;
        }
    }
}
