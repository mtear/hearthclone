using System.IO;
using System.Runtime.Serialization;
using UnityEngine;
using System;

namespace hearthclone_loginserver
{

	[Serializable]
	class LoginRequest
	{

		public string u;
		public string Username
		{
			get { return u; }
		}
			
		public string p;
		public string Password
		{
			get { return p; }
		}
			
		public string k;
		public string Key
		{
			get { return k; }
		}

		public string Json
		{
			get
			{
				return JsonUtility.ToJson(this);
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
			return JsonUtility.FromJson<LoginRequest>(json);
		}

	}
}
