using HS_Net;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using HS_Lib;
using hearthclone;

namespace hearthclone_server
{

    public class AsynchronousSocketListener
    {
        private static int connections = 0;
        private static Dictionary<HS_SocketDataWorker, ClientInfo> clients = new Dictionary<HS_SocketDataWorker, ClientInfo>();
        private static int NUM_PLAYERS = 3;

        private static Queue<HS_SocketDataWorker> legacy2PlayerQueue = new Queue<HS_SocketDataWorker>();

        private static bool DEBUG = true;

        static void Log(string msg)
        {
            if (DEBUG)
            {
                Console.WriteLine(msg);
            }
        }

        class ClientInfo
        {
            public string userid;
            public string symkey = null;
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
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Settings.Current.GameServerPort);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                Log("Server started");

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
                Log(e.ToString());
            }

            Log("\nPress ENTER to continue...");
            Console.Read();

        }

        public static void MessageReceived(HS_SocketDataWorker sdw, string message)
        {
            JsonDataMessage request = JsonDataMessage.Parse(message.Trim());
            Log(message);
            if(request.Data1 == "login")
            {
                clients[sdw].userid = request.Data2;
                //TODO make this async
                Log("API call");
                string stringResponse = NetUtil.PostSynchro(Settings.Current.InternalTokenLookupUrl,
                    new Dictionary<string, string> { { "code", Settings.Current.InternalApiAccessCode }, { "userid", request.Data2 } });
                Log("Response: " + stringResponse);
                ServerResponse response = ServerResponse.Parse(stringResponse);
                clients[sdw].symkey = response.Data1;
                Log("Sym token: " + response.Data1);
                sdw.Symkey = response.Data1;

                JsonDataMessage readyRequest = new JsonDataMessage("ready");
                Log("User joined, sending ready message: " + readyRequest.Json);
                sdw.Send(readyRequest.Json);
                legacy2PlayerQueue.Enqueue(sdw);
            }

            CheckGameStart();
        }

        public static void CheckGameStart()
        {
            if(legacy2PlayerQueue.Count == 2)
            {
                HS_SocketDataWorker p1 = legacy2PlayerQueue.Dequeue();
                HS_SocketDataWorker p2 = legacy2PlayerQueue.Dequeue();
                StartGame(p1, p2);
            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            Log("New client connected");
            connections++;

            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            //sockets.Add(new HS_SocketDataWorker(handler));
            HS_SocketDataWorker sdw = new HS_SocketDataWorker(handler);
            sdw.SetCallback(new HS_SocketDataWorker.HS_PlayerCommandCallback(MessageReceived));
            clients.Add(sdw, new ClientInfo());
        }
        
        public static void Broadcast(string msg)
        {
            foreach (HS_SocketDataWorker hsdw in clients.Keys){
                //hsdw.Send(msg);
            }
        }

        public static void StartGame(params HS_SocketDataWorker[] players)
        {
            Broadcast("Players connected. Setting up match");
            HS_GameInstance gi = new HS_GameInstance();
            foreach(HS_SocketDataWorker socket in players)
            {
                gi.AddPlayer(new HS_PlayerInstance(clients[socket].userid, new HS_Avatar(), new HS_TestDeck()), socket);
                clients.Remove(socket); //TODO is this right?
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
