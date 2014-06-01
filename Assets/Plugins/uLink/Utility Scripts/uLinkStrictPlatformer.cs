// (c)2011 MuchDifferent. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;
using uLink;

/// <summary>
/// A script example that can be use for players' avatars in a 3d platform game where 
/// the avatar is moving on a surface/platform and affected by gravity.
/// </summary>
/// <remarks>
/// When using this example script, it should be added as a component to the game object that a player controls.
/// The server should be authoritative when using this script (uLink.Network.isAuthoritativeServer = true).
/// The basic idea is that the server simulates all physics including gravity and checks if any player tries to cheat by 
/// sending movment orders as an RPC (The RPC name is ServerMove) with false coordinates to move faster than allowed in the game.
/// The server checks the incoming ServerMove RPC from the client and sends two kinds of RPCs back to the client.
/// If the client did move too fast (due to a cheating attempt or a bug or whatever) the server sends an RPC named
/// AdjustOwnerPos. If the position is good, the server sends an RPC named GoodOwnerPos. They are both sent as unreliable
/// RPCs from the server to the client to minimize server resources.
///
/// This script component also makes sure interpolation and extrapolation is used for the state synchronozation sent from
/// the server to clients. The state synchronization, arriving at the client, is stored in an internal array and the 
/// public properties interpolationBackTime and extrapolationLimit can be used to tune the correct behavior for every game. 
/// Please read the code for more details.
/// </remarks>

[AddComponentMenu("uLink Utilities/Strict Platformer")]
[RequireComponent(typeof(uLinkNetworkView))]
public class uLinkStrictPlatformer : uLink.MonoBehaviour
{
	private struct State
	{
		public double timestamp;
		public Vector3 pos;
		public Vector3 vel;
		public Quaternion rot;
	}

	private struct Move : IComparable<Move>
	{
		public double timestamp;
		public float deltaTime;
		public Vector3 vel;

		public static bool operator ==(Move lhs, Move rhs) { return lhs.timestamp == rhs.timestamp; }
		public static bool operator !=(Move lhs, Move rhs) { return lhs.timestamp != rhs.timestamp; }
		public static bool operator >=(Move lhs, Move rhs) { return lhs.timestamp >= rhs.timestamp; }
		public static bool operator <=(Move lhs, Move rhs) { return lhs.timestamp <= rhs.timestamp; }
		public static bool operator >(Move lhs, Move rhs) { return lhs.timestamp > rhs.timestamp; }
		public static bool operator <(Move lhs, Move rhs) { return lhs.timestamp < rhs.timestamp; }

		public override bool Equals(object other)
		{
			if (other == null || !(other is Move))
				return false;

			return this == (Move)other;
		}

		public override int GetHashCode()
		{
			return timestamp.GetHashCode();
		}

		public int CompareTo(Move other)
		{
			if (this > other)
				return 1;

			if (this < other)
				return -1;

			return 0;
		}
	}

	public double interpolationBackTime = 0.2;
	public double extrapolationLimit = 0.5;

	public float sqrMaxServerError = 300.0f;
	public float sqrMaxServerSpeed = 1000.0f;

	public float gravityAcceleration = 300.0f;

	public bool canJump = true;
	public float jumpHeight = 10.0f;

	private CharacterController character;

	// We store twenty states with "playback" information
	private State[] proxyStates = new State[20];
	// Keep track of what slots are used
	private int proxyStateCount;

	private float ownerInputVelX = 0;
	private float ownerInputVelZ = 0;
	private bool ownerInputJumping = false;

	private List<Move> ownerMoves = new List<Move>();

	private bool wasGrounded = false;
	private float lastVelY = 0;

	private double serverLastTimestamp = 0;
	private bool serverHasLastOwnerPos = false;
	private Vector3 serverLastOwnerPos = Vector3.zero;

	void Awake()
	{
		character = GetComponent<CharacterController>();
	}

	void uLink_OnNetworkInstantiate(uLink.NetworkMessageInfo info)
	{
		serverLastTimestamp = info.timestamp;
	}

