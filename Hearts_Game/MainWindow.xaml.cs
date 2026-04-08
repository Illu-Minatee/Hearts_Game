using Hearts_Game.GameAssets;
using Hearts_Logic.Actors;
using Hearts_Logic.Managers;
using Hearts_Logic.Models.Objects;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Hearts_Game
{
    public partial class MainWindow : Window
    {
        // Directory handling for asset loading - points to the project's visual resources
        public static readonly string resourceDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName ?? "";
        public static Dictionary<string, BitmapImage> cardFaceSprites = new Dictionary<string, BitmapImage>();
        public static BitmapImage? cardBack;
        private bool _isXRayActive = false;

        public MainWindow()
        {
            InitializeComponent();

            // Load visual assets
            string cardDirectory = "/GameAssets/Images/Cards/";
            cardFaceSprites = GetCardResources(cardDirectory);
            cardBack = LoadResources(resourceDir + cardDirectory + "cardBack_red3.png");

            // Logic Setup
            GameManager.Instance.SetupDeck();
            GameManager.Instance.DealCards();

            // Visual Deal - This replaces the logic that was stripped from GameManager
            RefreshGameBoard();
        }

        /// <summary>
        /// This method bridges the Logic Card to a UI Image.
        /// It satisfies Task 17's requirement of separating Data from View.
        /// </summary>
        public static Image GetVisualCard(Card logicCard)
        {
            // Create a temporary WPF Image to represent the data object
            Image cardImage = new Image
            {
                Width = 102,
                Height = 152,
                Stretch = Stretch.Fill,
                // Assign the source using the suit/value data from the logicCard
                Source = cardFaceSprites["card" + logicCard.Suit + logicCard.Value]
            };

            return cardImage;
        }

        /// <summary>
        /// Visual Dealing Logic
        /// Connects the Logic Hand to the WPF Zones using CardUI Custom Controls
        /// </summary>
        private void RefreshGameBoard()
        {
            // Clear all magenta placeholders first
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

                    // IF HUMAN: Use the face sprite and make it interactable
                    // IF AI: Use the 'cardBack' image and disable interaction
                    BitmapImage source;

                    // If the player is the Human OR if we turned on the X-Ray cheat...
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

                    visualCard.BindData(logicCard, source, isHuman);
                    visualCard.IsTabStop = isHuman;

                    // --- POSITIONING ---
                    visualCard.HorizontalAlignment = HorizontalAlignment.Left;
                    visualCard.VerticalAlignment = VerticalAlignment.Top;

                    if (p == 0) // Human (Bottom)
                    {
                        visualCard.Margin = new Thickness(i * 30, 0, 0, 0);
                    }
                    else if (p == 1) // AI 1 (Left Side)
                    {
                        visualCard.RenderTransform = new RotateTransform(90); // Turn it sideways
                        visualCard.Margin = new Thickness(0, i * 20, 0, 0);
                    }
                    else if (p == 2) // AI 2 (Right Side)
                    {
                        visualCard.RenderTransform = new RotateTransform(-90); // Turn it other way
                        visualCard.Margin = new Thickness(0, i * 20, 0, 0);
                    }
                    else if (p == 3) // AI 3 (Top)
                    {
                        visualCard.Margin = new Thickness(i * 30, 0, 0, 0);
                    }

                    zones[p].Children.Add(visualCard);
                }
            }
        }

        /// <summary>
        /// Loads images from directory into memory for high-speed UI updates.
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
            bmp.CacheOption = BitmapCacheOption.OnLoad; // Optimization for high performance
            bmp.EndInit();
            return bmp;
        }

        private void OnXRayClick(object sender, RoutedEventArgs e)
        {
            // UI Layer toggle for card visibility
            GameManager.Instance.XRayVision();
            // (RefreshGameBoard will now check the Logic Manager's flag)
            RefreshGameBoard();
        }

        /// <summary>
        /// Task 38: UI Themes. 
        /// Updates the cardBack resource and refreshes the visual board.
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

                // Redraw the board with the new theme
                RefreshGameBoard();
            }
        }


        private void OnWindowKeyDown(object sender, KeyEventArgs e)
        {
            // Mandatory Requirement: Exit or Menu on Escape
            if (e.Key == Key.Escape)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to quit the game?", "Exit", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
            }
            // NEW PART: Handle playing cards via Keyboard (Task 39)
            // If user hits Enter/Space, and the 'focused' item is one of our CardUI controls
            else if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                if (FocusManager.GetFocusedElement(this) is GameAssets.CardUI visualCard)
                {
                    if (visualCard.CardData != null)
                    {
                        // Ask Rule Engine if legal
                        bool isMoveLegal = GameManager.Instance.TryPlayCard(GameManager.Instance.players[0], visualCard.CardData);

                        if (isMoveLegal)
                        {
                            // VISUAL FIX: You MUST reset these before moving (Same as MouseDown)
                            visualCard.ResetHighlight(); // We will add this small helper next
                            this.ShowPlayedCard(visualCard, 0);
                        }
                        else
                        {
                            MessageBox.Show("Illegal move! You cannot lead with a Heart until hearts are broken.", "Hearts Rule Enforcer");
                        }

                        // IMPORTANT: Tell Windows we handled this key so it stops processing
                        e.Handled = true;
                    }
                }
            }
        }

        private void OnQuitClick(object sender, RoutedEventArgs e)
        {
            // Reuse the same logic from the Esc key
            OnWindowKeyDown(this, new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, Key.Escape));
        }

        /// <summary>
        /// Moves a card visually to the center of the table.
        /// Part of Task 17 (Bridging Logic to View).
        /// </summary>
        public void ShowPlayedCard(CardUI visualCard, int playerIndex)
        {
            // 1. Remove the card from the player's side zone
            Panel? parent = visualCard.Parent as Panel;
            parent?.Children.Remove(visualCard);

            // 2. Clear rotation so the card sits flat in the center
            visualCard.RenderTransform = new TranslateTransform();

            // 3. Position the card based on which player played it
            // (Standard "North, South, East, West" cross layout)
            double x = 0, y = 0;
            if (playerIndex == 0) y = 40;   // South (User)
            if (playerIndex == 1) x = -50;  // West
            if (playerIndex == 2) x = 50;   // East
            if (playerIndex == 3) y = -40;  // North

            visualCard.Margin = new Thickness(x, y, 0, 0);
            visualCard.HorizontalAlignment = HorizontalAlignment.Center;
            visualCard.VerticalAlignment = VerticalAlignment.Center;

            // 4. Add to the center
            trickCards.Children.Add(visualCard);
        }

        private void OnClearTableClick(object sender, RoutedEventArgs e)
        {
            // 1. Logic Check: Don't do anything if no cards are on the table
            if (GameManager.Instance.CurrentTrick.Count == 0) return;

            // 2. Calculation: Tell the Brain to find the winner and add penalty points (Task 33)
            GameManager.Instance.CalculateTrickPoints();

            // 3. UI Cleanup: Clear the visual cards from the center zone
            trickCards.Children.Clear();

            // 4. Update the View: Redraw the board to reflect removed cards
            RefreshGameBoard();

            // Optional Feedback for the Demo:
            MessageBox.Show("Trick Collected. Penalty points have been assigned to the winner.", "Round Progress");
        }

        private void OnNewGameClick(object sender, RoutedEventArgs e)
        {
            // 1. Tell the Logic Library to clear everything
            foreach (var player in GameManager.Instance.players)
            {
                player.ClearHand();
            }

            // 2. Clear the center trick area
            trickCards.Children.Clear();

            // 3. Setup a brand new deck and shuffle
            GameManager.Instance.SetupDeck();

            // 4. Deal the 52 cards logically
            GameManager.Instance.DealCards();

            // 5. Redraw all 4 hands and labels on the screen
            RefreshGameBoard();

            MessageBox.Show("New Hand Dealt!", "Hearts Game");
        }

    }
}