namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class QuickTurn : CharacterAction
    {
         public override int ActionID { get { return m_actionID = ActionTypeID.QuickTurn; } set { m_actionID = value; } }


        [SerializeField]
        protected int maxInputCount = 8;

        [Range(-1, 1)]
        protected float threshold = -0.75f;     //  The threshold required to start checking for inputs
        protected bool startInputChecks;
        protected int inputCounts;
        //  Remaing rotation when action is in motion.
        protected float rotationRemaining;
        //  Cach the input direction
        protected Vector3 inputDirection;
        //  What direction turning towards.
        protected float turnDirection;
        //  The starting angle for lerping.
        protected float startAngle = 135f;


        private Vector3 velocitySmoothDamp, rotationSmoothDamp;

        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if (!base.CanStartAction()) return false;

            if (!m_controller.isMoving || !m_controller.isGrounded) return false;


            if (startInputChecks)
            {
                inputCounts++;
                if (inputCounts >= maxInputCount)
                {
                    inputCounts = 0;
                    startInputChecks = false;
                    return true;
                }
                return false;
            }


            float dot = Vector3.Dot(m_controller.inputDirection.normalized, m_transform.forward);
            if (!startInputChecks && dot <= threshold)
            {
                startInputChecks = true;
                inputDirection = m_controller.inputVector.normalized;
                rotationRemaining = Vector3.Angle(m_transform.forward, inputDirection);
                //Debug.LogFormat("dot: {0} | rotationRemaining: {1}", dot, rotationRemaining);
            }
            return false;
        }



        protected override void ActionStarted()
        {
            //  Get the start angle.
            startAngle = rotationRemaining;
            //  Get the turn direction.
            turnDirection = Vector3.Cross(inputDirection, m_transform.forward).y >= 0 ? -1f : 1f;


            //  Set ActionID parameter.
            if (m_stateName.Length == 0)
                m_animator.SetInteger(HashID.ActionID, m_actionID);
        }


        public override bool CanStopAction()
        {
            rotationRemaining = Vector3.Angle(m_transform.forward, inputDirection);
            //return rotationRemaining <= 10;
            if (Time.time > m_ActionStartTime + 1 || rotationRemaining <= 10)
                return true;
            return false;
        }


        protected override void ActionStopped()
        {
            base.ActionStopped();

            Debug.LogFormat("inputDir: {0}", inputDirection);

            inputDirection = default;
            rotationRemaining = 0;
            turnDirection = 0;
        }




        public override bool UpdateMovement()
        {

            float dot = Vector3.Dot(inputDirection, m_transform.forward);
            m_rigidbody.velocity = Vector3.SmoothDamp(m_rigidbody.velocity, dot > 0 ? m_controller.RootMotionVelocity : Vector3.zero, ref velocitySmoothDamp, 0.1f);


            return false;
        }


        public override bool UpdateRotation()
        {
            float dot = Vector3.Dot(inputDirection, m_transform.forward);

            float degreesPerSecond = 120;
            turnDirection = Vector3.Cross(inputDirection, m_transform.forward).y >= 0 ? -1f : 1f;
            Vector3 rotationVector = Vector3.Lerp(Vector3.zero, new Vector3(0, degreesPerSecond * (turnDirection < 0 ? -1 : 1), 0), Mathf.Abs(turnDirection));
            Quaternion turnRotation = Quaternion.Euler(rotationVector);
			turnRotation = m_transform.rotation * turnRotation;



			turnRotation.ToAngleAxis(out float angle, out Vector3 axis);
            m_rigidbody.angularVelocity = Vector3.Lerp(m_rigidbody.angularVelocity, axis.normalized * angle, m_deltaTime * m_controller.rotationSpeed);


            // float rotationAngle = Vector3.Angle(m_transform.forward, inputDirection);
            // float t = rotationAngle / startAngle;
            // //float t = startAngle / (startAngle - rotationAngle); //  180 / (180 - X)
            // float u = (1 - t);
            // float percentage = 1 - (u * u * u);


            // Quaternion currentRotation = Quaternion.AngleAxis(turnDirection * rotationAngle, m_transform.up);
            // Quaternion targetRotation = Quaternion.AngleAxis(startAngle, m_transform.up);

            // m_rigidbody.MoveRotation(Quaternion.Slerp(currentRotation, targetRotation, percentage) );

            // Debug.LogFormat("<b><color=red>[QuickTurn] percentage: {0} </color></b>", percentage);

            return false;
        }





        protected float Interpolate(float current, float total)
        {
            float t = total / (total - current); //  180 / (180 - X)
            float percentage = t * t * t;

            return percentage;
        }


        //protected float SpikeInterpolate(float current, float total)
        //{
        //    var t = Interpolate(current, total);
        //    if(t <=)
        //}
    }

}

