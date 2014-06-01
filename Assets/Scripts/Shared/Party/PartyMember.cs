public interface PartyMember<T> where T : PartyMember<T> {
	void SetParty(Party<T> pty);
	Party<T> GetParty();
	string GetAccountId();
}