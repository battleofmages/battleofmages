using UnityEngine;
using System.Collections;

public class ThunderboltInstance : SkillInstance {

	// Use this for initialization
	void Start () {
		Destroy(this.gameObject, 2.0f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
