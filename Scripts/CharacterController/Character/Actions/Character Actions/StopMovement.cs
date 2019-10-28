namespace CharacterController
{
    using UnityEngine;
    using System;


    public class StopMovement : CharacterAction
    {
        public override int ActionID { get { return m_actionID = ActionTypeID.StopMovement; } set { m_actionID = value; } }

        [SerializeField] protected int maxInputCount = 4;
        [SerializeField, Range(0, 1)]
        protected float stopThreshold = 0.8f;
        //[SerializeField, Range(0.01f, 0.49f)]
        //protected float pivotWeightThreshold = 0.18f;

        protected int detectionCount;
        protected float lastMoveAmount;
        protected float currentMoveAmount;
        protected bool isStopMoving;
        protected float moveAmount;
        //  The direction the character is moving.
        protected float moveDirection;



        protected int successfulStarts;


        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if (!m_controller.isGrounded) return false;



            lastMoveAmount = currentMoveAmount;
            currentMoveAmount = Mathf.Clamp01(Mathf.Abs(m_controller.inputVector.x) + Mathf.Abs(m_controller.inputVector.z));

            if (lastMoveAmount > currentMoveAmount)
                isStopMoving = true;


            if (isStopMoving)
            {
                if (detectionCount >= maxInputCount && currentMoveAmount < stopThreshold)
                {
                    successfulStarts++;
                    //return true;
                }
                //if (detectionCount >= maxInputCount && currentMoveAmount < stopThreshold) {
                //    detectionCount = 0;
                //    isStopMoving = false;
                //    successfulStarts++;
                //    return true;
                //}
                detectionCount++;

                if (successfulStarts >= 3)
                {
                    successfulStarts = 0;
                    detectionCount = 0;
                    isStopMoving = false;
                    return true;
                }
            }


            if (currentMoveAmount <= 0 || lastMoveAmount <= currentMoveAmount) {
                detectionCount = 0;
                isStopMoving = false;
            }


            //CharacterDebug.Log("--- IsStopMoving", m_animator.pivotWeight);
            return false;
        }




        protected override void ActionStarted()
        {
            //  Set ActionID parameter.
            if (m_stateName.Length == 0)
                m_animatorMonitor.SetActionID(m_actionID);

            //  Determine if we should play walk or run stop.
            int actionIntData = 1;
            actionIntData = 2;  //  TODO: figure out wha tto do with speed or how to determine walk stop or run stop.
            m_animatorMonitor.SetActionIntData(actionIntData);

            //Debug.LogFormat("<b>[{0}]</b> ActionStarted.  FwdInput is", m_controller.m_forwardSpeed);

        }







        public override bool CanStopAction()
        {
            //if (m_animator.pivotWeight == 0.5f) {
            //    Debug.LogFormat("Stopping {0} by pivotWeight");
            //    return true;
            //}
                

            int layerIndex = 0;
            if (m_animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == 0 && m_animator.IsInTransition(layerIndex)) {
                Debug.LogFormat("{1} is exiting. | {0} is the next state.", m_animatorMonitor.GetStateName(m_animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash), this.GetType());
                Debug.Log(Mathf.Abs(m_rigidbody.velocity.x) + Mathf.Abs(m_rigidbody.velocity.z));
                return true;
            }
            //if (m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(m_DestinationStateName))
            //{
            //    if (m_animator.GetNextAnimatorStateInfo(0).shortNameHash != 0 && m_animator.IsInTransition(0))
            //    {
            //        Debug.LogFormat("{0} has stopped because it is entering Exit State", m_stateName);
            //        return true;
            //    }

            //    if (m_animator.IsInTransition(0))
            //    {
            //        return true;
            //    }
            //}

            //if (Mathf.Abs(m_rigidbody.velocity.x) + Mathf.Abs(m_rigidbody.velocity.z) <= 0.05f) return true;

            //if (m_animator.pivotWeight == 0.5f) return true;

            if (Time.time > m_ActionStartTime + .5f)
                return true;
            return false;
        }






    }

}

