using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hearts_Logic.Models.Objects;

namespace Hearts_Game.GameAssets
{
    public partial class CardUI : UserControl
    {
        public Card? CardData { get; private set; }
        private bool _isInteractable = false;

        // Visual "Pop-up" transform
        private TranslateTransform _moveTransform = new TranslateTransform();

        public CardUI()
        {
            InitializeComponent();
            this.RenderTransform = _moveTransform;

            // Accessibility Step 1: Make the control selectable via Keyboard
            this.Focusable = true;
            this.IsTabStop = true;

            // Accessibility Step 2: Show the yellow border when Tabbed onto
            this.GotFocus += (s, e) => SelectionBorder.BorderThickness = new Thickness(3);
            this.LostFocus += (s, e) => SelectionBorder.BorderThickness = new Thickness(0);

        }

        public void BindData(Card data, ImageSource source, bool canPlayerInteract)
        {
            this.CardData = data;
            this.CardImage.Source = source;
            this._isInteractable = canPlayerInteract;
            this.ToolTip = $"{data.Value} of {data.Suit}";
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (!_isInteractable) return;
            // Pop the card up by 20 pixels
            _moveTransform.Y = -20;
            SelectionBorder.BorderThickness = new Thickness(2);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _moveTransform.Y = 0;
            SelectionBorder.BorderThickness = new Thickness(0);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isInteractable) return;

            // 1. Get a reference to your main window
            if (Application.Current.MainWindow is MainWindow mainWin)
            {
                // 2. Call the move logic we just wrote (using player index 0 for Human)
                mainWin.ShowPlayedCard(this, 0);

                // 3. Optional: Logic hook for Task 15 (Points/Rules) later
                // GameManager.Instance.ProcessTurn(this.CardData);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Task 39: If User hits Enter or Space while the card is Tabbed/Focused
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                if (_isInteractable) // If it's the Human player's hand
                {
                    if (Application.Current.MainWindow is MainWindow mainWin)
                    {
                        // Logic: Jump to the center just like a mouse click
                        mainWin.ShowPlayedCard(this, 0);
                    }
                }
            }
        }

    }
}