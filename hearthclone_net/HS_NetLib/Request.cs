using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace hearthclone_net
{
    [DataContract]
    public class HS_Request
    {
        [DataMember]
        string name;
        public string Name
        {
            get { return name; }
        }

        [DataMember]
        string command;
        public string Command
        {
            get { return command; }
        }

        public string Json
        {
            get
            {
                MemoryStream stream = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(HS_Request));
                ser.WriteObject(stream, this);
                byte[] json = stream.ToArray();
                stream.Close();
                return Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }

        public HS_Request(string name, string command)
        {
            this.name = name;
            this.command = command;
        }

        public HS_Request Parse(string json)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(HS_Request));
            HS_Request request = ser.ReadObject(stream) as HS_Request;
            stream.Close();
            return request;
        }

    }
}
