using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackjackManager : MonoBehaviour
{
    [Header("References")]
    public DeckManager deckManager;
    public Transform playerHand;
    public Transform dealerHand;
    public Text playerTotalText;
    public Text dealerTotalText;
    public Text resultText;
    public GameObject playAgainButton;
    public GameObject quitButton;

    [Header("Gameplay State")]
    private List<Card> playerCards = new List<Card>();
    private List<Card> dealerCards = new List<Card>();
    private bool playerTurn = true;
    private bool roundOver = false;

    void Start()
    {
        StartCoroutine(StartRound());
    }

    int CalculateFromHand(Transform hand, bool hideFacedown = false)
    {
        int total = 0;
        int aceCount = 0;

        for (int i = 0; i < hand.childCount; i++)
        {
            var vis = hand.GetChild(i).GetComponent<CardVisual>();
            if (vis == null) continue;

            if (hideFacedown && vis.isFaceDown) continue;

            Card card = vis.GetCardData();
            string name = card.cardName.ToLower();

            if (name.Contains("ace"))
            {
                aceCount++;
                total += 1;
            }
            else if (name.Contains("king") || name.Contains("queen") || name.Contains("jack"))
            {
                total += 10;
            }
            else
            {
                total += card.value;
            }
        }

        while (aceCount > 0 && total + 10 <= 21)
        {
            total += 10;
            aceCount--;
        }

        return total;
    }

    IEnumerator StartRound()
    {
        yield return new WaitForSeconds(1f);
        yield return deckManager.StartCoroutine("DealStartingCards");

        CollectCardsFromHands();

        CheckBlackjack();
    }

    void CollectCardsFromHands()
    {
        playerCards.Clear();
        dealerCards.Clear();

        foreach (Transform card in playerHand)
        {
            CardVisual visual = card.GetComponent<CardVisual>();
            if (visual != null)
                playerCards.Add(visual.GetCardData());
        }

        foreach (Transform card in dealerHand)
        {
            CardVisual visual = card.GetComponent<CardVisual>();
            if (visual != null)
                dealerCards.Add(visual.GetCardData());
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        bool hideDealer = playerTurn && !roundOver;

        playerTotalText.text = "Player: " + CalculateFromHand(playerHand);
        dealerTotalText.text = "Dealer: " + CalculateFromHand(dealerHand, hideDealer);
    }

    int CalculateHandValue(List<Card> hand, bool hideHoleCard = false)
    {
        int total = 0;
        int aceCount = 0;


        for (int i = 0; i < hand.Count; i++)
        {
            if (hideHoleCard && i == hand.Count - 1)
                break;

            string name = hand[i].cardName.ToLower();

            if (name.Contains("ace"))
            {
                aceCount++;
                total += 1;
            }
            else if (name.Contains("king") || name.Contains("queen") || name.Contains("jack"))
            {
                total += 10;
            }
            else
            {
                total += hand[i].value;
            }
        }

        for (int i = 0; i < aceCount; i++)
        {
            if (total + 10 <= 21)
                total += 10;
        }

        return total;
    }

    void CheckBlackjack()
    {
        int playerTotal = CalculateHandValue(playerCards);
        int dealerTotal = CalculateHandValue(dealerCards);

        if (playerTotal == 21 && dealerTotal == 21)
        {
            EndRound("Push! Both have Blackjack!");
        }
        else if (playerTotal == 21)
        {
            EndRound("Player has Blackjack!");
        }
        else if (dealerTotal == 21)
        {
            EndRound("Dealer has Blackjack!");
        }
    }

    public void PlayerHit()
    {
        if (!playerTurn || roundOver) return;

        Card newCard = deckManager.DrawCard();
        deckManager.SpawnCard(newCard, playerHand);
        playerCards.Add(newCard);

        RefreshHands();
        int total = CalculateHandValue(playerCards);
        UpdateUI();

        if (total > 21)
        {
            EndRound("Player Busts!");
        }
    }

    public void PlayerStand()
    {
        if (!playerTurn || roundOver) return;
        playerTurn = false;
        StartCoroutine(DealerTurn());
    }

    IEnumerator DealerTurn()
    {
        deckManager.RevealDealerCard();

        yield return new WaitForSeconds(0.5f);

        RefreshHands() ;
        UpdateUI();
        
        int dealerTotal = CalculateHandValue(dealerCards);
        while (dealerTotal < 17)
        {
            Card newCard = deckManager.DrawCard();
            deckManager.SpawnCard(newCard, dealerHand, false);
            yield return new WaitForSeconds(0.6f);
            RefreshHands();
            dealerTotal = CalculateHandValue(dealerCards);
            UpdateUI();
        }

        int playerTotal = CalculateHandValue(playerCards);
        dealerTotal = CalculateHandValue(dealerCards);

        if (dealerTotal > 21) EndRound("Dealer Busts! Player Wins!");
        else if (dealerTotal > playerTotal) EndRound("Dealer Wins!");
        else if (dealerTotal < playerTotal) EndRound("Player Wins!");
        else EndRound("Push!");
    }

    void EndRound(string message)
    {
        roundOver = true;
        resultText.text = message;

        if (playAgainButton != null)
            playAgainButton.SetActive(true);

        if (quitButton != null)
            quitButton.SetActive(true);

        Debug.Log(message);
    }

    void RefreshHands()
    {
        playerCards.Clear();
        dealerCards.Clear();

        foreach (Transform card in playerHand)
        {
            CardVisual visual = card.GetComponent<CardVisual>();
            if (visual != null)
                playerCards.Add(visual.GetCardData());
        }

        foreach (Transform card in dealerHand)
        {
            CardVisual visual = card.GetComponent<CardVisual>();
            if (visual != null)
                dealerCards.Add(visual.GetCardData());
        }
    }
}
