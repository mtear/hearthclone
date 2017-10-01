using System.Net.Sockets;
using System.Text;

namespace HS_Net
{
    public class HS_SocketInputBuffer
    {

        public readonly static int BufferSize = 1024;
        public readonly static string EOF = "\0\0\0\0\0";

        private Socket socket = null;
        public Socket TargetSocket
        {
            get { return socket; }
        }

        private byte[] buffer = new byte[BufferSize];
        public byte[] Buffer
        {
            get { return buffer; }
        }

        private StringBuilder builder = new StringBuilder();
        public bool HasMessage
        {
            get { return builder.ToString().Contains(EOF); }
        }

        public void AppendData(string data)
        {
            builder.Append(data);
        }

        public HS_SocketInputBuffer(Socket socket)
        {
            this.socket = socket;
        }

        public string PopMessage()
        {
            //Get a string of the builder
            string data = builder.ToString();
            //Get the EOF index
            int eof = data.IndexOf(EOF);
            //No message if no EOF
            if (eof == -1) return null;
            //Get the message
            string message = data.Substring(0, eof);
            //Get the leftover and append it to a clear buffer
            string leftover = data.Substring(eof + EOF.Length);
            builder.Clear();
            builder.Append(leftover);
            //Return the message
            return message;
        }

    }
}
