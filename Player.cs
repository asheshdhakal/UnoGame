using System.Collections.Generic;
using UnoGame;

public class Player
{
    public List<Card> Hand { get; private set; }
    public bool IsHuman { get; private set; }

    public Player(bool isHuman)
    {
        Hand = new List<Card>();
        IsHuman = isHuman;
    }

    public void DrawCard(Deck deck)
    {
        var card = deck.Draw();
        if (card != null) Hand.Add(card);
       

    }
}