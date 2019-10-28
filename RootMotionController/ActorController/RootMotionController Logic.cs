using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

namespace JH.RootMotionController
{
    using RootMotionDebug;

    public partial class RootMotionController
    {

        public bool isMoving;
        public bool isGrounded;
        public Vector3 inputVector;// { get; private set; }

        public Vector3 inputVectorRaw{
            get {
                var inputX = inputVector.x > 0 || inputVector.x < 0 ? Mathf.Sign(inputVector.x) * 1 : 0;
                var inputZ = inputVector.z > 0 || inputVector.z < 0 ? Mathf.Sign(inputVector.z) * 1 : 0;
                return new Vector3(inputX, 0, inputZ);
            }
        }

        /// <summary>
        /// Returns movement settings turning speed as radians.
        /// </summary>
        private float rotationSpeed { get => m_motor.turningSpeed * Mathf.Deg2Rad; }

        public float colliderRadius {
            get { return m_actorCollider.radius * m_transform.lossyScale.x; }
            set { m_actorCollider.radius = value * m_transform.lossyScale.x; }
        }

        [DebugDisplay(RichTextColor.Magenta)]
        public float m_targetAngle;
        [DebugDisplay(RichTextColor.Magenta)]
        public float m_targetYRotation;
        [DebugDisplay(RichTextColor.Orange)]
        public float m_angleFromForward;
        [DebugDisplay(RichTextColor.Orange)]
        public float m_transformY;

        [DebugDisplay(RichTextColor.Blue)]
        public Vector3 inputDirection { get; private set; }
        [DebugDisplay(RichTextColor.Orange)]
        public Vector3 moveDirection { get; private set; }
        [DebugDisplay(RichTextColor.Green)]
        public Vector3 velocityVector { get; private set; }
        [DebugDisplay(RichTextColor.DarkGreen)]
        public Vector3 localVelocity { get; private set; }
        [DebugDisplay]
        public float currentSpeed { get; private set; }
        [DebugDisplay]
        public Vector3 angularVelocity { get; private set; }
        [DebugDisplay]
        public Quaternion moveRotation { get; private set; }



        public GroundInfo groundInfo;

        public LayerMask collisionMask { get { return m_collision.collisionsMask; } }

        private float m_groundAngle;
        private Vector3 m_previousVelocity;
        private Vector3 m_verticalVelocity;
        private Vector3 m_targetDirection;
        private Quaternion m_targetRotation;
        private float m_inputMagnitude;

        private float m_airTime;
        private float m_normalizedForwardSpeed;
        private float m_normalizedLateralSpeed;
        private float m_normalizedTurnSpeed;

        private float m_stickyForce;
        private float m_maxSpeed = 1;
        public bool m_isRunning;








        public void Move(float horizontalInput, float forwardInput, Quaternion rotation)
        {
            inputVector.Set(horizontalInput, 0, forwardInput);

            m_targetRotation = rotation;
            m_targetDirection = rotation * forward;
        }


        private void AnimatorMove()
        {
            //m_rigidbody.angularVelocity = Vector3.Lerp(m_rigidbody.angularVelocity, angularVelocity, rotationSpeed * m_deltaTime);
            m_rigidbody.MoveRotation(moveRotation);

            m_rigidbody.velocity = Vector3.Lerp(m_rigidbody.velocity, velocityVector, 20 * m_deltaTime);
        }





        private void Move()
        {
            var localMoveDirection = new Vector3(inputVector.x, 0, inputVector.z);
            //inputDirection = m_targetRotation * localMoveDirection.normalized;
            inputDirection = Quaternion.Inverse(m_transform.rotation) * m_transform.TransformDirection(m_targetRotation * localMoveDirection.normalized);

            m_targetAngle = m_transform.AngleFromForward(m_targetDirection);
            m_angleFromForward = m_targetRotation.eulerAngles.y;
            moveDirection = m_transform.rotation * GetMovementVector();
            velocityVector += moveDirection;


            //velocityVector = GetMovementVector() / m_deltaTime;
            //velocityVector = m_targetRotation * m_verticalVelocity;
            localVelocity = m_transform.InverseTransformDirection(velocityVector).normalized;


            currentSpeed = localVelocity.magnitude;
            isMoving = m_inputMagnitude > m_motor.moveThreshold;
        }


