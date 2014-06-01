using UnityEngine;

public class IceSwords : SkillInstance {
	// Use this for initialization
	void Start () {
		Destroy(this.gameObject, 7.0f);
		
		/*GameObject swordR = (GameObject)Instantiate(Resources.Load("IceKatana"));
		swordR.transform.parent = this.caster.rightHand;
		swordR.transform.localPosition = Vector3.zero;
		
		Destroy(swordR, 1.0f);
		Destroy(this.gameObject, 1.0f);*/
		
		/*GameObject swordL = (GameObject)Instantiate(Resources.Load("IceKatana"));
		swordL.transform.parent = this.caster.leftHand;
		swordL.transform.localPosition = Vector3.zero;*/
	}
}
