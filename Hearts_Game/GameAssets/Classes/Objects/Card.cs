using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts_Game.GameAssets.Classes.Objects
{
    internal class Card
    {
        public int Value { get; set; } = 0;
        public bool IsFaceUp { get; set; } = false;

        public Card() { }

        public Card(int value, bool faceUp = false)
        {
            Value = value;
            IsFaceUp = faceUp;
        }

        public void Flip()
        {
            IsFaceUp = !IsFaceUp;
        }
    }
}
