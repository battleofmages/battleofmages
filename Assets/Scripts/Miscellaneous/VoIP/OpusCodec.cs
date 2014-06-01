/*using UnityEngine;
using System.Collections;
using FragLabs.Audio.Codecs;

public class OpusCodec : AudioCodec {
	private OpusEncoder encoder;
	private OpusDecoder decoder;
	
	int _segmentFrames;
	int unitsPerSegment;
	float[] _notEncodedBuffer = new float[0];
	
	public OpusCodec() {
		encoder = OpusEncoder.Create(48000, 1, FragLabs.Audio.Codecs.Opus.Application.Voip);
		encoder.Bitrate = 8192 * 4;
		
		_segmentFrames = 960;
		encoder.MaxDataBytes = _segmentFrames * 2 * 4;
		unitsPerSegment = encoder.FrameByteCount(_segmentFrames);
		
		decoder = OpusDecoder.Create(48000, 1);
		decoder.MaxDataBytes = _segmentFrames * 2 * 4;
	}
	
	// Encode
	public void Encode(float[] floats, SendFunc send) {
		var soundBuffer = new float[floats.Length + _notEncodedBuffer.Length];
		for (int i = 0; i < _notEncodedBuffer.Length; i++)
			soundBuffer[i] = _notEncodedBuffer[i];
		for (int i = 0; i < floats.Length; i++)
			soundBuffer[i + _notEncodedBuffer.Length] = floats[i];
		
		int segmentCount = (int)Mathf.Floor((float)soundBuffer.Length / unitsPerSegment);
		int segmentsEnd = segmentCount * unitsPerSegment;
		int notEncodedCount = soundBuffer.Length - segmentsEnd;
		
		_notEncodedBuffer = new float[notEncodedCount];
		for(int i = 0; i < notEncodedCount; i++) {
			_notEncodedBuffer[i] = soundBuffer[segmentsEnd + i];
		}
		
		int _bytesSent = 0;
		
		for(int i = 0; i < segmentCount; i++) {
			float[] segment = new float[unitsPerSegment];
			for(int j = 0; j < segment.Length; j++)
				segment[j] = soundBuffer[(i * unitsPerSegment) + j];
			
			int len;
			byte[] buff = encoder.EncodeFloat(segment, segment.Length, out len);
			Debug.Log(string.Format("Sending packet length {0}, BytesSent: {1}, Floats.Length: {2}, Segments: {3}, SegmentsEnd: {4}", len, _bytesSent, floats.Length, segmentCount, segmentsEnd));
			//System.Array.Resize(ref buff, len);
			send(buff, len);
			_bytesSent += len;
			//buff = _decoder.Decode(buff, len, out len);
			//_playBuffer.AddSamples(buff, 0, len);
		}
	}
	
	// Decode
	public float[] Decode(byte[] bytes, int encoded) {
		int decodedLength;
		float[] floats = decoder.DecodeFloat(bytes, encoded, out decodedLength);
		Debug.Log(string.Format("Receiving packet length {0}, Decoded: {1}, Floats.Length: {2}", bytes.Length, decodedLength, floats.Length));
		
		//System.Array.Resize(ref floats, decodedLength);
		
		return floats;
	}
}*/