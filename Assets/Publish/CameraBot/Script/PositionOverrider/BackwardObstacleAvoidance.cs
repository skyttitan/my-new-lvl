using UnityEngine;
using Kit.Extend;
using Kit.Physic;

namespace CF.CameraBot.Parts
{
	[RequireComponent(typeof(Preset))]
	public class BackwardObstacleAvoidance : IDeaPositionBase, IDeaPosition
	{
		BoxcastData m_BoxcastData;
		public Vector3 m_HalfExtends = Vector3.one;

		[Header("Physics")]
		public bool m_Debug = true;
		/// <summary>Ignore nothing by default, recommand ignore chase target's layer</summary>
		public LayerMask m_LayerMask = Physics.DefaultRaycastLayers;
		public QueryTriggerInteraction m_QueryTriggerInteraction = QueryTriggerInteraction.UseGlobal;

		[Header("Debug")]
		[SerializeField] Color m_Color = Color.white.CloneAlpha(.5f);
		[SerializeField] Color m_HitColor = Color.red;

		protected override void OnValidate()
		{
			base.OnValidate();
			m_HalfExtends.x = Mathf.Clamp(m_HalfExtends.x, 0f, float.MaxValue);
			m_HalfExtends.y = Mathf.Clamp(m_HalfExtends.y, 0f, float.MaxValue);
			m_HalfExtends.z = Mathf.Clamp(m_HalfExtends.z, 0f, float.MaxValue);
		}

		void OnDrawGizmos()
		{
			if (m_Debug)
			{
				if (!Application.isPlaying)
					FixedUpdate();
				m_BoxcastData.DrawGizmos(m_Color, m_HitColor);
			}
		}

		void FixedUpdate()
		{
			m_BoxcastData.Update(GetCameraLookAt.position, GetCameraIdeaDirection, GetCameraIdeaDistance);
			m_BoxcastData.orientation = Quaternion.LookRotation(GetCameraIdeaDirection);
			m_BoxcastData.m_HalfExtends = m_HalfExtends;
			
			if (m_BoxcastData.Raycast(m_LayerMask, m_QueryTriggerInteraction))
			{
				ToLocalPosition = GetCameraIdeaPivot.InverseTransformPoint(m_BoxcastData.GetRayEndPoint());
			}
			else
			{
				ToLocalPosition = Vector3.zero;
			}
		}
	}
}

