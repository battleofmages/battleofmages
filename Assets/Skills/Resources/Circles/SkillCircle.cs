using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Projector))]

public class SkillCircle : MonoBehaviour {
	public SkillInstance skillInstance;
	
	// Use this for initialization
	void Start () {
		var castCircleProjector = this.GetComponent<Projector>();
		
		if(uLink.Network.isServer) {
			this.enabled = false;
			castCircleProjector.enabled = false;
			return;
		}
		
		// Set cast circle color
		if(castCircleProjector != null) {
			if(skillInstance.caster.layer != Player.main.layer) {
				castCircleProjector.material = GameServerParty.enemySkillCircleMaterial; //skillInstance.caster.party.skillCircleMaterial;
			} else {
				castCircleProjector.enabled = false;
			}
		}
	}
}
