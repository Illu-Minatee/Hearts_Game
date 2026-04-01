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

        public MainWindow()
        {
            InitializeComponent();

            // STEP 1: Load visual assets from the local filesystem
            string cardDirectory = "/GameAssets/Images/Cards/";
            cardFaceSprites = GetCardResources(cardDirectory);
            cardBack = LoadResources(resourceDir + cardDirectory + "cardBack_red3.png");

            // STEP 2: Trigger Logic initialization from the Library
            // GameManager is now a public Singleton from Hearts_Logic.Managers
            GameManager.Instance.SetupDeck();
            GameManager.Instance.DealCards();

            // Temporary: Linking Logic to UI zones. 
            // Task 17 will eventually move this into a specialized View/Presenter class.
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
        /// Orchestrates the visual display of cards currently held in logic players' hands.
        /// </summary>
        private void RefreshGameBoard()
        {
            // Placeholder: This logic would iterate through GameManager.Instance.players
            // and add GetVisualCard(card) to the respective zoneOne, zoneTwo, etc.
            // Detailed implementation will follow as we rebuild the DealHand UI logic.
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
        }
    }
}