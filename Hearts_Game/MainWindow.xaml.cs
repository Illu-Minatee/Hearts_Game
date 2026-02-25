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
        public MainWindow()
        {
            InitializeComponent();


            string targetDirectory = "/GameAssets/Images/Cards/";

            foreach (var suit in Enum.GetValues(typeof(CardSuit)))
            {
                for (int i = 1; i < 14; i++)
                {
                    Debug.WriteLine(resourceDir + targetDirectory + "card" + suit + i + ".png");
                }
            }

            //Testing Loading Resources and Making a card
           

            //BitmapImage testBM = LoadResources(final);
            //Card test = new Card();
            //test.Width = 100;
            //test.Height = 180;
            //test.Source = testBM;

            //rootGrid.Children.Add(test);

        }


        //Testing Area: This Stuff Cannot Stay Inside this class and will need to have to be abstracted.
        public static readonly string resourceDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName ?? "";

        public Dictionary<string, BitmapImage> cardFaceSprites = [];

        private BitmapImage LoadResources(string path)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            //bmp.CacheOption = BitmapCacheOption.OnLoad; // prevents file locking
            bmp.UriSource = new Uri(path, UriKind.Absolute);
            bmp.EndInit();
            //bmp.Freeze(); // makes it cross-thread safe
            return bmp;
        }
    }
}