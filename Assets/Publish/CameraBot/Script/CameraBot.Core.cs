#define FREE_VERSION
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Kit.Extend;

namespace CF.CameraBot
{
	/// <summary>
	/// CF Camera for Unity3D - a spherical coordinates camera handler.
	/// </summary>
	/// <see cref="http://www.clonefactor.com"/>
	public partial class CameraBot : MonoBehaviour
	{
		#region Const
		const float Circle = 360f;
		const float SemiCircle = 180f;
		const float QuarterCircle = 90f;
		#endregion

		// an internal getter to fall back position during lose target.
		internal Transform m_ChaseTarget
		{
			get { return ChaseTarget ? ChaseTarget : transform; }
			set { ChaseTarget = value; }
		}
		// an internal getter to fall back rotation during lose target.
		internal Transform m_TargetForward
		{
			get { return TargetForward ? TargetForward : transform; }
			set { TargetForward = value; }
		}
		
		#region system
		public void OnValidate()
		{
			if (ChaseTarget != null && TargetForward == null)
			{
				ChaseTargetHelper helper = ChaseTarget.GetComponentInChildren<ChaseTargetHelper>();
				if (helper != null)
				{
					SetChaseTarget(helper);
				}
				else
				{
					SetChaseTarget(ChaseTarget);
				}
			}

			if (ControlPosition != null && ControlRotation == null)
				ControlRotation = ControlPosition;

			// Only check within Editor Mode.
			if (!gameObject.IsPlayingOrWillChangePlaymode())
			{
				// Ensure preset format
				ValidElementOrder();
			}
			else
			{
				// in case developer change active camera in inspector at runtime
				HandlePresetActiveState();
			}
		}

		void Start()
		{
			LocateRequiredObject();
			InitCameraPosition();
			InitReference();
		}

		void LateUpdate()
		{
			if (!AdvanceSetting.FixedUpdate)
			{
				UpdateCameraStandTransformByChaseTarget();
				UpdateInstanceTransform();
				UpdateActiveCamera();
			}
		}
		void FixedUpdate()
		{
			if (AdvanceSetting.FixedUpdate)
			{
				UpdateCameraStandTransformByChaseTarget();
				UpdateInstanceTransform();
				UpdateActiveCamera();
			}
		}
		#endregion
		
		#region Init
		private void LocateRequiredObject()
		{
			if (PresetList.Count == 0)
			{
				this.enabled = false;
				throw new MissingReferenceException("CameraBot require at least one camera.");
			}
		}
		protected void InitReference()
		{
			for (int i = 0; i < PresetList.Count; i++)
			{
				// Init Camera Reference;
				UpdateOrbitCoordinateData(0f, 0f, 0f, PresetList[i]);
			}
		}
		
		protected void InitCameraPosition()
		{
			for (int i = 0; i < PresetList.Count; i++)
			{
				if (PresetList[i] == null)
					throw new System.NullReferenceException("Invalid config detected, please re-config your CameraBot setting");
				if (PresetList[i].InitReference == null) // this will trigger Init on each preset.
					throw new System.InvalidOperationException("All preset should init before this point.");
			}
			if (ControlPosition != null && ControlRotation != null)
			{
				SwitchCamera(Selected, true);
				UpdateCameraStandTransformByChaseTarget();
			}
		}
		#endregion
		
		#region core
        private void UpdateActiveCamera()
        {
			if (activePreset != null && Selected >= 0 && Selected < PresetList.Count)
			{
				UpdateActiveCameraPosition();
				UpdateActiveCameraRotation();
				ClearOneFrameRequests();
			}
			else
			{
				Selected = 0;
			}
        }
		private void UpdateActiveCameraPosition()
		{
			if (ControlPosition == null)
				return;

			if (activePreset.OneFrameSnapRequest)
			{
				ControlPosition.position = activePreset.Instance.CameraPivot.transform.position;
				return;
			}

			float time = (AdvanceSetting.FixedUpdate) ? Time.deltaTime : Time.fixedDeltaTime;
			float timeH = time * activePreset.m_Method.m_HorizontalPositionSpeed;
			float timeV = time * activePreset.m_Method.m_VerticalPositionSpeed;
			Vector3 horizontal = 
				(activePreset.m_Method.m_MoveMethod.Equals(MoveMethod.QuaternionLerp)) ? Vector3.Slerp(ControlPosition.position, activePreset.Instance.CameraPivot.transform.position, timeH) :
				(activePreset.m_Method.m_MoveMethod.Equals(MoveMethod.Lerp)) ? Vector3.Lerp(ControlPosition.position, activePreset.Instance.CameraPivot.transform.position, timeH) :
				(activePreset.m_Method.m_MoveMethod.Equals(MoveMethod.OrbitLerp)) ? GetOrbitLerpFramePosition(activePreset, timeH) : activePreset.Instance.CameraPivot.transform.position;
			Vector3 vertical =
				(activePreset.m_Method.m_MoveMethod.Equals(MoveMethod.QuaternionLerp)) ? Vector3.Slerp(ControlPosition.position, activePreset.Instance.CameraPivot.transform.position, timeV) :
				(activePreset.m_Method.m_MoveMethod.Equals(MoveMethod.Lerp)) ? Vector3.Lerp(ControlPosition.position, activePreset.Instance.CameraPivot.transform.position, timeV) :
				(activePreset.m_Method.m_MoveMethod.Equals(MoveMethod.OrbitLerp)) ? GetOrbitLerpFramePosition(activePreset, timeV) : activePreset.Instance.CameraPivot.transform.position;
			horizontal.y = vertical.y;
			ControlPosition.position = horizontal;
		}
        private void UpdateActiveCameraRotation()
		{
			if (ControlRotation == null)
				return;
			
			if(activePreset.OneFrameSnapRequest)
			{
				ControlRotation.rotation = activePreset.Instance.GetCameraRotation();
				return;
			}

			float time = (AdvanceSetting.FixedUpdate) ? Time.deltaTime : Time.fixedDeltaTime;
			time *= activePreset.m_Method.m_RotationSpeed;
			ControlRotation.rotation =
				(activePreset.m_Method.m_RotationMethod.Equals(RotationMethod.QuaternionLerp)) ? Quaternion.Slerp(ControlRotation.rotation, activePreset.Instance.GetCameraRotation(), time) :
				(activePreset.m_Method.m_RotationMethod.Equals(RotationMethod.Lerp)) ? Quaternion.Lerp(ControlRotation.rotation, activePreset.Instance.GetCameraRotation(), time) : activePreset.Instance.GetCameraRotation();
		}
		private void ClearOneFrameRequests()
		{
			activePreset.OneFrameSnapRequest = false;
		}

