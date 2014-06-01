using UnityEngine;
using System.Collections;

public class LightningAutoAttack : SkillInstance {
	public int minVertices = 2;
	public float verticesPerUnit = 1f;
	
	public Transform leftHandPivot;
	public Transform rightHandPivot;
	public Transform atHitPoint;
	
	public Transform explosionPrefab;
	public float explosionDelay = 0.1f;
	private float explosionTime;
	
	public LightningRenderer[] lightningRenderers;
	
	private Transform myTransform;
	
	// Start
	void Start () {
		myTransform = this.transform;
		
		if(!uLink.Network.isServer) {
			UpdateDistance();
			
			foreach(var lightningRenderer in lightningRenderers) {
				lightningRenderer.UpdateVectors();
			}
		}
		
		explosionTime = 0f;
		
		//Destroy(this.gameObject, 2.0f);
	}
	
	// Update
	void Update () {
		if(!uLink.Network.isServer)
			UpdateDistance();
		
		explosionTime -= Time.deltaTime;
		if(explosionTime <= 0f) {
			this.caster.SpawnExplosion(explosionPrefab, this.hitPoint, Quaternion.identity, this);
			explosionTime = explosionDelay;
		}
	}
	
	// Update distance
	void UpdateDistance() {
		myTransform.position = this.caster.myTransform.position;
		
		leftHandPivot.position = this.caster.leftHand.position;
		leftHandPivot.rotation = Quaternion.LookRotation(this.hitPoint - leftHandPivot.position);
		
		rightHandPivot.position = this.caster.rightHand.position;
		rightHandPivot.rotation = Quaternion.LookRotation(this.hitPoint - rightHandPivot.position);
		
		atHitPoint.position = new Vector3(this.hitPoint.x, this.hitPoint.y * 0.8f + myTransform.position.y * 0.2f, this.hitPoint.z);
		
		Vector3 distance = this.hitPoint - this.caster.handsCenter;
		
		float magnitude = distance.magnitude + 2f;
		int newVertexCount = minVertices + (int)(magnitude * verticesPerUnit);
		
		foreach(var lightningRenderer in lightningRenderers) {
			lightningRenderer.distance = magnitude;
			lightningRenderer.vertexCount = newVertexCount;
		}
	}
}
