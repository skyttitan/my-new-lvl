#define FREE_VERSION
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Kit.Extend;

namespace CF.CameraBot
{
    [CustomPropertyDrawer(typeof(ClampAngle))]
    public class ClampAngleDrawer : PropertyDrawer
    {
#if FREE_VERSION
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return 0f;
		}
		public static float GetStaticHeight(SerializedProperty property)
		{
			return 0f;
		}
#endif
	}
}
#endif