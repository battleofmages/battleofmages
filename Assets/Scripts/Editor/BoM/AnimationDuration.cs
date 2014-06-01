using UnityEngine;
using UnityEditor;

public class AnimationDuration : MonoBehaviour {
	[MenuItem("Battle of Mages/Update animation duration")]
	public static void UpdateAnimationDuration() {
		var controller = Selection.activeGameObject.GetComponent<Animator>().runtimeAnimatorController;

		if(controller is UnityEditorInternal.AnimatorController) {
			var stateMachine = ((UnityEditorInternal.AnimatorController)controller).GetLayer(Entity.AnimationLayer.Skill).stateMachine;
			
			for(int i = 0; i < stateMachine.stateCount; i++) {
				var state = stateMachine.GetState(i);
				var motion = state.GetMotion();
				if(motion != null)
					Debug.Log(motion.name + ": " + motion.averageDuration);

				// Obviously loading it depends on where/how clip is stored, best case its a resource, worst case you have to search asset database.
				/*var clip = (AnimationClip)Resources.LoadAssetAtPath(
					"Assets/Animations/" + stateMachine.GetState(i).GetMotion().name + ".anim",
					typeof(AnimationClip)
				);
				
				if(clip) {
					Debug.Log(clip.name + ": " + clip.length);
				}*/
			}
		}
	}
}
