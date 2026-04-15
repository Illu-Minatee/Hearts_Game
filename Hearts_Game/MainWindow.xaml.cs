using Hearts_Game.GameAssets;
using Hearts_Logic.Actors;
using Hearts_Logic.Managers;
using Hearts_Logic.Models.Objects;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;


namespace Hearts_Game
{
    public partial class MainWindow : Window
    {
        // Path used to load card images
        public static readonly string resourceDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName ?? "";
        public static Dictionary<string, BitmapImage> cardFaceSprites = new Dictionary<string, BitmapImage>();
        public static BitmapImage? cardBack;
        private string _humanPlayerName = "Player";
        private int _currentPlayerIndex = 0;
        private int _cardsPlayedThisTrick = 0;
        private int _trickNumber = 1;
        private bool _isAnimatingCard = false;
        private CardSuit? _leadSuit = null;


        public MainWindow()
        {
            InitializeComponent();

            string cardDirectory = "/GameAssets/Images/Cards/";
            cardFaceSprites = GetCardResources(cardDirectory);
            cardBack = LoadResources(resourceDir + cardDirectory + "cardBack_red3.png");

            StartScreen.Visibility = Visibility.Visible;
            GameScreen.Visibility = Visibility.Collapsed;
            LoadingOverlay.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Creates a card image for the UI.
        /// </summary>
        public static Image GetVisualCard(Card logicCard)
        {
            // Create image for the card
            Image cardImage = new Image
            {
                Width = 45,
                Height = 55,
                Stretch = Stretch.Fill,
                //  Set the card image
                Source = cardFaceSprites["card" + logicCard.Suit + logicCard.Value]
            };

            return cardImage;
        }

        private async void OnStartFromIntroClick(object sender, RoutedEventArgs e)
        {
            _humanPlayerName = string.IsNullOrWhiteSpace(txtPlayerName.Text)
                ? "Player"
                : txtPlayerName.Text.Trim();

            LoadingOverlay.Visibility = Visibility.Visible;
            await Task.Delay(1000);

            StartNewGame();

            LoadingOverlay.Visibility = Visibility.Collapsed;
            StartScreen.Visibility = Visibility.Collapsed;
            GameScreen.Visibility = Visibility.Visible;
        }

        private void StartNewGame()
        {
            GameManager.Instance.players.Clear();

            GameManager.Instance.players.Add(new HumanPlayer(_humanPlayerName)); // 0 → South
            GameManager.Instance.players.Add(new AIPlayer("CPU West"));          // 1 → West
            GameManager.Instance.players.Add(new AIPlayer("CPU North"));         // 2 → North
            GameManager.Instance.players.Add(new AIPlayer("CPU East"));          // 3 → East

            foreach (var player in GameManager.Instance.players)
            {
                player.ClearHand();
                player.Score = 0;
            }

            trickCards.Children.Clear();
            GameManager.Instance.ClearTrick();

            lstGameLog.Items.Clear();
            GameManager.Instance.SetupDeck();
            GameManager.Instance.DealCards();
            _currentPlayerIndex = 0;
            _cardsPlayedThisTrick = 0;
            _isAnimatingCard = false;
            _trickNumber = 1;

            RefreshGameBoard();
            RefreshInfoPanels();

            AddLog($"New game started for {_humanPlayerName}.");
            AddLog("Cards dealt to all 4 players.");
        }

        /// <summary>
        /// Draws all player cards on the table.
        /// </summary>
        private void RefreshGameBoard()
        {
            // Clear all existing visual cards first
            zoneOne.Children.Clear();
            zoneTwo.Children.Clear();
            zoneThree.Children.Clear();
            zoneFour.Children.Clear();

            Grid[] zones = { zoneOne, zoneTwo, zoneThree, zoneFour };

            // Loop through the 4 players defined in GameManager
            for (int p = 0; p < 4; p++)
            {
                var player = GameManager.Instance.players[p];
                bool isHuman = (p == 0); // Only player 0 is the human

                for (int i = 0; i < player.CardsInHand; i++)
                {
                    Card logicCard = player.PlayerHand.Cards[i];
                    GameAssets.CardUI visualCard = new GameAssets.CardUI();

                    // IF HUMAN: Use the face sprite and allow interaction only on the Human's turn
                    // AI cards use the card back
                    BitmapImage source;

                    // Show face card for player or X-Ray mode
                    if (isHuman || GameManager.Instance.IsXRayEnabled)
                    {
                        // Show the card face
                        source = cardFaceSprites["card" + logicCard.Suit + logicCard.Value];
                    }
                    else
                    {
                        // Otherwise, keep the AI's cards hidden (Card Back)
                        source = cardBack!;
                    }

                    // Only allow the Human to click their cards during their turn
                    bool canInteract = isHuman && _currentPlayerIndex == 0 && !_isAnimatingCard;

                    visualCard.BindData(logicCard, source, canInteract);
                    visualCard.IsTabStop = canInteract;

                    // Card position
                    visualCard.HorizontalAlignment = HorizontalAlignment.Left;
                    visualCard.VerticalAlignment = VerticalAlignment.Top;

                    if (p == 0) // Human (Bottom)
                    {
                        visualCard.Margin = new Thickness(i * 25, 0, 0, 0);
                    }
                    else if (p == 1) // AI 1 (Left Side)
                    {
                        visualCard.RenderTransform = new RotateTransform(90); // Turn it sideways
                        visualCard.Margin = new Thickness(0, i * 18, 0, 0);
                    }
                    else if (p == 2) // AI 2 (Right Side)
                    {
                        visualCard.RenderTransform = new RotateTransform(-90); // Turn it other way
                        visualCard.Margin = new Thickness(0, i * 18, 0, 0);
                    }
                    else if (p == 3) // AI 3 (Top)
                    {
                        visualCard.Margin = new Thickness(i * 25, 0, 0, 0);
                    }

                    zones[p].Children.Add(visualCard);
                }
            }

            // After the board is drawn, update the active player highlight
            UpdateTurnUI();
        }


        /// <summary>
        /// Highlights the current player's area.
        /// </summary>
        private void UpdateTurnUI()
        {
            // Make sure players exist
            if (GameManager.Instance.players.Count < 4)
                return;

            // Update the label on the left panel
            txtCurrentPlayerName.Text = GameManager.Instance.players[_currentPlayerIndex].Name;

            Grid[] zones = new Grid[4];

            // Correct table layout
            zones[0] = zoneOne;   // South (Human - bottom)
            zones[1] = zoneTwo;   // West (left)
            zones[2] = zoneFour;  // North (top) 
            zones[3] = zoneThree; // East (right)  

            for (int i = 0; i < zones.Length; i++)
            {
                // Highlight current player
                zones[i].Opacity = i == _currentPlayerIndex ? 1.0 : 0.65;
                Panel.SetZIndex(zones[i], i == _currentPlayerIndex ? 20 : 5);

                if (i == _currentPlayerIndex)
                {
                    // Add a gold glow around the current player
                    zones[i].Effect = new System.Windows.Media.Effects.DropShadowEffect
                    {
                        Color = Colors.Gold,
                        BlurRadius = 25,
                        ShadowDepth = 0,
                        Opacity = 0.9
                    };
                }
                else
                {
                    // Remove glow from inactive players
                    zones[i].Effect = null;
                }
            }
        }

        /// <summary>
        /// Loads card images.
        /// </summary>
        public static Dictionary<string, BitmapImage> GetCardResources(string dir)
        {
            Dictionary<string, BitmapImage> imgs = new Dictionary<string, BitmapImage>();

            foreach (var suit in Enum.GetValues(typeof(CardSuit)))
            {
                for (int i = 1; i <= 13; i++) // Updated range to match standard card ranks
                {
                    string key = "card" + suit + i;
                    string path = resourceDir + dir + key + ".png";

                    if (!File.Exists(path))
                    {
                        Debug.WriteLine($"Asset Error: Path Not Found.\n{path}");
                        continue; // Skip missing assets instead of crashing the app
                    }

                    imgs.Add(key, LoadResources(path));
                }
            }
            return imgs;
        }

        private static BitmapImage LoadResources(string path)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(path, UriKind.Absolute);
            bmp.CacheOption = BitmapCacheOption.OnLoad; //  Load image now
            bmp.EndInit();
            return bmp;
        }