	void uLink_OnSerializeNetworkView(uLink.BitStream stream, uLink.NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.Write(transform.position);
			stream.Write(character.velocity);
			stream.Write(transform.rotation);
		}
		else
		{
			State state;
			state.timestamp = info.timestamp;

			state.pos = stream.Read<Vector3>();
			state.vel = stream.Read<Vector3>();
			state.rot = stream.Read<Quaternion>();

			// Shift the buffer sideways, deleting state 20
			for (int i = proxyStates.Length - 1; i >= 1; i--)
			{
				proxyStates[i] = proxyStates[i - 1];
			}


			// Record current state in slot 0
			proxyStates[0] = state;

			// Update used slot count, however never exceed the buffer size
			// Slots aren't actually freed so this just makes sure the buffer is
			// filled up and that uninitalized slots aren't used.
			proxyStateCount = Mathf.Min(proxyStateCount + 1, proxyStates.Length);

			// Check if states are in order
			if (proxyStates[0].timestamp < proxyStates[1].timestamp)
				Debug.LogError("Timestamp inconsistent: " + proxyStates[0].timestamp + " should be greater than " + proxyStates[1].timestamp);
		}
	}

	// We have a window of interpolationBackTime where we basically play 
	// By having interpolationBackTime the average ping, you will usually use interpolation.
	// And only if no more data arrives we will use extra polation
	void Update()
	{
		if (uLink.Network.isAuthoritativeServer && uLink.Network.isServerOrCellServer)
			return;
		
		if (uLink.Network.isAuthoritativeServer && networkView.isMine)
		{
			// TODO: optimize by not sending rpc if no input and rotation. also add idleTime so server's timestamp is still in sync

			bool jumping = false;

			if (wasGrounded)
			{
				jumping = (canJump && ownerInputJumping);
				lastVelY = jumping ? Mathf.Sqrt(2 * jumpHeight * gravityAcceleration) : 0;
			}

			Move move;
			move.timestamp = uLink.Network.time;
			move.deltaTime = Time.deltaTime;

			lastVelY -= gravityAcceleration * move.deltaTime;
			move.vel = new Vector3(ownerInputVelX, lastVelY, ownerInputVelZ);
			ownerMoves.Add(move);

			CollisionFlags flags = character.Move(move.vel * move.deltaTime);
			wasGrounded = ((flags & CollisionFlags.CollidedBelow) != 0);

			networkView.UnreliableRPC("ServerMove", uLink.NetworkPlayer.server, transform.position, move.vel.x, move.vel.z, jumping, transform.rotation.eulerAngles.y);

			ownerInputVelX = 0;
			ownerInputVelZ = 0;
			ownerInputJumping = false;
		}
		else
		{
			// This is the target playback time of the rigid body
			double interpolationTime = uLink.Network.time - interpolationBackTime;

			// Use interpolation if the target playback time is present in the buffer
			if (proxyStates[0].timestamp > interpolationTime)
			{
				// Go through buffer and find correct state to play back
				for (int i = 0; i < proxyStateCount; i++)
				{
					if (proxyStates[i].timestamp <= interpolationTime || i == proxyStateCount - 1)
					{
						// The state one slot newer (<100ms) than the best playback state
						State rhs = proxyStates[Mathf.Max(i - 1, 0)];
						// The best playback state (closest to 100 ms old (default time))
						State lhs = proxyStates[i];

						// Use the time between the two slots to determine if interpolation is necessary
						double length = rhs.timestamp - lhs.timestamp;
						float t = 0.0F;
						// As the time difference gets closer to 100 ms t gets closer to 1 in 
						// which case rhs is only used
						// Example:
						// Time is 10.000, so sampleTime is 9.900 
						// lhs.time is 9.910 rhs.time is 9.980 length is 0.070
						// t is 9.900 - 9.910 / 0.070 = 0.14. So it uses 14% of rhs, 86% of lhs
						if (length > 0.0001)
							t = (float)((interpolationTime - lhs.timestamp) / length);

						// if t=0 => lhs is used directly
						transform.localPosition = Vector3.Lerp(lhs.pos, rhs.pos, t);
						transform.localRotation = Quaternion.Slerp(lhs.rot, rhs.rot, t);
						return;
					}
				}
			}
			// Use extrapolation
			else
			{
				State latest = proxyStates[0];

				float extrapolationLength = (float)(interpolationTime - latest.timestamp);
				// Don't extrapolation for more than 500 ms, you would need to do that carefully
				if (extrapolationLength < extrapolationLimit)
				{
					transform.position = latest.pos + latest.vel * extrapolationLength;
					transform.rotation = latest.rot;
					character.SimpleMove(latest.vel);
				}
			}
		}
	}

	void LateUpdate()
	{
		if (!uLink.Network.isAuthoritativeServer || networkView.owner == uLink.NetworkPlayer.server || uLink.Network.isClient)
			return;

		if (wasGrounded) lastVelY = 0;
		lastVelY -= gravityAcceleration * Time.deltaTime;

		CollisionFlags flags = character.Move(new Vector3(0, lastVelY * Time.deltaTime, 0));
		wasGrounded = ((flags & CollisionFlags.CollidedBelow) != 0);

		if (serverHasLastOwnerPos && (uLink.Network.isCellServer || networkView.owner.isConnected))
		{
			serverHasLastOwnerPos = false;
			Vector3 serverPos = transform.position;
			Vector3 diff = serverPos - serverLastOwnerPos;

			if (Vector3.SqrMagnitude(diff) > sqrMaxServerError)
			{
				networkView.UnreliableRPC("AdjustOwnerPos", uLink.RPCMode.Owner, serverPos);
			}
			else
			{
				networkView.UnreliableRPC("GoodOwnerPos", uLink.RPCMode.Owner);
			}
		}
	}

	public void SetInput(float velX, float velZ, bool jump)
	{
		//TODO: if not auth.

		ownerInputVelX = velX;
		ownerInputVelZ = velZ;
		ownerInputJumping = jump;
	}

	[RPC]
	void ServerMove(Vector3 ownerPos, float velX, float velZ, bool jumping, float rotY, uLink.NetworkMessageInfo info)
	{
		if (info.sender != networkView.owner || info.timestamp <= serverLastTimestamp)
		{
			return;
		}

		Vector3 rot = transform.rotation.eulerAngles;
		transform.rotation = Quaternion.Euler(rot.x, rotY, rot.z);

		if (velX * velX + velZ * velZ > sqrMaxServerSpeed)
		{
			velX = velZ = Mathf.Sqrt(sqrMaxServerSpeed) * 0.5f;
		}

		float deltaTime = (float)(info.timestamp - serverLastTimestamp);
		Vector3 deltaPos = new Vector3(velX * deltaTime, 0, velZ * deltaTime);

		if (wasGrounded && canJump && jumping)
		{
			lastVelY = Mathf.Sqrt(2 * jumpHeight * gravityAcceleration);
			wasGrounded = false;
		}

		character.Move(deltaPos);

		serverLastTimestamp = info.timestamp;
		serverHasLastOwnerPos = true;
		serverLastOwnerPos = ownerPos;
	}

	[RPC]
	void GoodOwnerPos(uLink.NetworkMessageInfo info)
	{
		Move goodMove;
		goodMove.timestamp = info.timestamp;
		goodMove.deltaTime = 0;
		goodMove.vel = Vector3.zero;

		int index = ownerMoves.BinarySearch(goodMove);
		if (index < 0) index = ~index;

		ownerMoves.RemoveRange(0, index);
	}

	[RPC]
	void AdjustOwnerPos(Vector3 pos, uLink.NetworkMessageInfo info)
	{
		GoodOwnerPos(info);

		transform.position = pos;

		foreach (Move move in ownerMoves)
		{
			character.Move(move.vel * move.deltaTime);
		}
	}
}
