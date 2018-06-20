using UnityEngine;
using Kit.Extend;

namespace Kit.Physic
{
	public struct BoxOverlapData : IOverlapStruct
	{
		public Vector3 origin { get; set; }
		public Vector3 halfExtends;
		public Quaternion orientation;
		public int hitCount { get; private set; }
		public bool hitted { get { return hitCount > 0; } }

		public static BoxOverlapData NONE { get { return default(BoxOverlapData); } }

		public static bool IsNullOrEmpty(BoxOverlapData obj)
		{
			return ReferenceEquals(null, obj) || obj.Equals(BoxOverlapData.NONE);
		}

		public BoxOverlapData(Vector3 _origin, Quaternion _orientation, Vector3 _halfExtends)
		{
			origin = _origin;
			orientation = _orientation;
			halfExtends = _halfExtends;
			hitCount = 0;
		}

		public void Update(Vector3 _origin, Quaternion _orientation, Vector3 _halfExtends)
		{
			origin = _origin;
			orientation = _orientation;
			halfExtends = _halfExtends;
		}

		public void Reset()
		{
			origin = halfExtends = Vector3.zero;
			orientation = Quaternion.identity;
			hitCount = 0;
		}

		public override string ToString()
		{
			return (hitted) ?
				string.Format("{0}, Extends: {1:F2}, Hit: {2}", origin.ToString("F2"), halfExtends.ToString("F2"), hitCount) :
				string.Format("{0}, Extends: {1:F2}, Hit: None", origin.ToString("F2"), halfExtends.ToString("F2"));
		}

		/// <summary>Overlap will not use the distance information.</summary>
		/// <param name="color"></param>
		/// <param name="hitColor"></param>
		public void DrawOverlapGizmos(ref Collider[] colliderResult, int validArraySize, Color color = default(Color), Color hitColor = default(Color))
		{
			Color cache = Gizmos.color;
			GizmosExtend.DrawBox(origin, halfExtends, orientation, color);
			Gizmos.color = hitColor;
			for (int i = 0; i < validArraySize && i < colliderResult.Length; ++i)
			{
				Gizmos.DrawLine(origin, colliderResult[i].transform.position);
			}
			Gizmos.color = cache;
		}

		public Collider[] Overlap(Vector3 _origin, Quaternion _orientation, Vector3 _halfExtends, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(_origin, _orientation, _halfExtends);
			return Overlap(layerMask, queryTriggerInteraction);
		}

		public Collider[] Overlap(int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Collider[] rst = Physics.OverlapBox(origin, halfExtends, orientation, layerMask, queryTriggerInteraction);
			hitCount = rst.Length;
			return rst;
		}

		public int OverlapNonAlloc(ref Collider[] results, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			hitCount = Physics.OverlapBoxNonAlloc(origin, halfExtends, results, orientation, layerMask, queryTriggerInteraction);
			return hitCount;
		}

		
	}
}