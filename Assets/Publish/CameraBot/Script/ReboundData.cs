using UnityEngine;
using Kit.Extend;
namespace CF.CameraBot
{
	public enum eReboundState
	{
		Idle = 0,
		WaitForDelay,
		Rebounding,
	}

	public class ReboundData
	{
		public eReboundState m_State;
		public float m_Delay, m_Duration, m_Delta, m_StartAngle, m_LastSendAngle;

		public ReboundData()
		{
			ResetAll();
		}
		public void ResetAll()
		{
			m_State = eReboundState.Idle;
			m_Delay = m_Duration = m_Delta = m_StartAngle = 0f;
		}
		public void WaitForRebound(float delay)
		{
			m_Delay = delay;
			m_State = eReboundState.WaitForDelay;
		}
		public float GetReboundForce(float currentAngle, AnimationCurve curve, float duration, float delta)
		{
			if (m_State != eReboundState.Idle)
			{
				if (m_State == eReboundState.WaitForDelay && m_Delta < m_Delay)
				{
					m_Delta += delta;
				}
				else if (m_State == eReboundState.WaitForDelay && m_Delta >= m_Delay)
				{
					m_StartAngle = currentAngle;
					m_Delta = 0f;
					m_LastSendAngle = 0f;
					m_Duration = duration;
					m_State = eReboundState.Rebounding;
					// return 0f anyway.
				}
				else if (m_State == eReboundState.Rebounding)
				{
					m_Delta += delta;
					float rst = Mathf.Lerp(0f, -m_StartAngle, curve.Evaluate(m_Delta / m_Duration * curve.keys[curve.length - 1].time));
					rst -= m_LastSendAngle; // instead of sending full angle, we sent angle diff
					m_LastSendAngle += rst;
					if (m_Delta >= m_Duration)
					{
						m_State = eReboundState.Idle;
					}
					return rst;
				}
			}
			return 0f;
		}
	}
}