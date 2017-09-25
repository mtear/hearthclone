using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;

public class ApiBridge : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartClient ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// The port number for the remote device.  
	private const int port = 1121;

	private static void StartClient()
	{
		// Connect to a remote device.  
		try
		{
			// Establish the remote endpoint for the socket.  
			// The name of the   
			// remote device is "host.contoso.com".  
			IPHostEntry ipHostInfo = Dns.Resolve("polybellum.com");
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

			// Create a TCP/IP socket.  
			Socket client = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);

			// Connect to the remote endpoint.  
			client.BeginConnect(remoteEP,
				new AsyncCallback(ConnectCallback), client);
			//connectDone.WaitOne();

			//Receive(client);

			// Release the socket.  
			//Console.WriteLine("Shutting down connection...");
			//client.Shutdown(SocketShutdown.Both);
			//client.Close();

		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}
	}

	private static void ConnectCallback(IAsyncResult ar)
	{
		try
		{
			// Retrieve the socket from the state object.  
			Socket client = (Socket)ar.AsyncState;

			// Complete the connection.  
			client.EndConnect(ar);

			Debug.Log("Socket connected to " + client.RemoteEndPoint.ToString());

		//	HS_Request r = new HS_Request(name, "NAMESET");
			//Send(client, r.Json+"<EOF>");

			// Signal that the connection has been made.  
			//connectDone.Set();
		}
		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
		}
	}

}
