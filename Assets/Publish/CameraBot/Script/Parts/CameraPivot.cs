using UnityEngine;
using System.Collections.Generic;
using System;
using Kit.Extend;

namespace CF.CameraBot.Parts
{
	[Serializable]
	public struct WeightPoint
	{
		public Vector3 m_Point;
		public float m_Weight;
	}

	public class CameraPivot : MonoBehaviour, IParts, IGizmo
	{
		#region Variables
		public Preset m_Preset;
		public CameraStand m_CameraStand;
		
		private List<IDeaPosition> m_Consultant = new List<IDeaPosition>();
		private List<WeightPoint> m_DestinationPoint = new List<WeightPoint>();
		private Vector3 m_LastPosition = Vector3Zero, m_LastLocalPosition = Vector3Zero;
		private Vector3 m_Velocity = Vector3Zero, m_LocalVelocity = Vector3Zero;
		public Vector3 Velocity { get { return m_Velocity; } }
		public Vector3 LocalVelocity { get { return m_LocalVelocity; } }
		public float Speed { get { return Vector3Zero.Distance(m_Velocity) / Time.fixedDeltaTime; } }
		public float LocalSpeed { get { return Vector3Zero.Distance(m_LocalVelocity) / Time.fixedDeltaTime; } }
		private readonly IDeaPositionComparer m_IDeaPositionComparer = new IDeaPositionComparer();
		private static readonly Vector3 Vector3Zero = Vector3.zero;
		#endregion

		#region System
		void Awake()
		{
			m_LastLocalPosition = transform.localPosition;
			m_LastPosition = transform.position;
			m_LocalVelocity = Vector3Zero;
			m_Velocity = Vector3Zero;
		}

		void FixedUpdate()
		{
			CalculateVelocity();
			PerformIdeasPosition();
			transform.localPosition = CenterOfVectors(m_DestinationPoint);
			// Who need correction ?! we don't care.
			//if (transform.localPosition.EqualRoughly(Vector3Zero, .3f))
			//	transform.localRotation = Quaternion.identity;
			//else
			//	transform.LookAt(m_Preset.Instance.CameraLookAt.position, m_Preset.Instance.CameraUpward());
		}
		#endregion

		#region Interface
		public void Init(Preset preset, CameraStand cameraStand)
		{
			m_Preset = preset;
			m_CameraStand = cameraStand;
		}

		public void ResetToInit()
		{
			m_LocalVelocity = m_Velocity = m_LastPosition = transform.localPosition = Vector3Zero;
			transform.localRotation = Quaternion.identity;
			transform.localPosition = Vector3Zero;
		}

		public void DrawGizmos()
		{
			// GizmosExtend.DrawLabel(transform.position + Vector3.up * 0.2f, "Velocity:" + m_Velocity, GUI.skin.textArea);
		}
		#endregion

		#region Core
		public void RegisterOperator(IDeaPosition who)
		{
			if (who != null)
				m_Consultant.Add(who);
		}

		public void UnregisterOperator(IDeaPosition who)
		{
			if (who != null)
				m_Consultant.Remove(who);
		}

		private void PerformIdeasPosition()
		{
			m_DestinationPoint.Clear();
			int groupPt = int.MaxValue;
			m_Consultant.Sort(m_IDeaPositionComparer);
			foreach (IDeaPosition obj in m_Consultant)
			{
				if (groupPt > obj.GroupNumber && // one for each group
					obj.Weight > 0f && // ignore zero-weight
					!obj.ToLocalPosition.Approximately(Vector3Zero)) // with result
				{
					groupPt = obj.GroupNumber;
					m_DestinationPoint.Add(new WeightPoint() { m_Point = obj.ToLocalPosition, m_Weight = obj.Weight });
				}
			}
		}
		private class IDeaPositionComparer : IComparer<IDeaPosition>
		{
			public int Compare(IDeaPosition lhs, IDeaPosition rhs)
			{
				int rst = lhs.GroupNumber.CompareTo(rhs.GroupNumber);
				return (rst == 0) ? lhs.Weight.CompareTo(rhs.Weight) : rst;
			}
		}

		private Vector3 CenterOfVectors(List<Vector3> points)
		{
			if (points == null || points.Count == 0)
				return Vector3Zero;

			Vector3 sum = Vector3Zero;
			foreach (Vector3 point in points)
			{
				sum += point;
			}
			return sum / points.Count;
		}

		private Vector3 CenterOfVectors(List<WeightPoint> points)
		{
			if (points == null || points.Count == 0)
				return Vector3Zero;

			Vector3 sum = Vector3Zero;
			float totalWeight = 0f;
			foreach (WeightPoint point in points)
			{
				sum += point.m_Point * point.m_Weight;
				totalWeight += point.m_Weight;
			}
			return sum / totalWeight;
		}

		private void CalculateVelocity()
		{
			m_Velocity = m_LastPosition.Direction(transform.position);
			m_LastPosition = transform.position;
			m_LocalVelocity = m_LastLocalPosition.Direction(transform.localPosition);
			m_LastLocalPosition = transform.localPosition;
		}
		#endregion
	}
}