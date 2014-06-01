using UnityEngine;
using System.Collections;

public class SetDepthOfFieldFocalTarget : MonoBehaviour {
	private DepthOfFieldScatter depthOfFieldScatter;
	
	// Use this for initialization
	void Start () {
		depthOfFieldScatter = this.GetComponent<DepthOfFieldScatter>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Player.main == null)
			return;
		
		if(depthOfFieldScatter.focalTransform == null)
			depthOfFieldScatter.focalTransform = Player.main.myTransform;
	}
}
