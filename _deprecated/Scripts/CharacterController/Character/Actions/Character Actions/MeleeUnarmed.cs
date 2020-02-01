namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class MeleeUnarmed : CharacterAction
    {

        [SerializeField] private string m_enterStateName = "MeleeUnarmed";
        [SerializeField] private string m_enterStateFullPath;
        [SerializeField] private int m_enterStateFullPathHash;
        [SerializeField] private string m_exitStateName = "Fists2Idle";
        [SerializeField] private string m_exitStateFullPath;
        [SerializeField] private int m_exitStateFullPathHash;
        //private int m_layerIndex = -1;
        private bool m_exiting;
        private AnimatorTransitionInfo m_previousTransition;


        private int m_intDataRandom;

        //
        // Methods
        //
        private void Start()
        {

            for (int i = 0; i < m_animator.layerCount; i++) {
                if (m_animator.HasState(i, Animator.StringToHash(m_exitStateName))){
                    m_layerIndex = i;
                    break;
                }
            }
            m_enterStateFullPath = m_animator.GetLayerName(m_layerIndex) + "." + GetType().Name + "." + m_enterStateName;
            m_exitStateFullPath = m_animator.GetLayerName(m_layerIndex) + "." + GetType().Name + "." + m_exitStateName;
            m_enterStateFullPathHash = Animator.StringToHash(m_enterStateFullPath);
            m_exitStateFullPathHash = Animator.StringToHash(m_exitStateFullPath);
        }


        public override bool CanStartAction()
        {
            if (!base.CanStartAction()) return false;

            if (this.IsActive) return false;

            if(m_animator.HasState(m_layerIndex, Animator.StringToHash(m_enterStateName))) {
                m_animator.CrossFade(m_enterStateFullPathHash, 0.1f, m_layerIndex);
                return true;
            }

            if(m_animator.HasState(m_layerIndex, Animator.StringToHash(m_enterStateName)) == false) {
                Debug.LogFormat("<color=red>[Animator]</color> Cannot find the enter state for {0}", GetType().Name);
                return false;
            }

            return false;
        }


        protected override void ActionStarted()
        {

            m_intDataRandom = UnityEngine.Random.Range(0, 2 + 1);
            m_animatorMonitor.SetActionIntData(m_intDataRandom);
        }



        public override bool Move()
        {
            
            return false;
        }


        public override bool UpdateMovement()
        {
            m_controller.moveDirection = Vector3.Lerp(m_controller.moveDirection, Vector3.zero, m_deltaTime * 8);
            //m_rigidbody.AddForce(m_controller.RootMotionVelocity * m_deltaTime, ForceMode.Acceleration);
            return false;
        }


        public override bool UpdateAnimator()
        {

            return base.UpdateAnimator();
        }



        public override bool CanStopAction()
        {
            if(m_exiting == false)
                m_exiting = m_animator.GetCurrentAnimatorStateInfo(m_layerIndex).fullPathHash == m_exitStateFullPathHash;

            AnimatorTransitionInfo currentTransition = m_animator.GetAnimatorTransitionInfo(m_layerIndex);
            int previousTransition = m_previousTransition.fullPathHash;
            if (currentTransition.fullPathHash != previousTransition) {
                return true;
            }

            return false;
        }



        public override string GetDestinationState(int layer)
        {
            if (layer == 0) {
                return "MeleeUnarmed.MeleeUnarmed";
            }
            return "";
        }


    }

}

