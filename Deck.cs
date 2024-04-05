using System;
using System.Collections.Generic;
using System.Linq;

public class Deck
{
    private List<Card> cards;
    private List<Card> playedCards;
    private Random random = new Random();

    public Deck()
    {
        cards = new List<Card>();
        playedCards = new List<Card>();
        InitializeCards();
        ShuffleCards();
    }

    private void InitializeCards()
    {
        string[] colors = { "red", "yellow", "green", "blue" };
        foreach (var color in colors)
        {
            // Add numbered cards 0-9. Note: '0' is added once per color, 1-9 twice
            for (int i = 0; i <= 9; i++)
            {
                string imageName = $"uno_card-{color}{i}.png";
                cards.Add(new Card(color, i.ToString(), imageName));
                if (i > 0) // Add a second set of cards 1-9
                {
                    cards.Add(new Card(color, i.ToString(), imageName));
                }
            }

            // Add two of each action card per color
            string[] actions = { "draw2", "reverse", "skip" };
            foreach (var action in actions)
            {
                string imageName = $"uno_card-{color}{action}.png";
                cards.Add(new Card(color, action, imageName));
                cards.Add(new Card(color, action, imageName));
            }
        }

        // Add Wild and Wild Draw Four cards, 4 of each
        for (int i = 0; i < 4; i++)
        {
            cards.Add(new Card("wild", "wild", "uno_card-wildchange.png"));
            cards.Add(new Card("wild", "draw4", "uno_card-wilddraw4.png"));
        }
    }

    private void ShuffleCards()
    {
        cards = cards.OrderBy(c => random.Next()).ToList();
    }

    public void RemoveClass(string color)
    {
        cards.RemoveAll(card => card.Color == color);
    }

    public void DistributeCards(List<Player> players, int numberOfCards)
    {
        foreach (var player in players)
        {
            for (int i = 0; i < numberOfCards; i++)
            {
                var card = Draw();
                if (card != null)
                    player.AddCardToHand(card);
            }
        }
    }

    public void AddToPlayedCards(Card card)
    {
        playedCards.Add(card);
    }

    public Card Draw()
    {
        if (!cards.Any())
        {
            RefillDeckFromPlayedCards();
        }

        if (cards.Any())
        {
            var card = cards.First();
            cards.RemoveAt(0);
            return card;
        }
        return null;
    }

    private void RefillDeckFromPlayedCards()
    {
        if (playedCards.Any())
        {
            cards.AddRange(playedCards);
            playedCards.Clear();
            ShuffleCards();
        }
        else
        {
            Console.WriteLine("No cards left to refill the deck.");
        }
    }
}
