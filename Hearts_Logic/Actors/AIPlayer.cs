using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hearts_Logic.Models.Objects;

using Hearts_Logic.Managers;
namespace Hearts_Logic.Actors
{
    /// <summary>
    /// Task 34: AI Brain — Integrated Strategic Selection.
    /// Implements automated decision logic based on current trick state.
    /// </summary>
    public class AIPlayer : Player
    {
        private Random _rng = new Random();

        public AIPlayer(string name)
        {
            this.Name = name;
        }

        public override Card PlayCard()
        {
            if (PlayerHand.CardsInHand == 0) return null!;

            var gm = GameManager.Instance;
            var handCards = PlayerHand.Cards;

            // CASE 1: Following a trick (cards already on the table)
            if (gm.CurrentTrick.Count > 0)
            {
                // Lead suit is determined by the first card played in the trick
                CardSuit leadSuit = gm.CurrentTrick[0].card.Suit;

                // Find cards in hand that match the lead suit
                var matchingSuit = handCards.Where(c => c.Suit == leadSuit).ToList();

                if (matchingSuit.Count > 0)
                {
                    // MUST follow suit — AI plays the lowest to stay safe
                    Card toPlay = matchingSuit.OrderBy(c => c.Value).First();
                    PlayerHand.RemoveCard(toPlay);
                    return toPlay;
                }
                else
                {
                    // VOID in lead suit — AI DUMPS its highest penalty card
                    Card toPlay = handCards
                        .OrderByDescending(c => c.GetPenaltyValue())   // Penalty cards first
                        .ThenByDescending(c => c.Value)                 // Then high face value
                        .First();

                    PlayerHand.RemoveCard(toPlay);
                    return toPlay;
                }
            }

            // CASE 2: Leading the trick
            List<Card> leadCandidates = handCards.ToList();

            // Enforce "Breaking Hearts" rule
            if (!gm.HeartsBroken)
            {
                var nonHearts = handCards.Where(c => c.Suit != CardSuit.Hearts).ToList();
                if (nonHearts.Count > 0)
                    leadCandidates = nonHearts;
            }

            // Safe lead: play the lowest non-penalty card available
            Card leadCard = leadCandidates
                .OrderBy(c => c.GetPenaltyValue())
                .ThenBy(c => c.Value)
                .First();

            PlayerHand.RemoveCard(leadCard);
            return leadCard;
        }
    }
}