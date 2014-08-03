public class ChatMember {
	public string accountId;

	// Constructor
	public ChatMember() {
		accountId = "";
	}

	// Constructor
	public ChatMember(string nName) {
		accountId = nName;
	}

	// WriteToBitStream
	public static void WriteToBitStream(uLink.BitStream stream, object val, params object[] args) {
		var myObj = (ChatMember)val;
		stream.WriteString(myObj.accountId);
	}

	// ReadFromBitStream
	public static object ReadFromBitStream(uLink.BitStream stream, params object[] args) {
		var myObj = new ChatMember(stream.ReadString());
		return myObj;
	}
}
