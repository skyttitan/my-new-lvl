using UnityEngine;
using UnityEditor;

namespace Kit
{
	[CustomEditor(typeof(Transform)), CanEditMultipleObjects]
	public class TransformInspector : Editor
	{
		static GUIContent
			labelWorldFold = new GUIContent("World Transform"),
			labelLocalFold = new GUIContent("Local Transform");

		static bool
			showWorld = true,
			showLocal = true;

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			showWorld = EditorGUILayout.Foldout(showWorld, labelWorldFold);
			if (showWorld) OnDrawWorldTransform();

			showLocal = EditorGUILayout.Foldout(showLocal, labelLocalFold);
			if (showLocal) OnDrawLocalTransform();
		}

		private void OnDrawWorldTransform()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			Vector3 position = EditorGUILayout.Vector3Field("Position", ((Transform)target).position);
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.Vector3Field("Rotation", ((Transform)target).eulerAngles);
			EditorGUILayout.Vector3Field("Scale", ((Transform)target).lossyScale);
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(GUILayout.Width(20f));
			if (GUILayout.Button("P")) position = Vector3.zero;
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObjects(targets, "Transform Change");
				foreach (Transform obj in targets)
				{
					obj.position = FixIfNaN(position);
				}
				serializedObject.ApplyModifiedProperties();
			}
		}

		private void OnDrawLocalTransform()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			Vector3 position = EditorGUILayout.Vector3Field("Position", ((Transform)target).localPosition);
			Vector3 eulerAngles = EditorGUILayout.Vector3Field("Rotation", ((Transform)target).localEulerAngles);
			Vector3 scale = EditorGUILayout.Vector3Field("Scale", ((Transform)target).localScale);
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(GUILayout.Width(20f));
			if (GUILayout.Button("P")) position = Vector3.zero;
			if (GUILayout.Button("R")) eulerAngles = Vector3.zero;
			if (GUILayout.Button("S")) scale = Vector3.one;
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObjects(targets, "Transform Change");
				foreach (Transform obj in targets)
				{
					obj.localPosition = FixIfNaN(position);
					obj.localEulerAngles = Repeat(eulerAngles);
					obj.localScale = FixIfNaN(scale);
				}
				serializedObject.ApplyModifiedProperties();
			}
		}

		private Vector3 FixIfNaN(Vector3 v)
		{
			if (float.IsNaN(v.x)) { v.x = 0; }
			if (float.IsNaN(v.y)) { v.y = 0; }
			if (float.IsNaN(v.z)) { v.z = 0; }
			return v;
		}
		private Vector3 Repeat(Vector3 v)
		{
			v.x = Mathf.Repeat(v.x, 360f);
			v.y = Mathf.Repeat(v.y, 360f);
			v.z = Mathf.Repeat(v.z, 360f);
			return v;
		}
	}
}