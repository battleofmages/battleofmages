#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace BoM.Editor {
	[CustomPropertyDrawer(typeof(Core.LayerAttribute))]
	class LayerAttributeEditor : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			property.intValue = EditorGUI.LayerField(position, label, property.intValue);
		}
	}
}
#endif
