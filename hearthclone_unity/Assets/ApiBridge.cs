using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;
using System.Linq;
using hearthclone;
using hearthclone_loginserver;
using HS_Net;
using UnityEngine.UI;

public class ApiBridge : MonoBehaviour {

	public static string VERSION_CODE = "0.0.2a";

	public InputField usernameTxt;
	public InputField passwordTxt;
	public Text progressTxt;

	public static string username;
	public static string userid;
	public static string password;

	string msg = "";

	static string public_key;
	static string sym_key;

	bool AMDEBUG = false;
	bool checkedUpdates = false;

	void Start(){
		if (!AMDEBUG) {
			WebClient wc = new WebClient ();
			wc.DownloadStringCompleted += (sender, e) => {
				Debug.Log (e.Result);
				if (VERSION_CODE != e.Result) {
					Environment.Exit (0);
				}
			};
			wc.DownloadStringAsync (new Uri("http://www.polybellum.com/hearthclone/latest/v.txt"));
		}
	}

	void Update(){
		progressTxt.text = VERSION_CODE + "\n" + msg;
	}

	public void Click () {
		if (!checkedUpdates)
			return;
		public_key = ((TextAsset)Resources.Load("pub", typeof(TextAsset))).text;
		username = usernameTxt.text;
		password = passwordTxt.text;
		LoginServerLogin ();
	}

	void LoginServerLogin(){
		Debug.Log ("Connecting to login server...");

		IPHostEntry ipHostInfo = Dns.Resolve(Settings.Current.ServerUrl);
		IPAddress ipAddress = ipHostInfo.AddressList[0];
		IPEndPoint remoteEP = new IPEndPoint(ipAddress, Settings.Current.LoginServerPort);

		Socket client = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp); 
		client.BeginConnect(remoteEP, new AsyncCallback(LoginConnectCallback), client);
	}

	void LoginConnectCallback(IAsyncResult ar)
	{
		try
		{ 
			Socket listener = (Socket)ar.AsyncState;
			listener.EndConnect(ar);

			sym_key = GenerateSymKey();
			LoginRequest lr = new LoginRequest(username, password, sym_key);
			Debug.Log("Json: " + lr.Json);
			string e = RSAHandler.Encrypt(public_key, lr.Json);

			HS_SocketDataWorker sdw = new HS_SocketDataWorker(listener);
			sdw.SetCallback(new HS_SocketDataWorker.HS_PlayerCommandCallback(LoginMessageReceived));
			sdw.Send(e);
			Debug.Log("Sending data: " + e);
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}
	}

	public void LoginMessageReceived(HS_SocketDataWorker sdw, string message)
	{
		try{
			string MSG = TDESHandler.Decrypt(sym_key, message);
			Debug.Log("Message received: " + MSG);
			ServerResponse response = ServerResponse.Parse(MSG);
			msg = response.Result;
			userid = response.Data1;
			//TODO close the socket
			//TODO check result with json
			GameServerLogin();
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}

	string GenerateSymKey()
	{
		System.Random r = new System.Random();
		const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		return new string(Enumerable.Repeat(chars, 32)
			.Select(s => s[r.Next(s.Length)]).ToArray());
	}

	void GameServerLogin(){
		Debug.Log ("Connecting to game server...");

		IPHostEntry ipHostInfo = Dns.Resolve(Settings.Current.ServerUrl);
		IPAddress ipAddress = ipHostInfo.AddressList[0];
		IPEndPoint remoteEP = new IPEndPoint(ipAddress, Settings.Current.GameServerPort);

		Socket client = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp); 
		client.BeginConnect(remoteEP, new AsyncCallback(GameConnectCallback), client);
	}

	void GameConnectCallback(IAsyncResult ar)
	{
		try
		{ 
			Socket listener = (Socket)ar.AsyncState;
			listener.EndConnect(ar);
			HS_SocketDataWorker sdw = new HS_SocketDataWorker(listener);
			sdw.SetCallback(new HS_SocketDataWorker.HS_PlayerCommandCallback(GameMessageReceived));
			JsonDataMessage r = new JsonDataMessage("login", userid);
			sdw.Send(r.Json);
			Debug.Log("Sending data: " + r.Json);
		}
		catch (Exception e)
		{
			Debug.Log(e.ToString());
		}
	}

	void GameMessageReceived(HS_SocketDataWorker sdw, string message)
	{
		try{
			Debug.Log("Raw message received: " + message);
			string MSG = TDESHandler.Decrypt(sym_key, message);
			Debug.Log("Message received: " + MSG);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}

}
