namespace CharacterController
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class CharacterIK : MonoBehaviour
    {
        private readonly float m_LookAtRayLength = 10;

        [SerializeField, DisplayOnly]
        private Vector3 bodyPosition;
        [SerializeField] private bool m_DebugDrawLookRay;

        [Header("-- Body --")]
        [SerializeField, Range(0, 1)] protected float m_LookAtAimBodyWeight = 1f;
        [SerializeField, Range(0, 1)] protected float m_LookAtBodyWeight = 0.05f;
        [SerializeField, Range(0, 1)] protected float m_LookAtHeadWeight = 0.425f;
        [SerializeField, HideInInspector, Range(0, 1)] protected float m_LookAtEyesWeight = 0f;
        [SerializeField, Range(0, 1)] protected float m_LookAtClampWeight = 0.35f;
        [SerializeField] protected float m_LookAtAdjustmentSpeed = 0.2f;
        [SerializeField] private Vector3 m_LookAtOffset;

        [Header("-- Hands --")]
        [SerializeField, Range(0, 1)] protected float m_HandIKWeight = 1f;
        [SerializeField] protected float m_HandIKAdjustmentSpeed = 0.2f;
        [SerializeField] protected Vector3 m_HandIKOffset;

        [Header("--  Feet --")]
        [Range(0, 2)] [SerializeField] private float heightFromGroundRaycast = 1.14f;
        [Range(0, 2)] [SerializeField] private float raycastDownDistance = 1.5f;
        [SerializeField] private float m_HipOffset;
        [Range(0, 1)] [SerializeField] private float m_HipsMovingPositionAdjustmentSpeed = 0.28f;
        [Range(0, 1)] [SerializeField] private float m_FootPositionAdjustmentSpeed = 0.5f;


        [Space(12)]
        [Header("--  Targets --")]
        [SerializeField] private Transform m_RightHandTarget;
        [SerializeField] private Transform m_LeftHandTarget;
        private Transform m_RightHand, m_LeftHand, m_UpperChest, m_RightShoulder, m_Head;
        private Transform m_AimPivot;

        [Header("--  States --")]
        private bool m_Aiming;


        private Item m_CurrentItem;

        private Vector3 m_LookAtPoint;
        private Vector3 m_TargetDirection;
        private Quaternion m_TargetRotation;


        private float m_TargetLookAtWeight;
        private float m_TargetHandWeight;
        private float m_HandIKAdjustmentVelocity;
        private float m_LookAtAdjustmentVelocity;

        private Vector3 rightHandPosition;
        private Vector3 rightHandIkPosition;
        private Vector3  rightHandTargetPosition;
        private Quaternion rightHandRotation, rightHandTargetRotation;
        private Vector3 leftHandPosition, leftHandIkPosition, leftHandTargetPosition;
        private Quaternion leftHandRotation, leftHandTargetRotation;


        private Vector3 rightFootPosition, leftFootPosition, leftFootIkPosition, rightFootIkPosition;
        private Quaternion leftFootIkRotation, rightFootIkRotation;
        private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;

        //  For Recoil
        private bool recoilInit;
        private float recoilTime;
        private Vector3 recoilBasePosition, recoilOffsetPosition, recoilBaseRotation, recoilOffsetRotation;


        private Animator m_Animator;
        private RigidbodyCharacterController m_controller;
        private LayerManager m_LayerManager;
        private Transform m_Transform;
        private GameObject m_GameObject;
        private float m_DeltaTime;



		private void Awake()
		{
            m_Animator = GetComponent<Animator>();
            m_controller = GetComponent<RigidbodyCharacterController>();
            m_LayerManager = GetComponent<LayerManager>();
            m_Transform = transform;
            m_GameObject = gameObject;
            m_DeltaTime = Time.deltaTime;


            m_RightHand = m_Animator.GetBoneTransform(HumanBodyBones.RightHand).transform;
            m_LeftHand = m_Animator.GetBoneTransform(HumanBodyBones.LeftHand).transform;
            m_UpperChest = m_Animator.GetBoneTransform(HumanBodyBones.UpperChest).transform;
            m_RightShoulder = m_Animator.GetBoneTransform(HumanBodyBones.RightShoulder).transform;
            m_Head = m_Animator.GetBoneTransform(HumanBodyBones.Head).transform;

            m_AimPivot = new GameObject("Aim Pivot").transform;
            m_AimPivot.transform.parent = gameObject.transform;
            m_AimPivot.position = m_RightShoulder.position;

            if(m_RightHandTarget == null){
                m_RightHandTarget = new GameObject("Right Hand Target").transform;
                m_RightHandTarget.localPosition = Vector3.zero;
                m_RightHandTarget.transform.parent = m_AimPivot;
            } else {
                var rightHandTargetPos = m_RightHandTarget.localPosition;
                var rightHandTargetRot = m_RightHandTarget.localRotation;
                m_RightHandTarget.transform.parent = m_AimPivot;
                m_RightHandTarget.localPosition = rightHandTargetPos;
                m_RightHandTarget.localRotation = rightHandTargetRot;
            }


            m_LeftHandTarget = new GameObject("Left Hand Target").transform;
            m_LeftHandTarget.transform.parent = m_AimPivot;
            m_LeftHandTarget.localPosition = Vector3.zero;
		}




        private void OnEnable()
        {
            EventHandler.RegisterEvent<Item>(m_GameObject, "OnInventoryEquip", HandleItem);
            EventHandler.RegisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAim);


        }


        private void OnDisable()
        {
            EventHandler.UnregisterEvent<Item>(m_GameObject, "OnInventoryEquip", HandleItem);
            EventHandler.UnregisterEvent<bool>(m_GameObject, "OnAimActionStart", OnAim);
        }





        #region Item Actions

        private void HandleItem(Item item)
        {
            //Debug.LogFormat("{0} currently equipped item is {1}.", gameObject.name, item == null ? "<null>" : item.ItemAnimName);
            if (item == null)
            {
                m_CurrentItem = null;
                m_RightHandTarget.localPosition = m_RightHand.localPosition;
                m_RightHandTarget.localRotation = m_RightHand.localRotation;

                m_LeftHandTarget.localPosition = m_LeftHand.localPosition;
                m_LeftHandTarget.localRotation = m_LeftHand.localRotation;
            }
            else
            {
                m_CurrentItem = item;
                //m_RightHandTarget.localPosition = item.PositionOffset;
                //m_RightHandTarget.localEulerAngles = item.RotationOffset;

                //m_LeftHandTarget.position = item.NonDominantHandPosition.position;
                //m_LeftHandTarget.rotation = item.NonDominantHandPosition.rotation;
                //rightHandTargetRotation = Quaternion.Euler(item.RotationOffset);
            }
        }



        private void OnAim(bool aim)
        {
            //Debug.LogFormat("{0} aiming is {1}.", gameObject.name, aim);
            if (aim){
                m_LookAtAimBodyWeight = 1;
                m_HandIKWeight = m_CurrentItem == null ? 0 : 1;
            } else {
                m_LookAtAimBodyWeight = 0;
                m_HandIKWeight = 0;
            }
        }


        private void OnUse(Item item)
        {
            if(recoilInit == false){
                recoilInit = true;
                recoilTime = 0;
                recoilOffsetPosition = Vector3.zero;
                recoilOffsetRotation = Vector3.zero;
            }

            if(recoilInit){
                recoilTime += m_DeltaTime;
                if(recoilTime > 1){
                    recoilTime = 1;
                    recoilInit = false;
                }

                recoilOffsetPosition = Vector3.forward * 0.1f;
                recoilOffsetRotation = Vector3.right * 90 * 0.1f;

                m_RightHandTarget.localPosition = recoilBasePosition + recoilOffsetPosition;
                m_RightHandTarget.localEulerAngles = recoilBaseRotation + recoilOffsetRotation;
            }


        }


        #endregion



        private void FixedUpdate()
        {
            if (m_Animator == null) { return; }

            m_AimPivot.position = m_RightShoulder.position;
            //m_LookAtPoint = m_Head.position + m_controller.LookDirection;
            //m_LookAtPoint = Vector3.Lerp(m_LookAtPoint, m_Head.position + m_controller.LookDirection, Time.deltaTime * 15);


            m_TargetDirection = m_LookAtPoint - m_AimPivot.position;
            //m_TargetDirection = (m_Head.position + m_Head.forward * 5) - m_AimPivot.position;
            if (m_TargetDirection == Vector3.zero)
                m_TargetDirection = m_AimPivot.forward;
            m_TargetRotation = Quaternion.LookRotation(m_TargetDirection);

            //m_TargetRotation = Quaternion.LookRotation(m_LookAtPoint);
            m_AimPivot.rotation = Quaternion.Slerp(m_AimPivot.rotation, m_TargetRotation, Time.deltaTime * 15);
            //m_RightHandTarget.rotation = Quaternion.Slerp(m_RightHandTarget.rotation, m_TargetRotation, Time.deltaTime * 15);


            AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
            AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);
            //  Find and raycast to the ground to find positions
            FeetPositionSolver(rightFootPosition, ref rightFootIkPosition, ref rightFootIkRotation); // handle the solver for right foot
            FeetPositionSolver(leftFootPosition, ref leftFootIkPosition, ref leftFootIkRotation); //handle the solver for the left foot
        }




        public bool disableIK;
		private void OnAnimatorIK()
		{
            if (m_Animator == null) return;

            bodyPosition = m_Animator.bodyPosition;

            if (disableIK) return;

            LookAtTarget();
            //PositionLowerBody();
            PositionHands();
		}

        float lookAtWeightVelocity;
        protected virtual void LookAtTarget()
        {
            Vector3 directionTowardsTarget = m_LookAtPoint - m_Transform.position;
            float angle = Vector3.Angle(m_Transform.forward, directionTowardsTarget);
            if (angle < 76)
                m_TargetLookAtWeight = Mathf.SmoothDamp(m_TargetLookAtWeight, 1, ref lookAtWeightVelocity, 0.1f);
            else
                m_TargetLookAtWeight = Mathf.SmoothDamp(m_TargetLookAtWeight, 0, ref lookAtWeightVelocity, 0.1f);



            m_TargetHandWeight = Mathf.SmoothDamp(m_TargetHandWeight, m_HandIKWeight, ref m_HandIKAdjustmentVelocity, m_HandIKAdjustmentSpeed);
            m_TargetLookAtWeight = Mathf.SmoothDamp(m_TargetLookAtWeight, m_LookAtAimBodyWeight, ref m_LookAtAdjustmentVelocity, m_LookAtAdjustmentSpeed);


            m_Animator.SetLookAtWeight(m_TargetLookAtWeight, m_LookAtBodyWeight, m_LookAtHeadWeight, m_LookAtEyesWeight, m_LookAtClampWeight);
            //m_Animator.SetLookAtPosition( m_controller.LookAtPoint + m_LookAtOffset);
            m_Animator.SetLookAtPosition(m_LookAtPoint + m_LookAtOffset);

        }


        protected virtual void PositionHands()
        {
            //if (m_CurrentItem != null){
            //    if (m_CurrentItem.NonDominantHandPosition != null){
            //        m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, m_TargetHandWeight);
            //        m_Animator.SetIKRotation(AvatarIKGoal.LeftHand, m_CurrentItem.NonDominantHandPosition.rotation);

            //        m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, m_TargetHandWeight);
            //        m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, m_CurrentItem.NonDominantHandPosition.position);
            //    }
            //}

            m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, m_TargetHandWeight);
            m_Animator.SetIKRotation(AvatarIKGoal.RightHand, m_RightHandTarget.rotation);

            m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, m_TargetHandWeight);
            m_Animator.SetIKPosition(AvatarIKGoal.RightHand, m_RightHandTarget.position);
        }



        protected virtual void PositionLowerBody()
        {
            MovePelvisHeight();

            //right foot ik position and rotation -- utilise the pro features in here
            m_Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            MoveFeetToIkPoint(AvatarIKGoal.RightFoot, rightFootIkPosition, rightFootIkRotation, ref lastRightFootPositionY);

            //left foot ik position and rotation -- utilise the pro features in here
            m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            MoveFeetToIkPoint(AvatarIKGoal.LeftFoot, leftFootIkPosition, leftFootIkRotation, ref lastLeftFootPositionY);
        }




        void MoveFeetToIkPoint(AvatarIKGoal foot, Vector3 positionIkHolder, Quaternion rotationIkHolder, ref float lastFootPositionY)
        {
            Vector3 targetIkPosition = m_Animator.GetIKPosition(foot);

            if (positionIkHolder != Vector3.zero)
            {
                targetIkPosition = transform.InverseTransformPoint(targetIkPosition);
                positionIkHolder = transform.InverseTransformPoint(positionIkHolder);

                float yVariable = Mathf.Lerp(lastFootPositionY, positionIkHolder.y, m_FootPositionAdjustmentSpeed);
                targetIkPosition.y += yVariable;

                lastFootPositionY = yVariable;

                targetIkPosition = transform.TransformPoint(targetIkPosition);

                m_Animator.SetIKRotation(foot, rotationIkHolder);
            }

            m_Animator.SetIKPosition(foot, targetIkPosition);
        }


        void MovePelvisHeight()
        {
            if (rightFootIkPosition == Vector3.zero || leftFootIkPosition == Vector3.zero || lastPelvisPositionY == 0)
            {
                lastPelvisPositionY = m_Animator.bodyPosition.y;
                return;
            }

            float lOffsetPosition = leftFootIkPosition.y - transform.position.y;
            float rOffsetPosition = rightFootIkPosition.y - transform.position.y;
            float totalOffset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;

            Vector3 newPelvisPosition = m_Animator.bodyPosition + Vector3.up * totalOffset;

            newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, m_HipsMovingPositionAdjustmentSpeed);

            m_Animator.bodyPosition = newPelvisPosition;

            lastPelvisPositionY = m_Animator.bodyPosition.y;
        }


        void FeetPositionSolver(Vector3 footPosition, ref Vector3 feetIkPositions, ref Quaternion feetIkRotations)
        {
            //raycast handling section 
            RaycastHit feetOutHit;

            if (Physics.Raycast(footPosition, Vector3.down, out feetOutHit, raycastDownDistance + heightFromGroundRaycast, m_LayerManager.SolidLayers))
            {
                //finding our feet ik positions from the sky position
                feetIkPositions = footPosition;
                feetIkPositions.y = feetOutHit.point.y + m_HipOffset;
                feetIkRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;
                return;
            }
            feetIkPositions = Vector3.zero; //it didn't work :(
        }


        private void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
        {
            feetPositions = m_Animator.GetBoneTransform(foot).position;
            feetPositions.y = transform.position.y + heightFromGroundRaycast;
        }





















		private void OnDrawGizmos()
		{
            if (Application.isPlaying)
            {
                if (m_DebugDrawLookRay)
                {
                    if (m_LookAtPoint != Vector3.zero)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawLine(m_Transform.position + Vector3.up * 1.35f, m_LookAtPoint);
                        Gizmos.DrawSphere(m_LookAtPoint, 0.1f);
                    }

                    if(m_controller.Aiming)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawRay(m_AimPivot.position, (m_AimPivot.forward * 50));
                        GizmosUtils.DrawString("Aim Direction", m_AimPivot.position, Color.magenta);
                    }

                }

            }

		}





        //private void HandleWeights()
        //{
        //    //if (m_Aiming)
        //    //{
        //    //    Vector3 directionTowardsTarget = m_LookAtPoint.position - m_Transform.position;
        //    //    float angle = Vector3.Angle(m_Transform.forward, directionTowardsTarget);
        //    //    if (angle < 90)
        //    //    {
        //    //        m_LookAtAimBodyWeight = 1;
        //    //    }
        //    //    else
        //    //    {
        //    //        m_LookAtAimBodyWeight = 0;
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    m_LookAtAimBodyWeight = 0;
        //    //}

        //    float targetLookAtWeight = 0;

        //    m_LookAtAimBodyWeight = Mathf.Lerp(m_LookAtAimBodyWeight, targetLookAtWeight, Time.deltaTime * 3);

        //    float targetMainHandWeight = 0;  //  target main hand weight

        //    //if (states.states.isAiming){
        //    //    t_m_weight = 1;
        //    //    m_LookAtBodyWeight = 0.4f;
        //    //}
        //    //else{
        //    //    m_LookAtBodyWeight = 0.3f;
        //    //}


        //    m_LookAtBodyWeight = 0.3f;


        //    //if (lh_target != null) o_h_weight = 1;
        //    //else o_h_weight = 0;

        //    //o_h_weight = 0;

        //    Vector3 directionTowardsTarget = m_controller.LookDirection - m_Transform.position;
        //    float angle = Vector3.Angle(m_Transform.forward, directionTowardsTarget);

        //    if (angle < 76)
        //        targetLookAtWeight = 1;
        //    else
        //        targetLookAtWeight = 0;
        //    if (angle > 45)
        //        targetMainHandWeight = 0;

        //    m_LookAtAimBodyWeight = Mathf.Lerp(m_LookAtAimBodyWeight, targetLookAtWeight, Time.deltaTime * 3);
        //    m_HandIKWeight = Mathf.Lerp(m_HandIKWeight, targetMainHandWeight, Time.deltaTime * 9);
        //}

	}
}