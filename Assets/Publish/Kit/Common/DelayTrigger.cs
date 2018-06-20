using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class DelayTrigger : MonoBehaviour
{
	public float m_TimeToDelay = 0f;
	public UnityEvent m_BeforeStart;
	public UnityEvent m_AfterDelay;

	void OnEnable()
	{
		StartCoroutine(action());
	}

	IEnumerator action()
	{
		m_BeforeStart.Invoke();
		yield return new WaitForSecondsRealtime(m_TimeToDelay);
		m_AfterDelay.Invoke();
	}
}
