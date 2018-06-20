using UnityEngine;

namespace Kit.Physic
{
	public struct SphereOverlapData : IOverlapStruct
	{
		public Vector3 origin { get; set; }
		public float radius;
		public int hitCount { get; private set; }
		public bool hitted { get { return hitCount > 0; } }

		public static SphereOverlapData NONE { get { return default(SphereOverlapData); } }
		public static bool IsNullOrEmpty(SphereOverlapData obj)
		{
			return ReferenceEquals(null, obj) || obj.Equals(SphereOverlapData.NONE);
		}
		public SphereOverlapData(Vector3 _origin, float _radius)
		{
			origin = _origin;
			radius = _radius;
			hitCount = 0;
		}

		public void Update(Vector3 _origin, float _radius)
		{
			origin = _origin;
			radius = _radius;
		}

		public void Reset()
		{
			origin = Vector3.zero;
			radius = 0f;
		}

		public override string ToString()
		{
			return (hitted) ?
				string.Format("{0}, Radius: {1:F2}, Hit: {2}", origin.ToString("F2"), radius, hitCount) :
				string.Format("{0}, Radius: {1:F2}, Hit: None", origin.ToString("F2"), radius);
		}

		/// <summary>Overlap will not use the distance information.</summary>
		/// <param name="color"></param>
		/// <param name="hitColor"></param>
		public void DrawOverlapGizmos(ref Collider[] colliderResult, int validArraySize, Color color = default(Color), Color hitColor = default(Color))
		{
			Color cache = Gizmos.color;
			Gizmos.color = color;
			Gizmos.DrawWireSphere(origin, radius);
			Gizmos.color = hitColor;
			for (int i = 0; i < validArraySize && i < colliderResult.Length; ++i)
			{
				Gizmos.DrawLine(origin, colliderResult[i].transform.position);
			}
			Gizmos.color = cache;
		}

		public Collider[] Overlap(Vector3 _origin, float _radius, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Update(_origin, _radius);
			return Overlap(layerMask, queryTriggerInteraction);
		}

		public Collider[] Overlap(int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			Collider[] rst = Physics.OverlapSphere(origin, radius, layerMask, queryTriggerInteraction);
			hitCount = rst.Length;
			return rst;
		}

		public int OverlapNonAlloc(ref Collider[] results, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			hitCount = Physics.OverlapSphereNonAlloc(origin, radius, results, layerMask, queryTriggerInteraction);
			return hitCount;
		}

		

		
	}
}