using Hearts_Game.GameAssets.Classes.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_Game.GameAssets.Classes
{
    public class Player
    {
        //Score could be tracked elsewhere, but this works for now.
        public int Score { get; set; } = 0;

        private List<Card> cards = [];

        public Player() { }

        public void PlayCard(Card card)
        {

        }

        public void AddCard(Card card)
        {
            cards.Add(card);
        }

        public void ShowHand()
        {
            foreach (Card c in cards)
            {
                c.Flip();
            }
        }

        public int cardsInHand => cards.Count;
    }
}

