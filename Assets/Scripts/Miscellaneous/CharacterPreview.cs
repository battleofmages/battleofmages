using UnityEngine;

public class CharacterPreview : SingletonMonoBehaviour<CharacterPreview> {
	public static Vector3 localPosition;
	public static Quaternion localRotation;
	
	// Awake
	protected override void Awake () {
		bool createdBefore = CharacterPreview.instance != null;
		
		base.Awake();
		
		if(CharacterPreview.instance != null) {
			CharacterPreview.instance.transform.parent = GameObject.Find("CharacterPreviewPivot").transform;
			
			if(!createdBefore) {
				localPosition = CharacterPreview.instance.transform.localPosition;
				localRotation = CharacterPreview.instance.transform.localRotation;
			} else {
				CharacterPreview.instance.transform.localPosition = localPosition;
				CharacterPreview.instance.transform.localRotation = localRotation;
			}
		}
	}
	
	// Start
	void Start() {
		this.gameObject.SetActive(false);
	}
}
