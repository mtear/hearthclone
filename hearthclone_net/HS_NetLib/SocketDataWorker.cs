using System;
using System.Net.Sockets;
using System.Text;

namespace HS_Net
{
    public class HS_SocketDataWorker
    {

        HS_SocketInputWorker inputWorker;
        Socket socket;

        public delegate void HS_PlayerCommandCallback(HS_SocketDataWorker sdw, string message);

        private HS_PlayerCommandCallback callback = null;

        public HS_SocketDataWorker(Socket socket)
        {
            this.socket = socket;
            inputWorker = new HS_SocketInputWorker(socket, new HS_SocketInputWorker.HS_MessageCallback(MessageReceived));
        }

        public void Send(string data)
        {
            SocketSend(socket, data);
        }

        private static void SocketSend(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

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
            callback?.Invoke(this, message);
        }

    }
}
