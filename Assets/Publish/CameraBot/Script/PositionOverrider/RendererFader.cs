using System.Collections.Generic;
using UnityEngine;
using Kit.Extend;

namespace CF.CameraBot.Parts
{
	/// <summary>Used to create and cache the martials shader. apply transparent on demend.</summary>
	public abstract class RendererFader : MonoBehaviour
	{
		[SerializeField] protected Renderer[] m_Renderers = { };
		protected Dictionary<Renderer, MaterialCache> m_RenderDict;
		protected struct MaterialCache
		{
			public Dictionary<int, MaterialSet> m_Materials;
		}
		protected struct MaterialSet
		{
			public int index;
			public Renderer m_Renderer;
			public Material m_OrgMaterial;
			public Material m_TranMaterial;
		}

		public bool IsTransparent { get; private set; }
		bool m_RenderDisabledFlag = false;

		protected virtual void Reset()
		{
			if (m_Renderers == null || m_Renderers.Length == 0)
			{
				m_Renderers = GetComponentsInChildren<Renderer>();
			}
		}

		protected virtual void OnValidate()
		{
			Reset();
		}

		protected virtual void Awake()
		{
			m_RenderDict = new Dictionary<Renderer, MaterialCache>();
			foreach (Renderer render in m_Renderers)
			{
				MaterialCache cache = new MaterialCache();
				cache.m_Materials = new Dictionary<int, MaterialSet>();
				for (int i = 0; i < render.materials.Length; i++)
				{
					if (render.materials[i] != null)
					{
						cache.m_Materials.Add(i, new MaterialSet()
						{
							m_Renderer = render,
							index = i,
							m_OrgMaterial = render.materials[i],
							m_TranMaterial = ConvertToFadeMode(ref render.materials[i], render.materials[i].shader),
						});

					}
				}
				m_RenderDict.Add(render, cache);
			}
		}

		protected virtual void OnDisable()
		{
			ApplyOriginalMode();
		}

		protected virtual Material ConvertToFadeMode(ref Material orgMat, Shader replaceShader)
		{
			Material material = new Material(replaceShader);
			// material.CopyPropertiesFromMaterial(orgMat);
			material.color = orgMat.color;
			material.mainTexture = orgMat.mainTexture;
			material.mainTextureOffset = orgMat.mainTextureOffset;
			material.mainTextureScale = orgMat.mainTextureScale;
			material.SetFloat("_Mode", 2);
			material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			material.SetInt("_ZWrite", 0);
			material.DisableKeyword("_ALPHATEST_ON");
			material.EnableKeyword("_ALPHABLEND_ON");
			material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = 3000;
			return material;
		}

		[ContextMenu("Apply Fade Mode")]
		public void ApplyFadeMode()
		{
			if (IsTransparent)
				return;
			IsTransparent = true;
			foreach (MaterialCache cache in m_RenderDict.Values)
			{
				Renderer renderer = null;
				List<Material> materials = new List<Material>();
				foreach (MaterialSet ms in cache.m_Materials.Values)
				{
					renderer = ms.m_Renderer;
					materials.Add(ms.m_TranMaterial);
				}
				if (renderer != null)
				{
					renderer.materials = materials.ToArray();
				}
			}
		}

		[ContextMenu("Apply Original Mode")]
		public void ApplyOriginalMode()
		{
			if (!IsTransparent)
				return;
			IsTransparent = false;
			foreach (MaterialCache cache in m_RenderDict.Values)
			{
				Renderer renderer = null;
				List<Material> materials = new List<Material>();
				foreach (MaterialSet ms in cache.m_Materials.Values)
				{
					renderer = ms.m_Renderer;
					materials.Add(ms.m_OrgMaterial);
				}
				if (renderer != null)
				{
					renderer.materials = materials.ToArray();
				}
			}
		}

		public void SetRenderersActive(bool state)
		{
			if (state == m_RenderDisabledFlag)
			{
				m_RenderDisabledFlag = !state;
				foreach (var render in m_Renderers)
				{
					render.enabled = state;
				}
			}
		}

		public void SetAlpha(float alpha)
		{
			if (Mathf.Approximately(0f, alpha))
			{
				SetRenderersActive(false);
				return;
			}

			SetRenderersActive(true);
			foreach (MaterialCache cache in m_RenderDict.Values)
			{
				foreach (MaterialSet ms in cache.m_Materials.Values)
				{
					ms.m_Renderer.materials[ms.index].color = ms.m_Renderer.materials[ms.index].color.CloneAlpha(alpha);
				}
			}
		}
	}
}