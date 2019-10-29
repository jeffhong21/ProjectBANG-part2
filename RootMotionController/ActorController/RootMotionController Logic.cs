using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

namespace JH.RootMotionController
{
    using RootMotionDebug;

    public partial class RootMotionController
    {
        private LocomotionState locomotionState = LocomotionState.Standing;

        public bool isMoving;
        public bool isGrounded;
        public bool isStrafing;

        [DebugDisplay(RichTextColor.Magenta)]
        public Vector3 inputVector;
        /// <summary>Direction of input vector.e</summary>
        [DebugDisplay(RichTextColor.Magenta)]
        public Vector3 inputDirection { get; private set; }



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
        [DebugDisplay]
        public float deltaAngle { get; internal set; }
        /// <summary>Total velocity of this frame.</summary>
        [DebugDisplay(RichTextColor.Green)]
        public Vector3 velocityVector { get; private set; }
        [DebugDisplay(RichTextColor.Green)]
        public float velocitySpeed { get; private set; }
        [DebugDisplay]
        public float maxMoveSpeed { get; private set; }

        public bool moveWithDirection { get; private set; }



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




        private Vector3 m_verticalVelocity;
        private Vector3 m_targetDirection;
        private Quaternion m_targetRotation;
        private float m_inputMagnitude;

        private float m_airTime;




        public void SetLocomotionState(LocomotionState locomotion)
        {
            locomotionState = locomotion;
        }


        public void Move(float horizontalInput, float forwardInput, Quaternion rotation)
        {
            inputVector.Set(horizontalInput, 0, forwardInput);
            if (inputVector.sqrMagnitude > 1)
                inputVector.Normalize();

            if (inputVector.sqrMagnitude > 1) inputVector.Normalize();

            m_targetRotation = rotation;

            if (isGrounded)
                m_targetDirection = rotation * inputVector;
            m_targetDirection = inputVector;
            //m_targetDirection = rotation * forward;


            ////  -- In default movement.
            //moveWithDirection = true;
            //isMoving = inputVector.z > m_motor.moveThreshold;



            //if (isGrounded)
            //    inputVector = Quaternion.FromToRotation(Vector3.up, surfaceNormal) * inputVector;
            //m_targetDirection = inputVector;



            //  Get deltaAngle.
            deltaAngle = 0;
            if (isMoving) {
                //  Find the difference between the current angle and the target angle in radians.
                var currentAngle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
                var targetAngle = Mathf.Atan2(m_targetDirection.x, m_targetDirection.z) * Mathf.Rad2Deg;
                deltaAngle = Mathf.DeltaAngle(currentAngle, targetAngle);
            }

            //inputVector = m_transform.InverseTransformVector(inputVector);

            float turnAmount = Mathf.Atan2(inputVector.x, inputVector.z);
            float forwardAmount = Mathf.Abs(inputVector.z);

            inputDirection.Set(turnAmount, 0, forwardAmount);
        }







        private void OnAnimatorMove()
        {

            bool animatePhysics = m_animator.updateMode == AnimatorUpdateMode.AnimatePhysics;
            deltaTime = animatePhysics ? m_fixedDeltaTime : m_deltaTime;
            deltaPosition = position - lastPosition;
            lastPosition = m_transform.position;


            ResetValues();

            if (deltaTime > 0 && deltaPosition != Vector3.zero) {
                velocityVector = deltaPosition / deltaTime;
                velocitySpeed = Vector3.ProjectOnPlane(velocityVector, -gravity).magnitude;
            }

            isMoving = inputDirection.z > m_motor.moveThreshold;
            locomotionState = isMoving ? LocomotionState.Walking : LocomotionState.Standing;
            maxMoveSpeed = Mathf.MoveTowards(maxMoveSpeed, Mathf.Clamp((int)locomotionState, 1, 5), deltaTime * 4);

            //  -----
            SetMovement();



            //  -----
            CheckGround();

            //  -----
            CheckMovement();


            //  -----
            UpdateRotation();


            //  -----
            UpdateMovement();


            //  -----
            ApplyMovement();
            //  -----
            UpdateAnimator();




            ////m_rigidbody.angularVelocity = Vector3.Lerp(m_rigidbody.angularVelocity, angularVelocity, rotationSpeed * deltaTime);
            //m_rigidbody.MoveRotation(moveRotation);
            //m_rigidbody.velocity = Vector3.Lerp(m_rigidbody.velocity, velocityVector, 20 * deltaTime);
        }





