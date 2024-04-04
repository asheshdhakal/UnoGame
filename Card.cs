﻿using System;
using System.IO;

namespace UnoGame
{
    public class Card
    {
        public string Color { get; set; }
        public string Value { get; set; }
        public string ImageName { get; set; }

        public Card(string color, string value, string imageName)
        {
            Color = color;
            Value = value;
            // Use a relative path to the Resources directory
            ImageName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\", imageName);
        }
    }
}
