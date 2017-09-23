using System;
using System.Net.Sockets;
using System.Text;

namespace HS_Net
{
    public class HS_SocketInputWorker
    {

        private HS_MessageCallback callback;

        private void ReadCallback(IAsyncResult ar)
        {
            // Get the socket input buffer from the callback result
            HS_SocketInputBuffer state = (HS_SocketInputBuffer)ar.AsyncState;
            Socket socket = state.TargetSocket;

            // Read data from the client   
            int bytesRead = socket.EndReceive(ar);
            if (bytesRead > 0)
            { 
                state.AppendData(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                // Pop a message if one is available 
                if (state.HasMessage)
                {
                    string message = state.PopMessage();
                    Console.WriteLine("Message : {0}", message);
                    callback(message);
                }

                //Keep reading
                socket.BeginReceive(state.Buffer, 0, HS_SocketInputBuffer.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
            }
        }

        public delegate void HS_MessageCallback(string message);

        public HS_SocketInputWorker(Socket socket, HS_MessageCallback callback)
        {
            this.callback = callback;

            //Start the reader listening on the socket
            HS_SocketInputBuffer state = new HS_SocketInputBuffer(socket);
            socket.BeginReceive(state.Buffer, 0, HS_SocketInputBuffer.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

    }
}
