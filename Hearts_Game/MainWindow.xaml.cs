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
        public static readonly string resourceDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName ?? "";

        public static Dictionary<string, BitmapImage> cardFaceSprites = [];

        public MainWindow()
        {
            InitializeComponent();

            //Loading Resources
            string cardDirectory = "/GameAssets/Images/Cards/";
            cardFaceSprites = GetCardResources(cardDirectory);
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
                    cardFaceSprites.Add(key, bmi);
                }
            }

            return imgs;
        }

        public Card NewCard(int value, CardSuit suit)
        {
            Card card = new Card(value, suit);
            card.Width = 100;
            card.Height = 180;
            card.Source = cardFaceSprites["card" + suit + value];
            return card;
        }

        private static BitmapImage LoadResources(string path)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(path, UriKind.Absolute);
            bmp.EndInit();
            return bmp;
        }
    }
}