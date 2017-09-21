using System;
using System.Collections.Generic;

namespace HS_Lib
{
    class HS_Game
    {

        private char turnindex = (char)0;
        private HS_Battlefield battlefield;
        public HS_Battlefield Battlefield
        {
            get { return battlefield; }
        }
        public List<HS_CreatureInstance> CurrentBattlefield
        {
            get { return battlefield.GetField(CurrentPlayer); }
        }
        private List<HS_PlayerInstance> players;
        public List<HS_PlayerInstance> CurrentPlayers
        {
            get { return players; }
        }
        public HS_PlayerInstance CurrentPlayer
        {
            get { return players[turnindex]; }
        }


        public HS_Game(HS_Battlefield battlefield, List<HS_PlayerInstance> players)
        {
            this.players = players;
            this.battlefield = battlefield;
            foreach(HS_PlayerInstance player in players)
            {
                this.battlefield.AddPlayer(player);
            }
        }

        public void DrawOpeningHands()
        {
            foreach(HS_PlayerInstance player in players)
            {
                player.Draw();
                player.Draw();
                player.Draw();
            }
        }

        public void EndTurn()
        {
            //Increment the turn index
            turnindex++;
            if(turnindex >= players.Count)
            {
                turnindex = (char)0;
            }
            //Start the turn of the next player
            StartTurn(players[turnindex]);
        }

        public void StartTurn(HS_PlayerInstance player)
        {
            player.AddManaCrystal();
            player.ResetMana();
            player.Draw();
        }

        public void StartGame()
        {
            turnindex = (char)new Random().Next(players.Count);
            DrawOpeningHands();
            StartTurn(players[turnindex]);
        }

    }
}
