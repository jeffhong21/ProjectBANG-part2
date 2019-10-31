using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

namespace JH.RootMotionController
{
    using RootMotionDebug;

    public partial class RootMotionController
    {
        private MovementState locomotionState = MovementState.Standing;

        public bool isMoving;
        public bool isGrounded;
        public bool isStrafing;


        [Header("--  Options  --")]
        /// <summary>   </summary>
        [DebugDisplay(RichTextColor.Magenta)]
        public Vector3 inputVector;
        /// <summary>   </summary>
        [DebugDisplay(RichTextColor.LightBlue)]
        public Vector3 moveDirection;
        /// <summary>   </summary>
        [DebugDisplay(RichTextColor.Orange)]
        public Quaternion moveRotation;
        /// <summary>Difference from the Last Frame and the Current Frame.</summary>
        [DebugDisplay(RichTextColor.Blue)]
        public Vector3 deltaPosition; // { get; private set; }
        /// <summary>World Position on the last Frame.</summary>
        public Vector3 lastPosition { get; internal set; }
        /// <summary>Difference between the Current Rotation and the desire Input Rotation. </summary>
        [DebugDisplay(RichTextColor.Magenta)]
        public float deltaAngle { get; internal set; }
        /// <summary>Total velocity of this frame.</summary>
        [DebugDisplay(RichTextColor.Green)]
        public Vector3 velocityVector { get; private set; }
        [DebugDisplay(RichTextColor.Magenta)]
        public float velocitySpeed { get; private set; }
        [DebugDisplay]
        public float maxMoveSpeed { get; private set; }




        public float rootMotionScale { get { return m_motor.rootMotionScale; } }
        /// <summary>Normalized forward speed.</summary>
        public float normalizedForwardSpeed { get; private set; }
        /// <summary>Normalized forward speed.</summary>
        public float normalizedLateralSpeed { get; private set; }
        /// <summary>Normalized forward speed.</summary>
        public float normalizedTurnSpeed { get; private set; }

        public GroundInfo groundInfo;

        public Vector3 surfaceNormal;

        public float surfaceAngle { get; private set; }
        /// <summary>Direction of ground.  Positive means going up, negative going down.</summary>
        public float surfaceSlope { get; private set; }


        /// <summary>
        /// Gets the ground or air cceleration from motor settings.
        /// </summary>
        private float acceleration{
            get {
                return m_motor.forwardAcceleration;
            }
        }
        private Vector3 m_verticalVelocity;
        private Vector3 m_targetDirection;
        private Quaternion m_targetRotation;
        private float m_inputMagnitude;

        private float m_airTime;




        public void SetMovementState(MovementState locomotion)
        {
            locomotionState = locomotion;
            maxMoveSpeed = Mathf.MoveTowards(maxMoveSpeed, Mathf.Clamp((int)locomotionState, 1, 5), deltaTime * 4);
        }


        public void Move(float horizontalInput, float forwardInput, Quaternion rotation)
        {
            inputVector.Set(horizontalInput, 0, forwardInput);
            if (inputVector.sqrMagnitude > 1)
                inputVector.Normalize();

            m_targetRotation = rotation;
            m_targetDirection = rotation * forward;
            m_targetDirection.y = 0;
            m_targetDirection.Normalize();
        }







        private void OnAnimatorMove()
        {

            bool animatePhysics = m_animator.updateMode == AnimatorUpdateMode.AnimatePhysics;

            //  Get delta position.
            deltaTime = animatePhysics ? m_fixedDeltaTime : m_deltaTime;
            //deltaPosition = position - lastPosition;
            deltaPosition = m_animator.deltaPosition;
            //  Get deltaAngle.
            deltaAngle = 0;
            if (inputVector.sqrMagnitude > 0.2f) {
                //  Find the difference between the current angle and the target angle in radians.
                var currentAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
                var targetAngle = Mathf.Atan2(m_targetDirection.x, m_targetDirection.z) * Mathf.Rad2Deg;
                deltaAngle = Mathf.DeltaAngle(currentAngle, targetAngle);
            }

            velocityVector = deltaPosition  / deltaTime;
            velocitySpeed = Vector3.ProjectOnPlane(velocityVector, surfaceNormal).magnitude;


            //  -----
            Move();

            //  -----
            CheckGround();

            ////  -----
            //CheckMovement();


            //  -----
            UpdateRotation();


            //  -----
            UpdateMovement();


            //  -----
            ApplyMovement();
            //  -----
            UpdateAnimator();





        }





