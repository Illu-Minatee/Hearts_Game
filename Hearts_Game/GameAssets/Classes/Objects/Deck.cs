
namespace Hearts_Game.GameAssets.Classes.Objects
{
    internal class Deck
    {

        private List<Card> cards = [];
        private static readonly Random Random = new();

        public Deck()
        {
            Intialize();
        }

        private void Intialize()
        {
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                for (int i = 0; i < 13; i++)
                {
                    Card c = MainWindow.NewCard(i+1, suit);
                    cards.Add(c);
                }
            }
        }

        //Fisher-Yates Shuffle Method.
        // Generate a random index k between 0 and n (inclusive)
        // Swap the elements at indices n and k
        public void Shuffle(int count)
        {
            int iter = cards.Count;
            for (int i = 0; i < count; i++)
            {        
                while (iter > 1)
                {
                    iter--;
                    int k = Random.Next(iter + 1);

                    //Short-Hand Method of Swapping Indices
                    (cards[k], cards[iter]) = (cards[iter], cards[k]);
                }

                iter = cards.Count;
            }
        }

        public Card DrawCard()
        {
            Card c = cards[0];
            cards.RemoveAt(0);
            return c;
        }
    }
}
