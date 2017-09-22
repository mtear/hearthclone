using System;
using System.Net.Sockets;
using System.Text;

namespace hearthclone_net
{
    public class HS_SocketInputWorker
    {
        private static void ReadCallback(IAsyncResult ar)
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
                }

                //Keep reading
                socket.BeginReceive(state.Buffer, 0, HS_SocketInputBuffer.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
            }
        }

        public HS_SocketInputWorker(Socket socket)
        {
            //Start the reader listening on the socket
            HS_SocketInputBuffer state = new HS_SocketInputBuffer(socket);
            socket.BeginReceive(state.Buffer, 0, HS_SocketInputBuffer.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

    }
}