		/// <summary>Sync <see cref="m_ChaseTarget"/> and <see cref="CameraStand"/>.</summary>
		/// <remarks>For Clamp angle</remarks>
		private void UpdateCameraStandTransformByChaseTarget()
		{
			if (Selected >= 0 &&
				Selected < PresetList.Count &&
				activePreset != null)
			{
				activePreset.Cache.m_LastFrameTargetPosition = activePreset.transform.position;
				activePreset.Cache.m_LastFrameTargetRotation = activePreset.transform.rotation;
				if (m_ChaseTarget != null)
					activePreset.transform.position = m_ChaseTarget.position;

				if (activePreset.ChaseTargetRotation != null)
					activePreset.transform.rotation = activePreset.ChaseTargetRotation.rotation;

				if (!activePreset.OneFrameSnapRequest)
				{
					RelativeTargetRotation();
				}
			}
		}
		#endregion
		
		#region tools
		private void HandlePresetActiveState()
		{
			bool changed = false;
			// disable other, except selected one.
			for (int i = 0; i < PresetList.Count; i++)
			{
				// Delect by changing all other camera are disable.
				if (!changed && i != Selected && PresetList[i].gameObject.activeSelf)
					changed = true;
				PresetList[i].gameObject.SetActive(i == Selected);
			}

			if (changed)
			{
				activePreset.m_DisplayOnScene = true;
				if (activePreset.m_Method.m_UpdateAngle == UpdateAngleMethod.ResetWhenActive)
				{
					// correct all preset angle
					activePreset.transform.position = m_ChaseTarget.position;
					activePreset.transform.rotation = m_ChaseTarget.rotation;
					// reset zoom section & angle when switch
					activePreset.ResetToInitStage();
				}
			}
		}
		internal void ValidElementOrder()
		{
			// update list when different
			IEnumerable<Preset> tmp = GetComponentsInChildren<Preset>(true).OrderBy(o => o.transform.GetSiblingIndex());
			if (tmp.Except(PresetList).Count() > 0 || PresetList.Except(tmp).Count() > 0)
			{
				PresetList = new List<Preset>(tmp);
			}

			// ensure hierarchy structure.
			for (int i=0; i< PresetList.Count; i++)
			{
				PresetList[i].m_Host = this;
				if (PresetList[i].transform.parent != transform)
				{
					PresetList[i].transform.SetParent(transform);
				}
			}
		}

        protected Vector3 GetOrbitLerpFramePosition(Preset preset, float delta)
        {
			Vector3 deltaUp = Vector3.Lerp(ControlPosition.up, preset.Instance.CameraPivot.transform.up, delta);
			Quaternion
				startQuat = Quaternion.LookRotation(preset.ChaseTarget.position.Direction(ControlPosition.position), deltaUp),
				endQuat = Quaternion.LookRotation(preset.ChaseTarget.position.Direction(preset.Instance.CameraPivot.transform.position), deltaUp);
			Vector3 deltaDirection = Quaternion.LerpUnclamped(startQuat, endQuat, delta).GetForward();

			float
				startDistance = (preset.ChaseTarget.position - ControlPosition.position).sqrMagnitude,
				endDistance = (preset.ChaseTarget.position - preset.Instance.CameraPivot.transform.position).sqrMagnitude,
				deltaDistance = Mathf.Sqrt(Mathf.Lerp(startDistance, endDistance, delta));
			return preset.ChaseTarget.position.PointOnDistance(deltaDirection, deltaDistance);
		}
		#endregion
    }
}