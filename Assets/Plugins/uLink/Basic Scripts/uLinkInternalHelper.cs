using UnityEngine;

/// <summary>
/// Do not use this script. For internal use only.
/// </summary>
[AddComponentMenu("")]
[ExecuteInEditMode]
public sealed class uLinkInternalHelper : uLink.InternalHelper
{
	void Update()
	{
		gameObject.hideFlags = 0;
	}
}