        private void OnXRayClick(object sender, RoutedEventArgs e)
        {
            // UI Layer toggle for card visibility
            GameManager.Instance.XRayVision();
            AddLog("X-Ray mode changed.");
            // (RefreshGameBoard will now check the Logic Manager's flag)
            RefreshGameBoard();
        }

        /// <summary>
        /// Changes the card back theme.
        /// </summary>
        private void OnThemeChange(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                // The 'Tag' in XAML tells us which filename to load
                string themeFile = item.Tag.ToString() ?? "cardBack_red3.png";
                string cardDirectory = "/GameAssets/Images/Cards/";

                // Load the new global back
                cardBack = LoadResources(resourceDir + cardDirectory + themeFile);

                string themeName = item.Header.ToString() ?? "Default";
                AddLog("Card theme changed to " + themeName + ".");

                // Redraw the board with the new theme
                RefreshGameBoard();
            }
        }


        private void OnWindowKeyDown(object sender, KeyEventArgs e)
        {
            // Exit or Menu on Escape
            if (e.Key == Key.Escape)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to quit the game?", "Exit", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void OnQuitClick(object sender, RoutedEventArgs e)
        {
            // Esc for exit
            OnWindowKeyDown(this, new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, Key.Escape));
        }

        /// <summary>
        /// Moves a card visually to the center of the table.
        /// </summary>
        public async void ShowPlayedCard(CardUI visualCard, int playerIndex)
        {
            if (_isAnimatingCard)
                return;

            if (playerIndex != _currentPlayerIndex)
                return;

            await PlayCardToCenterAsync(visualCard, playerIndex);
        }
        /// <summary>
        /// Moves a played card to the center with animation.
        /// </summary>
        private async Task PlayCardToCenterAsync(CardUI visualCard, int playerIndex)
        {
            if (visualCard.CardData == null)
                return;

            _isAnimatingCard = true;

            Card playedCard = visualCard.CardData;
            // Add card play to log
            AddLog(GameManager.Instance.players[playerIndex].Name + " played " + GetCardName(playedCard));

            // --- SET LEAD SUIT ---
            // If this is the first card of the trick, define the lead suit
            if (_cardsPlayedThisTrick == 0)
            {
                _leadSuit = playedCard.Suit;

                // Update UI
                RefreshInfoPanels();
            }

            // Remove from logical hand first
            GameManager.Instance.players[playerIndex].PlayerHand.RemoveCard(playedCard);

            // Remove from current visual parent if it came from a hand zone
            Panel parent = visualCard.Parent as Panel;

            if (parent != null)
            {
                parent.Children.Remove(visualCard);
            }

            // Stop clicks during animation
            visualCard.IsHitTestVisible = false;
            visualCard.RenderTransform = Transform.Identity;

            double finalX = 0;
            double finalY = 0;

            if (playerIndex == 0) finalY = 120;
            if (playerIndex == 1) finalX = -180;
            if (playerIndex == 2) finalY = -120;
            if (playerIndex == 3) finalX = 180;

            Thickness startMargin;
            Thickness endMargin = new Thickness(finalX, finalY, 0, 0);

            // start position for each player
            if (playerIndex == 0) // South / bottom
                startMargin = new Thickness(finalX, 220, 0, 0);
            else if (playerIndex == 1) // West / left
                startMargin = new Thickness(-220, finalY, 0, 0);
            else if (playerIndex == 2) // North / top
                startMargin = new Thickness(finalX, -220, 0, 0);
            else // playerIndex == 3 // East / right
                startMargin = new Thickness(220, finalY, 0, 0);

            visualCard.HorizontalAlignment = HorizontalAlignment.Center;
            visualCard.VerticalAlignment = VerticalAlignment.Center;
            visualCard.Margin = startMargin;

            trickCards.Children.Add(visualCard);

            ThicknessAnimation slideAnim = new ThicknessAnimation
            {
                From = startMargin,
                To = endMargin,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            visualCard.BeginAnimation(FrameworkElement.MarginProperty, slideAnim);

            await Task.Delay(330);

            _cardsPlayedThisTrick++;

            // Refresh again after the card is visually placed
            RefreshInfoPanels();

            await AdvanceTurnAsync();
        }

        /// <summary>
        /// Moves to the next turn.
        /// </summary>
        private async Task AdvanceTurnAsync()
        {
            // End of trick: for now just clear after 4 cards
            if (_cardsPlayedThisTrick >= 4)
            {
                await Task.Delay(700);

                // Find who won the trick
                Player winner = GameManager.Instance.GetTrickWinner();

                // Calculate trick points
                int points = GameManager.Instance.CalculateTrickPoints();

                // Add points to the winner
                winner.Score += points;

                AddLog(winner.Name + " won the trick.");
                AddLog(winner.Name + " received " + points + " point(s).");

                trickCards.Children.Clear();
                GameManager.Instance.ClearTrick();

                _cardsPlayedThisTrick = 0;
                _trickNumber++;
                _leadSuit = null;

                // Winner starts next trick
                _currentPlayerIndex = GameManager.Instance.players.IndexOf(winner);

                _isAnimatingCard = false;

                RefreshGameBoard();
                RefreshInfoPanels();

                if (_currentPlayerIndex != 0)
                    await PlaySimpleCpuTurnsAsync();

                return;
            }

            _currentPlayerIndex = (_currentPlayerIndex + 1) % 4;

            _isAnimatingCard = false;

            RefreshGameBoard();
            RefreshInfoPanels();

            if (_currentPlayerIndex != 0)
                await PlaySimpleCpuTurnsAsync();
        }
        private async Task PlaySimpleCpuTurnsAsync()
        {
            while (_currentPlayerIndex != 0 && _cardsPlayedThisTrick < 4)
            {
                await Task.Delay(500);

                var cpuPlayer = GameManager.Instance.players[_currentPlayerIndex];

                if (cpuPlayer.PlayerHand.CardsInHand == 0)
                    return;

                Card cpuCard = null;

                // Find the first valid card the CPU can play
                foreach (Card card in cpuPlayer.PlayerHand.Cards)
                {
                    if (GameManager.Instance.TryPlayCard(cpuPlayer, card))
                    {
                        cpuCard = card;
                        break;
                    }
                }

                if (cpuCard == null)
                {
                    return;
                }

                CardUI cpuVisual = new CardUI();
                BitmapImage face = cardFaceSprites["card" + cpuCard.Suit + cpuCard.Value];
                cpuVisual.BindData(cpuCard, face, false);

                await PlayCardToCenterAsync(cpuVisual, _currentPlayerIndex);
            }
        }
        private void OnClearTableClick(object sender, RoutedEventArgs e)
        {
            // Remove the 4 cards from the center trick area
            trickCards.Children.Clear();
        }

        private async void OnNewGameClick(object sender, RoutedEventArgs e)
        {
            LoadingOverlay.Visibility = Visibility.Visible;
            await Task.Delay(800);

            StartNewGame();

            LoadingOverlay.Visibility = Visibility.Collapsed;
        }
        private void RefreshInfoPanels()
        {
            RefreshCurrentPlayer();
            RefreshScoreboard();
            RefreshRoundInfo();
        }
        /// <summary>
        /// Updates the current player label.
        /// </summary>
        private void RefreshCurrentPlayer()
        {
            if (GameManager.Instance.players.Count == 0)
                return;

            txtCurrentPlayerName.Text = GameManager.Instance.players[_currentPlayerIndex].Name;
        }

        private void RefreshScoreboard()
        {
            if (GameManager.Instance.players.Count < 4)
                return;

            txtPlayer1Score.Text = $"{GameManager.Instance.players[0].Name}: {GameManager.Instance.players[0].Score}";
            txtPlayer2Score.Text = $"{GameManager.Instance.players[1].Name}: {GameManager.Instance.players[1].Score}";
            txtPlayer3Score.Text = $"{GameManager.Instance.players[2].Name}: {GameManager.Instance.players[2].Score}";
            txtPlayer4Score.Text = $"{GameManager.Instance.players[3].Name}: {GameManager.Instance.players[3].Score}";
        }

        private void RefreshRoundInfo()
        {
            if (_leadSuit == null)
            {
                txtLeadSuit.Text = "Lead Suit: None";
            }
            else
            {
                txtLeadSuit.Text = "Lead Suit: " + _leadSuit.ToString();
            }
            txtHeartsBroken.Text = "Hearts Broken: " +
                (GameManager.Instance.HeartsBroken ? "Yes" : "No");
            txtTrickCount.Text = "Trick #: " + _trickNumber;

            // Update card count labels
            txtSouthCount.Text = GameManager.Instance.players[0].Name + " Cards: " +
                                 GameManager.Instance.players[0].CardsInHand;

            txtWestCount.Text = "CPU West Cards: " + GameManager.Instance.players[1].CardsInHand;
            txtNorthCount.Text = "CPU North Cards: " + GameManager.Instance.players[2].CardsInHand;
            txtEastCount.Text = "CPU East Cards: " + GameManager.Instance.players[3].CardsInHand;
        }

        private void AddLog(string message)
        {
            lstGameLog.Items.Insert(0, $"{DateTime.Now:HH:mm:ss} - {message}");

            // Keep log from getting too long
            if (lstGameLog.Items.Count > 8)
            {
                lstGameLog.Items.RemoveAt(lstGameLog.Items.Count - 1);
            }
        }
        private string GetCardName(Card card)
        {
            string valueText;

            if (card.Value == 1)
            {
                valueText = "Ace";
            }
            else if (card.Value == 11)
            {
                valueText = "Jack";
            }
            else if (card.Value == 12)
            {
                valueText = "Queen";
            }
            else if (card.Value == 13)
            {
                valueText = "King";
            }
            else
            {
                valueText = card.Value.ToString();
            }

            return valueText + " of " + card.Suit;
        }

        private void OnHelpClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "How to Play\n\n" +
                "- Enter your name and start the game.\n" +
                "- You are the bottom player.\n" +
                "- Click a card to play your turn.\n" +
                "- You must follow the lead suit if possible.\n" +
                "- Hearts cannot be played until broken.\n\n" +

                "Features\n\n" +
                "- Game Log shows all actions.\n" +
                "- Scoreboard shows scores.\n" +
                "- X-Ray mode reveals all cards.\n" +
                "- Theme option changes card appearance.\n\n" +

                "Goal\n\n" +
                "Avoid collecting hearts and the Queen of Spades.\n" +
                "The player with the lowest score wins."
            );
        }

    }
}