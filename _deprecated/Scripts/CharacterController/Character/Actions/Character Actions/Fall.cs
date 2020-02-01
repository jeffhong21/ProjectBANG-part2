namespace CharacterController
{
    using UnityEngine;


    public class Fall : CharacterAction
    {
        public override int ActionID { get { return m_actionID = ActionTypeID.Fall; } set { m_actionID = value; } }


        [SerializeField]
        protected float m_minFallHeight = 1f;
        [SerializeField]
        protected SurfaceEffect m_landSurfaceImpact;
        [SerializeField]
        protected float m_minSurfaceImpactVelocity = 1f;


        protected bool m_isAirborne;
        protected Vector3 m_currentPosition;
        protected Vector3 m_startPosition;
        protected float m_fallHeight;
        protected float m_fallTime;


        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if(!m_controller.isGrounded && m_rigidbody.velocity.y < 0.01f)
            {
                m_fallTime += Time.deltaTime;
                m_currentPosition = m_transform.position;
                if (!m_isAirborne)
                {
                    m_startPosition = m_currentPosition;
                    m_isAirborne = true;
                }

                m_fallHeight = m_startPosition.y - m_currentPosition.y;

                if(Mathf.Abs(m_fallHeight) > m_minFallHeight)
                {
                    return true;
                }
            }
            else
            {
                if(m_controller.isGrounded || m_rigidbody.velocity.y >= 0 && m_isAirborne)
                {
                    m_fallTime = 0;
                    m_currentPosition = default;
                    m_startPosition = default;
                    m_fallHeight = 0;
                    m_isAirborne = false;
                }
            }



            return false;
		}


		protected override void ActionStarted()
        {
            m_animatorMonitor.SetActionID(ActionID);
        }


        //public override bool CheckGround()
        //{
        //    float radius = 0.1f;
        //    Vector3 origin = m_transform.position + Vector3.up * (0.1f);
        //    origin += Vector3.up * radius;

        //    if(Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit groundHit, 0.3f * 2, m_layers.SolidLayers))
        //    {
        //        m_controller.isGrounded = groundHit.distance < 0.3f;
        //    }
        //    else
        //    {
        //        m_controller.isGrounded = false;
        //    }


        //    if (m_controller.isGrounded && m_rigidbody.velocity.y > -1.01f)
        //    {
        //        m_animatorMonitor.SetActionID(0);
        //    }


        //    DebugUI.DebugUI.Log(this, "GroundDistance", groundHit.distance, DebugUI.RichTextColor.Red);

        //    return false;
        //}


        //public override bool UpdateMovement()
        //{
        //    m_controller.Velocity += m_controller.Gravity * 1.5f * Time.deltaTime;
        //    return false;
        //}



        public override bool CanStopAction()
        {
            if (m_controller.isGrounded && m_rigidbody.velocity.y > -0.01f)
            {
//                Debug.Log(m_fallTime);
                return true;
            }

            return false;
        }


        public override bool UpdateAnimator()
        {
            m_fallTime += Time.deltaTime;
            m_fallHeight = m_startPosition.y - m_currentPosition.y;
            m_animatorMonitor.SetActionFloatData(m_fallTime);



            return true;
        }


        protected override void ActionStopped()
        {

            m_currentPosition = default;
            m_startPosition = default;
            m_fallHeight = 0;
            m_fallTime = 0;
            m_isAirborne = false;


            DebugUI.DebugUI.Remove(this, "GroundDistance");
        }








    }
}

