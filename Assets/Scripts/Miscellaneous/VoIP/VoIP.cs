using UnityEngine;
using System;

public class VoIP : uLink.MonoBehaviour {
	public static float volumeMultiplier = 1f;

	private int _frequency = 48000;
	private float noiseLimit = 0.017f;
	
	public GameObject audioSourcesObject;
	public Texture2D visualizationTex;
	private AudioCodec codec = new LZFCodec();
	private AudioSource[] audioSources;
	private int lastPos;
	private AudioClip microphoneClip;
	private int sendPacketFrequency = 10;
	private int pushToTalkButton;
	private bool idleDetectionEnabled = false;
	private int sampleBufferSize;
	//private float lastSend = 0f;
	
	private int minFrequency;
	private int maxFrequency;
	
	private float[] samples;
	
	private float visualizationTime;
	private float[] visualizationSamples = null;
	private float[] nextVisualizationSamples;
	private int visualizationWidth = 62;
	
	private VoIPSpeaker speaker;
	
	protected bool networkViewIsMine = false;

	// Frequency
	public int frequency {
		get {
			return _frequency;
		}
		
		set {
			_frequency = value;

			if(GameManager.isClient)
				speaker.CreateAudioClip(_frequency);
		}
	}
	
	// Start
	void Start() {
#if UNITY_WEBPLAYER
		// TODO: Request authorization
		this.enabled = false;
		return;
#endif
		sampleBufferSize = frequency / sendPacketFrequency;
		nextVisualizationSamples = new float[sampleBufferSize];
		
		samples = null;
		audioSources = audioSourcesObject.GetComponents<AudioSource>();
		speaker = audioSourcesObject.GetComponent<VoIPSpeaker>();
		networkViewIsMine = networkView.isMine;
		
		if(networkViewIsMine) {
			pushToTalkButton = InputManager.instance.GetButtonIndex("push_to_talk");
			
			Microphone.GetDeviceCaps(null, out minFrequency, out maxFrequency);
			LogManager.General.Log(string.Format("[VoIP] Microphone frequencies: Min: {0}, Max: {1}", minFrequency, maxFrequency));
			
			if(_frequency < minFrequency)
				_frequency = minFrequency;
			
			if(_frequency > maxFrequency && maxFrequency != 0)
				_frequency = maxFrequency;

			LogManager.General.Log("[VoIP] Microphone sample buffer size: " + sampleBufferSize);
			LogManager.General.Log("[VoIP] Microphone frequency: " + frequency);
			
			microphoneClip = Microphone.Start(null, true, 10, frequency);
			while(Microphone.GetPosition(null) < 0) {} // HACK from Riro

			// Let the server know about my frequency
			networkView.RPC("ClientVoIPFrequency", uLink.RPCMode.Server, frequency);
		}
	}
	
	// FixedUpdate
	void FixedUpdate() {
		if(GameManager.isServer)
			return;
		
		if(networkViewIsMine) {
			if(InputManager.instance.GetButton(pushToTalkButton)) {
				int pos = Microphone.GetPosition(null);
				int diff;
				
				if(pos >= lastPos) {
					diff = pos - lastPos;
				} else {
					diff = (microphoneClip.samples - lastPos) + pos;
				}
				
				if(diff >= sampleBufferSize) {
					//LogManager.General.Log(Time.time - lastSend + ": " + diff);
					samples = new float[sampleBufferSize]; // * microphoneClip.channels
					microphoneClip.GetData(samples, lastPos);
					
					// Noise removal
					if(idleDetectionEnabled) {
						int idleCounter = 0;
						for(int i = 0; i < samples.Length; i++) {
							float val = samples[i];
							if(val < noiseLimit && val > -noiseLimit) {
								idleCounter++;
							}
						}
						
						// Don't send if it's completely calm
						if(idleCounter == samples.Length) {
							networkView.RPC("VoIPData", uLink.RPCMode.Server, new byte[0]);
							return;
						}
					}
					
					// Clamp
					for(int i = 0; i < samples.Length; i++) {
						samples[i] = Mathf.Clamp(samples[i] * volumeMultiplier, -1f, 1f);
					}
					
					codec.Encode(samples, (bytes, len) => {
						networkView.RPC("VoIPData", uLink.RPCMode.Server, bytes, len);
						OnVoIPData(bytes, len, null);
					});
					
					//lastSend = Time.time;
					lastPos = pos - (diff - sampleBufferSize);
					if(lastPos < 0)
						lastPos = microphoneClip.samples + lastPos;
				}
			} else {
				visualizationSamples = null;
				audioSources[0].Pause();
				lastPos = Microphone.GetPosition(null);
			}
		} else {
			if(!speaker.isPlaying) {
				visualizationSamples = null;
				audioSources[0].Pause();
			}
		}
		
		visualizationTime += Time.deltaTime * sendPacketFrequency;
	}
	
