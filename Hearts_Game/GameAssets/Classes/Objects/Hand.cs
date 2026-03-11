using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Hearts_Game.GameAssets.Classes.Objects
{
    public class Hand : Grid
    {
        public int cardsInHand = 0;
        public List<Card> cards = [];

        public Hand() 
        {
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;
        }

        public void AddCard(Card card)
        {
            card.HorizontalAlignment = HorizontalAlignment.Left;
            Children.Add(card);
            cards.Add(card);
            cardsInHand++;
        }
    }
}
