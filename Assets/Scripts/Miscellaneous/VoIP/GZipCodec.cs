/*using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.Compression;

public class GZipCodec : AudioCodec {
	// Encode
	public void Encode(float[] floats, SendFunc send) {
		byte[] bytes = Compress(SnapByte.EncodeFloats(floats));
		
		send(bytes, bytes.Length);
	}
	
	// Decode
	public float[] Decode(byte[] bytes, int encoded) {
		float[] floats = SnapByte.DecodeBytes(Decompress(bytes));
		
		return floats;
	}
	
	/// <summary>
	/// Compresses byte array to new byte array.
	/// </summary>
	public static byte[] Compress(byte[] raw) {
		using(MemoryStream memory = new MemoryStream()) {
			using(GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true)) {
				gzip.Write(raw, 0, raw.Length);
			}
			
			return memory.ToArray();
		}
	}
	
	public static byte[] Decompress(byte[] gzip) {
		using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress)) {
			const int size = 4096;
			byte[] buffer = new byte[size];
			
			using (MemoryStream memory = new MemoryStream()) {
				int count = 0;
				do {
					count = stream.Read(buffer, 0, size);
					if (count > 0) {
						memory.Write(buffer, 0, count);
					}
				} while (count > 0);
				
				return memory.ToArray();
			}
		}
	}
}*/