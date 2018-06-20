using System;
using UnityEngine;

namespace Kit.Physic
{
	public struct SpherecastData : IRaycastStruct
	{
		public float m_Radius;

		public static SpherecastData NONE { get { return default(SpherecastData); } }

		public static bool IsNullOrEmpty(SpherecastData obj)
		{
			return ReferenceEquals(null, obj) || obj.Equals(SpherecastData.NONE);
		}

		public SpherecastData(Ray ray, float distance, float radius)
		{
			m_RayBase = new RaycastData(ray, distance);
			m_Radius = radius;
		}

		public SpherecastData(Vector3 origin, Vector3 direction, float distance, float radius)
			: this(new Ray(origin, direction), distance, radius)
		{ }

		public bool Raycast(int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			RaycastHit hit;
			bool rst = Physics.SphereCast(origin, m_Radius, direction, out hit, distance, layerMask, queryTriggerInteraction);
			m_RayBase.hitResult = hit;
			return rst;
		}
		public int RaycastNonAlloc(RaycastHit[] hits, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			int hitCnt = Physics.SphereCastNonAlloc(origin, m_Radius, direction, hits, distance, layerMask, queryTriggerInteraction);
			if (hitCnt > 0)
			{
				/// to define it's being hit, <see cref="RaycastData.hitted"/>
				m_RayBase.hitResult = hits[0];
			}
			else
			{
				m_RayBase.hitResult = default(RaycastHit);
			}
			return hitCnt;
		}

		public void DrawGizmos(Color color = default(Color), Color hitColor = default(Color))
		{
			m_RayBase.DrawGizmos(color, hitColor);
			Color cache = Gizmos.color;
			Gizmos.color = (hitted)? hitColor : color;
			Gizmos.DrawWireSphere(m_RayBase.GetRayEndPoint(), m_Radius);
			Gizmos.color = cache;
		}
		
		#region redirect session
		private RaycastData m_RayBase;
		public Vector3 origin { get { return m_RayBase.origin; } set { m_RayBase.origin = value; } }
		public Vector3 direction { get { return m_RayBase.direction; } set { m_RayBase.direction = value; } }
		public float distance { get { return m_RayBase.distance; } set { m_RayBase.distance = value; } }
		public RaycastHit hitResult { get { return m_RayBase.hitResult; } }
		public bool hitted { get { return m_RayBase.hitted; } }
		
		public bool IsHittingSameObject(RaycastHit raycastHit, bool includeNull = false)
		{
			return m_RayBase.IsHittingSameObject(raycastHit, includeNull);
		}

		public void Update(Vector3 _origin, Vector3 _direction, float _distance, float _radius)
		{
			m_RayBase.Update(_origin, _direction, _distance);
			m_Radius = _radius;
		}
		
		public void Reset()
		{
			m_RayBase.Reset();
			m_Radius = 0f;
		}

		public override string ToString()
		{
			return m_RayBase.ToString();
		}

		public Vector3 GetRayEndPoint()
		{
			return m_RayBase.GetRayEndPoint();
		}
		#endregion
	}
}