using UnityEngine;

public class VoIPSpeaker : MonoBehaviour {
	private float[] samplesBuffer;
	private int playOffset;
	private int dataOffset;
	
	private long realPlayOffset;
	private long realDataOffsetStart;
	private long realDataOffsetEnd;
	
	private float lastSample;
	private int delay = 1024;

	// Awake
	void Awake() {
		int dspBufferSize;
		int dspBufferCount;
		AudioSettings.GetDSPBufferSize(out dspBufferSize, out dspBufferCount);
		LogManager.General.Log("[VoIP] DSP buffer size: " + dspBufferSize + ", " + dspBufferCount);
		
		samplesBuffer = new float[dspBufferSize * dspBufferCount * 16];
		realPlayOffset = 0;
		realDataOffsetStart = 0;
		realDataOffsetEnd = 0;
		playOffset = 0;
		dataOffset = 0;
	}

	// OnAudioFilterRead
	void OnAudioFilterRead(float[] data, int channels) {
		int dataLen = data.Length / channels;
		
		for(int i = 0; i < dataLen; i++) {
			if(realPlayOffset + i < realDataOffsetStart || realPlayOffset + i >= realDataOffsetEnd) {
				// Data didn't arrive yet
				lastSample *= 0.9f;
			} else {
				// Use the data we received
				lastSample = samplesBuffer[(playOffset + i) % samplesBuffer.Length];
			}
			
			for(int c = 0; c < channels; c++) {
				data[i * channels + c] *= lastSample;
			}
		}
		
		realPlayOffset += dataLen;
		playOffset = (int)(realPlayOffset % samplesBuffer.Length);
		
		if(realDataOffsetEnd < realPlayOffset) {
			//Debug.LogWarning("Not enough data! " + realDataOffset + " -> " + realPlayOffset);
			realDataOffsetStart = realPlayOffset;
			realDataOffsetEnd = realPlayOffset;
			dataOffset = (int)(realDataOffsetEnd % samplesBuffer.Length);
		}
	}

	// AddSamples
	public void AddSamples(float[] samples) {
		// Delay
		if(realPlayOffset == realDataOffsetEnd) {
			LogManager.Spam.Log("Audio data shift!");
			realDataOffsetEnd += delay;
			realDataOffsetStart = realDataOffsetEnd;
			dataOffset = (int)(realDataOffsetEnd % samplesBuffer.Length);
		}
		
		for(int i = 0; i < samples.Length; i++) {
			samplesBuffer[dataOffset] = samples[i];
			
			dataOffset++;
			realDataOffsetEnd++;
			
			if(dataOffset >= samplesBuffer.Length)
				dataOffset = 0;
		}
	}

	// Is playing
	public bool isPlaying {
		get {
			return realPlayOffset >= realDataOffsetStart && realPlayOffset < realDataOffsetEnd;
		}
	}
}
