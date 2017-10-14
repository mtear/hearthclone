using System;
using System.Net.Sockets;
using System.Text;

namespace HS_Net
{
    public class HS_SocketDataWorker
    {

        HS_SocketInputWorker inputWorker;
        Socket socket;
        private string symkey = null;
        public String Symkey
        {
            get { return symkey; }
            set { symkey = value; }
        }

        public delegate void HS_PlayerCommandCallback(HS_SocketDataWorker sdw, string message);

        private HS_PlayerCommandCallback callback = null;

        public HS_SocketDataWorker(Socket socket)
        {
            this.socket = socket;
            inputWorker = new HS_SocketInputWorker(socket, new HS_SocketInputWorker.HS_MessageCallback(MessageReceived));
        }

        public void Send(string data)
        {
            SocketSend(socket, data, symkey);
        }

        private static void SocketSend(Socket handler, String data, string symkey)
        {
            //Encrypt
            if(symkey != null)
            {
                data = Convert.ToBase64String(TDESHandler.Encrypt(symkey, data)) + HS_SocketInputBuffer.EOF;
            }

            Console.WriteLine(data);

            //Add message ending
            data += HS_SocketInputBuffer.EOF;

            // Convert the string data to byte data using UTF8 encoding.  
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void SetCallback(HS_PlayerCommandCallback callback)
        {
            this.callback = callback;
        }

        public void MessageReceived(string message)
        {
			if (callback != null) {
				callback.Invoke (this, message);
			}
        }

    }
}
