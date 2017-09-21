using HS_Lib;
using System;

namespace hearthclone
{
    static class ConsoleUtil
    {

        public static void PrintFields(HS_Game game)
        {
            foreach(HS_PlayerInstance player in game.CurrentPlayers)
            {
                Console.WriteLine(player.Name + " " + (int)player.Life + "HP (" + game.Battlefield.GetField(player).Count + "):");
                foreach (HS_CreatureInstance bi in game.Battlefield.GetField(player))
                {
                    Console.WriteLine(bi.Name + " " + bi.CreatureStats.Power + "/[" + bi.CreatureStats.Health + "/" + bi.BaseCreatureStats.Health + "]");
                }
                Console.WriteLine("");
            }
        }

        public static void PrintHands(HS_Game game)
        {
            foreach(HS_PlayerInstance player in game.CurrentPlayers)
            {
                Console.WriteLine(player.Name + " " + (int)player.Life + "HP " + player.Mana + "/" +
                    player.Crystals + " (" + player.Hand.Count + "):");
                PrintHand(player.Hand);
                Console.WriteLine("");
            }
        }

        public static void PrintHand(HS_Hand hand)
        {
            foreach(HS_CardInstance card in hand.Cards)
            {
                PrintCard(card);
            }
        }

        public static void PrintCard(HS_CardInstance card)
        {
            String msg = card.Name + " - " + HS_CardTypeUtil.GetCode(card.Stats.Type) + " " + card.Stats.Cost + "";
            //if (card.Stats.Type == CardType.Creature) msg += " [";
            Console.WriteLine(msg);
            if (card.Stats.Copy != "")
            {
                Console.WriteLine("*" + card.Stats.Copy);
            }
        }

        public static void Pause()
        {
            Console.ReadLine();
        }

        public static void Attack(HS_Game g, HS_PlayerInstance player, string[] args)
        {
            int playerIndex = Convert.ToInt32(args[1]);
            int fromIndex = Convert.ToInt32(args[2]);
            int toIndex = Convert.ToInt32(args[3]);
            g.Battlefield.Attack(g.CurrentPlayer, fromIndex, g.CurrentPlayers[playerIndex], toIndex);
        }

        public static void Play(HS_Game g, HS_PlayerInstance player, string[] args)
        {
            if(args.Length == 1)
            {
                Console.WriteLine("No card in hand index specified. Supply an index for which card in hand to play. Example: play 1 will play the card in hand at index 1.");
            }
            else
            {
                try
                {
                    int index = Convert.ToInt32(args[1]);
                    if(index >= player.Hand.Count)
                    {
                        Console.WriteLine("The index you supplied is outside the range of the cards in your hand.");
                    }
                    else
                    { // Play the card if able
                        if(CanPlayCard(g, player, index))
                        {
                            Console.WriteLine("Playing card " + player.Hand.Cards[index].Name);
                            player.Play(g, index);
                        }
                        else
                        {
                            Console.WriteLine("Can't play " + player.Hand.Cards[index].Name);
                        }
                    }
                }
                catch (Exception e)
                { //Parsing errors
                    Console.WriteLine("There was an error parsing your play command.\n" + e);
                }
            }
        }

        public static bool CanPlayCard(HS_Game g, HS_PlayerInstance player, int index)
        {
            try
            {
                HS_CardInstance card = player.Hand.Cards[index];
                //Check have enough mana
                if(player.Mana >= card.Stats.Cost)
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