        public void SetMovement()
        {
            //  -----
            float forwardAcceleration = 3;
            float lateralAcceleration = 4;
            float maxForwardSpeed = maxMoveSpeed;
            float maxLateralSpeed = 1;

            normalizedForwardSpeed = Mathf.MoveTowards(normalizedForwardSpeed, inputVector.z * maxForwardSpeed, deltaTime * forwardAcceleration);
            normalizedLateralSpeed = Mathf.MoveTowards(normalizedLateralSpeed, inputVector.x * maxLateralSpeed, deltaTime * lateralAcceleration);

            normalizedForwardSpeed = Mathf.Approximately(normalizedForwardSpeed, 0) ? 0 : normalizedForwardSpeed;
            normalizedLateralSpeed = Mathf.Approximately(normalizedLateralSpeed, 0) ? 0 : normalizedLateralSpeed;
        }




        private void CheckGround()  
        {
            bool groundDetected = false;
            float radius = m_spherecastRadius;
            float distance = radius + m_physics.skinWidth;
            RaycastHit groundHit = new RaycastHit();

            if (isGrounded)
            {
                if (SphereGroundCast(position, Vector3.down, radius, distance, out groundHit, true, null)) {
                    groundHit.distance = Mathf.Max(0.0f, groundHit.distance - k_collisionOffset);
                    groundDetected = true;
                }

                if (!groundDetected) {
                    radius = colliderRadius * 0.9f;
                    if (SphereGroundCast(position, Vector3.down, radius, distance, out groundHit, true, null)) {
                        groundHit.distance = Mathf.Max(0.0f, groundHit.distance - k_collisionOffset);
                        groundDetected = true;
                    }
                }
            }
            else
            {
                radius = colliderRadius * 0.9f;
                distance = 20;
                if (SphereGroundCast(position, Vector3.down, radius, distance, out groundHit, true, forward )) {
                    groundHit.distance = Mathf.Max(0.0f, groundHit.distance - k_collisionOffset);
                    groundDetected = true;
                }
            }



            if (groundDetected)
            {
                isGrounded = true;
                surfaceAngle = Vector3.Angle(groundHit.normal, Vector3.up);
                surfaceNormal = groundHit.normal;
            }
            else {
                isGrounded = false;
                surfaceAngle = 0;

            }

        }


        private void CheckMovement()
        {


        }

        private void UpdateMovement()
        {
            if (isGrounded) {

            }
            else {
                //m_verticalVelocity += gravity * deltaTime;
            }
        }


        private void UpdateRotation()
        {

            //if (isMoving)
            //{
            //    if (!isStrafing){
            //        Quaternion additiveRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(0, deltaAngle, 0),
            //                                                        deltaTime * ((rotationSpeed + 1) / 4) * ((5 + 1) * 4));
            //        moveRotation *= additiveRotation;
            //    }
            //    else {
            //        //  -- If strafing.
            //        var rotation = rotationSpeed * 10;
            //        //Add +Rotation when going Forward and -Rotation when going backwards
            //        var turnInput = Mathf.Clamp(normalizedLateralSpeed, -1, 1) * (inputVector.z >= 0 ? 1 : -1);  

            //        moveRotation *= Quaternion.Euler(0, rotation * turnInput * deltaTime, 0);
            //    }
            //}

            var targetAngle = m_transform.AngleFromForward(m_targetDirection);
            Quaternion targetRotation = Quaternion.Euler(0,  targetAngle * rotationSpeed * deltaTime, 0);
            moveRotation = targetRotation;


        }



