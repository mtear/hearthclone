using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace HS_Net
{
    [DataContract]
    public class JsonDataMessage
    {
        [DataMember]
        string[] d;

        public string Data1
        {
            get { return (d == null || d.Length < 1) ? null : d[0]; }
        }

        public string Data2
        {
            get { return (d == null || d.Length < 2) ? null : d[1]; }
        }

        public string Data3
        {
            get { return (d == null || d.Length < 3) ? null : d[2]; }
        }

        public string Data4
        {
            get { return (d == null || d.Length < 4) ? null : d[3]; }
        }

        public string Json
        {
            get
            {
                MemoryStream stream = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(JsonDataMessage));
                ser.WriteObject(stream, this);
                byte[] json = stream.ToArray();
                stream.Close();
                return Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }

        public JsonDataMessage(params string[] data)
        {
            this.d = data;
        }

        public static JsonDataMessage Parse(string json)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(JsonDataMessage));
            JsonDataMessage request = ser.ReadObject(stream) as JsonDataMessage;
            stream.Close();
            return request;
        }

    }
}
