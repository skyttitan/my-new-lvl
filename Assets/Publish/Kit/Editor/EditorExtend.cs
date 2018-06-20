#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Reflection;

namespace Kit
{
    public sealed class EditorExtend
    {
        #region SortingLayer
        public static string[] GetSortingLayerNames()
        {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }
        public static int[] GetSortingLayerUniqueIDs()
        {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
            return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
        }
        public static int SortingLayerField(Rect position, SerializedProperty property)
        {
            return SortingLayerField(position, property, property.displayName);
        }
        public static int SortingLayerField(Rect position, SerializedProperty property, string label)
        {
            int selectedIndex = property.intValue;
            string[] values = GetSortingLayerNames();
            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUI.Popup(position, label, selectedIndex, values);
            if (selectedIndex >= values.Length)
            {
                selectedIndex = 0; // hotfix
                property.intValue = selectedIndex;
            }
            if(EditorGUI.EndChangeCheck())
            {
                property.intValue = selectedIndex;
            }
            return selectedIndex;
        }
		#endregion

		#region Tag
		public static string TagField(Rect position, SerializedProperty property, GUIContent label)
		{
			string layerName = property.stringValue;
			EditorGUI.BeginChangeCheck();
			if (string.IsNullOrEmpty(layerName))
			{
				layerName = "Untagged";
				property.stringValue = layerName;
			}
			layerName = EditorGUI.TagField(position, label, layerName);
			if (EditorGUI.EndChangeCheck())
			{
				property.stringValue = layerName;
			}
			return layerName;
		}
        #endregion

        #region LayerField
        public static int LayerField(Rect position, SerializedProperty property, GUIContent label)
        {
            int layerId = property.intValue;
            EditorGUI.BeginChangeCheck();
			layerId = EditorGUI.LayerField(position, label, layerId);
			if (EditorGUI.EndChangeCheck())
				property.intValue = layerId;
            return layerId;
        }
		#endregion
	}
}
#endif