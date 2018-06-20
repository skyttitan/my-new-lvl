using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider))]
public class EditorLabel : MonoBehaviour
{
	[SerializeField] string text;

#if UNITY_EDITOR
	private static GUIStyle _style;
	private static GUIStyle m_Style
	{
		get
		{
			if (_style == null)
			{
				_style = new GUIStyle(EditorStyles.largeLabel);
				_style.alignment = TextAnchor.MiddleCenter;
				_style.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
				_style.fontSize = 32;
			}
			return _style;
		}
	}
	GUIContent _content;
	GUIContent m_Content
	{
		get
		{
			if (_content == null)
			{
				_content = new GUIContent(text);
			}
			return _content;
		}
	}

	Collider _collider;
	Collider m_Collider
	{
		get
		{
			if (_collider == null || _collider.gameObject != gameObject)
				_collider = GetComponent<Collider>();
			return _collider;
		}
	}
	
	void OnDrawGizmos()
	{
		RaycastHit hit;
		Ray r = new Ray(transform.position + Camera.current.transform.up * 8f, -Camera.current.transform.up);
		if (m_Collider.Raycast(r, out hit, Mathf.Infinity))
		{

			float dist = (Camera.current.transform.position - hit.point).magnitude;

			float fontSize = Mathf.Lerp(64, 12, dist / 10f);

			m_Style.fontSize = (int)fontSize;

			Vector3 wPos = hit.point + Camera.current.transform.up * dist * 0.07f;



			Vector3 scPos = Camera.current.WorldToScreenPoint(wPos);
			if (scPos.z <= 0)
			{
				return;
			}



			float alpha = Mathf.Clamp(-Camera.current.transform.forward.y, 0f, 1f);
			alpha = 1f - ((1f - alpha) * (1f - alpha));

			alpha = Mathf.Lerp(-0.2f, 1f, alpha);

			Handles.BeginGUI();


			scPos.y = Screen.height - scPos.y; // Flip Y


			Vector2 strSize = m_Style.CalcSize(m_Content);

			Rect rect = new Rect(0f, 0f, strSize.x + 6, strSize.y + 4);
			rect.center = scPos - Vector3.up * rect.height * 0.5f;
			GUI.color = new Color(0f, 0f, 0f, 0.8f * alpha);
			GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
			GUI.color = Color.white;
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
			GUI.Label(rect, text, m_Style);
			GUI.color = Color.white;

			Handles.EndGUI();
		}
	}
#endif
}