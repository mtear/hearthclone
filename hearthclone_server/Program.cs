using HS_Net;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using HS_Lib;

namespace hearthclone_server
{

    public class AsynchronousSocketListener
    {
        private static int connections = 0;
        //private static List<HS_SocketDataWorker> sockets = new List<HS_SocketDataWorker>();
        private static Dictionary<HS_SocketDataWorker, PlayerInfo> players = new Dictionary<HS_SocketDataWorker, PlayerInfo>();
        private static int NUM_PLAYERS = 3;

        class PlayerInfo
        {
            public string Name;
            public bool Set
            {
                get { return Name != "UNKNOWN"; }
            }
            public PlayerInfo(string name)
            {
                this.Name = name;
            }
        }

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public AsynchronousSocketListener()
        {
        }

        public static void StartListening()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 1121);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                Console.WriteLine("Server started");

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    if (connections < NUM_PLAYERS)
                    {
                        listener.BeginAccept(
                            new AsyncCallback(AcceptCallback),
                            listener);
                    }
                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static void MessageReceived(HS_SocketDataWorker sdw, string message)
        {
            HS_Request request = HS_Request.Parse(message.Trim());
            Console.WriteLine(message);
            if(request.Command == "NAMESET")
            {
                Console.WriteLine("CHANGING NAME");
                players[sdw].Name = request.Name;
            }

            CheckGameStart();
        }

        public static void CheckGameStart()
        {
            if(connections == NUM_PLAYERS)
            {
                bool good = true;
                foreach(PlayerInfo pi in players.Values)
                {
                    if (!pi.Set) { good = false; break; }
                }

                if (good) StartGame();
            }
        }

            public static void AcceptCallback(IAsyncResult ar)
        {
            Console.WriteLine("New client connected");
            connections++;

            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            //sockets.Add(new HS_SocketDataWorker(handler));
            HS_SocketDataWorker sdw = new HS_SocketDataWorker(handler);
            sdw.SetCallback(new HS_SocketDataWorker.HS_PlayerCommandCallback(MessageReceived));
            players.Add(sdw, new PlayerInfo("UNKNOWN"));

            if(connections == 1)
            {
                Broadcast("You are the first to join the room. Please wait");
            }
        }

        public static void Broadcast(string msg)
        {
            foreach (HS_SocketDataWorker hsdw in players.Keys){
                hsdw.Send(msg+"<EOF>");
            }
        }

        public static void StartGame()
        {
            Broadcast("Players connected. Setting up match");
            //HS_PlayerInstance p1 = new HS_PlayerInstance("nic", new HS_Avatar(), new HS_TestDeck());
            //HS_PlayerInstance p2 = new HS_PlayerInstance("mike", new HS_Avatar(), new HS_TestDeck());
            //HS_PlayerInstance p3 = new HS_PlayerInstance("scott", new HS_Avatar(), new HS_TestDeck());
            HS_GameInstance gi = new HS_GameInstance();
            //gi.AddPlayer(p1, sockets[0]);
            //gi.AddPlayer(p2, sockets[1]);
            //gi.AddPlayer(p3, sockets[2]);
            foreach(HS_SocketDataWorker socket in players.Keys)
            {
                gi.AddPlayer(new HS_PlayerInstance(players[socket].Name, new HS_Avatar(), new HS_TestDeck()), socket);
            }
            Broadcast("Starting game...");
            gi.StartGame();
        }

        public static int Main(String[] args)
        {
            StartListening();
            return 0;
        }
    }

}
