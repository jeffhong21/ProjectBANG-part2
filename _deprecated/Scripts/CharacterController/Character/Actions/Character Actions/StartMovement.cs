namespace CharacterController
{
    using UnityEngine;
    using System;


    public class StartMovement : CharacterAction
    {
        public override int ActionID {
            get { return m_actionID = ActionTypeID.StartMovement; }
            set { m_actionID = value; }
        }


        [SerializeField] protected int maxInputCount = 15;


        protected int detectionCount;
        protected float startAngle;
        protected float lastMoveAmount;
        protected float currentMoveAmount;
        protected bool isStartingToMove;
        protected float moveAmount;

        protected bool m_wasPreviouslyMoving;
        protected bool m_isCurrentlyMoving;

		//
		// Methods
		//
        public override bool CanStartAction()
        {
            if (!m_controller.isGrounded) return false;

            //m_wasPreviouslyMoving = m_isCurrentlyMoving;
            //m_isCurrentlyMoving = m_controller.isGrounded;

            //if (m_isCurrentlyMoving && !m_wasPreviouslyMoving)
            //    isStartingToMove = true;

            //if (m_wasPreviouslyMoving == false && m_isCurrentlyMoving == false) return false;

            //if (isStartingToMove) {
            //    if (detectionCount >= maxInputCount) {
            //        detectionCount = 0;
            //        isStartingToMove = false;
            //        return true;
            //    }
            //    detectionCount++;
            //}



            lastMoveAmount = currentMoveAmount;
            currentMoveAmount = Mathf.Clamp01(Mathf.Abs(m_controller.inputVector.x) + Mathf.Abs(m_controller.inputVector.z));

            if (Math.Abs(lastMoveAmount) < float.Epsilon && currentMoveAmount > 0)
                isStartingToMove = true;

            //if (lastMoveAmount < currentMoveAmount) 
            //    isStartingToMove = true;


            if (isStartingToMove) {
                if (detectionCount >= maxInputCount) {
                    detectionCount = 0;
                    isStartingToMove = false;
                    return true;
                }
                detectionCount++;
            }

            //if (currentMoveAmount >= 1) {
            //    detectionCount = 0;
            //    isStartingToMove = false;
            //}


            //CharacterDebug.Log("detectionCount", detectionCount);
            //CharacterDebug.Log("lastMoveAmount", lastMoveAmount);
            //CharacterDebug.Log("currentMoveAmount", currentMoveAmount);

            return false;
		}

		protected override void ActionStarted()
        {

            ////  Start walk angle
            switch (m_controller.movementType) {
                case (RigidbodyCharacterControllerBase.MovementTypes.Adventure):


                    //startAngle = m_controller.GetAngleFromForward(m_controller.LookRotation * m_transform.forward);
                    startAngle = m_transform.AngleFromForward(m_controller.lookDirection);
                    //Vector3 moveDirection = m_transform.InverseTransformDirection(m_controller.inputVector);
                    //Vector3 axisSign = Vector3.Cross(moveDirection, m_transform.forward);
                    //startAngle = Vector3.Angle(moveDirection, m_transform.forward) * (axisSign.y >= 0 ? -1f : 1f);
                    break;
                case (RigidbodyCharacterControllerBase.MovementTypes.Combat):
                    //moveDirection = m_transform.rotation * m_controller.inputVector;
                    //startAngle = Vector3.Angle(m_transform.forward, moveDirection) * m_controller.inputVector.x;
                    startAngle = Mathf.Atan2(m_controller.inputVector.x, m_controller.inputVector.z) * Mathf.Rad2Deg;
                    break;
            }

            startAngle = (float)Math.Round(startAngle, 2);
            startAngle = Mathf.Approximately(startAngle, 0) ? 0 : (float)Math.Round(startAngle, 2);


            if (m_stateName.Length == 0) m_animator.SetInteger(HashID.ActionID, m_actionID);
            m_animator.SetFloat(HashID.ActionFloatData, startAngle);


        }



        //public override bool UpdateRotation()
        //{
        //    var targetRotation = m_animator.deltaRotation;
        //    m_controller.MoveRotation = targetRotation;

        //    return false;
        //}



        public override bool CanStopAction()
        {
            ////if (m_animator.GetAnimatorTransitionInfo(0).anyState) {
            ////    var state = m_animatorMonitor.GetStateName(m_animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
            ////    var clip = m_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            ////    Debug.LogFormat("Current State is <color=blue>{0}</color>.  Current clip is <color=blue>{1}</color>", state, clip);
            ////}

            ////if (actionStarted) {
            ////    var state = m_animatorMonitor.GetStateName(m_animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
            ////    var clip = m_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            ////    Debug.LogFormat("Current State is <color=red>{0}</color>.  Current clip is <color=red>{1}</color>", state, clip);
            ////    actionStarted = false;
            ////}

            if (m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_stateName))
            {
                if (m_animator.GetNextAnimatorStateInfo(0).shortNameHash != 0 && m_animator.IsInTransition(0))
                {
                    Debug.LogFormat("{0} has stopped because it is entering Exit State", m_stateName);
                    return true;
                }

                if (m_animator.IsInTransition(0))
                {
                    return true;
                }
            }
            if (Time.time > m_ActionStartTime + 1)
                return true;
            return false;
        }


        protected override void ActionStopped(){
            //m_animator.CrossFade("LocomotionFwd", 0.2f, 0);
            startAngle = 0;
        }


        ////  Returns the state the given layer should be on.
        //public override string GetDestinationState( int layer )
        //{
        //    if (layer == 0)
        //        return m_stateName;
        //    return "";
        //}










	}

}