        private void ApplyMovement()
        {
            if (!m_rigidbody.isKinematic) {
                m_rigidbody.velocity = Vector3.zero;
                m_rigidbody.angularVelocity = Vector3.zero;
                if (deltaTime > 0) {
                    m_rigidbody.velocity = velocityVector; // moveDirection / deltaTime;
                }
            }
            //else {
            //    m_rigidbody.position += moveDirection;
            //}


            //m_rigidbody.rotation *= moveRotation;
            m_rigidbody.MoveRotation(moveRotation * m_transform.rotation);
        }


        private void UpdateAnimator()
        {
            m_animator.SetBool(Hash.Moving, isMoving);


            SetTurningSpeed(m_transform.rotation, moveRotation);



            m_animator.SetFloat(Hash.ForwardSpeed, normalizedForwardSpeed);
            m_animator.SetFloat(Hash.LateralSpeed, normalizedLateralSpeed);


        }







        private void SetTurningSpeed(Quaternion currentRotation, Quaternion newRotation)
        {
            float currentY = currentRotation.eulerAngles.y;
            float newY = newRotation.eulerAngles.y;
            float difference = (newY - currentY).Wrap() / deltaTime;

            //m_normalizedTurnSpeed = Mathf.Lerp(m_normalizedTurnSpeed, Mathf.Clamp(difference / rotationSpeed, -1, 1), deltaTime * rotationSpeed);
        }


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



        public bool SphereGroundCast(Vector3 currentPosition, Vector3 direction, float radius, float distance, out RaycastHit hit, bool useSkinWidth = false, Vector3? positionOffset = null)
        {
            float skinWidth = useSkinWidth ? m_physics.skinWidth : 0;
            Vector3 startPosition = currentPosition.WithY(radius + skinWidth);
            Vector3 spherePosition = positionOffset != null ? startPosition + positionOffset.Value : startPosition;
            float maxDistance = distance + skinWidth;

            bool result = Physics.SphereCast(spherePosition, radius, direction, out hit, maxDistance, collisionMask, m_advance.queryTrigger);

            return result;
        }






        private void ResetValues()
        {
            moveDirection = m_animator.deltaPosition;
            moveRotation = m_animator.deltaRotation;

            surfaceNormal = Vector3.down;
        }




    }
}









////public void StrafeMovement()
////{
////    moveWithDirection = false;

////    isMoving = inputVector.x > m_motor.moveThreshold || inputVector.z > m_motor.moveThreshold;

////    moveDirection = inputVector;

////    inputDirection = m_transform.TransformDirection(inputVector);
////}

//private void SetMovement()
//{
//    float forwardAcceleration = 3;
//    float lateralAcceleration = 4;
//    float maxForwardSpeed = maxMoveSpeed;
//    float maxLateralSpeed = 1;

//    normalizedForwardSpeed = Mathf.MoveTowards(normalizedForwardSpeed, inputVector.z * maxForwardSpeed, deltaTime * forwardAcceleration);
//    normalizedLateralSpeed = Mathf.MoveTowards(normalizedLateralSpeed, inputVector.x * maxLateralSpeed, deltaTime * lateralAcceleration);

//    //if (Mathf.Abs(normalizedForwardSpeed) < 0.001f) normalizedForwardSpeed = 0;
//    //if (Mathf.Abs(normalizedLateralSpeed) < 0.001f) normalizedLateralSpeed = 0;
//    normalizedForwardSpeed = Mathf.Approximately(normalizedForwardSpeed, 0) ? 0 : normalizedForwardSpeed;
//    normalizedLateralSpeed = Mathf.Approximately(normalizedLateralSpeed, 0) ? 0 : normalizedLateralSpeed;
//}
