using UnityEngine;

public interface CasterOnServer {
	[RPC]
	void ClientStartCast(byte slotId, uLink.NetworkMessageInfo info);

	[RPC]
	void ClientAdvanceCast(uLink.NetworkMessageInfo info);

	[RPC]
	void ClientCouldntCast(uLink.NetworkMessageInfo info);

	[RPC]
	void ClientEndCast(Vector3 hitPoint, uLink.NetworkMessageInfo info);

	[RPC]
	void ClientInstantCast(byte skillId, Vector3 hitPoint, uLink.NetworkMessageInfo info);

	[RPC]
	void ClientStopHolding(uLink.NetworkMessageInfo info);

	[RPC]
	void ClientSetTarget(ushort entityId, uLink.NetworkMessageInfo info);
}
