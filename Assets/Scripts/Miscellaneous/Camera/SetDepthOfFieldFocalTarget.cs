using UnityEngine;

public class SetDepthOfFieldFocalTarget : MonoBehaviour {
	private DepthOfFieldScatter depthOfFieldScatter;
	
	// Start
	void Start() {
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
