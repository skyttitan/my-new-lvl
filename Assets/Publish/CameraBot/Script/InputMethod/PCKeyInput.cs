using UnityEngine;
namespace CF.CameraBot
{
    [DisallowMultipleComponent]
    public class PCKeyInput : CameraBotSlave
    {
        public string MouseHorizontal = "Mouse X";
        public string MouseVertical = "Mouse Y";
        public string MouseZoom = "Mouse ScrollWheel";

		[Header("Depend on lock (optional)")]
		[SerializeField] CursorLock m_CursorLock;

		private void OnValidate()
		{
			Reset();
		}

		private void Reset()
		{
			if (m_CursorLock == null)
			{
				m_CursorLock = GetComponent<CursorLock>();
			}
		}

		void Update()
        {
			if (m_CursorLock != null && !m_CursorLock.IsOwnedCursor)
				return;

            CameraBot.UpdatePosition(
				Input.GetAxis(MouseHorizontal),
				Input.GetAxis(MouseVertical),
				Input.GetAxis(MouseZoom));
        }
    }
}

