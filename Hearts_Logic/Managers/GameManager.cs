using Hearts_Logic.Models.Objects;
using Hearts_Logic.Actors;
using System;
using System.Collections.Generic;


namespace Hearts_Logic.Managers
{
    // Singleton Class. Creates a static reference to itself if one does not already exist.
    // Handles majority of game setup functions as well as contains core game state properties.
    public class GameManager
    {
        public static GameManager Instance { get; } = new GameManager();
        public bool IsXRayEnabled { get; set; } = false;
        public List<Player> players = new List<Player>();
        private int currentPlayer = 0;
        private int dealerPosition = 0;

        private Deck? testDeck;
        private int shuffleCount = 3;
        private int cardsToDeal = 13;
        public List<(Player player, Card card)> CurrentTrick { get; private set; } = new();
        private CardSuit? _leadSuit = null;
        private bool _heartsBroken = false;
        public CardSuit? LeadSuit => _leadSuit;

        public bool HeartsBroken
        {
            get
            {
                return _heartsBroken;
            }
        }

        // Private constructor prevents class from being instantiated externally.
        private GameManager()
        {
            // Clear any existing players just in case
            players.Clear();

            // 1. Add the Local Human Player (Index 0)
            players.Add(new HumanPlayer("User"));

            // 2. Add 3 AI Opponents (Indices 1, 2, and 3)
            players.Add(new AIPlayer("CPU West"));
            players.Add(new AIPlayer("CPU North"));
            players.Add(new AIPlayer("CPU East"));
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
            // Toggle the logical state of the cheat/developer mode
            IsXRayEnabled = !IsXRayEnabled;

        }

        private bool PlayerHasSuit(Player player, CardSuit suit)
        {
            foreach (Card c in player.PlayerHand.Cards)
            {
                if (c.Suit == suit)
                {
                    return true;
                }
            }
            return false;
        }
        public bool TryPlayCard(Player player, Card card)
        {
            // First card in trick
            if (CurrentTrick.Count == 0)
            {
                // Cannot lead hearts until broken,
                // unless player has only hearts left
                if (card.Suit == CardSuit.Hearts && !_heartsBroken)
                {
                    bool hasNonHeart = false;

                    foreach (Card c in player.PlayerHand.Cards)
                    {
                        if (c.Suit != CardSuit.Hearts)
                        {
                            hasNonHeart = true;
                            break;
                        }
                    }

                    if (hasNonHeart)
                        return false;
                }

                _leadSuit = card.Suit;
            }
            else
            {
                // Must follow lead suit if possible
                if (_leadSuit != null &&
                    card.Suit != _leadSuit &&
                    PlayerHasSuit(player, _leadSuit.Value))
                {
                    return false;
                }
            }

            // If any heart is played, hearts are broken
            if (card.Suit == CardSuit.Hearts)
            {
                _heartsBroken = true;
            }

            CurrentTrick.Add((player, card));
            return true;
        }

        public Player GetTrickWinner()
        {
            if (CurrentTrick.Count == 0 || _leadSuit == null)
                return null!;

            Player winner = CurrentTrick[0].player;
            Card winningCard = CurrentTrick[0].card;

            foreach (var play in CurrentTrick)
            {
                if (play.card.Suit == _leadSuit &&
                    GetCardRank(play.card) > GetCardRank(winningCard))
                {
                    winner = play.player;
                    winningCard = play.card;
                }
            }

            return winner;
        }
        public int CalculateTrickPoints()
        {
            int total = 0;

            foreach (var play in CurrentTrick)
            {
                total += play.card.GetPenaltyValue();
            }

            return total;
        }
        public void ClearTrick()
        {
            CurrentTrick.Clear();
            _leadSuit = null;
        }
        private int GetCardRank(Card card)
        {
            if (card.Value == 1)
                return 14; // Ace is highest

            return card.Value;
        }
    }
}
