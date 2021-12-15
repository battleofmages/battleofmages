using UnityEngine;

public class Animations : MonoBehaviour {
	public Animator animator;
	public Player player;

	private void Update() {
		animator.SetFloat("Speed", player.controller.velocity.sqrMagnitude);
		animator.SetFloat("Gravity", player.gravity.Speed);
		animator.SetBool("Grounded", player.controller.isGrounded);
		// animator.SetBool("Attack", fire);
		// animator.SetBool("Block", block);
		// fire = false;
	}
}
