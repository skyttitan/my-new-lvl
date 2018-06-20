using System;
using UnityEngine;
using Kit.Extend;

namespace Kit.Physic
{
	public struct BoxcastData : IRaycastStruct
	{
		public Vector3 m_HalfExtends;
		public Quaternion orientation;

		public static BoxcastData NONE { get { return default(BoxcastData); } }

		public static bool IsNullOrEmpty(BoxcastData obj)
		{
			return ReferenceEquals(null, obj) || obj.Equals(BoxcastData.NONE);
		}

		public BoxcastData(Ray ray, float distance, Vector3 size, Quaternion localRotation)
		{
			m_RayBase = new RaycastData(ray, distance);
			m_HalfExtends = size;
			orientation = localRotation;
		}

		public BoxcastData(Vector3 origin, Vector3 direction, float distance, Vector3 size, Quaternion localRotation)
			: this(new Ray(origin, direction), distance, size, localRotation)
		{ }

		public bool Raycast(int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			RaycastHit hit;
			bool rst = Physics.BoxCast(origin, m_HalfExtends, direction, out hit, orientation, distance, layerMask, queryTriggerInteraction);
			m_RayBase.hitResult = hit;
			return rst;
		}

		public int RaycastNonAlloc(RaycastHit[] hits, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			int hitCnt = Physics.BoxCastNonAlloc(origin, m_HalfExtends, direction, hits, orientation, distance, layerMask, queryTriggerInteraction);
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
			GizmosExtend.DrawBoxCastBox(m_RayBase.origin, m_HalfExtends, orientation, m_RayBase.direction, m_RayBase.distance, color);
			if (m_RayBase.hitted)
				GizmosExtend.DrawBoxCastOnHit(m_RayBase.origin, m_HalfExtends, orientation, m_RayBase.direction, m_RayBase.hitResult.distance, hitColor);
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

		public void Update(Vector3 _origin, Vector3 _direction, float _distance)
		{
			m_RayBase.Update(_origin, _direction, _distance);
		}

		public void Update(Vector3 _origin, Vector3 _direction, float _distance, Vector3 _halfExtends, Quaternion _orientation)
		{
			m_RayBase.Update(_origin, _direction, _distance);
			m_HalfExtends = _halfExtends;
			orientation = _orientation;
		}

		public void Reset()
		{
			m_RayBase.Reset();
			m_HalfExtends = Vector3.zero;
			orientation = Quaternion.identity;
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