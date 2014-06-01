using UnityEngine;
using System.Collections;

public class SingleIceSword : SkillInstance {
	public Transform skillTransform;
	public Transform explosionPrefab;
	
	private Quaternion targetRotation;
	private Quaternion currentRotation;
	
	//private bool exploded = false;
	
	// Use this for initialization
	void Start () {
		SkillInstance inst = skillTransform.GetComponent<SkillInstance>();
		this.caster = inst.caster;
		this.skill = inst.skill;
		this.skillStage = inst.skillStage;
		this.hitPoint = inst.hitPoint;
		
		this.gameObject.layer = skillTransform.gameObject.layer;
		
		currentRotation = transform.rotation;
		targetRotation = Quaternion.LookRotation(this.transform.position - this.hitPoint);
		
		Invoke("StartMoving", 1.5f);
	}
	
	// Update is called once per frame
	void Update () {
		currentRotation = Quaternion.Lerp(currentRotation, targetRotation, Time.deltaTime * 3);
		
		// Rotation correction
		this.transform.rotation = currentRotation;
		Vector3 angles = this.transform.eulerAngles;
		angles.x += 90;
		this.transform.eulerAngles = angles;
	}
	
	void StartMoving() {
		rigidbody.AddForce(targetRotation * Vector3.forward * -5000);
	}
	
	void OnTriggerEnter(Collider coll) {
		//if(true) {//!exploded) {
		this.caster.SpawnExplosion(explosionPrefab, transform.position, transform.rotation, this);
			//exploded = true;
		//}
	}
}
