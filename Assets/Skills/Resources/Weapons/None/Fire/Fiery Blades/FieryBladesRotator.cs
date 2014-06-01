using UnityEngine;
using System.Collections;

public class FieryBladesRotator : SkillInstance {
	public SkillInstance skillInstanceParent;
	//public float duration;
	public float rotationSpeed;
	public Vector3 offset;
	
	private Transform myTransform;
	
	// Use this for initialization
	void Start () {
		this.caster = skillInstanceParent.caster;
		this.skill = skillInstanceParent.skill;
		this.skillStage = skillInstanceParent.skillStage;
		this.gameObject.layer = skillInstanceParent.gameObject.layer;
		
		myTransform = this.transform;
		//Destroy(this.gameObject, duration);
	}
	
	// Update is called once per frame
	void Update () {
		myTransform.position = this.caster.myTransform.position + offset;
		myTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
	}
}
