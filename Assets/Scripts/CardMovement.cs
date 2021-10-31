using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour
{
    Transform lastParent;
    int lastIndex;
    Vector3 lastPosition;
    Vector3 lastPointerPosition;
    Transform currentSnap;
    DeckController deckController;
    GameController gameController;
    SceneController sceneController;
    bool isDragging;


    void Start()
    {
        deckController = GameObject.FindGameObjectWithTag("Draw Deck").GetComponent<DeckController>();
        gameController = GameObject.FindGameObjectWithTag("Main Canvas").GetComponent<GameController>();
        sceneController = GameObject.FindGameObjectWithTag("Main Canvas").GetComponent<SceneController>();
    }

    public void openCard(bool canMove)
    {
        if (canMove)
        {
            setEventTrigger();
        }
        else
        {
            Destroy(GetComponent<EventTrigger>());
        }
        transform.Find("Back Face").gameObject.SetActive(false);
    }

    public void closeCard()
    {
        Destroy(GetComponent<EventTrigger>());
        transform.Find("Back Face").gameObject.SetActive(true);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        currentSnap = other.transform;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        currentSnap = null;
    }


    void setEventTrigger()
    {
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry onClick = new EventTrigger.Entry();
        onClick.eventID = EventTriggerType.PointerClick;
        onClick.callback.AddListener((data) => { onPointerClick((PointerEventData)data); });
        trigger.triggers.Add(onClick);

        EventTrigger.Entry BeginDrag = new EventTrigger.Entry();
        BeginDrag.eventID = EventTriggerType.BeginDrag;
        BeginDrag.callback.AddListener((data) => { onBeginDrag((PointerEventData)data); });
        trigger.triggers.Add(BeginDrag);

        EventTrigger.Entry Dragging = new EventTrigger.Entry();
        Dragging.eventID = EventTriggerType.Drag;
        Dragging.callback.AddListener((data) => { onDragging((PointerEventData)data); });
        trigger.triggers.Add(Dragging);

        EventTrigger.Entry EndDrag = new EventTrigger.Entry();
        EndDrag.eventID = EventTriggerType.EndDrag;
        EndDrag.callback.AddListener((data) => { onEndDrag((PointerEventData)data); });
        trigger.triggers.Add(EndDrag);

        EventTrigger.Entry PointerEnter = new EventTrigger.Entry();
        PointerEnter.eventID = EventTriggerType.PointerEnter;
        PointerEnter.callback.AddListener((data) => { onPointerEnter((PointerEventData)data); });
        trigger.triggers.Add(PointerEnter);

        EventTrigger.Entry PointerExit = new EventTrigger.Entry();
        PointerExit.eventID = EventTriggerType.PointerExit;
        PointerExit.callback.AddListener((data) => { onPointerExit((PointerEventData)data); });
        trigger.triggers.Add(PointerExit);
    }


    void onPointerClick(PointerEventData data)
    {
        if (!isDragging && gameController.isRealPlayer() && !gameController.isChallenging && !gameController.isReporting)
        {
            Card topCard = deckController.stackingDeck.GetChild(deckController.stackingDeck.childCount-1).GetComponent<Card>();
            Card thisCard = GetComponent<Card>();
            if (thisCard.color == Color.black || thisCard.color == topCard.color || thisCard.type == topCard.type)
            {
                StartCoroutine(stackCard(false, true));
            }
        }
    }

    void onBeginDrag(PointerEventData data)
    {
        if (!isDragging && !LeanTween.isTweening(gameObject))
        {
            isDragging = true;
            lastParent = transform.parent;
            lastPosition = transform.position;
            lastPointerPosition = Input.mousePosition;
            lastIndex = transform.GetSiblingIndex();
            transform.SetParent(deckController.draggingPanel);
        }
    }

    void onDragging(PointerEventData data)
    {
        if (isDragging)
        {
            transform.position = lastPosition + Input.mousePosition - lastPointerPosition;
        }
    }

    void onEndDrag(PointerEventData data)
    {
        if (isDragging)
        {
            if (currentSnap == null)
            {
                returnCard();
            }
            else
            {
                if ((gameController.isRealPlayer() || deckController.debugMode)  && !gameController.isChallenging && !gameController.isReporting)
                {
                    tryToStack();
                }
                else
                {
                    returnCard();
                }
            }
            isDragging = false;
        }
    }

    void onPointerEnter(PointerEventData data)
    {
        if (!LeanTween.isTweening(gameObject))
        {
            //transform.LeanMoveLocal(new Vector3(transform.localPosition.x, 50), 0.25f).setEaseOutQuint();
        }
    }

    void onPointerExit(PointerEventData data)
    {
        if (!LeanTween.isTweening(gameObject))
        {
            //transform.LeanMoveLocal(new Vector3(transform.localPosition.x, 0), 0.25f).setEaseOutQuint();
        }
    }

    public void stackCard_AI()
    {
        StartCoroutine(stackCard(true));
    }

    void returnCard()
    {
        transform.SetParent(lastParent);
        transform.SetSiblingIndex(lastIndex);
        transform.LeanMove(lastPosition, 0.5f).setEaseOutQuint();
    }

    void tryToStack()
    {
        Card topCard = deckController.stackingDeck.GetChild(deckController.stackingDeck.childCount-1).GetComponent<Card>();
        Card thisCard = GetComponent<Card>();
        if (thisCard.color == Color.black)
        {
            StartCoroutine(stackCard());
        }
        else
        {    
            if ((topCard.color == thisCard.color || topCard.type == thisCard.type))
            {
                StartCoroutine(stackCard());
            }
            else
            {
                returnCard();
            }
        }
    }

    IEnumerator stackCard(bool isAI = false, bool isClicked = false)
    {
        Card thisCard = GetComponent<Card>();
        if (thisCard.type == "mute" && gameController.punishment == 0)
        {
            snapCard();
            yield return new WaitForSeconds(0.25f);
            gameController.nextPlayer(true);
        }
        else if (thisCard.type == "reverse" && gameController.punishment == 0)
        {
            snapCard();
            yield return new WaitForSeconds(0.25f);
            gameController.turnDirection *= -1;
            gameController.nextPlayer(gameController.playingPlayers.Count == 2);
        }
        else if (thisCard.type == "+2")
        {
            snapCard();
            yield return new WaitForSeconds(0.25f);
            gameController.punishment += 2;
            gameController.nextPlayer();
        }
        else if (thisCard.type == "color picker" && gameController.punishment == 0)
        {
            snapCard();
            yield return new WaitForSeconds(0.25f);
            if (!isAI)
            {
                sceneController.openColorPicker();
            }
            else
            {
                gameController.pickColor(null);
            }
        }
        else if (thisCard.type == "+4")
        {
            snapCard();
            yield return new WaitForSeconds(0.25f);
            gameController.punishment += 4;
            gameController.isChallenging = true;
            if (!isAI)
            {
                sceneController.openColorPicker();
            }
            else
            {
                gameController.pickColor(null);
            }
        }
        else
        {
            if (gameController.punishment == 0)
            {
                snapCard();
                yield return new WaitForSeconds(0.25f);
                gameController.nextPlayer();
            }
            else
            {
                if (!isClicked)
                {
                    returnCard();
                }
            }
        }
    }

    void snapCard()
    {
        this.openCard(false);
        transform.SetParent(deckController.stackingDeck);
        transform.LeanMoveLocal(Vector3.zero, 0.25f).setEaseOutQuint();
        transform.LeanRotateZ(0f, 0.25f).setEaseOutQuint();
        deckController.updatePlayerHand(gameController.playerTurn);
    }
}
