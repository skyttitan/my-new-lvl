using UnityEngine;

namespace Kit
{
	public class SceneReferenceAttribute : PropertyAttribute
	{
		public readonly bool IsShowLabel;
		public SceneReferenceAttribute(bool showLabel = true)
		{
			IsShowLabel = showLabel;
		}
	}
}