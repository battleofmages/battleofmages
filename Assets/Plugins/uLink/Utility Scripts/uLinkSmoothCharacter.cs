// (c)2011 MuchDifferent. All Rights Reserved.

using UnityEngine;
using uLink;

/// <summary>
/// A script example that can be use for players' avatars in a game where the server is authoritative or non-authoritative.
/// </summary>
/// <remarks>
/// When using this example script, it should be added as a component to the game object that a player controls.
/// The game object must have a character controller component, not a rigidbody.
/// The observed property of the uLinkNetworkView for the game object should be set to this component.
///
/// Auth Server: The basic idea for this script is that the owner (controlling this object) sends unreliable "Move" RPCs to 
/// the auth server with a rate dictated by uLink.Network.sendRate. The auth server receives this RPC and moves the 
/// character. If you want to add security checks in the server because it should be authoritative, you can do that. Make a 
/// copy of the script and do your modifications.
///
/// Non-auth Server: The basic idea for this script is that the owner (controlling this object) sends statesync to the 
/// non-auth server with a rate dictated by uLink.Network.sendRate. The auth server receives this statesync and moves the 
/// character. 
///
/// Both auth server and non-auth server sends statesync to clients with proxies.
///
/// This script component also makes sure the movement is smooth in the server and in the clients showing the proxy objects.  
/// </remarks>

[AddComponentMenu("uLink Utilities/Smooth Character")]
[RequireComponent(typeof(uLinkNetworkView))]
[RequireComponent(typeof(CharacterController))]
public class uLinkSmoothCharacter : uLink.MonoBehaviour
{
	public float maxSpeed = 4.6f;
	public float maxDistance = 10f;
	public float arrivalDistance = 1;
	
	[HideInInspector]
	public Vector3 velocity;

	public bool moveCharacter = true;
	
	[HideInInspector]
	public float arrivalSpeed;
	
	private Vector3 targetDir;
	private float targetDistance;
	
	private bool firstState = true;

	public float rotationDamping = 0.85f;

	private Quaternion curRot;
	private Quaternion targetRot;

	private CharacterController character;

	private double serverLastTimestamp = 0;

	private bool isInitiaized = false;

	void Awake()
	{
		arrivalSpeed = maxSpeed / arrivalDistance;
		
		curRot = transform.rotation;
		
		character = GetComponent<CharacterController>();
	}

	void Start()
	{
		if (networkView.viewID == uLink.NetworkViewID.unassigned) return;

		isInitiaized = true;

		if (!networkView.isMine) return;

		enabled = false;

		if (uLink.Network.isAuthoritativeServer && uLink.Network.isClient)
		{
			InvokeRepeating("SendToServer", 0, 1.0f / uLink.Network.sendRate);
		}
	}

	void uLink_OnSerializeNetworkView(uLink.BitStream stream, uLink.NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			// Send information to all proxies (opponent player's computers)
			// This code is executed on the creator (server) when server is auth, or on the owner (client) when the server is non-auth.
			stream.Write(transform.position);
			stream.Write(character.velocity);
			stream.Write(transform.rotation);
		}
		else
		{
			// Update the proxy state when statesync arrives.
			Vector3 pos = stream.Read<Vector3>();
			Vector3 vel = stream.Read<Vector3>();
			Quaternion rot = stream.Read<Quaternion>();

			UpdateState(pos, vel, rot, info.timestamp);
		}
	}

	private void UpdateState(Vector3 pos, Vector3 vel, Quaternion rot, double timestamp)
	{
		float deltaTime = (float)(uLink.Network.time - timestamp);
		Vector3 target = pos + vel * deltaTime;
		
		if (firstState)
		{
			firstState = false;
			targetDistance = 0;
			transform.position = target;
			return;
		}
		
		targetRot = rot;
		Vector3 offset = target - transform.position;
		targetDistance = offset.magnitude;
		
		if (targetDistance > maxDistance)
		{
			// Detected a too big distance error! Snap to correct position!
			targetDistance = 0;
			transform.position = target;
		}
		else if (targetDistance > 0)
		{
			targetDir = offset / targetDistance;
		}
	}

	void SendToServer()
	{
		// This code is only executed on the client which is the owner of this game object
		// Sends Movement RPC to server. The nice part is that this code works when using 
		// an auth server or non-auth server. Both can handle this RPC!
		networkView.UnreliableRPC("Move", uLink.NetworkPlayer.server, transform.position, character.velocity, transform.rotation);
	}
	
	void Update()
	{
		if (!isInitiaized && networkView.viewID != uLink.NetworkViewID.unassigned)
		{
			Start();
			return;
		}

		// Executes the smooth movement of character controller for proxies and the game object in the server.
		curRot = Quaternion.Lerp(targetRot, curRot, rotationDamping);
		transform.rotation = curRot;
			
		if (targetDistance == 0)
		{
			return;
		}

		float speed = (targetDistance > arrivalDistance) ? maxSpeed : arrivalSpeed * targetDistance;

		velocity = speed * targetDir; 
		
		if (moveCharacter)
		{
			character.SimpleMove(velocity);
			
			targetDistance -= speed * Time.deltaTime;
		}
	}

	[RPC]
	void Move(Vector3 pos, Vector3 vel, Quaternion rot, uLink.NetworkMessageInfo info)
	{
		// This code is only executed in the auth server
		if (info.sender != networkView.owner || info.timestamp <= serverLastTimestamp)
		{
			// Make sure we throw away late and duplicate RPC messages. And trow away messages 
			// from the wrong client (they could trying to cheat this way) 
			return;
		}

		serverLastTimestamp = info.timestamp;

		// Add some more code right here if the server is authoritave and you want to do more security checks
		// The server state is updated with incoming data from the client beeing the "owner" of this game object
		UpdateState(pos, vel, rot, info.timestamp);
	}
}