        public void Move()
        {
            //  Get move Direction.
            //moveDirection = m_transform.InverseTransformVector(inputVector);

            //float turnAmount = Mathf.Atan2(inputVector.x, inputVector.z);
            //float forwardAmount = Mathf.Abs(inputVector.z);
            //Vector3 moveVector = new Vector3(turnAmount, m_rigidbody.velocity.y, forwardAmount);
            //moveDirection = velocityVector + m_transform.InverseTransformDirection(moveVector);
            //moveDirection = Quaternion.FromToRotation(m_transform.forward, m_targetDirection) * moveVector;
            ////moveDirection = new Vector3(turnAmount, m_rigidbody.velocity.y, forwardAmount);
            moveDirection = GetMovementVector() + velocityVector;


            float currentSpeed = Vector3.Dot(moveDirection, forward);
            //  Calculate drag
            Vector3 drag = moveDirection * MathUtil.SmoothStop3(m_motor.moveDamping);
            if (inputVector.sqrMagnitude > 0)
                drag *= 1 - Vector3.Dot(forward, m_targetDirection.normalized);
            if (drag.sqrMagnitude > moveDirection.sqrMagnitude)
                moveDirection = Vector3.zero;
            else
                moveDirection -= drag;

            //moveDirection = m_transform.TransformDirection(inputVector) * (m_motor.forwardAcceleration * deltaTime);
            moveDirection += m_animator.deltaPosition * m_inputMagnitude;
            if (moveDirection.sqrMagnitude > m_motor.maxSpeed * m_motor.maxSpeed)
                moveDirection = moveDirection.normalized * currentSpeed;


            isMoving = inputVector.z > 0.1f;
            SetMovementState(isMoving ? MovementState.Walking : MovementState.Standing);
        }




        private void CheckGround()  
        {
            bool groundDetected = false;
            float radius = m_spherecastRadius;
            float distance = radius + m_physics.skinWidth;
            RaycastHit groundHit; // = new RaycastHit();

            if (SphereGroundCast(position, Vector3.down, radius, distance, out groundHit, true, null)) {
                groundHit.distance = Mathf.Max(0.0f, groundHit.distance - k_collisionOffset);
                groundDetected = true;
            }




            if (groundDetected)
            {
                isGrounded = true;
                surfaceAngle = Vector3.Angle(groundHit.normal, Vector3.up);
                surfaceNormal = groundHit.normal;
                //  If dot product is greater than zero, than we are going down slope.  If dot p is negative, we are going up.
                surfaceSlope = Vector3.Dot(moveDirection, surfaceNormal) >= 0 ? -1 : 1;
            } else {
                isGrounded = false;
                surfaceAngle = 0;
            }


        }


        private void CheckMovement()
        {


        }


        private void UpdateRotation()
        {

            if (isMoving) {
                var targetAngle = m_transform.AngleFromForward(m_targetDirection);
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle * rotationSpeed * deltaTime, 0);
                moveRotation = targetRotation * m_transform.rotation;
            }



        }


        private void UpdateMovement()
        {


            if (isGrounded)
            {
                float currentSpeed = Vector3.Dot(moveDirection, forward);

                //  Reorient move vector to the slope of the ground.
                moveDirection = GetDirectionTangentToSurface(moveDirection, surfaceNormal);
                // Reorient target velocity.
                Vector3 right = Vector3.Cross(moveDirection, m_transform.up);
                Vector3 targetVelocity = Vector3.Cross(surfaceNormal, right).normalized * currentSpeed;
                moveDirection = Vector3.Lerp(targetVelocity, m_targetDirection, deltaTime * acceleration);
                moveDirection.y = 0;
            }
            //else {

            //    m_verticalVelocity += -gravity * deltaTime;
            //    var velocityY = Mathf.Min(m_verticalVelocity.y, m_physics.terminalVelocity) ;
            //    m_verticalVelocity.y = velocityY;


            //    moveDirection += m_verticalVelocity;
            //}



        }






