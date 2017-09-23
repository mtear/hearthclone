using HS_Net;
using HS_Lib;
using System.Collections.Generic;

namespace hearthclone_server
{
    class HS_GameInstance
    {
        private HS_Game game;
        private Dictionary<HS_PlayerInstance, HS_SocketDataWorker> sockets;
        private Dictionary<HS_SocketDataWorker, HS_PlayerInstance> playerInstances;
        private List<HS_PlayerInstance> players;

        public HS_GameInstance()
        {
            sockets = new Dictionary<HS_PlayerInstance, HS_SocketDataWorker>();
            playerInstances = new Dictionary<HS_SocketDataWorker, HS_PlayerInstance>();

            players = new List<HS_PlayerInstance>();
            game = new HS_Game(new HS_Battlefield(), players);
        }

        public void AddPlayer(HS_PlayerInstance player, HS_SocketDataWorker sdw)
        {
            sockets.Add(player, sdw);
            playerInstances.Add(sdw, player);
            players.Add(player);
            sdw.SetCallback(new HS_SocketDataWorker.HS_PlayerCommandCallback(PlayerCommandReceived));
        }

        public void StartGame()
        {
            game.StartGame();
        }

        public void PlayerCommandReceived(HS_SocketDataWorker sdw, string message)
        {
            HS_PlayerInstance callingPlayer = playerInstances[sdw];

            string response = "";
            foreach (HS_PlayerInstance player in game.CurrentPlayers)
            {
                response += player.Name + " " + (int)player.Life + "HP " + player.Mana + "/" +
                    player.Crystals + " (" + player.Hand.Count + "):\n";

                foreach (HS_CardInstance card in player.Hand.Cards)
                {
                    if (callingPlayer == player)
                    {
                        response += card.Name + " - " + HS_CardTypeUtil.GetCode(card.Stats.Type) + " " + card.Stats.Cost + "\n";
                    }
                    else response += "*****\n";
                }
                response += "\n";
            }
            sdw.Send(response+"<EOF>");
        }

    }
}
