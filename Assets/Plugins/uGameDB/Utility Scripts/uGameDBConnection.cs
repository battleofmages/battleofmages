// (c)2011 MuchDifferent. All Rights Reserved.

using System;
using UnityEngine;
using uGameDB;

/// <summary>
/// This script sets up a connection to one or more Riak nodes.
/// </summary>
/// <remarks>
/// You need to scpecify a host and port as well as a
/// unique string id for each node. The id is only used locally to identify the node connection. You can also specify
/// how many sockets uGameDB should use for each node to send requests through. More sockets mean that more requests
/// can be sent in parallel, but also use more memory.
/// 
/// The connection to Riak is made in the Awake() method, so you can send requests in any subsequent Unity callback.
/// </remarks>
public class uGameDBConnection : MonoBehaviour
{
	[Serializable]
	public class RiakNode
	{
		public string HostName = "localhost";
		public int RiakPbcPort = 8087;
		public string UniqueId = "riak";
		public int SocketPoolSize = 10;
	}

	public RiakNode[] RiakNodes = new RiakNode[1];
		
	public void Awake()
	{
		ReconnectDB();
	}

	private void ReconnectDB()
	{
		Database.Disconnect();
		Database.RemoveAllNodes();
		foreach (RiakNode node in RiakNodes)
		{
			Database.AddNode(node.UniqueId, node.HostName, node.RiakPbcPort, node.SocketPoolSize, Defaults.WriteTimeout, Defaults.ReadTimeout);
		}
		Database.Connect(OnDBConnect, OnDBError);
	}

	private void OnDBConnect()
	{
		Debug.Log("OnDBConnect successful");
	}

	private void OnDBError(string error)
	{
		Debug.LogError("Connect error: " + error);
	}

}
