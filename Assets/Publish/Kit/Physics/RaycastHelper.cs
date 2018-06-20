using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Kit.Physic
{
	/// <summary>A helper class for visualize common Phyical raycast and overlap.
	/// also provide the Gizmos call for debug usage.</summary>
	public class RaycastHelper : MonoBehaviour
	{
		#region setting
		public enum eRayType
		{
			Raycast = 0,
			SphereCast = 10,
			SphereOverlap,
			BoxCast = 20,
			BoxOverlap,
		}

		private RaycastData m_RayData;
		private SpherecastData m_SphereRayData;
		private SphereOverlapData m_SphereOverlapData;
		private BoxcastData m_BoxcastData;
		private BoxOverlapData m_BoxOverlapData;

		[SerializeField] private eRayType m_RayType = eRayType.Raycast;
		public eRayType RayType
		{
			get { return m_RayType; }
			set
			{
				if (m_RayType != value)
				{
					m_RayType = value;
					Init();
				}
			}
		}
		public float m_Distance = 1f;
		public Vector3 m_LocalPosition = Vector3.zero;

		// Sphere
		public float m_Radius = 1f;

		// Boxcast
		public Vector3 m_LocalRotation = Vector3.zero;
		public Vector3 m_HalfExtends = Vector3.one;
		public bool m_UnSyncRotation = false;

		[Header("Physics")]
		public bool m_FixedUpdate = true;
		public LayerMask m_LayerMask = Physics.DefaultRaycastLayers;
		public QueryTriggerInteraction m_QueryTriggerInteraction = QueryTriggerInteraction.UseGlobal;

		// Overlap
		private Collider[] m_OverlapColliders;
		[SerializeField] private int m_MemoryArraySize = 1;
		public int m_OverlapHittedCount { get; private set; }
		
		[Header("Debug")]
		[SerializeField] Color m_Color = Color.white;
		[SerializeField] Color m_HitColor = Color.red;

		[Header("Events")]
		public HitEvent OnHit;
		private Callback m_DrawGizmos = null;
		private delegate bool PhysicRaycast();
		private PhysicRaycast m_PhysicRaycast = null;
		[System.Serializable] public class HitEvent : UnityEvent<Transform> { }
		private int m_FixedUpdateCount = 0;
		private int m_CurrentFixedUpdate = -1;
		#endregion

		#region System
		private void OnValidate()
		{
			m_Distance = Mathf.Clamp(m_Distance, 0f, float.MaxValue);
			m_Radius = Mathf.Clamp(m_Radius, 0f, float.MaxValue);
			m_MemoryArraySize = Mathf.Max(m_MemoryArraySize, 0);
			m_HalfExtends.x = Mathf.Clamp(m_HalfExtends.x, 0f, float.MaxValue);
			m_HalfExtends.y = Mathf.Clamp(m_HalfExtends.y, 0f, float.MaxValue);
			m_HalfExtends.z = Mathf.Clamp(m_HalfExtends.z, 0f, float.MaxValue);
			Init();
		}

		private void Awake()
		{
			Init();
		}

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying)
			{
				Init();
				CheckPhysic();
			}

			m_DrawGizmos();
		}

		private void FixedUpdate()
		{
			m_FixedUpdateCount++;

			if (!m_FixedUpdate)
				return;
			
			CheckPhysic(false);
		}
		#endregion

		#region Public API
		/// <summary>Unity Physic, if current raycastData hit anything.</summary>
		/// <param name="forceUpdate">for the case you need to update more then one time within same fixedUpdate, e.g. change position</param>
		/// <returns>true = hit something in the frame.</returns>
		public bool CheckPhysic(bool forceUpdate = false)
		{
			if (forceUpdate || m_CurrentFixedUpdate != m_FixedUpdateCount)
			{
				m_CurrentFixedUpdate = m_FixedUpdateCount;
				return m_PhysicRaycast();
			}
			else
			{
				// getting same result, and event are already trigger within this fixedUpdate;
				return GetCurrentStruct().hitted;
			}
		}

		/// <summary>Get colliders which are overlap the preset area. only work on Overlap type.</summary>
		/// <returns>list of collider overlapped</returns>
		public IEnumerable<Collider> GetOverlapColliders()
		{
			// if (m_RayType == eRayType.BoxOverlap || m_RayType == eRayType.SphereOverlap)
			if (m_RayType == eRayType.BoxOverlap || m_RayType == eRayType.SphereOverlap)
			{
				if (m_MemoryArraySize == 0)
					Debug.LogWarning("You alloced \"0\" memory size, this will always return empty list.", this);
				for (int i = 0; i < m_OverlapHittedCount && i < m_OverlapColliders.Length; ++i)
					yield return m_OverlapColliders[i];
			}
			else
				throw new System.Exception(GetType().Name + " : this method cannot use on " + m_RayType.ToString("F") + " type.");
		}
		#endregion

		#region Private function
		private void Init()
		{
			m_RayData.Reset();
			m_SphereRayData.Reset();
			m_SphereOverlapData.Reset();
			m_BoxcastData.Reset();
			m_BoxOverlapData.Reset();
			// Binding Physic & gizmos callback, also reset the others
			if (m_RayType == eRayType.Raycast)
			{
				m_OverlapColliders = new Collider[0];
				m_PhysicRaycast = () =>
				{
					m_RayData.Update(transform.TransformPoint(m_LocalPosition), transform.forward, m_Distance);
					if (m_RayData.Raycast(m_LayerMask, m_QueryTriggerInteraction))
					{
						OnHit.Invoke(m_RayData.hitResult.transform);
					}
					return m_RayData.hitted;
				};
				m_DrawGizmos = () =>
				{
					m_RayData.DrawGizmos(m_Color, m_HitColor);
				};
			}
			else if (m_RayType == eRayType.SphereCast)
			{
				m_OverlapColliders = new Collider[0];
				m_PhysicRaycast = () =>
				{
					m_SphereRayData.Update(transform.TransformPoint(m_LocalPosition), transform.forward, m_Distance, m_Radius);
					if (m_SphereRayData.Raycast(m_LayerMask, m_QueryTriggerInteraction))
					{
						OnHit.Invoke(m_SphereRayData.hitResult.transform);
					}
					return m_SphereRayData.hitted;
				};
				m_DrawGizmos = () =>
				{
					m_SphereRayData.DrawGizmos(m_Color, m_HitColor);
				};
			}
			else if (m_RayType == eRayType.SphereOverlap)
			{
				m_OverlapColliders = new Collider[m_MemoryArraySize];
				m_PhysicRaycast = () =>
				{
					m_SphereOverlapData.Update(transform.TransformPoint(m_LocalPosition), m_Radius);
					m_OverlapHittedCount = m_SphereOverlapData.OverlapNonAlloc(ref m_OverlapColliders, m_LayerMask, m_QueryTriggerInteraction);
					if (m_OverlapHittedCount > 0)
					{
						foreach (Collider collider in GetOverlapColliders())
						{
							OnHit.Invoke(collider.transform);
						}
					}
					return m_OverlapHittedCount > 0;
				};
				m_DrawGizmos = () =>
				{
					m_SphereOverlapData.DrawOverlapGizmos(ref m_OverlapColliders, m_OverlapHittedCount, m_Color, m_HitColor);
				};
			}
			else if (m_RayType == eRayType.BoxCast)
			{
				m_OverlapColliders = new Collider[0];
				m_PhysicRaycast = () =>
				{
					m_BoxcastData.Update(transform.TransformPoint(m_LocalPosition), transform.forward, m_Distance, m_HalfExtends, (m_UnSyncRotation) ? Quaternion.Euler(m_LocalRotation) : transform.rotation * Quaternion.Euler(m_LocalRotation));
					if (m_BoxcastData.Raycast(m_LayerMask, m_QueryTriggerInteraction))
					{
						OnHit.Invoke(m_BoxcastData.hitResult.transform);
					}
					return m_BoxcastData.hitted;
				};
				m_DrawGizmos = () =>
				{
					m_BoxcastData.DrawGizmos(m_Color, m_HitColor);
				};
			}
			else if (m_RayType == eRayType.BoxOverlap)
			{
				m_OverlapColliders = new Collider[m_MemoryArraySize];
				m_PhysicRaycast = () =>
				{
					m_BoxOverlapData.Update(transform.TransformPoint(m_LocalPosition), (m_UnSyncRotation) ? Quaternion.Euler(m_LocalRotation) : transform.rotation * Quaternion.Euler(m_LocalRotation), m_HalfExtends);
					m_OverlapHittedCount = m_BoxOverlapData.OverlapNonAlloc(ref m_OverlapColliders, m_LayerMask, m_QueryTriggerInteraction);
					if (m_OverlapHittedCount > 0)
					{
						foreach (Collider collider in GetOverlapColliders())
						{
							OnHit.Invoke(collider.transform);
						}
					}
					return m_OverlapHittedCount > 0;
				};
				m_DrawGizmos = () =>
				{
					m_BoxOverlapData.DrawOverlapGizmos(ref m_OverlapColliders, m_OverlapHittedCount, m_Color, m_HitColor);
				};
			}
			else
			{
				throw new System.NotImplementedException();
			}

			/// think about the relateionship between m_CurrentFixedUpdate,
			/// force reset m_CurrentFixedUpdate without modify it's value.
			/// case : first Awake(), change <see cref="RayType"/> & <see cref="CheckPhysic"/> within same frame
			/// case : runtime, call <see cref="CheckPhysic"/> and then change <see cref="RayType"/> & <see cref="CheckPhysic"/> within same frame
			m_FixedUpdateCount = 0;
			m_CurrentFixedUpdate = -1;
		}

		public IPhysicsRayStruct GetCurrentStruct()
		{
			switch (m_RayType)
			{
				case eRayType.Raycast: return m_RayData;
				case eRayType.SphereCast: return m_SphereRayData;
				case eRayType.SphereOverlap: return m_SphereOverlapData;
				case eRayType.BoxCast: return m_BoxcastData;
				case eRayType.BoxOverlap:return m_BoxOverlapData;
				default:
					throw new System.NotImplementedException();
			}
		}
		#endregion
	}
}