        private void ApplyMovement()
        {
            lastPosition = m_transform.position;

            if (!m_rigidbody.isKinematic) {
                //m_rigidbody.velocity = Vector3.zero;
                m_rigidbody.angularVelocity = Vector3.zero;
                m_rigidbody.velocity = moveDirection;

                float moveScale = Vector3.Dot(moveDirection, forward);
                moveDirection.y = 0;
                m_rigidbody.velocity = Vector3.Slerp(m_rigidbody.velocity, moveDirection, deltaTime * acceleration);


            }


            //m_rigidbody.rotation *= moveRotation;
            m_rigidbody.MoveRotation(moveRotation);


            ////m_rigidbody.angularVelocity = Vector3.Lerp(m_rigidbody.angularVelocity, angularVelocity, rotationSpeed * deltaTime);
            //m_rigidbody.MoveRotation(moveRotation);
            //m_rigidbody.velocity = Vector3.Lerp(m_rigidbody.velocity, velocityVector, 20 * deltaTime);
        }


        private void UpdateAnimator()
        {
            float maxForwardSpeed = maxMoveSpeed;
            float maxLateralSpeed = 1;

            normalizedForwardSpeed = Mathf.MoveTowards(normalizedForwardSpeed, inputVector.z * maxForwardSpeed, deltaTime * m_motor.forwardAcceleration);
            normalizedLateralSpeed = Mathf.MoveTowards(normalizedLateralSpeed, inputVector.x * maxLateralSpeed, deltaTime * m_motor.lateralAcceleration);

            normalizedForwardSpeed = Mathf.Approximately(normalizedForwardSpeed, 0) ? 0 : normalizedForwardSpeed;
            normalizedLateralSpeed = Mathf.Approximately(normalizedLateralSpeed, 0) ? 0 : normalizedLateralSpeed;

            m_animator.SetBool(Hash.Moving, isMoving);


            //SetTurningSpeed(m_transform.rotation, moveRotation);
            m_animator.SetFloat(Hash.TurnSpeed, deltaAngle);


            m_animator.SetFloat(Hash.ForwardSpeed, normalizedForwardSpeed);
            m_animator.SetFloat(Hash.LateralSpeed, normalizedLateralSpeed);


        }










        //private void SetTurningSpeed(Quaternion currentRotation, Quaternion newRotation)
        //{
        //    float currentY = currentRotation.eulerAngles.y;
        //    float newY = newRotation.eulerAngles.y;
        //    float difference = (newY - currentY).Wrap() / deltaTime;

        //    //m_normalizedTurnSpeed = Mathf.Lerp(m_normalizedTurnSpeed, Mathf.Clamp(difference / rotationSpeed, -1, 1), deltaTime * rotationSpeed);
        //}


        public Vector3 GetMovementVector()
        {
            var moveVector = m_animator.deltaPosition * m_motor.rootMotionScale;
            return moveVector;
        }

        //public float GetRotationAngle()
        //{
        //    //var angle = Quaternion.Angle()
        //    m_animator.deltaRotation.ToAngleAxis(out float rotationAngle, out Vector3 axis);
        //    return rotationAngle * m_motor.rootMotionScale;
        //}




        //public float GetAverageSlopeAngle()
        //{

        //}



        public bool SphereGroundCast(Vector3 currentPosition, Vector3 direction, float radius, float distance, out RaycastHit hit, bool useSkinWidth = false, Vector3? positionOffset = null)
        {
            float skinWidth = useSkinWidth ? m_physics.skinWidth : 0;
            Vector3 startPosition = currentPosition.WithY(radius + skinWidth);
            Vector3 spherePosition = positionOffset != null ? startPosition + positionOffset.Value : startPosition;
            float maxDistance = distance + skinWidth;

            bool result = Physics.SphereCast(spherePosition, radius, direction, out hit, maxDistance, collisionMask, m_advance.queryTrigger);

            return result;
        }


        public Vector3 GetDirectionTangentToSurface(Vector3 direction, Vector3 surfaceNormal)
        {
            float scale = direction.magnitude;
            Vector3 temp = Vector3.Cross(surfaceNormal, direction);
            Vector3 tangent = Vector3.Cross(temp, surfaceNormal);
            tangent = tangent.normalized * scale;
            return tangent;
        }




        private bool ClimbSlope(ref Vector3 velocity, float moveAmount, float slopeAngle)
        {
            //  Climb slope.
            float directionY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * colliderRadius;
            if (velocity.y <= directionY)
            {
                velocity.y = directionY;
                moveAmount = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * colliderRadius;
                velocity = velocity.normalized * moveAmount;


                return true;
            }
            return false;
        }

    }
}








