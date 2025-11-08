using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    [Header("Game References")]
    public GameObject cardPrefab;
    public Transform playerHand;
    public Transform dealerHand;
    public Transform deckPosition;
    public Sprite cardBackSprite;

    [Header("Deck Data")]
    public List<Card> deck = new List<Card>();
    private List<Card> runtimeDeck = new List<Card>();

    void Start()
    {
        runtimeDeck = new List<Card>(deck);
        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < runtimeDeck.Count; i++)
        {
            int rand = Random.Range(i, runtimeDeck.Count);
            Card temp = runtimeDeck[i];
            runtimeDeck[i] = runtimeDeck[rand];
            runtimeDeck[rand] = temp;
        }
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

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(RevealAllCards());
    }

    IEnumerator AnimateDraw(Transform hand)
    {
        Card drawn = DrawCard();
        if (drawn == null) yield break;

        GameObject cardGO = Instantiate(cardPrefab, deckPosition.parent);
        RectTransform rect = cardGO.GetComponent<RectTransform>();
        Image img = cardGO.GetComponent<Image>();
        rect.localScale = Vector3.one;

        rect.position = deckPosition.position;

        CardVisual visual = cardGO.AddComponent<CardVisual>();
        visual.Setup(drawn, img, cardBackSprite);

        Vector3 endPos = hand.position + new Vector3(hand.childCount * 140f, 0f, 0f);

        float t = 0;
        float duration = 0.4f;

        while (t < 1)
        {
            t += Time.deltaTime / duration;
            rect.position = Vector3.Lerp(deckPosition.position, endPos, t);
            yield return null;
        }

        rect.SetParent(hand, worldPositionStays: true);
    }

    IEnumerator RevealAllCards()
    {
        foreach (Transform card in playerHand)
        {
            CardVisual visual = card.GetComponent<CardVisual>();
            if (visual != null)
            {
                visual.Flip();
                yield return new WaitForSeconds(0.3f);
            }
        }

        for (int i = 0; i < dealerHand.childCount; i++)
        {
            CardVisual visual = dealerHand.GetChild(i).GetComponent<CardVisual>();
            if (visual != null && i != dealerHand.childCount - 1)
            {
                visual.Flip();
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
    public void RevealDealerCard()
    {
        if (dealerHand.childCount > 0)
        {
            Transform lastCard = dealerHand.GetChild(dealerHand.childCount - 1);
            CardVisual visual = lastCard.GetComponent<CardVisual>();
            if (visual != null)
                visual.Flip();
        }
    }

    public GameObject SpawnCard(Card drawn, Transform hand, bool faceDown = false)
    {
        GameObject cardGO = Instantiate(cardPrefab, deckPosition.position, Quaternion.identity, deckPosition.parent);
        Image cardImage = cardGO.GetComponent<Image>();

        CardVisual visual = cardGO.AddComponent<CardVisual>();
        visual.Setup(drawn, cardImage, cardBackSprite);

        if (!faceDown)
        {
            visual.Flip();
        }

        cardGO.transform.SetParent(hand, false);

        RectTransform rect = cardGO.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(hand.childCount * 140f, 0f);

        return cardGO;
    }
}