        private void CheckGround()
        {

            bool groundDetected = false;
            float radius = m_spherecastRadius;
            float distance = radius + m_physics.skinWidth;
            RaycastHit groundHit = new RaycastHit
            {
                point = position,
                normal = up
            };


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


            groundInfo = new GroundInfo(groundHit);
            if (groundDetected)
            {
                isGrounded = true;

                m_groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
                groundInfo.angle = m_groundAngle;
                var verticalVelocity = Vector3.ProjectOnPlane(velocityVector, gravity);
                float forwardSpeed = Vector3.Dot(verticalVelocity, Vector3.down);
                m_stickyForce = groundInfo.distance * m_physics.groundStickiness * forwardSpeed;


            }
            else {
                isGrounded = false;
                m_groundAngle = 0;
                groundInfo.angle = 0;
                m_stickyForce = 0;
            }

        }


        private void CheckMovement()
        {


        }


        private void UpdateRotation()
        {
            //m_targetAngle = Vector3.SignedAngle(m_transform.forward, m_targetDirection, up);
            //m_targetAngle = MathUtil.ClampAngle(m_targetAngle, 180);
            //m_targetYRotation = Mathf.Lerp(0, m_targetAngle, rotationSpeed * m_deltaTime);

            //m_angleFromForward = m_transform.AngleFromForward(inputVector);


            Quaternion targetRotation = Quaternion.Euler(0, m_targetAngle * rotationSpeed * m_deltaTime, 0);
            moveRotation = targetRotation * m_transform.rotation;
            //if (isMoving) {
            //    var turnAngle = Vector3.SignedAngle(m_transform.forward, m_targetDirection, up) * Mathf.Rad2Deg;
            //    //m_targetYRotation = Mathf.Lerp(0, turnAngle * rotationSpeed, rotationSpeed * m_deltaTime);

            //    m_targetYRotation = turnAngle ;
            //    var rotation = angularVelocity;
            //    rotation.y = m_targetYRotation;
            //    angularVelocity = rotation;

            //    Quaternion targetRotation = Quaternion.Euler(0, m_targetAngle * rotationSpeed * m_deltaTime, 0);
            //    moveRotation = targetRotation;// * m_transform.rotation;
            //}
            //else {
            //    var turnAngle = Mathf.Atan2(inputVector.x, inputVector.z) * Mathf.Abs(inputVector.x);
            //    m_targetYRotation = Mathf.Lerp(0, turnAngle * rotationSpeed, rotationSpeed * m_deltaTime);
            //    var rotation = angularVelocity;
            //    rotation.y = m_targetYRotation;
            //    angularVelocity = rotation;

            //    //moveRotation = Quaternion.Euler(angularVelocity);
            //}


        }


        private void UpdateMovement()
        {
            if (isGrounded) {

            }
            else {
                m_verticalVelocity += gravity * m_deltaTime;
            }
        }


        private void UpdateAnimator()
        {
            m_animator.SetBool("Moving", isMoving);


            SetTurningSpeed(m_transform.rotation, moveRotation);


            m_normalizedForwardSpeed = Mathf.Lerp(m_normalizedForwardSpeed, inputVector.z, m_deltaTime);
            m_normalizedLateralSpeed = Mathf.Lerp(m_normalizedLateralSpeed, inputVector.x, m_deltaTime);

            m_normalizedForwardSpeed = Mathf.Approximately(inputVector.z, 0) ? 0 : inputVector.z;
            m_normalizedLateralSpeed = Mathf.Approximately(inputVector.x, 0) ? 0 : inputVector.x;

            m_animator.SetFloat("ForwardInput", m_normalizedForwardSpeed, 0.1f, m_deltaTime);
            m_animator.SetFloat("HorizontalInput", m_normalizedLateralSpeed, 0.1f, m_deltaTime);

            //m_animator.SetFloat("ForwardInput", Mathf.Clamp((inputVector.z + localVelocity.z) * 0.5f, -1 * m_maxSpeed, m_maxSpeed));
            //m_animator.SetFloat("HorizontalInput", Mathf.Clamp(inputVector.x, -1 * m_maxSpeed, m_maxSpeed));
        }


