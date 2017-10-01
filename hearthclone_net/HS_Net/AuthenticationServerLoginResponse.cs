using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace HS_Net
{
    [DataContract]
    class ServerResponse
    {
        [DataMember]
        string result = null;
        public string Result
        {
            get { return result; }
        }

        [DataMember]
        string error = null;
        public string Error
        {
            get { return error; }
        }

        [DataMember]
        string redirect = null;
        public string Redirect
        {
            get { return redirect; }
        }

        [DataMember]
        string data1 = null;
        public string Data1
        {
            get { return data1; }
        }

        [DataMember]
        string data2 = null;
        public string Data2
        {
            get { return data2; }
        }

        [DataMember]
        string data3 = null;
        public string Data3
        {
            get { return data3; }
        }


        public static ServerResponse Parse(string json)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ServerResponse));
            ServerResponse request = ser.ReadObject(stream) as ServerResponse;
            stream.Close();
            return request;
        }
    }
}
