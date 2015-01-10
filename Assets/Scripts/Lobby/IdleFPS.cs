using UnityEngine;
using System.Collections;

public class IdleFPS : SingletonMonoBehaviour<IdleFPS> {
	public bool lowerFPSWhenIdle = true;
	public int lobbyFrameRate;
	public int lobbyFrameRateIdle;

	// On application focus
	void OnApplicationFocus(bool focused) {
		if(!lowerFPSWhenIdle)
			return;
		
		if(focused) {
			Application.targetFrameRate = GameManager.inGame ? -1 : lobbyFrameRate;
			LogManager.Spam.Log("Application focused. Target frame rate: " + Application.targetFrameRate);
		} else {
			Application.targetFrameRate = lobbyFrameRateIdle;
			LogManager.Spam.Log("Application unfocused. Target frame rate: " + Application.targetFrameRate);
		}
	}
}
