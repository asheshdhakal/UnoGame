﻿using System.Collections.Generic;
using UnoGame;

public class Player
{
    public List<Card> Hand { get; private set; }
    public bool IsHuman { get; private set; }
    public string Name { get; set; } // Add player name

    public Player(bool isHuman, string name) // Adjust constructor to accept a name
    {
        Hand = new List<Card>();
        IsHuman = isHuman;
        Name = name; // Initialize name
    }

    public void DrawCard(Deck deck)
    {
        var card = deck.Draw();
        if (card != null) Hand.Add(card);
       

    }
}