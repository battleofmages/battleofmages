using UnityEngine;
using System.Collections;

public class NoAudioCompression : AudioCodec {
	// Encode
	public void Encode(float[] floats, SendFunc send) {
		int len = floats.Length * 4;
		byte[] bytes = new byte[len];
		
		int pos = 0;
		
		// TODO: Pretty sure this could be done faster...
		foreach(float f in floats) {
			byte[] data = System.BitConverter.GetBytes(f);
			System.Array.Copy(data, 0, bytes, pos, 4);
			pos += 4;
		}
		
		send(bytes, bytes.Length);
	}
	
	// Decode
	public float[] Decode(byte[] bytes, int encoded) {
		int len = encoded / 4;
		float[] floats = new float[len];
		
		for(int i = 0; i < encoded; i+=4) {
			floats[i / 4] = System.BitConverter.ToSingle(bytes, i);
		}
		
		return floats;
	}
}