using UnityEngine;
using System.Collections;

public static class Cache {
	public static Quaternion quaternionIdentity = Quaternion.identity;
	public static Vector3 vector3Zero = Vector3.zero;
	public static Quaternion upRotation = Quaternion.LookRotation(Vector3.up);
	public const float rotationShortToFloat = 5.493247882810712f;
	public const float rotationFloatToShort = 1.0f / 5.493247882810712f;
}
