using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]

public class Spawn : MonoBehaviour {
	public float angleStep;
	
	private Transform myTransform;
	private float currentAngle;
	private float radius;

	// Awake
	void Awake() {
		radius = GetComponent<SphereCollider>().radius;
		angleStep *= Mathf.Deg2Rad;
		currentAngle = 0f;
		myTransform = transform;
	}

	// GetNextSpawnPosition
	public Vector3 GetNextSpawnPosition() {
		currentAngle += angleStep;
		
		return GetSpawnPosition(currentAngle, radius, myTransform);
	}

	// GetSpawnPosition
	public static Vector3 GetSpawnPosition(float angle, float nRadius, Transform nTransform) {
		// TODO: Raycast to the terrain to see how much height we need
		return nTransform.position + new Vector3(Mathf.Cos(angle) * nRadius, 2f, Mathf.Sin(angle) * nRadius);
	}
}
