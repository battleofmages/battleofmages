using UnityEngine;
using System.Collections;

public class SnapByte : AudioCodec {
	public const float byteDecodeMultiplier = 1f / 127f;
	
	// Encode
	public void Encode(float[] floats, SendFunc send) {
		byte[] bytes = new byte[floats.Length];
		
		for(int i = 0; i < bytes.Length; i++) {
			bytes[i] = (byte)(floats[i] * 127f + 128);
		}
		
		send(bytes, bytes.Length);
	}
	
	// Decode
	public float[] Decode(byte[] bytes, int encoded) {
		float[] floats = new float[encoded];
		
		for(int i = 0; i < encoded; i++) {
			floats[i] = (bytes[i] - 128) * byteDecodeMultiplier;
		}
		
		return floats;
	}
	
	// Encode float
	public static byte EncodeFloat(float val) {
		return (byte)(val * 127f + 128);
	}
	
	// Decode byte
	public static float DecodeByte(byte val) {
		return (val - 128) * byteDecodeMultiplier;
	}
	
	// EncodeFloats
	public static byte[] EncodeFloats(float[] floats) {
		byte[] bytes = new byte[floats.Length];
		
		for(int i = 0; i < bytes.Length; i++) {
			bytes[i] = (byte)(floats[i] * 127f + 128);
		}
		
		return bytes;
	}
	
	// DecodeBytes
	public static float[] DecodeBytes(byte[] bytes) {
		float[] floats = new float[bytes.Length];
		
		for(int i = 0; i < bytes.Length; i++) {
			floats[i] = (bytes[i] - 128) * byteDecodeMultiplier;
		}
		
		return floats;
	}
}