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
            // Simple logic for testing: just play the first card in hand
            Card card = hand[0];
            hand.RemoveAt(0);
            return card;
        }
    }
}
