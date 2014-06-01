using System.Collections.Generic;

public class Party<T> where T : PartyMember<T> {
	public const int Undefined = -1;
	
	protected int _id;
	public string name;
	protected List<T> _members;
	public int expectedMemberCount;
	
	// Constructor
	public Party() {
		name = "";
		_id = 0;
		_members = new List<T>();
		expectedMemberCount = 0;
	}
	
	// Adds an entity to the party
	public void AddMember(T p) {
		if(p.GetParty() == this)
			return;
		
		_members.Add(p);
		p.SetParty(this);
	}
	
	// Removes an entity from the party
	public void RemoveMember(T p) {
		if(p.GetParty() != this)
			return;
		
		_members.Remove(p);
		p.SetParty(null);
	}
	
	// Removes all members
	public void RemoveAllMembers() {
		foreach(var member in _members) {
			RemoveMember(member);
		}
	}
	
#region Properties
	// Members
	public List<T> members {
		get { return _members; }
	}
	
	// Member count
	public int memberCount {
		get {
			return _members.Count;
		}
	}
	
	// Party ID
	public int id {
		get { return _id; }
	}
#endregion
}
