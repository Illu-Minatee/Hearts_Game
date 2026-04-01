using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hearts_Logic.Models.Objects;


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
            // Task 16: Basic AI Brain implementation.
            // Currently chooses a random card from its hand to avoid predictable patterns.
            if (hand.Count > 0)
            {
                int index = _rng.Next(hand.Count);
                Card selected = hand[index];
                hand.RemoveAt(index);
                return selected;
            }
            return null;
        }
    }
}