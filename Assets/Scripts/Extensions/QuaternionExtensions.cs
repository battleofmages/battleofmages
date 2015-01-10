using UnityEngine;

static class QuaternionExtensions {
	/// <summary>
	/// Determines whether the quaternion is safe for interpolation or use with transform.rotation.
	/// </summary>
	/// <returns><c>false</c> if using the quaternion in Quaternion.Lerp() will result in an error (eg. NaN values or zero-length quaternion).</returns>
	/// <param name="quaternion">Quaternion.</param>
	public static bool IsValid(this Quaternion quaternion) {
		bool isNaN = float.IsNaN(quaternion.x + quaternion.y + quaternion.z + quaternion.w);
		bool isZero = quaternion.x == 0f && quaternion.y == 0f && quaternion.z == 0f && quaternion.w == 0f;
		
		return !(isNaN || isZero);
	}
}