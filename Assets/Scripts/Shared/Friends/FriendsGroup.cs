using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class FriendsGroup : JSONSerializable<FriendsGroup> {
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