using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using HS_Net;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Collections.Specialized;
using hearthclone_loginserver;
using System.Linq;
using hearthclone;

namespace hearthclone_client
{


    // State object for receiving data from remote device.  
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousClient
    {
        static string public_key;
        static string private_key;

        public static String name = "";

        static string skey = "";

        // The port number for the remote device.  
        private const int port;

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        // The response from the remote device.  
        private static String response = String.Empty;

        private static void StartClient()
        {
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // The name of the   
                // remote device is "host.contoso.com".  
                IPHostEntry ipHostInfo = Dns.Resolve(Settings.Current.ServerUrl);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                Receive(client);

                String command = "";
                while (command != "disconnect")
                {
                    command = Console.ReadLine();
                    if(command == "clear")
                    {
                        Console.Clear(); continue;
                    }else if(command == "status" || command == "s" || command == "")
                    {
                        Console.Clear();
                        HS_Request r = new HS_Request(name, "hands");
                        HS_Request r2 = new HS_Request(name, "fields");
                        //HS_Request r3 = new HS_Request(name, "whoturn");
                        //Send(client, r3.Json);
                        Send(client, r.Json);
                        Send(client, r2.Json);
                        continue;
                    }
                    HS_Request request = new HS_Request(name, command);
                    // Send test data to the remote device.  
                    Send(client, request.Json);
                    //sendDone.WaitOne();

                    // Receive the response from the remote device.  
                    //receiveDone.WaitOne();

                }

                // Release the socket.  
                Console.WriteLine("Shutting down connection...");
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                //HS_Request r = new HS_Request("mtear", "NAMESET");
                //Send(client, r.Json);

                string symkey = GenerateSymKey();
                skey = symkey;
                LoginRequest lr = new LoginRequest("mtear", "ashton", symkey);
                string e = RSAHandler.Encrypt(public_key, lr.Json);
                Console.WriteLine("Sending data");
                Send(client, e);

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            //Console.WriteLine("Being receiving data");
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            Console.WriteLine("Receive callback");
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    //Console.WriteLine("Recieve callback! - grabbing more data...");

                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));

                    string content = state.sb.ToString();
                    Console.WriteLine(content);
                    if (content.Contains(HS_SocketInputBuffer.EOF))
                    {
                        //Response receieved
                        content = content.Replace(HS_SocketInputBuffer.EOF, ""); //MAKE SURE THIS WORKS MULTIPLE MESSAGES
                        Console.WriteLine(content);
                        string MSG = TDESHandler.Decrypt(skey, content);
                        Console.WriteLine(MSG);
                        //Listen again
                        Receive(client);
                    }
                    else
                    {
                        // Get the rest of the data.  
                        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(ReceiveCallback), state);
                    }
                }
                /*else
                {
                    Console.WriteLine("Recieve callback! - data transmission done");

                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                        Console.WriteLine("Recieved: " + response);
                        Receive(client);
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }*/
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Socket client, String data)
        {
            data += HS_SocketInputBuffer.EOF;

            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }



        public static int Main(String[] args)
        {
            port = Settings.Current.LoginServerPort;

            //while (AsynchronousClient.name.Trim() == "")
            //{
            //    Console.WriteLine("Enter your name: ");
            //    name = Console.ReadLine();
            //}

            //string[] keys = RSAHandler.GenerateKeyPair(2048);
            //File.WriteAllText("pubkey.txt", keys[0]);
            //File.WriteAllText("privkey.txt", keys[1]);

            /////// THE RSA
            /*
            String k = RSAHandler.Encrypt(public_key, "I wanna jump on a pogo stick.");
            String ue = RSAHandler.Decrypt(private_key, k);
            Console.WriteLine("Local decrpyt: " + ue);

            string URI = "http://www.polybellum.com/hearthclone/encrypttest.php";
            string myParameters = "content=" + k;

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = wc.UploadString(URI, myParameters);
                Console.WriteLine("Response: " + HtmlResult);
            }*/

            ////// TDES

            /*String key = TDESHandler.GenerateKey();
            Console.WriteLine("Key: " + key);
            string cypher = Convert.ToBase64String(TDESHandler.Encrypt(key, "I like turtles they are cool"));
            Console.WriteLine("Cypher: " + cypher);
            string text = TDESHandler.Decrypt(key, cypher);
            Console.WriteLine("Text: " + text);*/

            //TDES_UnitTest();
            //RSA_UnitTest();

            /*String[] c = RSAHandler.GenerateKeyPair(1024);
            File.WriteAllText("pub.key", c[0]);
            File.WriteAllText("priv.key", c[1]);*/

            public_key = File.ReadAllText("pub.key");
            StartClient();
            Console.Read();
            return 0;
        }

        public static void TDES_UnitTest()
        {
            for (int i = 0; i < 1000000; i++)
            {
                String key = TDESHandler.GenerateKey();
                String data = GenerateRandomString(2048);
                string cypher = Convert.ToBase64String(TDESHandler.Encrypt(key, data));
                string text = TDESHandler.Decrypt(key, cypher);
                if(data != text)
                {
                    Console.WriteLine("\n\nFailed!");
                    Console.WriteLine("Data: " + data);
                    File.WriteAllText("err_dump.txt", data);
                    return;
                }
                else
                {
                    Console.WriteLine(i + " passed! " + data.Substring(0, (data.Length < 10) ? data.Length : 10));
                }
            }
            Console.WriteLine("Test passed!");
        }

        public static void RSA_UnitTest()
        {
            for (int i = 0; i < 1000000; i++)
            {
                String[] keys = RSAHandler.GenerateKeyPair(1024);
                String publickey = keys[0];
                String privatekey = keys[1];
                String data = GenerateRandomString(78);
                string cypher = RSAHandler.Encrypt(publickey, data);
                string text = RSAHandler.Decrypt(privatekey, cypher);
                if (data != text)
                {
                    Console.WriteLine("\n\nFailed!");
                    Console.WriteLine("Data: " + data);
                    Console.WriteLine("Text: " + text);
                    for(int a = 0; a < data.Length; a++)
                    {
                        if(data[a] != text[a])
                        {
                            int x = data[a];
                            int y = text[a];
                            int z = 0;
                        }
                    }
                    File.WriteAllText("err_dump.txt", data);
                    return;
                }
                else
                {
                    Console.WriteLine(i + " passed! " + data.Substring(0, (data.Length < 10) ? data.Length : 10));
                }
            }
            Console.WriteLine("Test passed!");
        }

        public static string GenerateSymKey()
        {
            Random r = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 32)
              .Select(s => s[r.Next(s.Length)]).ToArray());
        }

        public static string GenerateRandomString(int maxlen)
        {
            Random r = new Random();
            StringBuilder sb = new StringBuilder();
            int length = r.Next(maxlen);
            for(int i = 0; i < length; i++)
            {
                char c = (char)(r.Next(150)+1);
                if (c == '\a') c = 'a';
                sb.Append(c);
            }
            return sb.ToString();
        }

    }

}
