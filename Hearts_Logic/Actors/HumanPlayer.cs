using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hearts_Logic.Models.Objects;

namespace Hearts_Logic.Actors
{
    /// <summary>
    /// Represents the local human user. 
    /// Inherits from Player to satisfy OOP requirements.
    /// </summary>
    public class HumanPlayer : Player
    {
        public HumanPlayer(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Polymorphic override. 
        /// In a full implementation, this would tie into a UI selection event.
        /// </summary>
        public override Card PlayCard()
        {
            // Placeholder: Returning the first card. 
            // In Task 17, we will link this to the GUI click.
            // "hand" is a Hand object, so it has .CardsInHand and .Cards[0]
            if (PlayerHand.CardsInHand > 0)
            {
                Card selected = PlayerHand.Cards[0];
                PlayerHand.RemoveCard(selected);
                return selected;
            }
            return null!; // ! indicates that we expect this to never be null in a valid game state.
        }
    }
}