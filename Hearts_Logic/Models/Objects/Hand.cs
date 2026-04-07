using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Hearts_Logic.Models.Objects
{
    // STEP 1: Removed ": Grid" inheritance to decouple from UI
    public class Hand
    {
        public List<Card> Cards = new List<Card>();
        public int CardsInHand => Cards.Count;

        public void AddCard(Card card)
        {
            // Removed Alignment and Children.Add logic (This belongs in WPF project now)
            Cards.Add(card);
        }

        public void RemoveCard(Card card)
        {
            if (Cards.Contains(card))
            {
                Cards.Remove(card);
            }
        }
    }
}