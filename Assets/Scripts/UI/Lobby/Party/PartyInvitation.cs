using UnityEngine;
using uLobby;

public class PartyInvitation : MonoBehaviour {
	public PlayerAccount account;

	// Accept
	public void Accept() {
		Lobby.RPC("AcceptPartyInvitation", Lobby.lobby, account.id);
	}

	// Deny
	public void Deny() {
		Lobby.RPC("DenyPartyInvitation", Lobby.lobby, account.id);
	}
}
