using HS_Net;
using HS_Lib;
using System.Collections.Generic;
using System;

namespace hearthclone_server
{
    class HS_GameInstance
    {
        private HS_Game game;
        private Dictionary<HS_PlayerInstance, HS_SocketDataWorker> sockets;
        private Dictionary<HS_SocketDataWorker, HS_PlayerInstance> playerInstances;
        private List<HS_PlayerInstance> players;
        private Dictionary<HS_PlayerInstance, string> names;
        private bool gameon = true;

        private int ropetimer;
        private int MAX_ROPE = 60000;

        public HS_GameInstance()
        {
            sockets = new Dictionary<HS_PlayerInstance, HS_SocketDataWorker>();
            playerInstances = new Dictionary<HS_SocketDataWorker, HS_PlayerInstance>();

            players = new List<HS_PlayerInstance>();
            names = new Dictionary<HS_PlayerInstance, string>();

        }

        public void AddPlayer(HS_PlayerInstance player, HS_SocketDataWorker sdw)
        {
            sockets.Add(player, sdw);
            playerInstances.Add(sdw, player);
            players.Add(player);
            names.Add(player, "Player");
            sdw.SetCallback(new HS_SocketDataWorker.HS_PlayerCommandCallback(PlayerCommandReceived));
        }

        public void StartGame()
        {
            game = new HS_Game(new HS_Battlefield(), players);
            game.StartGame();
            Broadcast(game.CurrentPlayer.Name.ToUpper() + "'S TURN");

            ropetimer = MAX_ROPE;
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            ropetimer -= 1000;
            if(ropetimer <= 5000)
            {
                Broadcast(""+(ropetimer / 1000));
            }
            if(ropetimer <= 0)
            {
                Broadcast("TIME OVER");
                EndTurn(game.CurrentPlayer);
            }
        }

        public void PlayerCommandReceived(HS_SocketDataWorker sdw, string message)
        {
            if (!gameon) return;
            HS_PlayerInstance callingPlayer = playerInstances[sdw];

            JsonDataMessage request = JsonDataMessage.Parse(message.Trim());
            string query = request.Data1;
            if(query == "login")
            {
                names[callingPlayer] = request.Data2;
            }
            int spaceindex = query.IndexOf(' ');
            string q = (spaceindex > 0) ? query.Substring(0, spaceindex) : query;
            string[] cs = query.Split(' ');
            string response = "";
            switch (q)
            {
                case "hands": response = PrintHands(callingPlayer); break;
                case "fields": response = PrintFields(); break;
                //explain card 
                case "play": response = Play(callingPlayer, cs); break;
                case "attack": response = Attack(callingPlayer, cs); break;
                case "a": response = Attack(callingPlayer, cs); break;
                case "allface": response = AllFace(callingPlayer, cs); break;
                //case "help": break;
                //case "clear": System.Console.Clear(); break;
                case "end": response = EndTurn(callingPlayer); break;
                case "hero": response = Hero(callingPlayer); break;
                case "whoturn": response = WhoTurn(); break;
                default: response = "Unknown command"; break;
            }
            sdw.Send(response);
            CheckDeadPlayers();
        }

        string WhoTurn()
        {
            return "*** " + game.CurrentPlayer.Name + "'s TURN ***\n";
        }

        string AllFace(HS_PlayerInstance player, string[] args)
        {
            if (args.Length != 2) return "Invalid arguments";
            string response = "";
            byte i = 0;
            foreach(HS_CreatureInstance ci in game.Battlefield.GetField(player))
            {
                response += Attack(player, new string[] { "a", args[1], i.ToString(), "-1" }) + "\n";
                i++;
            }
            return response;
        }

        string Hero(HS_PlayerInstance player)
        {
            if (player != game.CurrentPlayer) return "It's not your turn";
            if (player.Mana < 2) return "Not enough mana";
            if (player.UsedHero) return "You already used hero power this turn";

            Broadcast(player.Name + " created Soldier (Hero)");
            player.UsedHero = true;
            player.SpendMana(2);
            HS_CreatureInstance ci = new HS_CreatureInstance("Soldier", new HS_CreatureCard(1, 1,
                new List<HS_CreatureType>(new HS_CreatureType[] { HS_CreatureType.Beast }),
                "", 1, "a", HS_CardRarity.Common, HS_CardType.Creature, null));
            game.Battlefield.AddCreature(player, ci);
            return "";
        }

        string EndTurn(HS_PlayerInstance player)
        {
            if(player == game.CurrentPlayer)
            {
                ropetimer = MAX_ROPE;
                Broadcast("ENDING TURN");
                game.EndTurn();
                Broadcast(game.CurrentPlayer.Name.ToUpper() + "'S TURN");
                return "";
            }
            else
            {
                return "It's not your turn to end!";
            }
        }

        void Broadcast(string message)
        {
            foreach (HS_PlayerInstance player in players)
            {
                sockets[player].Send("****** " +message + " ******");
            }
        }

