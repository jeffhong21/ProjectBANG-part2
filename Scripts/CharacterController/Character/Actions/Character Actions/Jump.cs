namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Jump : CharacterAction
    {
        public override int ActionID {
            get { return m_actionID = ActionTypeID.Jump; }
            set { m_actionID = value; }
        }


        [SerializeField]
        protected float m_jumpHeight = 2;
        [SerializeField]
        protected float m_recurrenceDelay = 0.2f;

        private float m_nextJump;


        protected Vector3 m_verticalVelocity;
        protected Vector3 m_startPosition, m_verticalDirection;
        protected float m_verticalDistance;
        protected float m_elapsedTime;

        [SerializeField]
        protected bool m_startJump;
        [SerializeField]
        protected bool m_hasReachedApex;
        [SerializeField]
        protected bool m_hasLanded;


        private bool useGravityCache;


        /// <summary>
        /// Calculate the jump speed.
        /// </summary>
        /// <param name="jumpHeight">Height of the jump.</param>
        /// <param name="gravity">Downward force applied</param>
        /// <returns></returns>
        public float CalculateJumpSpeed(float jumpHeight, float gravity)
        {
            return Mathf.Sqrt(2 * jumpHeight * gravity);
        }

        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if (!base.CanStartAction()) return false;

            return m_nextJump < Time.time && m_controller.isGrounded && m_rigidbody.velocity.y > -0.01f;
		}


		protected override void ActionStarted()
        {
            m_nextJump = Time.time + m_recurrenceDelay;
            //  Set ActionID parameter.
            if (m_stateName.Length == 0)
                m_animatorMonitor.SetActionID(m_actionID);

            m_verticalVelocity = m_controller.moveDirection;
            m_verticalVelocity.y = CalculateJumpSpeed(m_jumpHeight, m_controller.gravity);
            m_controller.moveDirection = m_verticalVelocity;

            Debug.LogFormat("[{0}] Vertical Velocity: {1}", this.GetType(), m_verticalVelocity);
            //m_rigidbody.isKinematic = false;
            //m_rigidbody.velocity = m_verticalVelocity;
            //m_rigidbody.AddForce(m_verticalVelocity, ForceMode.VelocityChange);
        }



        public override bool CanStopAction()
        {
            return m_rigidbody.velocity.y <= 0;
        }

        protected override void ActionStopped()
        {
            base.ActionStopped();

            //m_rigidbody.useGravity = useGravityCache;
            //m_controller.Velocity = Vector3.zero;
            m_startJump = m_hasReachedApex = m_hasLanded = false;
            m_verticalVelocity = m_startPosition = Vector3.zero;

            m_rigidbody.isKinematic = true;
        }


        public override bool CheckGround()
        {
            //float velocityY = (verticalDistance / time) + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

            if (Time.time < m_ActionStartTime + 0.2f) {
                m_controller.isGrounded = false;
                return false;
            }


            float radius = 0.1f;
            Vector3 origin = m_transform.position.WithY(0.1f);
            origin += Vector3.up * radius;

            if (Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit groundHit, 0.3f * 2, m_layers.SolidLayers)) {
                m_controller.isGrounded = groundHit.distance < 0.3f;
            }
            else {
                m_controller.isGrounded = false;
            }





            return false;
        }


        //public override bool UpdateMovement()
        //{



        //    if (m_rigidbody.velocity.y < 0) {
        //        m_rigidbody.useGravity = useGravityCache;
        //        m_controller.moveDirection += m_controller.Gravity * m_deltaTime;
        //    }




        //    m_elapsedTime += m_deltaTime;
        //    m_verticalDirection = m_transform.position - m_startPosition;
        //    //m_verticalDirection = Vector3.Project(m_verticalDirection, Vector3.up);
        //    m_verticalDistance = (m_verticalDirection.y / m_elapsedTime) + Mathf.Abs(Physics.gravity.y) * m_elapsedTime;
        //    m_verticalDistance *= m_deltaTime;

        //    Vector3 velocity = m_controller.Velocity;
        //    float percentage = 0;
        //    if (m_verticalDistance < m_jumpHeight) {
        //        percentage = Mathf.Clamp01(m_verticalDistance / m_jumpHeight);
        //        percentage *= percentage * percentage;
        //        velocity.y = Mathf.Lerp(m_rigidbody.velocity.y, 0, percentage);
        //    }
        //    else {
        //        percentage = 1;
        //    }


        //    //var verticalVelocity = -(2 * m_jumpHeight) / (m_verticalVelocity.y * m_verticalVelocity.y);
        //    ////m_verticalVelocity = m_controller.Gravity * m_deltaTime + (Vector3.zero * verticalVelocity);
        //    //var velocity = m_controller.Velocity;
        //    velocity += m_controller.Gravity * m_deltaTime;
        //    m_controller.Velocity = velocity;
        //    ////m_rigidbody.velocity += m_verticalVelocity;
        //    Debug.LogFormat("time: {2} | percentage: {0} | distance: {1}", percentage, m_verticalDistance, m_elapsedTime);


        //    if (m_verticalDistance >= m_jumpHeight) {
        //        if (reachedJumpHeight == false) {
        //            reachedJumpHeight = true;
        //            Debug.LogFormat("<b>percentage: {0} | distance: {1}\n vv: {2} | rb: {3}  | tie: {4}</b>",
        //                percentage, m_verticalDistance, m_verticalVelocity, m_rigidbody.velocity, m_elapsedTime);
        //        }
        //    }
        //    else {
        //        reachedJumpHeight = false;
        //    }


        //    return false;
        //}





    }

}

