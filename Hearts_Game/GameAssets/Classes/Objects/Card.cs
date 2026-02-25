using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Hearts_Game.GameAssets.Classes.Objects
{
    public class Card : Image
    {
        public int Value { get; } = 0;    
        public CardSuit? Suit { get; }
        public bool IsFaceUp { get; set; } = false;

        private BitmapImage? cardFace;                  
        private BitmapImage? cardBack;

        public Card() 
        {
            MouseDown += OnMouseClick;
        }

        public Card(int value, CardSuit suit, bool faceUp = false)
        {
            Value = value;
            Suit = suit;
            IsFaceUp = faceUp;        
        }

        public void Flip()
        {
            IsFaceUp = !IsFaceUp;
        }

        private void OnMouseClick(object obj, MouseButtonEventArgs args)
        {
            Debug.WriteLine($"{Value} of {Suit}");
        }
    }

    public enum CardSuit
    {
        Hearts, Spades, Clubs, Diamonds
    }
}
