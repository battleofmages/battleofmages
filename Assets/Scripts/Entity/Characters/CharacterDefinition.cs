using UnityEngine;

public class CharacterDefinition : MonoBehaviour {
	//public GameObject model;
	//private Transform modelTransform;

	public string hairName = "Hair";
	public string bodyName = "Body";
	public string eyesName = "Eyes";
	public string cloakName = "Cloak";
	public string legWearName = "LegWear";
	public string topWearName = "TopWear";
	public string bootsName = "Boots";
	public string leftHandName = "S_Hand_L";
	public string rightHandName = "S_Hand_R";
	public string hipsName = "S_Hips";
	public string weaponBoneName = "S_Weapon";

	// Awake
	void Awake() {
		/*modelTransform = ((GameObject)Instantiate(model)).transform;
		modelTransform.parent = transform;
		modelTransform.localPosition = Cache.vector3Zero;
		modelTransform.localRotation = Cache.quaternionIdentity;*/

		hair = GetChild(hairName);
		body = GetChild(bodyName);
		eyes = GetChild(eyesName);
		cloak = GetChild(cloakName);
		legWear = GetChild(legWearName);
		topWear = GetChild(topWearName);
		boots = GetChild(bootsName);
		leftHand = GetChild(leftHandName);
		rightHand = GetChild(rightHandName);
		hips = GetChild(hipsName);
		weaponBone = GetChild(weaponBoneName);
	}

	// GetChild
	Transform GetChild(string objName) {
		return transform.FindChild(objName);
	}

	// Hair
	public Transform hair {
		get;
		protected set;
	}

	// Body
	public Transform body {
		get;
		protected set;
	}

	// Eyes
	public Transform eyes {
		get;
		protected set;
	}

	// Cloak
	public Transform cloak {
		get;
		protected set;
	}

	// Leg wear
	public Transform legWear {
		get;
		protected set;
	}

	// Top wear
	public Transform topWear {
		get;
		protected set;
	}

	// Boots
	public Transform boots {
		get;
		protected set;
	}

	// Left hand
	public Transform leftHand {
		get;
		protected set;
	}

	// Right hand
	public Transform rightHand {
		get;
		protected set;
	}

	// Hips
	public Transform hips {
		get;
		protected set;
	}

	// Weapon
	public Transform weaponBone {
		get;
		protected set;
	}
}