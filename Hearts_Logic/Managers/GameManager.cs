using Hearts_Logic.Models.Objects;
using Hearts_Logic.Actors;
using System;
using System.Collections.Generic;

namespace Hearts_Logic.Managers
{
    // Singleton Class. Creates a static reference to itself if one does not already exist.
    // Handles majority of game setup functions as well as contains core game state properties.
    internal class GameManager
    {
        public static GameManager Instance { get; } = new GameManager();

        private List<Player> players = new List<Player>();
        private int currentPlayer = 0;
        private int dealerPosition = 0;

        private Deck? testDeck;
        private int shuffleCount = 3;
        private int cardsToDeal = 13;

        // Private constructor prevents class from being instantiated externally.
        private GameManager()
        {
            // Initializing with TestPlayers until Human/AI actors are implemented.
            for (int i = 0; i < 4; i++)
            {
                // We use TestPlayer because Player is now abstract (Task 09).
                Player p = new TestPlayer();
                players.Add(p);
            }
        }

        public void SetupDeck()
        {
            testDeck = new Deck();
            testDeck.Shuffle(shuffleCount);
        }

        // Deals cards to the internal hand objects of each player.
        // Fails if players or deck have not been properly initialized.
        public void DealCards()
        {
            if (testDeck == null) return;

            // Deal cards to all 4 players
            for (int i = 0; i < 4; i++)
            {
                DealHand(players[i]);
            }
        }

        // Moves cards from the Deck into the specific Player's Hand object.
        private void DealHand(Player player)
        {
            // testDeck is checked for null in the calling method DealCards.
            for (int i = 0; i < cardsToDeal; i++)
            {
                Card c = testDeck!.DrawCard();

                // Card now stores only logic data. 
                // Visual properties like Rotation/Flip are handled in the UI Layer later.
                player.AddCard(c);
            }
        }

        // Logic to increment through the list of players until the last index (3).
        public void SwitchPlayer()
        {
            currentPlayer++;
            if (currentPlayer > 3) { currentPlayer = 0; }
        }

        // Determines which player starts the hand and rotates the role clockwise.
        public void SwitchDealer()
        {
            dealerPosition++;
            if (dealerPosition > 3) { dealerPosition = 0; }
        }

        // This will be expanded for Actor Implementation (Task 11).
        public void AddPlayer(Player p)
        {
            players.Add(p);
        }

        // Placeholder for XRay logic - implementation will move to UI Layer (Task 20).
        public void XRayVision()
        {
            // In a decoupled architecture, logic merely signals the UI to show values.
            return;
        }
    }
}
