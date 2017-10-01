
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace hearthclone_loginserver
{

    [DataContract]
    class LoginRequest
    {

        [DataMember]
        string u;
        public string Username
        {
            get { return u; }
        }

        [DataMember]
        string p;
        public string Password
        {
            get { return p; }
        }

        [DataMember]
        string k;
        public string Key
        {
            get { return k; }
        }

        public string Json
        {
            get
            {
                MemoryStream stream = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LoginRequest));
                ser.WriteObject(stream, this);
                byte[] json = stream.ToArray();
                stream.Close();
                return System.Text.Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }

        public LoginRequest(string u, string p, string k)
        {
            this.u = u;
            this.p = p;
            this.k = k;
        }

        public static LoginRequest Parse(string json)
        {
            MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LoginRequest));
            LoginRequest request = ser.ReadObject(stream) as LoginRequest;
            stream.Close();
            return request;
        }

    }
}
