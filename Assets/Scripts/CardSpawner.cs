using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSpawner : MonoBehaviour
{
    public List<Color> colors;
    public List<string> types;
    public List<string> specials;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Sprite[] typeSprites;
    [SerializeField] Sprite[] specialSprites;

    
    void Awake()
    {
        spawnCards();
    }

    void spawnCards()
    {
        /*
        spawn normal card
        */
        cardPrefab.transform.Find("Type Top").gameObject.SetActive(true);
        cardPrefab.transform.Find("Type Bottom").gameObject.SetActive(true);
        cardPrefab.transform.Find("Special Face").gameObject.SetActive(false);
        foreach (var color in colors)
        {
            for (int t = 0; t < types.Count; t++)
            {
                int count = types[t] == "0" ? 1 : 2;
                cardPrefab.transform.Find("Color").GetComponent<Image>().color = color;
                cardPrefab.transform.Find("Type").GetComponent<Image>().color = color;
                cardPrefab.transform.Find("Type").GetComponent<Image>().sprite = typeSprites[t];
                if (types[t] == "+2")
                {
                    cardPrefab.transform.Find("Type Top").GetComponent<Image>().sprite = typeSprites[t+1];
                    cardPrefab.transform.Find("Type Bottom").GetComponent<Image>().sprite = typeSprites[t+1];
                }
                else
                {
                    cardPrefab.transform.Find("Type Top").GetComponent<Image>().sprite = typeSprites[t];
                    cardPrefab.transform.Find("Type Bottom").GetComponent<Image>().sprite = typeSprites[t];
                }
                for (int i = 0; i < count; i++)
                {
                    GameObject newCard = Instantiate(cardPrefab, transform.position, Quaternion.identity, transform);
                    newCard.GetComponent<Card>().color = color;
                    newCard.GetComponent<Card>().type = types[t];
                }
            }
        }
        /*
        spawn special cards
        */
        cardPrefab.transform.Find("Type Top").gameObject.SetActive(false);
        cardPrefab.transform.Find("Type Bottom").gameObject.SetActive(false);
        cardPrefab.transform.Find("Special Face").gameObject.SetActive(true);
        cardPrefab.transform.Find("Color").GetComponent<Image>().color = Color.black;
        for (int i = 0; i < 8; i++)
        {
            cardPrefab.transform.Find("Special Face").GetComponent<Image>().sprite = i < 4 ? specialSprites[0] : specialSprites[1];
            GameObject newCard = Instantiate(cardPrefab, transform.position, Quaternion.identity, transform);
            newCard.GetComponent<Card>().color = Color.black;
            newCard.GetComponent<Card>().type = i < 4 ? specials[0] : specials[1];
        }
    }
}
