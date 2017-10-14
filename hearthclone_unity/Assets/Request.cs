using System.IO;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace HS_Net
{
	[System.Serializable]
    public class JsonDataMessage
    {
		[DataMember]
		public string[] d;

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
				return JsonUtility.ToJson(this);
            }
        }

		public JsonDataMessage(params string[] data)
		{
			this.d = data;
		}

		public static JsonDataMessage Parse(string json)
        {
			return JsonUtility.FromJson<JsonDataMessage>(json);
        }

    }
}
