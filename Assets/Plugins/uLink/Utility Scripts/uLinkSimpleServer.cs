// (c)2011 MuchDifferent. All Rights Reserved.

using System;
using UnityEngine;
using uLink;

/// <summary>
/// A script example that can be used to start a very simple Unity server 
/// listening for uLink connection attempts from clients.
/// </summary>
/// <remarks>
/// The server is listening for UDP traffic on one port number. Default value is 7100.
/// The port number can be changed to whatever port number you like.
/// Another imporant property is targetFrameRate. This value dictates 
/// how many times per second the server reads incoming network traffic 
/// and sends outgoing traffic. It also dictates the actual frame rate for
/// the server (sometimes called tick rate). Read more about tick rate in
/// the Server Operations chapter in the uLink manual.
/// The property called registerHost dictates if this game server should
/// try to register iteself in a uLink Master Server. Read the Master Server & Proxy
/// manual chapter for more info.
/// </remarks>
[AddComponentMenu("uLink Utilities/Simple Server")]
public class uLinkSimpleServer : uLink.MonoBehaviour
{
	[Serializable]
	public class InstantiateOnConnected
	{
		public Vector3 startPosition = new Vector3(0, 3, 0);
		public Vector3 startRotation = new Vector3(0, 0, 0);

		public GameObject ownerPrefab;
		public GameObject proxyPrefab;
		public GameObject serverPrefab;

		public bool appendLoginData = false;

		public void Instantiate(uLink.NetworkPlayer player)
		{
			if (ownerPrefab != null && proxyPrefab != null && serverPrefab != null)
			{
				Quaternion rotation = Quaternion.Euler(startRotation);
				object[] initialData = appendLoginData ? uLink.Network.loginData : new object[0];

				uLink.Network.Instantiate(player, proxyPrefab, ownerPrefab, serverPrefab, startPosition, rotation, 0, initialData);
			}
		}
	}

	public int port = 7100;
	public int maxConnections = 64;
	
	public bool cleanupAfterPlayers = true;
	
	public bool registerHost = false;

	public int targetFrameRate = 60;

	public bool dontDestroyOnLoad = false;

	public InstantiateOnConnected instantiateOnConnected = new InstantiateOnConnected();
	
	void Start()
	{
		Application.targetFrameRate = targetFrameRate;

		if (dontDestroyOnLoad) DontDestroyOnLoad(this);
	
		uLink.Network.InitializeServer(maxConnections, port);
	}

	void uLink_OnServerInitialized()
	{
		Debug.Log("Server successfully started on port " + uLink.Network.listenPort);
		
		if (registerHost) uLink.MasterServer.RegisterHost();
	}

	void uLink_OnPlayerDisconnected(uLink.NetworkPlayer player)
	{
		if (cleanupAfterPlayers)
		{
			uLink.Network.DestroyPlayerObjects(player);
			uLink.Network.RemoveRPCs(player);
			
			// this is not really necessery unless you are removing NetworkViews without calling uLink.Network.Destroy
			uLink.Network.RemoveInstantiates(player);
		}
	}

	void uLink_OnPlayerConnected(uLink.NetworkPlayer player)
	{
		instantiateOnConnected.Instantiate(player);
	}
}
