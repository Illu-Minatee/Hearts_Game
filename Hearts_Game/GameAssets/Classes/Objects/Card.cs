using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hearts_Game.GameAssets.Classes.Objects
{
    public class Card : Image
    {
        public int Value { get; } = 0;    
        public CardSuit? Suit { get; }
        public bool IsFaceUp { get; set; } = false;
        public bool Clickable { get; set; } = false;

        private BitmapImage? cardFace;                  
        private BitmapImage? cardBack;

        public Card() 
        {
            MouseDown += OnMouseClick;
            RenderTransformOrigin = new Point(0.5, 0.5);
        }

        public Card(int value, CardSuit suit, BitmapImage face, BitmapImage back, bool faceUp = false)
        {
            Value = value;
            Suit = suit;
            cardFace = face;
            cardBack = back;
            IsFaceUp = faceUp;
            MouseDown += OnMouseClick;
            RenderTransformOrigin = new Point(0.5, 0.5);
            Source = IsFaceUp ? cardFace : cardBack;
        }

        public void Flip()
        {
            IsFaceUp = !IsFaceUp;
            Source = IsFaceUp ? cardFace : cardBack;
        }

        public void Rotate(float angle)
        {
            RenderTransform = new RotateTransform(angle);
        }

        //This will need to play the "players" card. Clickable should be set to false on npc cards
        //Right now we are registering to the event regardless. This probably isnt an issue, the alternative is just subscribe "if" clickable. (constructor will need updating
        private void OnMouseClick(object obj, MouseButtonEventArgs args)
        {
            if (Clickable)
            {
                Debug.WriteLine($"{Value} of {Suit}");
            } 
        }
    }

    public enum CardSuit
    {
        Hearts, Spades, Clubs, Diamonds
    }
}
