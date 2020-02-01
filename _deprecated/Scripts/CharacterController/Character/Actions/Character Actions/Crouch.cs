namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Crouch : CharacterAction
    {
        public override int ActionID { get { return m_actionID = ActionTypeID.Crouch; } set { m_actionID = value; } }


        protected float m_defaultHeight = 0.6f;


        //
        // Methods
        //


        public override bool CanStartAction()
        {
            bool canStart = base.CanStartAction();

            return canStart;
        }

        protected override void ActionStarted()
        {
            m_animator.SetFloat(HashID.Height, m_defaultHeight);
            m_animatorMonitor.SetMovementSetID(ActionID);

            Debug.LogFormat("[{0}] Start: {1}",GetType().Name, Time.time);
        }



        protected override void ActionStopped()
        {
            m_animator.SetFloat(HashID.Height, 1);
            m_animatorMonitor.SetMovementSetID(0);

            Debug.LogFormat("[{0}] Stop: {1}", GetType().Name, Time.time);
        }



        public override bool IsConcurrentAction()
        {
            return true;
        }





    }

}

