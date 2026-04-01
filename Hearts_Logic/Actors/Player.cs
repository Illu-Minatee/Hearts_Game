using Hearts_Logic.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_Logic.Actors
{
    public abstract class Player
    {
        public string Name { get; set; } = "Player";
        public int Score { get; set; } = 0;

        // STEP 1: Changed from List<Card> to the actual Hand class
        protected Hand hand = new Hand();

        // STEP 2: Logic change - reference hand.CardsInHand
        public int CardsInHand => hand.CardsInHand;

        public abstract Card PlayCard();

        public void AddCard(Card card)
        {
            // STEP 3: Uses the Hand class's AddCard method
            hand.AddCard(card);
        }

        public void ClearHand()
        {
            // Note: Since we don't have a clear method in Hand yet, 
            // we just make a new one.
            hand = new Hand();
        }
    }
}