	// OnGUI
	public void OnGUI() {
		if(GameManager.isServer)
			return;
		
		//noiseLimit = GUI.HorizontalSlider(new Rect(5, 5, Screen.width - 10, 24), noiseLimit, 0f, 0.017f);
		
		if(visualizationSamples == null)
			return;
		
		if(visualizationTex == null)
			return;
		
		Vector3 screenPosition = Camera.main.WorldToScreenPoint(audioSourcesObject.transform.position);
		screenPosition.y = Screen.height - (screenPosition.y + 1);
		
		var width = visualizationWidth + 2;
		var height = 24;
		var maxWidth = Mathf.Min(visualizationSamples.Length, visualizationWidth);
		var halfHeight = height / 2;
		
		GUI.BeginGroup(new Rect(screenPosition.x + 28f, screenPosition.y - halfHeight, width, height));
		GUI.Box(new Rect(0, 0, width, height), "");
		
		for(int i = 0; i < maxWidth; i++) {
			GUI.DrawTexture(new Rect(i + 1, halfHeight, 1, Mathf.Lerp(visualizationSamples[i], nextVisualizationSamples[i], visualizationTime) * (halfHeight - 1)), visualizationTex);
		}
		
		GUI.EndGroup();
	}
	
	// Calculate new visualization
	float[] CalculateNewVisualization() {
		var newSamples = new float[visualizationWidth];
		
		if(samples == null || samples.Length == 0)
			return newSamples;
		
		var combineNum = Mathf.CeilToInt((float)samples.Length / newSamples.Length);
		
		int counter = -1;
		for(int i = 0; i < samples.Length; i++) {
			if(i % combineNum == 0) {
				if(counter != -1)
					newSamples[counter] /= combineNum;
				counter++;
			}
			
			newSamples[counter] += samples[i];
		}
		
		// Divide last one
		newSamples[visualizationWidth - 1] /= combineNum;
		
		return newSamples;
	}
	
	// OnVoIPData
	public void OnVoIPData(byte[] bytes, int len, uLink.NetworkMessageInfo info) {
		if(GameManager.isServer) {
			if(info.sender != networkView.owner)
				return;
			
			networkView.RPC("VoIPData", uLink.RPCMode.OthersExceptOwner, bytes, len);
			// ExceptOwner
			return;
		}
		
		// Noise detection
		if(bytes.Length == 0) {
			samples = new float[0];
			return;
		}

		// Decode samples
		samples = codec.Decode(bytes, len);
		
		// Play
		if(!networkViewIsMine || Config.instance.hearMyself) {
			if(!audioSources[0].isPlaying)
				audioSources[0].Play();
			
			speaker.AddSamples(samples);
		}
		
		// Visualization
		visualizationSamples = nextVisualizationSamples;
		nextVisualizationSamples = CalculateNewVisualization();
		visualizationTime = 0f;
	}
}