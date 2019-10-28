namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Sprint : CharacterAction
    {
        [SerializeField]
        protected float m_SpeedChangeMultiplier = 2f;
        [SerializeField]
        protected float m_MinSpeedChange = -2f;
        [SerializeField]
        protected float m_MaxSpeedChange = 2f;
        [SerializeField]
        protected float m_StaminaDecreaseRate = 0.5f;
        [SerializeField]
        protected float m_StaminaIncreaseRate = 0.1f;
        [SerializeField]
        protected float m_MaxStanima = 100;

        [SerializeField, DisplayOnly]
        private float m_CurrentStanima = 100;
        [SerializeField, DisplayOnly]
        private Vector3 m_SpeedInput;


		//
		// Methods
		//
		protected override void Awake()
		{
            base.Awake();

            m_CurrentStanima = m_MaxStanima;
		}

        public override bool CanStartAction()
        {
            if (base.CanStartAction()){
                return m_CurrentStanima > (m_MaxStanima * 0.1f);
            }
            return false;
		}

		public override void UpdateAction()
		{
            if(m_CurrentStanima < m_MaxStanima){
                m_CurrentStanima = Mathf.Clamp(m_CurrentStanima + m_StaminaIncreaseRate, 0, 100);
            }
		}



        protected override void ActionStarted()
        {
            
        }


        public override bool CanStopAction()
        {
            return base.CanStopAction() || (m_CurrentStanima <= 0);
        }

        protected override void ActionStopped()
        {


        }


		public override bool UpdateAnimator()
		{
            m_SpeedInput = m_controller.inputVector;
            m_SpeedInput.z = Mathf.Clamp(m_controller.inputVector.z * m_SpeedChangeMultiplier, m_MinSpeedChange, m_MaxSpeedChange);
            m_controller.inputVector = m_SpeedInput;
            return true;
		}


		public override bool UpdateMovement()
        {
            m_CurrentStanima = Mathf.Clamp(m_CurrentStanima - m_StaminaDecreaseRate, 0, 100);

            ////m_rigidbody.AddForce(m_transform.forward * m_SpeedChangeMultiplier * m_deltaTime, ForceMode.VelocityChange);

            //var velocity = (m_animator.deltaPosition / m_deltaTime);
            //velocity.y = m_controller.isGrounded ? 0 : m_rigidbody.velocity.y;
            ////m_rigidbody.velocity = Vector3.SmoothDamp(m_rigidbody.velocity, m_Velocity, ref m_velocitySmooth, m_Moving ? m_Acceleration : m_MotorDamping);
            ////m_rigidbody.velocity = Vector3.Lerp(m_rigidbody.velocity, m_Velocity, m_MovementSpeed);
            //m_rigidbody.velocity = velocity;

            return true;
        }


		public override bool IsConcurrentAction()
		{
            return true;
		}






	}

}

