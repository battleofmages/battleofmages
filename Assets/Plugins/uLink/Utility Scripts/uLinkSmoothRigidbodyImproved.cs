// (c)2012 MuchDifferent. All Rights Reserved.

using UnityEngine;
using uLink;

/// <summary>
/// A script example that can be used for game objects with a rigidbody in a game when the server 
/// is authoritative and physics is done on the server.
/// </summary>
/// <remarks>
/// When using this example script, it should be added as a component to the game object that a player 
/// controls or the server controls.
///
/// Add this script to the creator prefab, the owner prefab and the proxy prefab for this game object.
/// You can skip the owner prefab if it is a server owned object. Then just use the creator and proxy prefab.
/// The game object must have a rigidbody, not a character controller component.
/// The observed property of the uLinkNetworkView for the game object should be set to this component.
///
/// The client side prefabs (the proxy prefab and the owner prefab) should set the properties in the 
/// rigidbody component like this: 
/// Drag = 0
/// Use Gravity = false (should be true on the server, the creator prefab, if you are using gravity in the game)
/// Is Kinematic = false (must be set in proxy and owner - because this script does set rigidbody.velocity directly to move the object in the client)
/// Interpolate = Interpolate (required) - Default value is None, so make sure you do change this value. 
///
/// How it works: The basic idea for this script is that the creator object moves in the server by some logic 
/// outside of this script. The movement could be controlled by AI logic or path finding logic or RPCs from some client adding 
/// forces to the creater object. The server does all the collision detection (using the physics engine in Unity) and then the server 
/// sends state sync messages to all proxies (and the owner) including position and rotation.
/// 
/// The client side prefabs (the proxy prefab and the owner prefab) should NOT have a collider component.
/// 
/// The client shows a smooth movement for proxies and the owner, it is not exaclty like the server simulation,
/// but a good aproximation, and that is all it can be due to latency and packet loss over the network. 
/// The smoothing logic in the clients compares the object's current position (in the client) and the position in the 
/// LAST arriving state sync message. This way the client side object will always try to move in the direction of the most
/// "fresh" position informtion that has arrived to the client. 
///
/// A more complete example of usage: The client controlling this objects sends mouse and keyboard input to the server
/// using RPCs. The creator object in the server recieves this. The creator object will move due to the client input and other
/// forces like objects colliding. The resulting position and rotation is send from the server to all clients using state sync.
/// This script does state sync sending. The clients receives the state sync (done by this script) and calcualtes a smooth 
/// new velocity for the client side object and applices this velocity to the rigidbody.
/// 
/// There is an example project using this script among the list of uLink example projects: 
/// http://developer.unitypark3d.com/examples/
/// 
/// </remarks>

[AddComponentMenu("uLink Utilities/Smooth Rigidbody (Improved)")]
[RequireComponent(typeof(uLink.NetworkView))]
[RequireComponent(typeof(Rigidbody))]
public class uLinkSmoothRigidbodyImproved : uLink.MonoBehaviour
{
	// The maximum distance "error". This distance is between the correct position (creator's 
	// position) and my client side calculated position (proxy position or owner position).
	// When the calculated distance is larger than the maxDistance, the proxy object will "snap" 
	// to the correct position. This can happen after massive packet loss on the network or when the
	// creator prefab moves very fast. 
	// NOTE: You have to tune this value for every object. And this is some basic guidelines:
	// Fast objects needs a high value, slow objects can have a low value.
	// Set this value higher for handling packet loss smoother. Set this value lower to make sure
	// the clients can see the accurate position of this object as fast as possible. 
	// This value is only used in the proxy and owner, not creator.
	public float maxDistanceError = 8f;

	// This is the timeframe used in clients to show object movements a short time after the server.
	// as long the client gets state sync packets from the server inside this timeframe, the client
	// can use the interpolation algorith in this script to show a smooth and very close to correct movement.
	// This value should be about 100-200 ms higher than the ping time between the client and the server. 
	// This value is only used in the proxy and owner, not creator
	public float interpolationBackTimeMs = 200f;

	// This is the timeframe in the client that an object will continue to move in a straight line even if no 
	// state sync packets arrive from the server. After this timeframe the object will stop.
	// This value is only used in the proxy and owner, not creator
	public float maxExtrapolationTimeMs = 500f;

	// Trim this value to make rotation start and stop in a smooth way in clients (proxies and owner)
	public float rotationDamping = 0.85f;
		
