using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FriendsGroup : JsonSerializable<FriendsGroup> {
	public string name;
	public Color color;
	public List<Friend> friends;

	// Constructor
	public FriendsGroup() {
		name = "";
		color = Color.white;
		friends = new List<Friend>();
	}

	// Constructor
	public FriendsGroup(string nName) {
		name = nName;
		color = Color.white;
		friends = new List<Friend>();
	}
}