        private void SetTurningSpeed(Quaternion currentRotation, Quaternion newRotation)
        {
            float currentY = currentRotation.eulerAngles.y;
            float newY = newRotation.eulerAngles.y;
            float difference = (newY - currentY).Wrap() / m_deltaTime;

            m_normalizedTurnSpeed = Mathf.Lerp(m_normalizedTurnSpeed, Mathf.Clamp(difference / rotationSpeed, -1, 1), m_deltaTime * rotationSpeed);
        }


        public Vector3 GetMovementVector()
        {
            var moveVector = m_animator.deltaPosition * m_motor.rootMotionScale;
            return moveVector;
        }

        public float GetRotationAngle()
        {
            //var angle = Quaternion.Angle()
            m_animator.deltaRotation.ToAngleAxis(out float rotationAngle, out Vector3 axis);
            return rotationAngle * m_motor.rootMotionScale;
        }



        public bool SphereGroundCast(Vector3 currentPosition, Vector3 direction, float radius, float distance, out RaycastHit hit, bool useSkinWidth = false, Vector3? positionOffset = null)
        {
            float skinWidth = useSkinWidth ? m_physics.skinWidth : 0;
            Vector3 startPosition = currentPosition.WithY(radius + skinWidth);
            Vector3 spherePosition = positionOffset != null ? startPosition + positionOffset.Value : startPosition;
            float maxDistance = distance + skinWidth;

            bool result = Physics.SphereCast(spherePosition, radius, direction, out hit, maxDistance, collisionMask, m_advance.queryTrigger);

            return result;
        }










        private void OnDrawGizmos()
        {
            if(Application.isPlaying)
            {
                if (debugMode.showMotionVectors)
                {
                    var drawHeight = 0.1f;
                    var drawStep = 0.05f;
                    var drawSubstep = 0.025f;

                    DebugDrawer.DrawArrow(position.WithY(drawHeight), forward, debugMode.options.forwardDirectionColor, 1, debugMode.options.arrowTip, debugMode.options.arrowWidth);


                    if (m_targetDirection != Vector3.zero) {
                        DebugDrawer.DrawArrow(position.WithY(0.5f), m_targetDirection, debugMode.options.targetDirectionColor);
                    }
                    if (moveDirection != Vector3.zero) {
                        DebugDrawer.DrawArrow(position.WithY(drawHeight + drawSubstep), Quaternion.Inverse(m_transform.rotation) * moveDirection, debugMode.options.moveDirectionColor);
                    }

                    if (inputDirection != Vector3.zero) {
                        DebugDrawer.DrawArrow(position.WithY(drawHeight), m_transform.rotation * inputDirection, debugMode.options.inputDirectionColor);
                    }

                    if (velocityVector != Vector3.zero) {
                        DebugDrawer.DrawArrow(position.WithY(drawHeight + (1 * drawStep)), velocityVector, debugMode.options.velocityColor);
                    }
                    if (m_rigidbody.velocity != Vector3.zero) {
                        DebugDrawer.DrawArrow(position.WithY(drawHeight + (1 * drawStep) + drawSubstep),m_rigidbody.velocity,debugMode.options.velocityColor.Darker());
                    }
                }
            }

        }



        GUIStyle guiStyle = new GUIStyle();
        Rect rect = new Rect();
        private void OnGUI()
        {
            if (animator == null) return;
            guiStyle.normal.textColor = Color.white;
            guiStyle.fontStyle = FontStyle.Bold;
            rect.width = Screen.width * 0.25f;
            rect.x = (Screen.width * 0.5f) - (rect.width * 0.5f) + 18;
            rect.y = (Screen.height * 0.5f) - (rect.height * 0.5f) - 18;
            rect.height = 16 + rect.y;
            GUI.Label(rect, "pivotWeight: " + animator.pivotWeight.ToString(), guiStyle);
            rect.y += rect.height = 16;
            rect.height += rect.height;
            GUI.Label(rect, "feetPivotActive: " + animator.feetPivotActive.ToString(), guiStyle);


        }
    }
}