	private Vector3 targetDir;
	private float targetDistance;
	private Vector3 optimalSmoothVelocity;
	private bool firstState = true;

	private class State
	{
		public double timestamp = 0;
		public Vector3 pos;
		public Quaternion rot;
	}

	private State mostCurrentReceivedState = new State();

	private Quaternion curRot;
	private Quaternion targetRot;

	void Awake()
	{
		curRot = transform.rotation;
	}

	void uLink_OnSerializeNetworkViewOwner(uLink.BitStream stream, uLink.NetworkMessageInfo info)
	{
		// This callback implementation is needed for getting statesync to the 
		// owner prefab (from the creator prefab on server)
		uLink_OnSerializeNetworkView(stream, info);
	}

	void uLink_OnSerializeNetworkView(uLink.BitStream stream, uLink.NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			// Send information to all proxies and the owner (clients)
			// This code is executed on the creator (server) 
			stream.Write(transform.position);
			stream.Write(GetComponent<Rigidbody>().rotation); 

		}
		else
		{
			//This code is executed in the clients
			mostCurrentReceivedState.pos = stream.Read<Vector3>();
			mostCurrentReceivedState.rot = stream.Read<Quaternion>(); 
			
			mostCurrentReceivedState.timestamp = info.timestamp + (interpolationBackTimeMs / 1000);
		}
	}

	private void UpdateState()
	{
		if (mostCurrentReceivedState.timestamp != 0 && firstState)
		{
			firstState = false;
			targetDistance = 0;
			transform.position = mostCurrentReceivedState.pos;
			return;
		}

		float timeLeftToTarget = (float)(mostCurrentReceivedState.timestamp - uLink.Network.time);

		Vector3 target;
		if (timeLeftToTarget > 0)
		{
			//we can use interpolation
			target = mostCurrentReceivedState.pos; 
		}
		else if (timeLeftToTarget <= 0 && (maxExtrapolationTimeMs / 1000) > (uLink.Network.time - mostCurrentReceivedState.timestamp))
		{
			//Can not use extrapoaltion any more.
			target = mostCurrentReceivedState.pos;
		} 
		else 
		{
			//This is extrapolation, algorithm = let's fake a new position using current velocity
			target = mostCurrentReceivedState.pos + GetComponent<Rigidbody>().velocity * Time.fixedDeltaTime;
			//Update mostCurrentReceivedState to a locally constructed (faked) timestamp and position 
			mostCurrentReceivedState.timestamp += Time.fixedDeltaTime;
			mostCurrentReceivedState.pos = target;
		}
		
		targetRot = mostCurrentReceivedState.rot; //TODO: better rotation handling, make use if interpolationBacktime.

		Vector3 offset = target - transform.position;
		float endTargetDistance = offset.magnitude;

		if (timeLeftToTarget > Time.fixedDeltaTime)
		{
			//This object will not reach it's target during the next FixedUpdate time tick
			// targetDistance = How far this object should move during the next FixedUpdate time tick
			targetDistance = endTargetDistance * timeLeftToTarget / Time.fixedDeltaTime;
			// New optimal calculated velocity that will replace current velocity
			optimalSmoothVelocity = offset / timeLeftToTarget;
		}
		else
		{
			//This object will reach it's target position during the next FixedUpdate time tick
			targetDistance = endTargetDistance;
			optimalSmoothVelocity = offset;
		}

		if (endTargetDistance > maxDistanceError)
		{
			// Detected a too big distance error! Snap to correct position!
			targetDistance = 0;
			GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
			transform.position = target;
		}
	}


	void FixedUpdate()
	{
		if (networkView.viewID == uLink.NetworkViewID.unassigned)
		{
			return;
		}

		// Only execute the smooth rotation for proxies and owner, not authority
		if (networkView.hasAuthority)
		{
			return;
		}

		//No state sync message has arrived to this client => Do nothing.
		if (mostCurrentReceivedState.timestamp == 0)
		{
			return;
		}
		
		UpdateState();

		curRot = Quaternion.Lerp(targetRot, curRot, rotationDamping);
		transform.rotation = curRot;

		if (targetDistance < 0.01)
		{
		   // If we are this close to end position, don't move at all to avoid "flickering".
			return;
		}

		GetComponent<Rigidbody>().velocity = optimalSmoothVelocity;
	}

}
