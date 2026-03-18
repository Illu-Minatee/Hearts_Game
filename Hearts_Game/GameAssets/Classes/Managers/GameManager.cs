using Hearts_Game.GameAssets.Classes.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Hearts_Game.GameAssets.Classes.Managers
{
    //Singleton Class. Creates a static reference to itself if one does not already exist.
    //Handles majority of game setup and menuing functions as well as contains most game properties.
    internal class GameManager
    {
        public static GameManager Instance { get; } = new GameManager();

        private List<Player> players = [];
        private Grid[] playerZones = [];
        private int currentPlayer = 0;
        private int dealerPosition = 0;

        private Deck? testDeck;
        private int shuffleCount = 3;
        private int cardsToDeal = 13;

        //Private constructor prevents class from being instatiated.
        private GameManager() 
        {
            //This is temporary and should not be intialized here.
            for (int i = 0; i < 4; i++) 
            {
                Player p = new Player();
                players.Add(p);
            }
        }

        //Simple helper function that allows set zones for cards to be dealt to.
        public void SetZones(Grid[] zones) { playerZones = zones; }

        public void SetupDeck()
        {
            testDeck = new Deck();
            testDeck.Shuffle(shuffleCount);
        }

        //Validate deck has cards to be dealt. Then deal cards to players. Fails if players have not been created.
        //Currently Tightly coupled to players. Turn order will have to be a seperate array of ints 0-3 to handle this atm.
        public void DealCards()
        {
            //Horizontal Layout
            DealHand(players[0],true, 0, 0, true);
            DealHand(players[3], false, 3, 0, true);

            //Vartical Layout
            DealHand(players[1],false, 1, 90, false);
            DealHand(players[2],false, 2, 90, false);

        }

        //This function currently handles dealing cards appropriatly based on argument. Rotation should be 90 or 270 if IsHorizontal. 
        //WILL NEED TO ADD CARDS TO PLAYERS HANDS. CURRENTLY JUST ADDING TO ZONES ON THE TABLE.
        private void DealHand(Player player, bool isHuman, int zone, float rotation, bool isHorizontal)
        {
            for (int i = 0; i < cardsToDeal; i++)
            {
                Card c = testDeck.DrawCard();
                c.Clickable = isHuman;
                c.Margin = GetCardMargin(i, isHorizontal);
                c.Rotate(rotation);
                if (!isHuman) c.Flip();
                player.AddCard(c);
                playerZones[zone].Children.Add(c);
            }
        }

        //This can be moved to a static tools class. 
        private static Thickness GetCardMargin(int count, bool isHoriztonal = false)
        {
            float value = count * 60 - 360;
            return isHoriztonal ? new Thickness(value, 0, 0, 0) : new Thickness(0, value, 0, 0);
        }


        //This will be used to add players to the game. May need seperate functions for eac Human & computer players.
        public void AddPlayer()
        {
            return;
        }

        //This should determine who will play first and then order pleyers accordingly in list.
        public void SwitchDealer()
        {
            dealerPosition++;
            if (dealerPosition > 3) { dealerPosition = 0; }
        }

        //This should increment through the list of players untill the last index of list. Calling function will handle what happens if at end of list.
        public void SwitchPlayer()
        {
            currentPlayer++;
            if (currentPlayer > 3) { currentPlayer = 0; }
        }

        //Cheat/Developer Stuff. Seperate at some point
        public void XRayVison()
        {
            for (int i = 1; i < 4; i++) 
            {
                players[i].ShowHand();
            }
        }
        
    }
}
