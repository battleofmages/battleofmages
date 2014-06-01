using UnityEngine;

public class CharacterAnimationTest : MonoBehaviour {
	public bool katanaCombo1;
	public Animator animator;

	// Start
	void Start() {
		
	}
	
	// OnGUI
	void OnGUI() {
		if(GUILayout.Button("FireSideSlashLToR"))
			animator.SetBool("FireSideSlashLToR", true);
		else
			animator.SetBool("FireSideSlashLToR", false);
	}
}
