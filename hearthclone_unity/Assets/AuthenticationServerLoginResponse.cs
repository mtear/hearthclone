using System.IO;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace HS_Net
{
	[System.Serializable]
    class ServerResponse
    {
        [DataMember]
        public string result = null;
        public string Result
        {
            get { return result; }
        }

        [DataMember]
        public string error = null;
        public string Error
        {
            get { return error; }
        }

        [DataMember]
        public string redirect = null;
        public string Redirect
        {
            get { return redirect; }
        }

        [DataMember]
		public string data1 = null;
        public string Data1
        {
            get { return data1; }
        }

        [DataMember]
		public string data2 = null;
        public string Data2
        {
            get { return data2; }
        }

        [DataMember]
		public string data3 = null;
        public string Data3
        {
            get { return data3; }
        }


        public static ServerResponse Parse(string json)
        {
			return JsonUtility.FromJson<ServerResponse>(json);
        }
    }
}
