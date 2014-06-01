using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]

public class LightningRenderer : MonoBehaviour {
	public float width = 0.25f;
	public float jump = 1f;
	public float interpolationSpeed = 35f;
	public float delay = 0.02f;
	public float distance = 100f;
	public int startVertexCount = 100;
	public float fadeOutTime = 0f;
	
	private int _vertexCount;
	
	private LineRenderer lineRenderer;
	private float remainingTime;
	
	private Vector3[] currentPosition;
	private Vector3[] nextPosition;
	
	private float currentFadeOutTime = 0f;
	
	// Use this for initialization
	void Awake() {
		lineRenderer = this.GetComponent<LineRenderer>();
		vertexCount = startVertexCount;
		
		lineRenderer.SetWidth(width, width);
		remainingTime = delay;
		
		if(fadeOutTime != 0f) {
			FadeOut(fadeOutTime);
		}
	}
	
	// Update
	void Update() {
		if(!uLink.Network.isServer) {
			if(delay == 0f)
				delay = 0.001f;
			
			remainingTime -= Time.deltaTime;
			
			if(remainingTime < 0f) {
				UpdateVectors();
				remainingTime = 0f;
			}
			
			for(int i = 0; i < _vertexCount; i++) {
				currentPosition[i] = Vector3.Lerp(currentPosition[i], nextPosition[i], (delay - remainingTime) / delay);
				lineRenderer.SetPosition(i, currentPosition[i]);
			}
			
			if(remainingTime <= 0f) {
				remainingTime = delay;
			}
		}
		
		// Fade out
		if(fadeOutTime != 0f) {
			currentFadeOutTime += Time.deltaTime;
			if(currentFadeOutTime < fadeOutTime) {
				float newWidth = width - width * (currentFadeOutTime / fadeOutTime);
				lineRenderer.SetWidth(newWidth, newWidth);
			} else {
				Destroy(this.gameObject);
			}
		}
	}
	
	// Update vectors
	public void UpdateVectors() {
		float zScale = distance / _vertexCount;
		
		// First
		nextPosition[0] = Vector3.zero;
		currentPosition[0] = nextPosition[0];
		
		// Middle
		//Debug.Log("Update vectors: " + _vertexCount + ", " + zScale + ", " + distance);
		for(int b = 1; b < _vertexCount; b++) {
			nextPosition[b] = new Vector3(Random.Range(-jump, jump), Random.Range(-jump, jump), b * zScale);
			currentPosition[b].z = nextPosition[b].z;
		}
		
		// Last
		nextPosition[_vertexCount - 1] = new Vector3(0, 0, (_vertexCount - 1) * zScale);
		currentPosition[_vertexCount - 1].z = nextPosition[_vertexCount - 1].z;
	}
	
	// Fade out
	public void FadeOut(float nFadeOutTime) {
		this.transform.parent = Config.particlesRoot;
		fadeOutTime = nFadeOutTime;
		currentFadeOutTime = 0f;
		Destroy(this.gameObject, nFadeOutTime);
	}
	
	// Vertex count
	public int vertexCount {
		get {
			return _vertexCount;
		}
		
		set {
			if(_vertexCount == value)
				return;
			
			//Debug.Log("Update vertex count: " + _vertexCount);
			
			_vertexCount = value;
			
			if(currentPosition != null) {
				var currentPositionTmp = currentPosition;
				var nextPositionTmp = nextPosition;
				
				currentPosition = new Vector3[_vertexCount];
				nextPosition = new Vector3[_vertexCount];
				
				var length = Mathf.Min(_vertexCount, currentPositionTmp.Length);
				
				float zScale = distance / _vertexCount;
				for(int i = 0; i < length; i++) {
					currentPosition[i] = currentPositionTmp[i];
					nextPosition[i] = nextPositionTmp[i];
					
					nextPosition[i].z = i * zScale;
					currentPosition[i].z = nextPosition[i].z;
				}
			} else {
				currentPosition = new Vector3[_vertexCount];
				nextPosition = new Vector3[_vertexCount];
			}
			
			lineRenderer.SetVertexCount(_vertexCount);
		}
	}
}
