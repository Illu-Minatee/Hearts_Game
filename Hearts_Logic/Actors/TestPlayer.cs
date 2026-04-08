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
            if (PlayerHand.CardsInHand > 0)
            {
                Card selected = PlayerHand.Cards[0];
                PlayerHand.RemoveCard(selected);
                return selected; ;
            }
            return null!;
        }
    }
}
