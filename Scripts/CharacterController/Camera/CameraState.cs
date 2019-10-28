using UnityEngine;

namespace CharacterController
{
    [CreateAssetMenu(menuName = "Character Controller/Camera State")]
    public class CameraState : ScriptableObject
    {
        public string StateName;
        //[Header("--  Move Settings  --")]
        //public float MoveSpeed = 9;
        //[Tooltip("The lerp speed when handling camera position.")]
        //public float AdaptSpeed = 8;

        [Header("--  Move Settings  --")]
        public float MoveSpeed = 16;
        [Header("--  Position Settings  --")]
        public float ViewDistance = 6;
        public float VerticalOffset;


        [Tooltip("Limit horizontal axis?")]
        public bool ApplyYawLimit = true;
        [Tooltip("Minimum and maximum left and right rotation. ")]
        public float MinYaw = -180;
        public float MaxYaw = 180;

        [Tooltip("Limit vertical axis?")]
        public bool ApplyPitchLimit = true;
        [Tooltip("Minimum and maximum up and down rotation. ")]
        public float XAxisMin = -60;
        public float XAxisMax = 70;

        public bool ApplyCameraOffset;
        [Tooltip("The offset between the anchor and the location of the camera.  This determines the cameras look direction.")]
        public Vector3 CameraOffset = new Vector3(0.5f, 0.9f, -2f);

        [Tooltip("The camera field of view")]
        [Range(0, 180)]
        public float FieldOfView = 60f;
        [Tooltip("The speed at which the FOV transitions field of views")]
        public float FieldOfViewSpeed = 5;

        public bool ApplyTurn;
        [Tooltip("The amount of smoothing to apply to the pitch and yaw. ")]
        public float TurnSmooth = 0.12f;
        [Tooltip("The speed at which the camera turns.")]
        public float TurnSpeed = 3f;

        public bool ApplyRotation;
        [Tooltip("The speed at which the camera rotates.")]
        public float RotationSpeed = 24;


        [Header("--  Zoom Settings  --")]
        public bool ApplyStepZoom;
        public float StepZoomSensitivity = 1;
        [Range(0, 1)]
        public float StepZoomSmooth = 0.15f;
        public float MinStepZooom = -1;
        public float MaxStepZoom = 10;

        //LayerMask m_IgnoreLayerMask = LayerManager.Mask.IgnoreInvisibleLayersPlayerWater



        [HideInInspector]
        public Vector3 LookDirection = Vector3.forward;





		public void OnEnable()
		{
            StateName = this.name;
		}
	}
}