        string PrintFields()
        {
            string response = "";
            foreach (HS_PlayerInstance player in game.CurrentPlayers)
            {
                response += player.Name + "[" + players.IndexOf(player) + "] " + (int)player.Life + "HP (" + game.Battlefield.GetField(player).Count + "):\n";
                byte i = 0;
                foreach (HS_CreatureInstance bi in game.Battlefield.GetField(player))
                {
                    response += "[" + i + "] " + bi.Name + " " + bi.CreatureStats.Power + "/[" + bi.CreatureStats.Health + "/" + bi.BaseCreatureStats.Health + "] " + ((bi.CanAttack) ? "A" : "X") + "\n";
                    i++;
                }
                response += "\n";
            }
            return response;
        }

        string PrintHands(HS_PlayerInstance callingPlayer)
        {
            string response = "";
            foreach (HS_PlayerInstance player in game.CurrentPlayers)
            {
                response += player.Name + "[" + players.IndexOf(player) + "] " + (int)player.Life + "HP " + player.Mana + "/" +
                    player.Crystals + " (" + player.Hand.Count + "):\n";
                byte i = 0;
                foreach (HS_CardInstance card in player.Hand.Cards)
                {
                    if (callingPlayer == player)
                    {
                        response += "[" + i + "] " + card.Name + " - " + HS_CardTypeUtil.GetCode(card.Stats.Type) + " " + card.Stats.Cost + "\n";
                        i++;
                    }
                }
                response += "\n";
            }
            return response;
        }

        string Attack(HS_PlayerInstance callingPlayer, string[] args)
        {
            string response = "";
            try
            {
                HS_PlayerInstance targetPlayer = game.CurrentPlayers[Convert.ToInt32(args[1])];
                if (callingPlayer == targetPlayer) return "You can't attack yourself\n";
                sbyte fromIndex = Convert.ToSByte(args[2]);
                HS_CreatureInstance from = game.Battlefield.GetField(callingPlayer)[fromIndex];
                if (!from.CanAttack) return "This creature can't attack.";
                sbyte toIndex = Convert.ToSByte(args[3]);
                HS_CreatureInstance to = null;
                if(toIndex >= 0)
                {
                    to = game.Battlefield.GetField(targetPlayer)[toIndex];
                }
                game.Battlefield.Attack(callingPlayer, fromIndex, targetPlayer, toIndex);
                from.AlreadyAttacked = true;
                if(!targetPlayer.Alive && toIndex == -1)
                {
                    return "Can't attack a dead player.";
                }
                string attacktarget = targetPlayer.Name;
                if(toIndex >= 0)
                {
                    attacktarget += "'s " + to.Name;
                }
                Broadcast(callingPlayer.Name + " attacks " + attacktarget + " with " + from.Name);
            }
            catch (Exception e)
            {
                response = "Invalid attack command\n";
            }
            return response;
        }

        void CheckDeadPlayers()
        {
            byte alivecount = 0;
            HS_PlayerInstance alivePlayer = null;
            foreach(HS_PlayerInstance player in players)
            {
                if(player.Life <= 0 && player.Alive)
                {
                    player.Alive = false;
                    Broadcast(player.Name + " has been defeated.");
                }
                else if (player.Alive)
                {
                    alivePlayer = player;
                    alivecount++;
                }
            }
            if(alivecount == 1)
            {
                Broadcast(alivePlayer.Name + " wins the match!");
                Broadcast("GAME OVER");
                gameon = false;
            }
        }

        string Play(HS_PlayerInstance player, string[] args)
        {
            string response = "";
            if(player != game.CurrentPlayer)
            {
                response = "It's not your turn!\n";
            }
            else if (args.Length == 1)
            {
                response = "No card in hand index specified. Supply an index for which card in hand to play. Example: play 1 will play the card in hand at index 1.\n";
            }
            else
            {
                try
                {
                    int index = Convert.ToInt32(args[1]);
                    if (index >= player.Hand.Count)
                    {
                        response = "The index you supplied is outside the range of the cards in your hand.\n";
                    }
                    else
                    { // Play the card if able
                        if (CanPlayCard(game, player, index))
                        {
                            response = "Playing card " + player.Hand.Cards[index].Name + "\n";
                            Broadcast(player.Name + " played " + player.Hand.Cards[index].Name);
                            player.Play(game, index);
                        }
                        else
                        {
                            response = "Can't play " + player.Hand.Cards[index].Name +"\n";
                        }
                    }
                }
                catch (Exception e)
                { //Parsing errors
                    Console.WriteLine("There was an error parsing your play command.\n" + e);
                }
            }
            return response;
        }

        bool CanPlayCard(HS_Game g, HS_PlayerInstance player, int index)
        {
            try
            {
                HS_CardInstance card = player.Hand.Cards[index];
                //Check have enough mana
                if (player.Mana >= card.Stats.Cost)
                {
                    //TODO check full battlefield
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

    }
}
