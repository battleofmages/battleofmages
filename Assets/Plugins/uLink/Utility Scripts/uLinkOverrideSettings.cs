// (c)2011 MuchDifferent. All Rights Reserved.

using System;
using UnityEngine;

// TODO: add more settings...

[AddComponentMenu("uLink Utilities/Override Settings")]
public class uLinkOverrideSettings : MonoBehaviour
{
	[Serializable]
	public abstract class Settings
	{
		[SerializeField]
		private bool _override = false;

		public void Override()
		{
			if (_override) Apply();
		}

		protected abstract void Apply();
	}

	[Serializable]
	public class General : Settings
	{
		public float sendRate = uLink.Network.sendRate;
		public int maxManualViewIDs = uLink.Network.maxManualViewIDs;
		public bool isAuthoritativeServer = uLink.Network.isAuthoritativeServer;
		public int minimumAllocatableViewIDs = uLink.Network.minimumAllocatableViewIDs;
		public int minimumUsedViewIDs = uLink.Network.minimumUsedViewIDs;
		public bool useDifferentStateForOwner = uLink.Network.useDifferentStateForOwner;

		protected override void Apply()
		{
			uLink.Network.sendRate = sendRate;
			uLink.Network.maxManualViewIDs = maxManualViewIDs;
			uLink.Network.isAuthoritativeServer = isAuthoritativeServer;
			uLink.Network.minimumAllocatableViewIDs = minimumAllocatableViewIDs;
			uLink.Network.minimumUsedViewIDs = minimumUsedViewIDs;
			uLink.Network.useDifferentStateForOwner = useDifferentStateForOwner;
		}
	}

	[Serializable]
	public class Client : Settings
	{
		public bool requireSecurityForConnecting = uLink.Network.requireSecurityForConnecting;
		public int symmetricKeySize = uLink.Network.symmetricKeySize;

		protected override void Apply()
		{
			uLink.Network.requireSecurityForConnecting = requireSecurityForConnecting;
			uLink.Network.symmetricKeySize = symmetricKeySize;
		}
	}

	[Serializable]
	public class Server : Settings
	{
		public string incomingPassword = uLink.Network.incomingPassword;
		public bool useProxy = uLink.Network.useProxy;
		public bool useRedirect = uLink.Network.useRedirect;
		public string redirectIP = uLink.Network.redirectIP;
		public int redirectPort = uLink.Network.redirectPort;

		protected override void Apply()
		{
			uLink.Network.incomingPassword = incomingPassword;
			uLink.Network.useProxy = useProxy;
			uLink.Network.useRedirect = useRedirect;
			uLink.Network.redirectIP = redirectIP;
			uLink.Network.redirectPort = redirectPort;
		}
	}

	[Serializable]
	public class CellServer : Settings
	{
		public float trackRate = uLink.Network.trackRate;
		public float trackMaxDelta = uLink.Network.trackMaxDelta;

		protected override void Apply()
		{
			uLink.Network.trackRate = trackRate;
			uLink.Network.trackMaxDelta = trackMaxDelta;
		}
	}

	[Serializable]
	public class MasterServer : Settings
	{
		public string comment = uLink.MasterServer.comment;
		public bool dedicatedServer = uLink.MasterServer.dedicatedServer;
		public string gameLevel = uLink.MasterServer.gameLevel;
		public string gameMode = uLink.MasterServer.gameMode;
		public string gameName = uLink.MasterServer.gameName;
		public string gameType = uLink.MasterServer.gameType;
		public string ipAddress = uLink.MasterServer.ipAddress;
		public string password = uLink.MasterServer.password;
		public int port = uLink.MasterServer.port;
		public float updateRate = uLink.MasterServer.updateRate;

		protected override void Apply()
		{
#if !UNITY_2_6 && !UNITY_2_6_1
			if (Application.webSecurityEnabled)
			{
				Security.PrefetchSocketPolicy(ipAddress, 843);
			}
#endif

			uLink.MasterServer.comment = comment;
			uLink.MasterServer.dedicatedServer = dedicatedServer;
			uLink.MasterServer.gameLevel = gameLevel;
			uLink.MasterServer.gameMode = gameMode;
			uLink.MasterServer.gameName = gameName;
			uLink.MasterServer.gameType = gameType;
			uLink.MasterServer.ipAddress = ipAddress;
			uLink.MasterServer.password = password;
			uLink.MasterServer.port = port;
			uLink.MasterServer.updateRate = updateRate;
		}
	}

	public General general;
	public Client client;
	public Server server;
	public CellServer cellServer;
	public MasterServer masterServer;

	public bool dontDestroyOnLoad = false;

	void Awake()
	{
		if (dontDestroyOnLoad) DontDestroyOnLoad(this);

		general.Override();
		client.Override();
		server.Override();
		cellServer.Override();
		masterServer.Override();
	}

	void uLink_OnPreStartNetwork(uLink.NetworkStartEvent nsEvent)
	{
		general.Override();

		switch (nsEvent)
		{
			case uLink.NetworkStartEvent.Client: client.Override(); break;
			case uLink.NetworkStartEvent.Server: server.Override(); break;
			case uLink.NetworkStartEvent.CellServer: cellServer.Override(); break;
			case uLink.NetworkStartEvent.MasterServer: masterServer.Override(); break;
		}
	}
}
