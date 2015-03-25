using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoM.Friends {
	// FriendsGroup
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

		// ContainsAccount
		public bool ContainsAccount(string accountId) {
			return friends.Any(friend => friend.accountId == accountId);
		}
	}
}