using UnityEngine;
using System.Collections;

[System.Serializable]
public class KeyValue<T> : JsonSerializable<KeyValue<T>> {
	public string key;
	public T val;
	
	public KeyValue() {
		key = default(string);
		val = default(T);
	}
	
	// BitStream Writer
	public static void WriteToBitStream(uLink.BitStream stream, object val, params object[] args) {
		var obj = (KeyValue<T>)val;
		
		stream.WriteString(obj.key);
		stream.Write<T>(obj.val);
	}
	
	// BitStream Reader
	public static object ReadFromBitStream(uLink.BitStream stream, params object[] args) {
		var obj = new KeyValue<T>();
		
		obj.key = stream.ReadString();
		obj.val = stream.Read<T>();
		
		return obj;
	}
}