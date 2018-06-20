using System.Collections.Generic;
using UnityEngine;
using Kit.Extend;

namespace CF.CameraBot.Parts
{
	/// <summary>Fade depend on target object's distance</summary>
	/// <remarks>This function are depend on Stardard Shader.</remarks>
	public class FadeByDistance : RendererFader
	{
		[SerializeField] Transform m_FadeFactor;
		[System.Serializable]
		public class ShaderMap
		{
			public Shader m_From;
			public Shader m_To;
		}
		[SerializeField] ShaderMap[] m_ShaderMap = { };

		[SerializeField] Transform m_Offset;
		[SerializeField] float m_StartCheckingDistance = 5f;
		[SerializeField] float m_StartFadeDistance = 1f;
		[SerializeField] float m_CompleteFadeDistance = 0.5f;
		[SerializeField] AnimationCurve m_BlendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		private bool m_ResetAlpha = false;
		protected Bounds m_Bounds = new Bounds();
		private float m_Distance = float.MaxValue;

		protected override void Reset()
		{
			base.Reset();
#if UNITY_EDITOR
			if (m_FadeFactor == null && Camera.main != null)
			{
				m_FadeFactor = Camera.main.transform;
			}

			if (m_Offset == null)
			{
				m_Offset = transform;
			}
#endif
		}

		protected override void OnDisable()
		{
			SetAlpha(1f);
			base.OnDisable();
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			m_StartFadeDistance = Mathf.Clamp(m_StartFadeDistance, 0f, float.MaxValue);
			m_BlendCurve.Clamp(0f, 0f, 1f, 1f);

			if (m_ShaderMap.Length == 0 && m_Renderers.Length > 0)
			{
				HashSet<Shader> cache = new HashSet<Shader>();
				foreach (Renderer render in m_Renderers)
				{
					foreach(Material mat in render.materials)
					{
						cache.Add(mat.shader);
					}
				}

				List<ShaderMap> map = new List<ShaderMap>();
				Shader standard = Shader.Find("Standard");
				foreach(Shader shader in cache)
				{
					map.Add(new ShaderMap() { m_From = shader, m_To = standard });
				}
				m_ShaderMap = map.ToArray();
			}
		}

		private void LateUpdate()
		{
			m_ResetAlpha = true;

			if (m_FadeFactor == null)
			{
				m_FadeFactor = Camera.main.transform;
			}

			m_Distance = Vector3.Distance(m_Offset.transform.position, m_FadeFactor.transform.position);
			if (m_Distance <= m_StartCheckingDistance)
			{
				BoundSizeCalculate();

				m_Distance = Vector3.Distance(m_Bounds.ClosestPoint(m_FadeFactor.transform.position), m_FadeFactor.transform.position);
				if (m_Distance <= m_StartFadeDistance)
				{
					if (!IsTransparent)
						ApplyFadeMode();
					m_ResetAlpha = false;
					float alpha = m_BlendCurve.Evaluate(m_Distance.Scale01(m_CompleteFadeDistance, m_StartFadeDistance));
					SetAlpha(alpha);
				}
			}

			if(m_ResetAlpha && IsTransparent)
			{
				SetAlpha(1f);
				ApplyOriginalMode();
			}
			m_ResetAlpha = false;
		}

		private void BoundSizeCalculate()
		{
			bool dirty = false;
			for (int i = 0; i < m_Renderers.Length; i++)
			{
				if (!dirty)
					m_Bounds = m_Renderers[i].bounds;
				else
					m_Bounds.Encapsulate(m_Renderers[i].bounds);
				dirty = true;
			}
		}

		protected override Material ConvertToFadeMode(ref Material orgMat, Shader ignore = null)
		{
			if (m_ShaderMap.Length > 0)
			{
				foreach(var map in m_ShaderMap)
				{
					if (map.m_From != null && map.m_To != null &&
						orgMat.shader == map.m_From)
					{
						return base.ConvertToFadeMode(ref orgMat, map.m_To);
					}
				}
			}
			return base.ConvertToFadeMode(ref orgMat, orgMat.shader);
		}
	}
}