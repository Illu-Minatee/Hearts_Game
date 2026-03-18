using Hearts_Game.GameAssets.Classes.Managers;
using Hearts_Game.GameAssets.Classes.Objects;
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
        //Resources and Directory. Needs to be abstracted to a seperate class handling this stuff.
        public static readonly string resourceDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName ?? "";
        public static Dictionary<string, BitmapImage> cardFaceSprites = [];
        public static BitmapImage? cardBack;
    
        public MainWindow()
        {
            InitializeComponent();

            //Loading Resources
            string cardDirectory = "/GameAssets/Images/Cards/";
            cardFaceSprites = GetCardResources(cardDirectory);
            cardBack = LoadResources(resourceDir + cardDirectory + "cardBack_red3.png");

            //Testing Setup and Dealing cards.
            GameManager.Instance.SetZones([zoneOne, zoneTwo, zoneThree, zoneFour]);
            GameManager.Instance.SetupDeck();
            GameManager.Instance.DealCards();

        }

        //This should be moved but has references that will need to be address. Move with care and attention.
        //Face and Back are assumed to be instatiated. This could use error handling.
        static public Card NewCard(int value, CardSuit suit)
        {
            Card card = new(value, suit, cardBack, cardFaceSprites["card" + suit + value]);
            card.Width = 102;
            card.Height = 152;
            card.Stretch = Stretch.Fill;
            return card;
        }

        //Attempts to load images from internal directory and returns a dictonary of bitmap image.
        public static Dictionary<string, BitmapImage> GetCardResources(string dir)
        {
            Dictionary<string, BitmapImage> imgs = [];

            foreach (var suit in Enum.GetValues(typeof(CardSuit)))
            {
                for (int i = 1; i < 14; i++)
                {
                    string key = "card" + suit + i;
                    string path = resourceDir + dir + key + ".png";

                    if (!File.Exists(path))
                    {
                        Debug.WriteLine($"Unable to load card images: Path Not Found.\n{path}");
                        return imgs;
                    }

                    BitmapImage bmi = LoadResources(path);
                    imgs.Add(key, bmi);
                }
            }
            return imgs;
        }

        //Returns a single bitmap Image to be used as a source for a sprite.
        private static BitmapImage LoadResources(string path)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(path, UriKind.Absolute);
            bmp.EndInit();
            return bmp;
        }

        private void OnXRayClick(object sender, RoutedEventArgs e)
        {
            GameManager.Instance.XRayVison();
        }
    }
}