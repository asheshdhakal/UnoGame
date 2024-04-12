using System.Collections.Generic;

namespace UnoGame
{
    public abstract class BasePlayer
    {
        public List<Card> Hand { get; protected set; }
        public string Name { get; set; }

        protected BasePlayer(string name)
        {
            Hand = new List<Card>();
            Name = name;
        }

        public virtual void DrawCard(Deck deck)
        {
            var card = deck.Draw();
            if (card != null)
                Hand.Add(card);
        }
    }
}