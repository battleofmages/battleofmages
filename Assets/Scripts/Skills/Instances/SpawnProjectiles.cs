using BoM.Core;
using UnityEngine;
using UnityEngine.Pool;

namespace BoM.Skills.Instances {
	// Data
	public abstract class SpawnProjectilesData : Instance {
		[SerializeField] protected Projectile projectilePrefab;
		[SerializeField] protected Transform spawn;
		[SerializeField] protected float radius;
		[SerializeField] protected float interval;
		[SerializeField] protected float duration;
		[SerializeField] protected bool randomizeStartPoint;
		[SerializeField] protected bool randomizeEndPoint;

		protected float totalTime;
		protected float intervalTime;
		protected ObjectPool<Instance> projectilePool;
	}

	// Logic
	public class SpawnProjectiles : SpawnProjectilesData {
		private void Awake() {
			projectilePool = PoolManager.GetPool(projectilePrefab);
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
			var offset = Const.ZeroVector;

			if(randomizeStartPoint) {
				var random = Random.insideUnitCircle * radius;
				offset = new Vector3(random.x, 0f, random.y);
			}

			var projectile = projectilePool.Get();
			var position = spawn.position + offset;
			var rotation = spawn.rotation;

			if(randomizeEndPoint) {
				var targetPoint = Random.insideUnitCircle * radius;
				var targetOffset = new Vector3(targetPoint.x, 0f, targetPoint.y);
				var target = transform.localPosition + targetOffset;
				rotation = Quaternion.LookRotation(target - position);
			}

			projectile.transform.SetLayer(gameObject.layer);
			projectile.transform.SetPositionAndRotation(position, rotation);
			projectile.skill = skill;
			projectile.caster = caster;
			projectile.pool = projectilePool;
			projectile.Init();
		}
	}
}
