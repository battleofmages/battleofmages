using UnityEngine;
using System.Collections;

public class MagicCircle : CastEffect {
	// Start
	void Start() {
		transform.parent = this.caster.myTransform;
		transform.localPosition = new Vector3(0, 1, 0);
		transform.localRotation = Quaternion.AngleAxis(this.caster.magicCircleYRotation, Vector3.up) * Quaternion.LookRotation(Vector3.down);
	}
	
	// Stop
	public override void Stop() {
		this.caster.magicCircleYRotation = transform.localRotation.eulerAngles.y;
		GetComponent<Projector>().enabled = false;
		GetComponent<Light>().enabled = false;
	}
}
