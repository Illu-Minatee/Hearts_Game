using Hearts_Logic.Managers;
using Hearts_Logic.Models.Objects;
using Hearts_Logic.Actors;
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
            // Reuse the same logic from the Esc key
            OnWindowKeyDown(this, new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, Key.Escape));
        }

    }
}