using UnityEngine;
using System.Collections;

public class LZFCodec : AudioCodec {
	// Encode
	public void Encode(float[] floats, SendFunc send) {
		byte[] bytes = CLZF2.Compress(SnapByte.EncodeFloats(floats));
		
		send(bytes, bytes.Length);
	}
	
	// Decode
	public float[] Decode(byte[] bytes, int encoded) {
		float[] floats = SnapByte.DecodeBytes(CLZF2.Decompress(bytes));
		
		return floats;
	}
	
}