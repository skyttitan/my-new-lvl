using UnityEngine;
using System.Collections;

namespace CF.CameraBot
{
    public class CursorLock : MonoBehaviour
    {
        public KeyCode ToggleCursorKey = KeyCode.Escape;
        public eCursorMode m_CursorMode = eCursorMode.AutoTaken;

		public enum eCursorMode
		{
			AutoTaken = 0, // accept anykey
			ToggleToTaken = 1,
			HoldToTaken = 2, // hold down preset key to own
		}

        private bool m_OwnCursor = false;
		public bool IsOwnedCursor { get { return m_OwnCursor; } }

#if UNITY_WEBPLAYER || UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_WEBGL
        private void Awake()
        {
            useGUILayout = false;
        }
        private void Update()
        {
            HandleCursor();
        }

		private void HandleCursor()
		{
			if (m_CursorMode == eCursorMode.AutoTaken)
			{
				if (!IsOwnedCursor && Input.anyKey)
					CursorHideLock();
			}
			else if (m_CursorMode == eCursorMode.ToggleToTaken)
			{
				if (Input.GetKeyUp(ToggleCursorKey))
				{
					if (!IsOwnedCursor)
						CursorHideLock();
					else
						CursorShowUnlock();
				}
			}
			else if (m_CursorMode == eCursorMode.HoldToTaken)
			{
				if (Input.GetKeyDown(ToggleCursorKey))
				{
					CursorHideLock();
				}
				else if (Input.GetKeyUp(ToggleCursorKey))
				{
					CursorShowUnlock();
				}
			}
        }
#endif

        private void CursorHideLock()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            m_OwnCursor = (!Cursor.visible && Cursor.lockState == CursorLockMode.Locked);
        }
        private void CursorShowUnlock()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            m_OwnCursor = !(Cursor.visible && Cursor.lockState == CursorLockMode.None);
        }
    }
}
