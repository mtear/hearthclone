using HS_Lib;
using System.Collections.Generic;

namespace hearthclone
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello!");
            System.Console.WriteLine("Loading decks...");
            HS_PlayerInstance p1 = new HS_PlayerInstance("nic", new HS_Avatar(), new HS_TestDeck());
            HS_PlayerInstance p2 = new HS_PlayerInstance("mike", new HS_Avatar(), new HS_TestDeck());
            HS_Game g = new HS_Game(new HS_Battlefield(), new List<HS_PlayerInstance>(new HS_PlayerInstance[]{ p1, p2 }));
            g.StartGame();

            System.Console.WriteLine("Game started");
            string query = "";
            while (query != "exit")
            {
                System.Console.WriteLine(g.CurrentPlayer.Name + "'s turn");
                System.Console.Write("Command: ");
                query = System.Console.ReadLine().Trim();
                int spaceindex = query.IndexOf(' ');
                string q = (spaceindex > 0) ? query.Substring(0, spaceindex) : query;
                string[] cs = query.Split(' ');
                switch (q)
                {
                    case "hands": ConsoleUtil.PrintHands(g); break;
                    case "fields": ConsoleUtil.PrintFields(g); break;
                    //explain card 
                    case "play": ConsoleUtil.Play(g, g.CurrentPlayer, cs); break;
                    case "attack": ConsoleUtil.Attack(g, g.CurrentPlayer, cs); break;
                    case "help": break;
                    case "clear": System.Console.Clear(); break;
                    case "end": g.EndTurn(); break;
                    default: System.Console.WriteLine("Unknown command. Use \"help\" to see available commands."); break;
                }
            }
        }
    }
}
