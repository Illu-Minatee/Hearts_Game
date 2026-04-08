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
        public Hand PlayerHand { get; set; } = new Hand();

        // STEP 2: Logic change - reference hand.CardsInHand
        public int CardsInHand => PlayerHand.CardsInHand;

        public abstract Card PlayCard();

        public void AddCard(Card card)
        {
            PlayerHand.AddCard(card);
        }

        public void ClearHand()
        {
            // Re-initialize the player's hand for the next game
            PlayerHand = new Hand();
        }
    }
}

