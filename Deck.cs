using System.Collections.Generic;
using System;
using UnoGame;
using System.Linq;
using System.Windows.Forms;

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
        cards = new List<Card>();

        // Add cards with values 0-9 for each color (red, green, blue, yellow)
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
                    cards.Add(new Card(color, value, imageName)); // Add two of each card, except for 0
                }
            }
        }

        foreach (string color in colors)
        {
            string imageName = $"uno_card-{color}draw2.png"; // Adjusted according to your file naming convention
            cards.Add(new Card(color, "draw2", imageName));
            cards.Add(new Card(color, "draw2", imageName)); // Add two of each Draw Two card
        }


        foreach (string color in colors)
        {
            // Reverse cards
            string reverseImageName = $"uno_card-{color}reverse.png";
            cards.Add(new Card(color, "reverse", reverseImageName));
            cards.Add(new Card(color, "reverse", reverseImageName)); // Add two of each Reverse card

            // Skip cards
            string skipImageName = $"uno_card-{color}skip.png";
            cards.Add(new Card(color, "skip", skipImageName));
            cards.Add(new Card(color, "skip", skipImageName)); // Add two of each Skip card
        }

        for (int i = 0; i < 4; i++)
        {
            cards.Add(new Card("wild", "wild", "uno_card-wildchange.png"));
            cards.Add(new Card("wild", "draw4", "uno_card-wilddraw4.png"));
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
        if (cards.Count == 0)
        {
            // Check if there are played cards to reshuffle back into the deck
            if (playedCards.Count > 0)
            {
                ReshufflePlayedCards();
            }
            else
            {
                // Handle the case where there are no cards left to draw or reshuffle
                MessageBox.Show("No cards left in the deck or the pile of played cards. Check the game logic.");
                return null;
            }
        }

        var card = cards.First();
        cards.RemoveAt(0);
        return card;
    }

    public void ReshufflePlayedCards()
    {
        cards.AddRange(playedCards);
        ShuffleCards(); 
        playedCards.Clear(); 
    }
}