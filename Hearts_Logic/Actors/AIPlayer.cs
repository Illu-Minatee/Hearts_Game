using Hearts_Logic.Managers;
using Hearts_Logic.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Hearts_Logic.Actors
{
    /// <summary>
    /// Represents a computer-controlled opponent.
    /// Inherits from Player and implements automated decision logic.
    /// </summary>
    public class AIPlayer : Player
    {
        private Random _rng = new Random();

        public AIPlayer(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Polymorphic override.
        /// Automated logic for card selection.
        /// </summary>
        public override Card PlayCard()
        {
            if (PlayerHand.CardsInHand == 0)
                return null!;

            CardSuit? leadSuit = GameManager.Instance.LeadSuit;

            // Follow the lead suit if possible
            if (leadSuit != null)
            {
                foreach (Card card in PlayerHand.Cards)
                {
                    if (card.Suit == leadSuit)
                    {
                        return card;
                    }
                }
            }

            // If no matching suit, play the first card
            return PlayerHand.Cards[0];
        }
    }
}