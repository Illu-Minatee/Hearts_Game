using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hearts_Logic.Models.Objects
{
    // STEP 1: Remove ": Image" (This makes it a Data Object, not a UI element)
    public class Card
    {
        public int Value { get; }
        public CardSuit Suit { get; }
        public bool IsFaceUp { get; set; } = false;

        // Note: 'Clickable' and 'Rotate' moved out of here 
        // because those are GUI behaviors, not Card logic.

        public Card(int value, CardSuit suit)
        {
            this.Value = value;
            this.Suit = suit;
        }

        // Logic-based penalty calculation for Milestone 3 (Scoring)
        public int GetPenaltyValue()
        {
            if (Suit == CardSuit.Hearts) return 1;
            if (Suit == CardSuit.Spades && Value == 12) return 5; // Queen of Spades
            return 0;
        }

        public override string ToString()
        {
            return $"{Value} of {Suit}";
        }
    }

    public enum CardSuit
    {
        Hearts, Spades, Clubs, Diamonds
    }
}