using UnityEngine;
using System.Collections;
using uLink;

public class GenerateSecurityKeys : uLink.MonoBehaviour {
	public int bitStrength = 128;
	
	// Use this for initialization
	void Start() {
		//var privateKey = uLink.PrivateKey.Generate(bitStrength);
		//LogManager.General.Log(privateKey);
		
		var pkey = new uLink.PublicKey(
			"td076m4fBadO7bRuEkoOaeaQT+TTqMVEWOEXbUBRXZwf1uR0KE8A/BbOWNripW1eZinvsC+skgVT/G8mrhYTWVl0TrUuyOV6rpmgl5PnoeLneQDEfrGwFUR4k4ijDcSlNpUnfL3bBbUaI5XjPtXD+2Za2dRXT3GDMrePM/QO8xE=",
			"EQ=="
		);
		
		LogManager.General.Log(pkey);
	}
}
