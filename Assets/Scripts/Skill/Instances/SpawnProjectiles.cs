using UnityEngine;
using UnityEngine.Pool;

namespace BoM.Skill.Instances {
	public class SpawnProjectiles : Instance {
		public Projectile projectile;
		public Transform spawn;
		public float radius;
		public float interval;
		public float duration;
		public bool randomizeAngle;

		private float totalTime;
		private float intervalTime;
		private ObjectPool<Instance> projectilePool;

		private void Awake() {
			projectilePool = PoolManager.GetPool(projectile);
		}

		public override void Init() {
			totalTime = 0f;
			intervalTime = 0f;
		}

		private void Update() {
			totalTime += Time.deltaTime;
			intervalTime += Time.deltaTime;

			while(intervalTime >= interval) {
				intervalTime -= interval;
				SpawnProjectile();
			}

			if(totalTime >= duration) {
				Release();
			}
		}

		void SpawnProjectile() {
			var random = Random.insideUnitCircle * radius;
			var offset = new Vector3(random.x, 0f, random.y);
			var instance = projectilePool.Get();
			instance.transform.position = spawn.position + offset;

			if(randomizeAngle) {
				var targetPoint = Random.insideUnitCircle * radius;
				var targetOffset = new Vector3(targetPoint.x, 0f, targetPoint.y);
				var target = transform.position + targetOffset;
				instance.transform.rotation = Quaternion.LookRotation(target - instance.transform.position);
			} else {
				instance.transform.rotation = spawn.rotation;
			}

			instance.skill = skill;
			instance.pool = projectilePool;
			instance.Init();
		}
	}
}
