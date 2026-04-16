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

            // There is a lead suit - AI must try to follow it
            if (leadSuit != null)
            {
                // Collect all cards of the lead suit
                List<Card> sameSuit = new List<Card>();
                foreach (Card card in PlayerHand.Cards)
                {
                    if (card.Suit == leadSuit)
                    {
                        sameSuit.Add(card);
                    }
                }

                // If AI has the lead suit, play the lowest card of that suit
                if (sameSuit.Count > 0)
                {
                    return GetLowestCard(sameSuit);
                }

                // AI cannot follow suit - dump a bad card
                // 1. Dump Queen of Spades if we have it
                foreach (Card card in PlayerHand.Cards)
                {
                    if (card.Suit == CardSuit.Spades && card.Value == 12)
                    {
                        return card;
                    }
                }

                // 2. Otherwise dump the highest Heart if we have one
                List<Card> hearts = new List<Card>();
                foreach (Card card in PlayerHand.Cards)
                {
                    if (card.Suit == CardSuit.Hearts)
                    {
                        hearts.Add(card);
                    }
                }

                if (hearts.Count > 0)
                {
                    return GetHighestCard(hearts);
                }

                // 3. Otherwise play the highest card in hand
                return GetHighestCard(PlayerHand.Cards);
            }

            // No lead suit - AI is leading the trick
            bool heartsBroken = GameManager.Instance.HeartsBroken;

            if (!heartsBroken)
            {
                // Collect all non-Heart cards
                List<Card> nonHearts = new List<Card>();
                foreach (Card card in PlayerHand.Cards)
                {
                    if (card.Suit != CardSuit.Hearts)
                    {
                        nonHearts.Add(card);
                    }
                }

                // Play lowest non-Heart if possible
                if (nonHearts.Count > 0)
                {
                    return GetLowestCard(nonHearts);
                }

                // Only Hearts left - must play a Heart
                return GetLowestCard(PlayerHand.Cards);
            }

            // Hearts are broken - just play the lowest card
            return GetLowestCard(PlayerHand.Cards);
        }

        /// <summary>
        /// Finds the lowest ranked card in a list. Ace is treated as highest.
        /// </summary>
        private Card GetLowestCard(List<Card> cards)
        {
            Card lowest = cards[0];

            foreach (Card card in cards)
            {
                if (card.GetRank() < lowest.GetRank())
                {
                    lowest = card;
                }
            }

            return lowest;
        }

        /// <summary>
        /// Finds the highest ranked card in a list. Ace is treated as highest.
        /// </summary>
        private Card GetHighestCard(List<Card> cards)
        {
            Card highest = cards[0];

            foreach (Card card in cards)
            {
                if (card.GetRank() > highest.GetRank())
                {
                    highest = card;
                }
            }

            return highest;
        }

        /// <summary>
        /// Returns card rank where Ace is highest (14).
        /// </summary>
       
    }
}