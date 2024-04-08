using System.Collections.Generic;
using System;
using UnoGame;
using System.Linq;

public class Deck
{
    public List<Card> cards;
    public List<Card> playedCards;
    private Random random = new Random();

    public Deck()
    {
        cards = new List<Card>();
        playedCards = new List<Card>();
        InitializeCards();
        ShuffleCards();
        PopulateFirstCrad();
    }


    private void InitializeCards()
    {
        string[] colors = { "red", "green", "blue", "yellow" };
        string[] values = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "draw2", "reverse", "skip" };

        foreach (string color in colors)
        {
            foreach (string value in values)
            {
                string imageName = $"uno_card-{color}{value}.png";
                cards.Add(new Card(color, value, imageName));
                if (value != "0" && value != "draw2" && value != "reverse" && value != "skip")
                {
                    cards.Add(new Card(color, value, imageName));
                }
            }
        }

        for (int i = 0; i < 4; i++)
        {
           /* cards.Add(new Card("wild", "wild", "uno_card-wildchange.png"));
            cards.Add(new Card("wild", "draw4", "uno_card-wilddraw4.png"));*/
        }
    }
    public void ShuffleCards()
    {
        // Implementing Fisher-Yates shuffle
        int n = cards.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            Card temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }
    }
    public Card getFirstCard()
    {
        return cards.First();
    }
    private void PopulateFirstCrad()
    {
        if (cards.Any())
        {
            var card = cards.First();
            playedCards.Add(card);
            cards.RemoveAt(0);
        }
    }
    public Card Draw()
    {


        if (cards.Any())
        {
            var card = cards.First();
            cards.RemoveAt(0);
            return card;
        }
        return null;
    }
    public void ReshufflePlayedCards()
    {
        cards.AddRange(playedCards);
        ShuffleCards(); 
        playedCards.Clear(); 
    }
}