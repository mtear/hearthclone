using hearthclone;
using HS_Net;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace hearthclone_loginserver
{
    class LoginServer
    {

        private static string private_key;
        static int port = Settings.Current.LoginServerPort;

        static void Main(string[] args)
        {
            //Get the private key
            try
            {
                private_key = File.ReadAllText("private.key");
            }
            catch
            {
                throw new System.Exception("Private key file not found! Please create private.key!");
            }

            //Start listening for connections
            StartListening();
        }

        public static ManualResetEvent allDone = new ManualResetEvent(false);

        static void StartListening()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

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
                        listener.BeginAccept(
                            new AsyncCallback(AcceptCallback),
                            listener);

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

        static void AcceptCallback(IAsyncResult ar)
        {
            Console.WriteLine("New client connected");

            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            HS_SocketDataWorker sdw = new HS_SocketDataWorker(handler);
            sdw.SetCallback(new HS_SocketDataWorker.HS_PlayerCommandCallback(MessageReceived));
        }

        static void MessageReceived(HS_SocketDataWorker sdw, string message)
        {
            //Decrypt
            string json = RSAHandler.Decrypt(private_key, message);
            LoginRequest request = LoginRequest.Parse(json);

            //Check login
            //TODO make this async
            string stringResponse = NetUtil.PostSynchro(Settings.Current.InternalLoginUrl,
                new System.Collections.Generic.Dictionary<string, string>()
                { { "code", Settings.Current.InternalApiAccessCode }, {"userid",request.Username },
                    { "password", request.Password }, {"key", request.Key } });
            ServerResponse response = ServerResponse.Parse(stringResponse);
            Console.WriteLine(stringResponse);

            if(response != null)
            {
                if(response.Result == "success")
                {
                    Console.WriteLine("success sending back");
                    string sec = Convert.ToBase64String(TDESHandler.Encrypt(request.Key, "turtle turtle"));
                    sdw.Send(sec);
                }else
                {

                }
            }
            
        }

    }
}
