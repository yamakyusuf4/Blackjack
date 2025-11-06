using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class CManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform playerHand;
    public Transform dealerHand;
    public Transform deckPosition;

    [Header("Deck Data")]
    public List<Card> deck = new List<Card>();

    private List<Card> runtimeDeck = new List<Card>();
    void Start()
    {
        runtimeDeck = new List<Card>(deck);
        ShuffleDeck();

        StartCoroutine(DealStartingCards());
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, runtimeDeck.Count);
            Card temp = runtimeDeck[0];
            runtimeDeck[i] = runtimeDeck[rand];
            runtimeDeck[rand] = temp;
        }
        Debug.Log("Deck shuffled!");
    }

    public Card DrawCard()
    {
        if (runtimeDeck.Count == 0)
        {
            Debug.LogWarning("Deck is empty!");
            return null;
        }

        Card drawn = runtimeDeck[0];
        runtimeDeck.RemoveAt(0);
        return drawn;
    }

    IEnumerator DealStartingCards()
    {
        for (int i = 0; i < 2; i++)
        {
            yield return StartCoroutine(AnimateDraw(playerHand));
            yield return new WaitForSeconds(0.4f);
            yield return StartCoroutine(AnimateDraw(dealerHand));
            yield return new WaitForSeconds(0.4f);
        }
    }
    IEnumerator AnimateDraw(Transform hand)
    {
        Card drawn = DrawCard();
        if (drawn == null) yield break;

        GameObject cardGO = Instantiate(cardPrefab, deckPosition.position, Quaternion.identity, deckPosition.parent);
        Image cardImage = cardGO.GetComponent<Image>();
        cardImage.sprite = drawn.sprite;

        Vector3 endPos = hand.position + new Vector3(hand.childCount * 140f, 0f, 0f);

        float duration = 0.4f;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            cardGO.transform.position = Vector3.Lerp(deckPosition.position, endPos, t);
            yield return null;
        }

        cardGO.transform.SetParent(hand);
    }
}
