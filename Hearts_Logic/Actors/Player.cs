using Hearts_Logic.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_Logic.Actors
{
    // Step 1: Add "abstract"
    public abstract class Player
    {
        public string Name { get; set; } = "Player";
        public int Score { get; set; } = 0;

        // This is the player's collection of cards
        protected List<Card> hand = new List<Card>();

        public int CardsInHand => hand.Count;

        // Abstract method: Forces AI and Human to define how they choose a card
        public abstract Card PlayCard();

        public void AddCard(Card card)
        {
            hand.Add(card);
        }

        public void ClearHand()
        {
            hand.Clear();
        }
    }
}

