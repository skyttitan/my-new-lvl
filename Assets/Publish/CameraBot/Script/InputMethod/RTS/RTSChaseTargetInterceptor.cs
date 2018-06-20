using UnityEngine;
using System.Collections;

namespace CF.CameraBot
{
    [RequireComponent(typeof(IDevice))]
    [DisallowMultipleComponent]
    public class RTSChaseTargetInterceptor : ChaseTargetInterceptor, IControlSTR
    {
        public enum FollowMethod
        {
            None = 0,           // Manually call MoveTo(Transform/Vector3);
            RelativeToAvatar,   // keep focus on Avatar.
        }
        public FollowMethod CameraMoveMethod;
        private Transform watchingCamera { get { return CameraBot.ControlPosition; } }

        public void Trigger(ControlType type)
        {
            if(type==ControlType.STR)
            {
                // use mouse for rotate
            }
        }

        public string Horizontal = "Horizontal";
        public string Vertical = "Vertical";
        void FixedUpdate()
        {
            CameraBot.UpdatePosition(Input.GetAxis(Horizontal), Input.GetAxis(Vertical), 0f);
        }

        void Update()
        {
            if (CameraMoveMethod == FollowMethod.None)
                return;
            else if(CameraMoveMethod == FollowMethod.RelativeToAvatar)
            {
                MoveTo(ChaseTargetBak);
            }
        }

        #region Movement
        public MoveMethod MoveMethod;
        [Range(0.0001f, 30f)]
        public float PositionSpeed = 4f; // 30 fps, it's almost snap to position.
        public void MoveTo(Transform trans)
        {
            MoveTo(trans.position);
        }
        public void MoveTo(Vector3 position)
        {
            if (MoveMethod.Equals(MoveMethod.Snap))
                ChaseTarget.transform.position = position;
            else if(MoveMethod.Equals(MoveMethod.Lerp))
                ChaseTarget.transform.position = Vector3.Lerp(ChaseTarget.transform.position, position, Time.deltaTime * PositionSpeed);
            else
                ChaseTarget.transform.position = Vector3.Slerp(ChaseTarget.transform.position, position, Time.deltaTime * PositionSpeed);
        }
        #endregion
    }
}