using System.Collections.Generic;
using System;
using UnoGame;
using System.Linq;
using System.Windows.Forms;


// represents a deck of Uno cards

public class Deck
{
    public List<Card> cards;
    public List<Card> playedCards;
    private Random random = new Random();

    // constructor to initialize the deck

    public Deck()
    {
        cards = new List<Card>();
        playedCards = new List<Card>();
        InitializeCards();
        ShuffleCards();
        PopulateFirstCard();
    }

    // initializes the cards in the deck
    private void InitializeCards()
    {
        cards = new List<Card>();

        string[] colors = { "red", "green", "blue", "yellow" };
        string[] values = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        foreach (string color in colors)
        {
            foreach (string value in values)
            {
                string imageName = $"uno_card-{color}{value}.png";
                cards.Add(new Card(color, value, imageName));
                if (value != "0")
                {
                    cards.Add(new Card(color, value, imageName));
                }
            }
        }

        foreach (string color in colors)
        {
            string imageName = $"uno_card-{color}draw2.png"; 
            cards.Add(new Card(color, "draw2", imageName));
            cards.Add(new Card(color, "draw2", imageName)); 
        }

        foreach (string color in colors)
        {
            // Reverse cards
            string reverseImageName = $"uno_card-{color}reverse.png";
            cards.Add(new Card(color, "reverse", reverseImageName));
            cards.Add(new Card(color, "reverse", reverseImageName)); 

            // Skip cards
            string skipImageName = $"uno_card-{color}skip.png";
            cards.Add(new Card(color, "skip", skipImageName));
            cards.Add(new Card(color, "skip", skipImageName)); 
        }

        for (int i = 0; i < 4; i++)
        {
            cards.Add(new Card("wild", "wild", "uno_card-wildchange.png"));
            cards.Add(new Card("wild", "draw4", "uno_card-wilddraw4.png"));
        }
    }
    //shuffling the cards in the deck
    public void ShuffleCards()
    {
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

    //adds the first card to the pile of played cards
    private void PopulateFirstCard()
    {
        if (cards.Any())
        {
            var card = cards.First();
            playedCards.Add(card);
            cards.RemoveAt(0);
        }
    }
    // draws a card from the deck
    public Card Draw()
    {
        if (cards.Count == 0)
        {
            if (playedCards.Count > 0)
            {
                ReshufflePlayedCards();
            }
            else
            {
                MessageBox.Show("No cards left in the deck or the pile of played cards. Check the game logic.");
                return null;
            }
        }

        var card = cards.First();
        cards.RemoveAt(0);
        return card;
    }

    // reshuffles the played cards pile into the deck
    public void ReshufflePlayedCards()
    {
        cards.AddRange(playedCards);
        ShuffleCards();
        playedCards.Clear();
    }
}