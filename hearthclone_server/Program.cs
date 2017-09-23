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
        private static List<HS_SocketDataWorker> sockets = new List<HS_SocketDataWorker>();

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
                    if (connections < 2)
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

        public static void AcceptCallback(IAsyncResult ar)
        {
            Console.WriteLine("New client connected");
            connections++;

            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            sockets.Add(new HS_SocketDataWorker(handler));

            if(connections == 1)
            {
                Broadcast("You are the first to join the room. Please wait");
            }
            else if(connections == 2)
            {
                StartGame();
            }
        }

        public static void Broadcast(string msg)
        {
            foreach (HS_SocketDataWorker hsdw in sockets){
                hsdw.Send(msg+"<EOF>");
            }
        }

        public static void MessageCallback(HS_SocketDataWorker socket, string message)
        {

        }

        public static void StartGame()
        {
            Broadcast("Players connected. Setting up match");
            Broadcast("Loading decks...");
            HS_PlayerInstance p1 = new HS_PlayerInstance("nic", new HS_Avatar(), new HS_TestDeck());
            HS_PlayerInstance p2 = new HS_PlayerInstance("mike", new HS_Avatar(), new HS_TestDeck());
            HS_GameInstance gi = new HS_GameInstance();
            gi.AddPlayer(p1, sockets[0]);
            gi.AddPlayer(p2, sockets[1]);
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
