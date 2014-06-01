using UnityEngine;
using System.Collections;

[System.Serializable]
public class AttunementBuild : JsonSerializable<AttunementBuild> {
	public int attunementId;
	public int[] skills;

	// Constructor
	public AttunementBuild() {
		
	}
	
	// BitStream Writer
	public static void WriteToBitStream(uLink.BitStream stream, object val, params object[] args) {
		var obj = (AttunementBuild)val;
		
		stream.WriteByte((byte)obj.attunementId);
		stream.Write<int[]>(obj.skills);
	}
	
	// BitStream Reader
	public static object ReadFromBitStream(uLink.BitStream stream, params object[] args) {
		var obj = new AttunementBuild();
		
		obj.attunementId = (int)stream.ReadByte();
		obj.skills = stream.Read<int[]>();
		
		return obj;
	}
}
