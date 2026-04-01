using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hearts_Logic.Models.Objects;

namespace Hearts_Logic.Actors
{
    // A concrete version of Player so the game can run for now
    public class TestPlayer : Player
    {
        public override Card PlayCard()
        {
            // Access the .Cards list inside the hand object
            if (hand.CardsInHand > 0)
            {
                // We pick the first card logic-wise
                Card selected = hand.Cards[0];

                // Use the method we wrote in Hand.cs
                hand.RemoveCard(selected);

                return selected;
            }
            return null!;
        }
    }
}
