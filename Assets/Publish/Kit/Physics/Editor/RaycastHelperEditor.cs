using UnityEditor;
using Kit.Physic;

[CustomEditor(typeof(RaycastHelper))]
public class RaycastHelperEditor : Editor
{
	SerializedProperty
		rayTypeProp, distanceProp, localPositionProp, radiusProp,
		memoryArraySizeProp, unSyncRotationProp, localRotationProp,
		halfExtendsProp, fixedUpdateProp,
		layerMaskProp, queryTriggerInteractionProp,
		colorProp, hitColorProp, onHitProp;
	private void OnEnable()
	{
		rayTypeProp = serializedObject.FindProperty("m_RayType");
		distanceProp = serializedObject.FindProperty("m_Distance");
		localPositionProp = serializedObject.FindProperty("m_LocalPosition");
		radiusProp = serializedObject.FindProperty("m_Radius");
		memoryArraySizeProp = serializedObject.FindProperty("m_MemoryArraySize");
		unSyncRotationProp = serializedObject.FindProperty("m_UnSyncRotation");
		localRotationProp = serializedObject.FindProperty("m_LocalRotation");
		halfExtendsProp = serializedObject.FindProperty("m_HalfExtends");
		fixedUpdateProp = serializedObject.FindProperty("m_FixedUpdate");

		layerMaskProp = serializedObject.FindProperty("m_LayerMask");
		queryTriggerInteractionProp = serializedObject.FindProperty("m_QueryTriggerInteraction");
		colorProp = serializedObject.FindProperty("m_Color");
		hitColorProp = serializedObject.FindProperty("m_HitColor");
		onHitProp = serializedObject.FindProperty("OnHit");
	}
	public override void OnInspectorGUI()
	{
		serializedObject.UpdateIfDirtyOrScript();
		EditorGUI.BeginChangeCheck();
		RaycastHelper.eRayType type = (RaycastHelper.eRayType) rayTypeProp.intValue;

		EditorGUILayout.PropertyField(rayTypeProp);
		
		if (type == RaycastHelper.eRayType.Raycast)
		{
			EditorGUILayout.PropertyField(distanceProp);
			EditorGUILayout.PropertyField(localPositionProp);
		}
		else if (type == RaycastHelper.eRayType.SphereCast)
		{
			EditorGUILayout.PropertyField(distanceProp);
			EditorGUILayout.PropertyField(localPositionProp);
			EditorGUILayout.PropertyField(radiusProp);
		}
		else if (type == RaycastHelper.eRayType.SphereOverlap)
		{
			EditorGUILayout.PropertyField(localPositionProp);
			EditorGUILayout.PropertyField(radiusProp);
			EditorGUILayout.PropertyField(memoryArraySizeProp);
		}
		else if (type == RaycastHelper.eRayType.BoxCast)
		{
			EditorGUILayout.PropertyField(distanceProp);
			EditorGUILayout.PropertyField(localPositionProp);
			EditorGUILayout.PropertyField(localRotationProp);
			EditorGUILayout.PropertyField(unSyncRotationProp);
			EditorGUILayout.PropertyField(halfExtendsProp);
		}
		else if (type == RaycastHelper.eRayType.BoxOverlap)
		{
			EditorGUILayout.PropertyField(localPositionProp);
			EditorGUILayout.PropertyField(localRotationProp);
			EditorGUILayout.PropertyField(unSyncRotationProp);
			EditorGUILayout.PropertyField(halfExtendsProp);
			EditorGUILayout.PropertyField(memoryArraySizeProp);
		}
		
		EditorGUILayout.PropertyField(fixedUpdateProp);
		EditorGUILayout.PropertyField(layerMaskProp);
		EditorGUILayout.PropertyField(queryTriggerInteractionProp);

		EditorGUILayout.PropertyField(colorProp);
		EditorGUILayout.PropertyField(hitColorProp);

		EditorGUILayout.PropertyField(onHitProp);

		if (EditorGUI.EndChangeCheck())
			serializedObject.ApplyModifiedProperties();
	}
}
