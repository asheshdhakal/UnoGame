using System;
using System.IO;

namespace UnoGame
{
    public class Card
    {   
        // represents a card in the Uno game

        public string Color { get; set; }
        public string Value { get; set; }
        public string ImageName { get; set; }

        public Card(string color, string value, string imageName)
        {
            Color = color;
            Value = value;
            ImageName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\", imageName);
        }
    }
}
