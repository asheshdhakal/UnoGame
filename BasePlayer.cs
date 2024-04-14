using System.Collections.Generic;

namespace UnoGame
{

    // represents a base class for Uno players
    public abstract class BasePlayer
    {
        public List<Card> Hand { get; protected set; }
        public string Name { get; set; }

        protected BasePlayer(string name)
        {
            Hand = new List<Card>();
            Name = name;
        }

        // method to draw a card from the deck and add it to the player's hand
        public virtual void DrawCard(Deck deck)
        {
            var card = deck.Draw();
            if (card != null)
                Hand.Add(card);
        }
    }
}