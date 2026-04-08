using Hearts_Logic.Models.Objects;
using Hearts_Logic.Actors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hearts_Logic.Managers
{
    // Singleton Class: The central brain that manages rules and scoring.
    public class GameManager
    {
        public static GameManager Instance { get; } = new GameManager();

        public List<Player> players = new List<Player>();
        public bool IsXRayEnabled { get; set; } = false; // Dev cheat flag (Zehahahaha!)
        private int currentPlayer = 0;
        private int dealerPosition = 0;

        private Deck? testDeck;
        private int shuffleCount = 3;
        private int cardsToDeal = 13;

        // Task 20: Pairs the Player with the Card they played to identify winners.
        public List<(Player player, Card card)> CurrentTrick { get; private set; } = new();

        // Task 32: Rule flag - Hearts can't lead until a Heart has been discarded.
        private bool _heartsBroken = false;
        public bool HeartsBroken => _heartsBroken;

        private GameManager()
        {
            players.Clear();
            // Default setup: 1 Human vs 3 CPU (Mandatory Requirement)
            players.Add(new HumanPlayer("User"));
            players.Add(new AIPlayer("CPU West"));
            players.Add(new AIPlayer("CPU North"));
            players.Add(new AIPlayer("CPU East"));
        }

        // Resets state and prepares the 52 cards.
        public void SetupDeck()
        {
            testDeck = new Deck();
            testDeck.Shuffle(shuffleCount);
            _heartsBroken = false;
            // Clear any leftover cards from the logic-trick area
            CurrentTrick.Clear();
        }

        // Logic loop to distribute 13 cards to every hand object.
        public void DealCards()
        {
            if (testDeck == null) return;
            foreach (var p in players) { DealHand(p); }
        }

        private void DealHand(Player player)
        {
            for (int i = 0; i < cardsToDeal; i++)
            {
                player.AddCard(testDeck!.DrawCard());
            }
        }

        // Task 32 Rule Engine: Validates moves before they hit the table.
        // Returns FALSE if the player tries to break the "Breaking Hearts" rule.
        public bool TryPlayCard(Player player, Card card)
        {
            bool isLeadingTrick = CurrentTrick.Count == 0;

            if (isLeadingTrick && card.Suit == CardSuit.Hearts && !_heartsBroken)
            {
                // Verify if player is forced to lead Hearts (no other suits left)
                bool hasNonHeart = player.PlayerHand.Cards.Any(c => c.Suit != CardSuit.Hearts);
                if (hasNonHeart) return false; // Illegal move!
                _heartsBroken = true;
            }

            if (card.Suit == CardSuit.Hearts) { _heartsBroken = true; }

            // Logical transfer of card from Hand to Trick
            player.PlayerHand.RemoveCard(card);
            CurrentTrick.Add((player, card));
            return true;
        }

        // Task 20: Finds the player who played the highest card of the led suit.
        public Player DetermineCurrentTrickWinner()
        {
            if (CurrentTrick.Count == 0) return players[0];

            CardSuit leadSuit = CurrentTrick[0].card.Suit;
            return CurrentTrick
                .Where(p => p.card.Suit == leadSuit)
                .OrderByDescending(p => p.card.Value)
                .First().player;
        }

        // Task 33: Totals the points in the middle and gives them to the trick winner.
        public void CalculateTrickPoints()
        {
            if (CurrentTrick.Count == 0) return;

            Player winner = DetermineCurrentTrickWinner();
            winner.Score += CurrentTrick.Sum(p => p.card.GetPenaltyValue());
            CurrentTrick.Clear();
        }

        public void XRayVision() { IsXRayEnabled = !IsXRayEnabled; }

        public void SwitchPlayer()
        {
            currentPlayer = (currentPlayer + 1) % 4;
        }
    }
}