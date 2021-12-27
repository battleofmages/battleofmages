using Unity.Netcode;

namespace BoM.Players {
	public class Flight : NetworkBehaviour {
		public NetworkVariable<bool> isActive;
		public Movement movement;
		public Gravity gravity;
		public Animations animations;
		public Health health;
		public float speed;
		private float controllerHeight;

		public bool canFly {
			get {
				return health.isAlive;
			}
		}

		private void Awake() {
			isActive.OnValueChanged += OnFlightStateChanged;
		}

		public override void OnNetworkSpawn() {
			if(IsClient) {
				isActive.OnValueChanged(enabled, isActive.Value);
			}
		}

		private void OnFlightStateChanged(bool oldState, bool newState) {
			enabled = newState;
		}

		public void Activate() {
			if(!canFly) {
				return;
			}

			enabled = true;
			StartServerRpc();
		}

		public void Deactivate() {
			enabled = false;
			StopServerRpc();
		}

		private void OnEnable() {
			animations.Animator.SetBool("Flying", true);
			movement.speed = speed;
			controllerHeight = movement.controller.height;
			movement.controller.height = 1f;
		}

		private void OnDisable() {
			animations.Animator.SetBool("Flying", false);
			movement.speed = movement.baseSpeed;
			movement.controller.height = controllerHeight;
		}

		private void Update() {
			gravity.Speed = 0f;
		}

		[ServerRpc]
		public void StartServerRpc() {
			if(!canFly) {
				return;
			}

			isActive.Value = true;
		}

		[ServerRpc]
		public void StopServerRpc() {
			isActive.Value = false;
		}
	}
}
