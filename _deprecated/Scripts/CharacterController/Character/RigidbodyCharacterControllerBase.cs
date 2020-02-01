using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections.Generic;





namespace CharacterController
{
    using DebugUI;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody), typeof(LayerManager))]
    public abstract partial class RigidbodyCharacterControllerBase : MonoBehaviour
    {
        protected const float k_collisionOffset = 0.001f;
        protected const float k_groundedDistance = 0.001f;
        protected const float k_wallAngle = 85f;
        protected readonly float k_gravity = -9.81f;

        public enum MovementTypes { Adventure, Explorer, Combat };

        #region Parameters
        [Serializable]
        public class MovementSettings
        {

            [Header("Root Motion Settings")]
            [Tooltip("Should root motion be used to move the character.")]
            public bool useRootMotionPosition;
            [Tooltip("If using root motion, applies a multiplier to the delta position.")]
            public float rootMotionSpeedMultiplier = 1;
            [Tooltip("Should root motion be used to rotate the character.")]
            public bool useRootMotionRotation;
            [Tooltip("If using root motion, applies a multiplier to the delta rotation.")]
            public float rootMotionRotationMultiplier = 1;
            // ----- 
            [Header("Ground movementType Settings")]
            [Tooltip("Rate at which the character accelerates while grounded."), Range(0.01f, 1)]
            public float groundAcceleration = 0.18f;
            [Tooltip("Max speed the character moves while grounded.")]
            public float groundSpeed = 4f;
            [Tooltip("Rate at which the character decelerates while on the ground."), Range(0.01f, 1)]
            public float groundDamping = 0.3f;
            [Tooltip("Multiplier to ground speed when moving backwards."), Range(0, 2)]
            public float backwardsMultiplier = 0.7f;
            [Tooltip("Max speed when grounded.")]
            public float groundMaxSpeed = 8f;
            // ----- 
            [Header("Airborne Settings")]
            [Tooltip("Rate at which the character accelerates while airborne."), Range(0.01f, 1)]
            public float airborneAcceleration = 0.12f;
            [Tooltip("Max speed the character moves while airborne.")]
            public float airborneSpeed = 1f;
            [Tooltip("Rate at which the character decelerates while on the airborne."), Range(0.01f, 1)]
            public float airborneDamping = 0.3f;
            [Tooltip("Max forward speed when airborne.")]
            public float airborneMaxSpeed = 2f;
            // ----- 
            [Header("Rotation Settings")]
            [Tooltip("Rotation speed when in motion.")]
            public float rotationSpeed = 300f;
            [Tooltip("Rotation speed when in motion.")]
            public float airRotationSpeedScale = 0.25f;
            [Tooltip("Rotation speed when standing idle.")]
            public float idleRotationSpeedScale = 1.5f;
            [Tooltip("Rotation speed when standing idle.")]
            public float idleRotationSpeed = 180f;

        }

        [Serializable]
        public class PhysicsSettings
        {
            [Tooltip("The mass of the character.  Is used to calculate how much force to add when colliding with another rigidbody.")]
            public float mass = 100;
            [Tooltip("NotYetImplemented")]
            public float skinWidth = 0.08f;
            [Tooltip("NotYetImplemented")]
            public float maxSlopeAngle = 45f;
            [Tooltip("NotYetImplemented")]
            public float maxStepHeight = 0.4f;
            [Tooltip("NotYetImplemented"), Range(1, 4)]
            public float gravityModifier = 1.5f;
            [Tooltip("Maximum downward velocity the character can reach when falling.")]
            public float terminalVelocity = 10;
            [Tooltip("NotYetImplemented")]
            public float groundStickiness = 2f;
            [Serializable]
            public class ColliderSettings
            {
                [Tooltip("Height of the character."), Min(0)]
                public float colliderHeight = 2f;
                [Tooltip("Radius of the character collider."), Min(0)]
                public float colliderRadius = 0.4f;
                [Tooltip("NotYetImplemented")]
                public PhysicMaterial physicMaterial;
                [Tooltip("Colliders to ignore when determing collision detection.")]
                public Collider[] ignoreColliders = new Collider[0];
            }
            public ColliderSettings colliderSettings = new ColliderSettings();
        }

        [Serializable]
        public class CollisionSettings
        {
            [Tooltip("NotYetImplemented")]
            public LayerMask collisionsMask;
            [Tooltip("Should character check for collisions in horizontal space.")]
            public bool detectHorizontalCollisions = true;
            [Tooltip("Should character check for collisions in vertical space.")]
            public bool detectVerticalCollisions = false;
            [Tooltip("NotYetImplemented")]
            public int maxCollisionsCount = 64;
            [Tooltip("The maximum number of iterations to detect collision overlaps.")]
            public int maxOverlapIterations = 20;
        }

        [Serializable]
        public class AdvanceSettings
        {
            [Tooltip("<Not Implemented>")]
            public QueryTriggerInteraction queryTrigger = QueryTriggerInteraction.Ignore;
            [Tooltip("<Not Implemented>"), Range(0, 4)]
            public float timeScale = 1;
        }

        [Serializable]
        public class DebugSettings
        {
            public bool showMotionVectors;
            public bool showGroundCheck;
            public bool showCollisions;
        }


        [SerializeField]
        protected MovementSettings m_motor = new MovementSettings();
        [SerializeField]
        protected PhysicsSettings m_physics = new PhysicsSettings();
        [SerializeField]
        protected CollisionSettings m_collision = new CollisionSettings();
        [SerializeField]
        protected AdvanceSettings m_advance = new AdvanceSettings();
        [SerializeField]
        protected DebugSettings m_debug = new DebugSettings();

        [SerializeField]
        protected CharacterControllerDebugger debugger = new CharacterControllerDebugger();








        #endregion


        public class GroundInfo
        {
            public bool grounded;
            public Vector3 point;
            public Vector3 normal;
            public float angle;

            public bool onLedge;

            public GroundInfo()
            {
                point = Vector3.zero;
                normal = Vector3.zero;
                angle = 0;
            }

        }


        #region Inspector properties






        #endregion

        protected float m_moveSpeed;
        protected Vector3 m_velocity;
        protected PhysicMaterial m_physicsMaterial;
        protected Collider[] m_probedColliders;



        protected Vector3 m_currentPosition { get { return m_transform.position; } }



        protected float m_targetAngle, m_viewAngle;
        protected Vector3 m_movePosition;

        //  This is the target rotation.
        protected Quaternion m_targetRotation;
        // This is set by an external script.
        protected Quaternion m_lookRotation;
        //  Rotation to rotate by.
        protected Quaternion m_moveRotation;

        protected float m_stickyForce;

        //  root motion variables.


        protected float m_airborneThreshold = 0.3f;
        protected float m_spherecastRadius = 0.1f;


        //protected Quaternion m_rootMotionRotation;
        //protected Vector3 m_deltaPosition;
        protected float m_groundAngle;
        protected RaycastHit m_groundHit;
        //protected float m_previousAngle;
        //protected Quaternion m_deltaRotation;
        //protected Quaternion m_previousRotation;


        protected float m_inputMagnitude;
        public float m_forwardSpeed;
        protected float m_lateralSpeed;
        protected float m_yaw;
        protected float m_currentSpeed;




        //  Used to get the vertical velocity.
        protected float m_currentVerticalVelocity;


        protected Animator m_animator;
        protected AnimatorMonitor m_animatorMonitor;
        protected LayerManager m_layerManager;
        protected Rigidbody m_rigidbody;
        protected GameObject m_gameObject;
        protected Transform m_transform;
        protected float m_deltaTime;



        protected float acceleration { get { return isGrounded ? m_motor.groundAcceleration : m_motor.airborneAcceleration; } }



        public MovementTypes movementType { get; protected set; } = MovementTypes.Adventure;
        public bool isMoving;
        public bool isAiming;
        public bool isGrounded;
        public Vector3 moveDirection;
        public Vector3 inputVector;
        public Vector3 inputDirection { get; protected set; }
        public Vector3 lookDirection { get; protected set; }
        public bool useRootMotionPosition { get { return m_motor.useRootMotionPosition; } set { m_motor.useRootMotionPosition = value; } }
        public bool useRootMotionRotation { get { return m_motor.useRootMotionRotation; } set { m_motor.useRootMotionRotation = value; } }
        public CapsuleCollider characterCollider { get; protected set; }
        public float gravity { get => k_gravity * m_physics.gravityModifier; }
        public float fallTime { get; protected set; }
        public GroundInfo groundInfo = new GroundInfo();

        public float moveSpeed { get => Vector3.Dot(moveDirection, moveDirection.normalized); }
        public float maxSpeed { get => isGrounded ? m_motor.groundMaxSpeed : m_motor.airborneMaxSpeed; }
        public float rotationSpeed { get => m_motor.rotationSpeed * Mathf.Deg2Rad; }
        public float airRotationSpeed { get => (m_motor.rotationSpeed * Mathf.Deg2Rad) * m_motor.airRotationSpeedScale; }


        public Vector3 transformFwd { get => m_transform.forward; }
        public Vector3 transformUp { get => m_transform.up; }
        public Vector3 transformDown { get => -m_transform.up; }
        public Animator animator { get => m_animator; }
        public AnimatorMonitor animatorMonitor { get => animatorMonitor; }

        #region Properties







        //public Quaternion LookRotation { get { return m_lookRotation; } set { m_lookRotation = value; } }

        public float Speed { get { return Mathf.Abs(m_currentSpeed); } set { m_currentSpeed = Mathf.Abs(value); } }








        public RaycastHit GroundHit { get { return m_groundHit; } }


        protected Vector3 m_rootMotionVelocity;
        public Vector3 RootMotionVelocity { get { return m_rootMotionVelocity; } set { m_rootMotionVelocity = value; } }







        //public float LookAngle
        //{
        //    get {
        //        var dir = m_transform.InverseTransformDirection(LookRotation * m_transform.forward);
        //        var axisSign = Vector3.Cross(dir, m_transform.forward);
        //        return Vector3.Angle(m_transform.forward, dir) * (axisSign.y >= 0 ? -1f : 1f);
        //    }
        //}




        protected Vector3 m_colliderCenter;
        protected float m_colliderHeight, m_colliderRadius;

        #endregion



        protected void InitializeVariables()
        {
            m_animator = GetComponent<Animator>();
            m_animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
            m_animator.applyRootMotion = true;

            //  Rigidbody settings
            m_rigidbody = GetComponent<Rigidbody>();
            m_rigidbody.mass = m_physics.mass;
            m_rigidbody.drag = 0;
            m_rigidbody.useGravity = false;
            m_rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            m_rigidbody.isKinematic = true;
            //  Not kinematic
            if (!m_rigidbody.isKinematic) {
                m_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }
            else {
                //m_rigidbody.interpolation = RigidbodyInterpolation.None;
                m_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }


            //  Collider settings
            characterCollider = GetComponent<CapsuleCollider>();
            if (characterCollider == null) {
                characterCollider = gameObject.AddComponent<CapsuleCollider>();
                characterCollider.radius = 0.36f;
                characterCollider.height = MathUtil.Round(gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center.y * 2);
                characterCollider.center = new Vector3(0, characterCollider.height / 2, 0);
            }
            //  Colider properties.
            m_colliderCenter = characterCollider.center;
            m_colliderHeight = characterCollider.height * m_transform.lossyScale.x;
            m_colliderRadius = characterCollider.radius * m_transform.lossyScale.x;

            //  Collision settings
            m_layerManager = GetComponent<LayerManager>();
            m_collision.collisionsMask = m_layerManager.SolidLayers;
            m_probedColliders = new Collider[m_collision.maxCollisionsCount];

            //  Cached variables.
            lookDirection = m_transform.forward;
            m_lookRotation = Quaternion.LookRotation(m_transform.forward);
            m_moveRotation = Quaternion.identity;

            m_velocity = Vector3.zero;


            //m_previousRotation = m_transform.rotation;

            m_deltaTime = Time.deltaTime;

            if (!m_rigidbody.isKinematic) Debug.LogFormat("<b><color=yellow>[{0}]</color> is NOT kinematic. </b>", this.gameObject.name);

            m_rigidbody.isKinematic = false;
        }



        protected virtual void AnimatorMove()
        {
            m_animator.applyRootMotion = true;




            //if (m_motor.useRootMotionPosition) {
            //    //m_previousRotation *= m_animator.deltaRotation;


            //}

            if (!m_rigidbody.isKinematic) {
                m_rigidbody.MoveRotation(m_moveRotation);
                m_rigidbody.velocity = Vector3.Lerp(m_rigidbody.velocity, m_velocity, m_deltaTime * 20);
            }

        }



        protected virtual void InternalMove()
        {
            if (inputVector.sqrMagnitude > 1)
                inputVector.Normalize();


            m_velocity = m_animator.velocity;
            //m_deltaRotation = m_transform.rotation * Quaternion.Inverse(m_previousRotation);
            //m_previousRotation = m_transform.rotation;
            ////m_deltaRotation.ToAngleAxis(out float deltaAngle1, out Vector3 axis);



            lookDirection = m_lookRotation * m_transform.forward;
            //m_previousAngle = m_viewAngle;
            m_viewAngle = m_transform.AngleFromForward(lookDirection);

            if (!isGrounded) fallTime += m_deltaTime;
            else fallTime = 0;


            //DebugDraw.Arrow(m_currentPosition.WithY(0.2f), lookDirection, Color.green);
        }


        Vector3 targetDirection;
        protected void VELOCITY_MOVE()
        {
            m_deltaTime = Time.fixedDeltaTime;

            //InternalMove();
            
            if (m_rigidbody.isKinematic) {
                //Debug.LogFormat("<b><color=yellow>[{0}]</color> Changing {1} isKinematic to false </b>", this.name,  gameObject.name);
                m_rigidbody.isKinematic = false;
                m_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                m_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                m_animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
            }

            //Vector3 previousVelocity = (m_animator.deltaPosition * m_motor.rootMotionSpeedMultiplier) / m_deltaTime;
            Vector3 previousVelocity = m_animator.deltaPosition / m_deltaTime;
            m_velocity = previousVelocity;

            m_animator.deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
            angle = (angle * m_motor.rootMotionRotationMultiplier * Mathf.Deg2Rad) / m_deltaTime;
            m_moveRotation = Quaternion.AngleAxis(angle, axis);
            isGrounded = true;


            m_inputMagnitude = inputVector.magnitude;

            float targetAngle = 0;
            lookDirection = m_lookRotation * m_transform.forward;

            if (!isAiming) {
                //inputDirection = Quaternion.Inverse(m_transform.rotation) * m_transform.TransformDirection(inputVector);
                inputDirection = Quaternion.Inverse(m_transform.rotation) * m_transform.InverseTransformDirection(inputVector);
                //inputDirection = m_transform.forward * m_inputMagnitude;
                m_forwardSpeed = inputVector.z;
                //targetAngle = Mathf.Atan2(inputDirection.x, 0); // * (m_deltaTime * rotationSpeed);
                targetAngle = m_transform.AngleFromForward(inputDirection);
                var targetRotation = Mathf.Sign(inputDirection.x) * Mathf.Clamp(inputVector.x * targetAngle, -1, 1);
                m_lateralSpeed = Mathf.Lerp(m_lateralSpeed, targetRotation, m_deltaTime * rotationSpeed);

                var rot = Quaternion.AngleAxis(targetAngle, Vector3.up);
                targetDirection = rot * m_transform.forward;


            }
            //else if (isAiming) {
            //    inputDirection = m_transform.TransformDirection(inputVector);
            //}
            //else {
            //    inputDirection = m_transform.forward * (inputVector.z * inputVector.z) +
            //                     m_transform.right * (inputVector.x * inputVector.x) ;

            //    m_forwardSpeed = inputVector.z;v
            //    m_lateralSpeed = inputVector.x;

            //    targetDirection = m_lookRotation * m_transform.forward;
            //}


            DebugDraw.Arrow(m_currentPosition.WithY(0.1f), inputDirection, Color.blue);





            // -- Rotation --
            if(Mathf.Abs(inputVector.x) > 0.1f) {
                targetDirection.Normalize();
                targetDirection.y = 0;
                //targetDirection.Set(targetDirection.x, 0, targetDirection.z);
                if (targetDirection == Vector3.zero) {
                    targetDirection = m_transform.forward;
                }
                DebugDraw.Arrow(m_currentPosition.WithY(0.15f), targetDirection, Color.magenta);

                Quaternion rot = Quaternion.LookRotation(targetDirection);
                //var newPos = new Vector3(0, rot.eulerAngles.y, 0);
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle * rotationSpeed * m_deltaTime, 0);
                m_moveRotation = Quaternion.Lerp(m_transform.rotation, targetRotation * m_transform.rotation, rotationSpeed * m_deltaTime);

                //if (!isAiming) {
                //    //targetAngle = targetAngle * Mathf.Rad2Deg;
                //    m_moveRotation = Quaternion.AngleAxis(targetAngle * rotationSpeed, m_transform.up);
                //}
                //else {
                //    targetDirection.Normalize();
                //    targetDirection.Set(targetDirection.x, 0, targetDirection.z);
                //    if (targetDirection == Vector3.zero) {
                //        targetDirection = m_transform.forward;
                //    }

                //    targetAngle = m_transform.AngleFromForward(targetDirection);
                //    float actualRotationSpeed = targetAngle * (rotationSpeed * Mathf.Deg2Rad);
                //    float moveOverride = m_inputMagnitude;
                //    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                //    //m_moveRotation *= Quaternion.Slerp(m_transform.rotation, targetRotation, m_deltaTime * moveOverride * actualRotationSpeed);
                //    m_moveRotation *= targetRotation;
                //}
            }





            // -- Movement --
            //float drag = 100;
            //Vector3 verticalVelocity = Vector3.zero;


            if (isGrounded)
            {
                //if(m_inputMagnitude > 0.2f) {
                //    m_rigidbody.isKinematic = false;
                //    m_rigidbody.drag = 0;
                //}
                //else {
                //    float abs = .75f;

                //    if(abs > 0.2f) {
                //        m_rigidbody.isKinematic = false;
                //        m_velocity.y = 0;
                //        m_rigidbody.drag = 0;
                //    }

                //}
                m_velocity.y = m_rigidbody.velocity.y;
            }
            else {
                //verticalVelocity = Vector3.Project(previousVelocity, Vector3.up * gravity);
                //verticalVelocity *= gravity * m_deltaTime;

                m_velocity += Vector3.up * (gravity * m_deltaTime);
            }

            //m_velocity += verticalVelocity;
            //m_velocity *= Mathf.Clamp01(1f - drag * m_deltaTime);


            //m_rigidbody.velocity = m_velocity;
            isMoving = m_inputMagnitude > 0.2f;

            //  --  Update animator --
            UpdateAnimator();
        }



        /// <summary>
        /// Apply rotation.
        /// </summary>
        protected virtual void ApplyRotation()
        {
            m_rigidbody.rotation = m_moveRotation;
            //m_rigidbody.MoveRotation(m_moveRotation);
        }


        /// <summary>
        /// Apply position values.
        /// </summary>
        protected virtual void ApplyMovement()
        {
            if (!m_rigidbody.isKinematic) {
                m_velocity = moveDirection;
                m_rigidbody.velocity = m_velocity;
                return;
            }

            m_velocity = moveDirection;
            m_movePosition = m_rigidbody.position + moveDirection;
            m_rigidbody.MovePosition(m_movePosition);

            if (GetPenetrationInfo(m_rigidbody.position, out float penetrationDistance, out Vector3 penetrationDirection)) {
                m_rigidbody.MovePosition(m_rigidbody.position + penetrationDirection * penetrationDistance);
            }
        }



        public virtual void Move(float horizontal, float forward, Quaternion rotation)
        {
            inputVector.Set(horizontal, 0, forward);
            m_lookRotation = rotation;
        }



        protected virtual void Move()
        {
            m_inputMagnitude = inputVector.magnitude;
            //  Set the input vector, move direction and rotation angle based on the movement type.
            switch (movementType) {
                case (MovementTypes.Adventure):
                    //  Get the correct input direction.
                    inputDirection = m_transform.forward * m_inputMagnitude;
                    //inputDirection = m_transform.TransformDirection(inputVector);
                    //  Set forward and lateral speeds.
                    m_forwardSpeed = inputDirection.z;
                    m_lateralSpeed = Mathf.Atan2(inputVector.x, 0);

                    //  Get the correct target rotation angle.
                    m_targetAngle = m_transform.AngleFromForward(lookDirection);
                    //Vector3 local = m_transform.rotation * inputDirection;
                    //m_targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
                    break;
                case (MovementTypes.Combat):
                    ////  Get the correct input direction.
                    //inputDirection = m_transform.TransformDirection(inputVector);

                    ////  Set forward and lateral speeds.
                    //m_forwardSpeed = inputVector.z;
                    //m_lateralSpeed = inputVector.x;
                    ////  Get the correct target rotation angle.
                    //m_targetAngle = m_transform.AngleFromForward(lookDirection);

                    break;
            }


            isMoving = m_inputMagnitude > 0;
            //moveDirection = inputDirection * m_motor.groundSpeed * m_inputMagnitude;
            moveDirection = GetMovementVector() + m_velocity;



            m_currentSpeed = Vector3.Dot(m_velocity, m_transform.forward);
            //  Is there enough movement to be considered moving.


            //DebugDraw.Arrow(m_currentPosition.WithY(0.1f), inputDirection, Color.magenta);

        }


        /// <summary>
        /// Perform checks to determine if the character is on the ground.
        /// </summary>
        protected virtual void CheckGround()
        {
            m_stickyForce = 0;

            float distance = m_spherecastRadius + m_physics.skinWidth;
            RaycastHit groundHit;
            bool didCollide = false;

            if (SphereGroundCast(m_currentPosition, Vector3.down, m_spherecastRadius, distance, out groundHit, true, null, DebugGroundCheck, true)) {
                groundHit.distance = Mathf.Max(0.0f, groundHit.distance - k_collisionOffset);
                didCollide = true;
            }

            if (!didCollide) {
                if (SphereGroundCast(m_currentPosition, Vector3.down, m_colliderRadius, distance + k_groundedDistance, out groundHit, true, null, DebugGroundCheck, true)) {
                    groundHit.distance = Mathf.Max(0.0f, groundHit.distance - k_collisionOffset);
                    didCollide = true;
                }
            }

            if (didCollide) {
                float groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
                isGrounded = true;
                ////  Check if cast is hitting against a wall.
                //if (groundAngle > k_wallAngle) {
                //    if (SphereGroundCast(m_currentPosition, Vector3.down, m_spherecastRadius, m_colliderHeight * 0.5f + k_collisionOffset, out groundHit, true, m_colliderCenter, DebugGroundCheck, false)) {
                //        groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
                //    }
                //    else {
                //        isGrounded = false;
                //    }
                //}
                ////  Check if on a ledge.



                float fwdVelocity = Vector3.ProjectOnPlane(moveDirection, Vector3.up * gravity).magnitude;
                m_stickyForce = groundHit.distance * m_physics.groundStickiness * fwdVelocity;
                //moveDirection = moveDirection - transformUp * stickyForce * m_deltaTime;
                UpdateGroundInfo(isGrounded, groundAngle, groundHit.point, groundHit.normal);
            }
            else {
                isGrounded = false;


                m_currentVerticalVelocity = 0;

                UpdateGroundInfo(false);
            }

        }



        /// <summary>
        /// Update the character’s position values.
        /// </summary>
        protected virtual void UpdateMovement()
        {

            if (isGrounded)
            {
                Vector3 drag = moveDirection * MathUtil.SmoothStop3(m_motor.groundDamping);
                if (inputDirection.sqrMagnitude > 0)
                    drag *= (1 - Vector3.Dot(inputDirection.normalized, moveDirection.normalized));
                if (drag.sqrMagnitude > moveDirection.sqrMagnitude)
                    moveDirection = Vector3.zero;
                else
                    moveDirection -= drag;

                moveDirection += inputDirection * (m_motor.groundAcceleration * m_deltaTime);
                if (moveDirection.sqrMagnitude > m_motor.groundSpeed * m_motor.groundSpeed)
                    moveDirection = moveDirection.normalized * m_motor.groundSpeed;

                ////  Calculate move vector
                //if (isMoving) {
                //    moveDirection += inputDirection * (m_motor.groundAcceleration * m_deltaTime);
                //    if (moveDirection.sqrMagnitude > m_motor.groundSpeed * m_motor.groundSpeed)
                //        moveDirection = moveDirection.normalized * m_motor.groundSpeed;
                //}
                //else {
                //    Vector3 drag = moveDirection * MathUtil.SmoothStop3(m_motor.groundDamping);
                //    if (inputDirection.sqrMagnitude > 0)
                //        drag *= (1 - Vector3.Dot(inputDirection.normalized, moveDirection.normalized));
                //    if (drag.sqrMagnitude > moveDirection.sqrMagnitude)
                //        moveDirection = Vector3.zero;
                //    else
                //        moveDirection -= drag;
                //}

                ////  If greater than 0, than we are going down a slope.
                //if (Vector3.Dot(moveDirection, groundInfo.normal) >= 0) {

                //}
                ////  We are going up the slope if it is negative.
                //else {

                //}

                //moveDirection = GetDirectionTangentToSurface(moveDirection, groundInfo.normal);
                //// Reorient target velocity.
                //Vector3 inputRight = Vector3.Cross(moveDirection, m_transform.up);
                //Vector3 targetMovementVelocity = Vector3.Cross(groundInfo.normal, inputRight).normalized * m_currentSpeed;// moveDirection.magnitude;
                //moveDirection = Vector3.Lerp(moveDirection, targetMovementVelocity * m_deltaTime, 1f - Mathf.Exp(-5 * m_deltaTime));

                //moveDirection -= m_transform.up * m_stickyForce * m_deltaTime;
            }
            else {
                //Vector3 verticalVelocity = Vector3.Project(m_velocity, m_gravity);
                m_currentVerticalVelocity += Mathf.Min(gravity * fallTime, m_physics.terminalVelocity);
                moveDirection.y = m_currentVerticalVelocity;
            }




        }



        /// <summary>
        /// Update the character’s rotation values.
        /// </summary>
        protected virtual void UpdateRotation()
        {
            if (isMoving)
            {
                //  Get a target roation.
                float turnScale = 3;
                float turnSpeed = isGrounded ? rotationSpeed : airRotationSpeed * turnScale;
                Quaternion targetRotation = Quaternion.Euler(0, m_targetAngle * turnSpeed * m_deltaTime, 0);
                m_moveRotation = targetRotation * m_transform.rotation;

            }
            else {
                m_moveRotation = m_transform.rotation;
            }

            DebugUI.Log(this, "m_moveRotation", m_moveRotation, RichTextColor.Orange);
        }



        /// <summary>
        /// Ensure the current movement direction is valid.
        /// </summary>
        protected virtual void CheckMovement()
        {
            var velocity = moveDirection * m_deltaTime;
            if (m_collision.detectHorizontalCollisions && velocity != Vector3.zero) {
                if (DetectHorizontalCollisions(ref velocity, groundInfo.angle, true)) {
                    if (GetPenetrationInfo(m_rigidbody.position, out float penetrationDistance, out Vector3 penetrationDirection)) {
                        velocity += penetrationDirection * penetrationDistance;
                    }
                }

            }

            //if (m_collision.detectVerticalCollisions) {
            //    collisionDetected = DetectHorizontalCollisions(ref moveDirection, groundInfo.angle, out hit, true);
            //}



            moveDirection = velocity;
        }


        /// <summary>
        /// Updates the animator.
        /// </summary>
        protected virtual void UpdateAnimator()
        {
            m_animatorMonitor.SetIsMoving(isMoving);


            //SetYaw(m_transform.rotation, m_moveRotation);
            //m_animatorMonitor.SetYawValue(m_yaw);


            m_currentSpeed = 2;
            m_animator.SetFloat(HashID.Speed, m_currentSpeed);


            m_animatorMonitor.SetForwardInputValue(m_forwardSpeed * m_currentSpeed);
            m_animatorMonitor.SetHorizontalInputValue(m_lateralSpeed);



        }



        protected Vector3 GetMovementVector()
        {
            if (useRootMotionPosition) {
                return (m_animator.deltaPosition * m_motor.rootMotionSpeedMultiplier) / m_deltaTime;
            }

            m_moveSpeed = Mathf.Lerp(m_moveSpeed, m_motor.groundSpeed, m_motor.groundAcceleration.Squared());
            return inputDirection * m_moveSpeed * m_inputMagnitude;
        }





        private bool GetPenetrationInfo(Vector3 currentPosition, out float getDistance, out Vector3 getDirection, bool useSkinWidth = true, Vector3? offsetPosition = null)
        {
            getDistance = 0;
            getDirection = Vector3.zero;



            float skinWidth = useSkinWidth ? m_physics.skinWidth : 0;
            var origin = currentPosition.WithY(m_colliderCenter.y);
            var offset = offsetPosition != null ? offsetPosition.Value : Vector3.zero;
            var p1 = GetBottomCapsulePoint(origin, offset);
            var p2 = GetTopCapsulePoint(origin, offset);
            var radius = m_colliderRadius + skinWidth;
            int overlapCount = Physics.OverlapCapsuleNonAlloc(p1, p2, radius, m_probedColliders, GetCollisionMask(), m_advance.queryTrigger);

            Vector3 localPosition = Vector3.zero;
            bool overlap = false;
            if (overlapCount > 0)  //   || m_probedColliders.Length >= 0
            {
                for (int i = 0; i < overlapCount; i++) {
                    Collider collision = m_probedColliders[i];
                    if (collision == characterCollider) { continue; }
                    if (collision == null) { break; }

                    Vector3 direction;
                    float distance;
                    Transform colliderTransform = collision.transform;

                    if (ComputePenetration(currentPosition, offset,
                                            collision, colliderTransform.position, colliderTransform.rotation,
                                            out direction, out distance)) {
                        localPosition += direction * (distance + k_collisionOffset);
                        overlap = true;
                    }
                }
            }

            //if (overlap) moveDirection += localPosition.normalized;
            if (overlap) {
                getDistance = localPosition.magnitude;
                getDirection = localPosition.normalized;
            }
            //m_rigidbody.MovePosition(m_rigidbody.position + localPosition.normalized);

            return overlap;
        }


        protected bool DetectHorizontalCollisions(ref Vector3 direction, float groundAngle, bool useSkinWidth = true)
        {
            bool climbingSlope = false;
            float skinWidth = useSkinWidth ? m_physics.skinWidth : 0;
            float distanceOffset = m_colliderRadius + skinWidth;
            float distance = Vector3.Dot(direction, direction.normalized);

            RaycastHit hit;
            bool hitDetected = CapsuleCast(m_currentPosition, direction, distance, out hit, useSkinWidth, DebugCollisions);

            if (hitDetected) {
                DebugDrawer.DrawPoint(hit.point, Color.green);
                //  Climb Slope.
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (slopeAngle <= m_physics.maxSlopeAngle) {
                    Vector3 vectorToHitPoint = Vector3.zero;
                    if (Math.Abs(slopeAngle - groundAngle) > float.Epsilon) {
                        //  Get the adjusted distance to the hit point
                        float distanceToSlopeStart = hit.distance - distanceOffset;
                        //  Set the direction vector to the hitPoint.
                        vectorToHitPoint = direction.normalized * distanceToSlopeStart;
                        //  Adjust the move direction to account for accending slope.
                        direction -= vectorToHitPoint;
                    }
                    //  Climb slope.
                    climbingSlope = ClimbSlope(ref direction, distance, slopeAngle);
                    //  Add the slope start distance that was previous subtracted.
                    direction += vectorToHitPoint;
                }

                if (slopeAngle > m_physics.maxSlopeAngle) {
                    distance = (hit.distance - distanceOffset) * moveDirection.magnitude;
                    if (climbingSlope) {
                        direction.y = Mathf.Tan(groundAngle * Mathf.Deg2Rad) * distance;
                    }
                }
            }

            return hitDetected;
        }


        protected bool DetectVerticalCollisions(ref Vector3 direction, out RaycastHit hit, bool useSkinWidth = true)
        {
            float distance = Vector3.Dot(direction, direction.normalized);
            bool hitDetected = CapsuleCast(m_currentPosition, direction, moveSpeed, out hit, useSkinWidth);

            return hitDetected;
        }


        protected bool ClimbSlope(ref Vector3 direction, float moveAmount, float slopeAngle)
        {
            //  Climb slope.
            float yDirection = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * m_colliderRadius;
            if (direction.y <= yDirection) {
                moveAmount = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * m_colliderRadius;
                direction = direction.normalized * moveAmount;
                direction.y = yDirection;

                return true;
            }
            return false;
        }


        protected void UpdateGroundInfo(bool grounded, float groundAngle = 0, Vector3? hitPoint = null, Vector3? groundNormal = null)
        {
            groundInfo.grounded = grounded;
            groundInfo.angle = grounded ? groundAngle : 0;
            groundInfo.point = grounded ? hitPoint.Value : m_currentPosition;
            groundInfo.normal = grounded ? groundNormal.Value : m_transform.up;
        }


        protected void SetYaw(Quaternion currentRotation, Quaternion targetRotation)
        {
            //  Set turning speeed;
            float currentY = currentRotation.eulerAngles.y;
            float targetY = targetRotation.eulerAngles.y;
            float difference = isMoving ? (targetY - currentY).Wrap() / m_deltaTime : 0;

            m_yaw = Mathf.Lerp(m_yaw, Mathf.Clamp(difference / rotationSpeed, -1, 1), m_deltaTime * rotationSpeed);
        }







        #region Public Functions

        public LayerMask GetCollisionMask()
        {
            return m_collision.collisionsMask;
        }


        Color green2 = new Color(0, 1, 0, 0.5f);
        Color red2 = new Color(1, 0, 0, 0.5f);
        Color grey2 = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public bool SphereGroundCast(Vector3 currentPosition, Vector3 direction,
                                    float radius, float distance, out RaycastHit hit,
                                    bool useSkinWidth = false, Vector3? positionOffset = null, bool debug = false, bool useSphere = false)
        {
            float skinWidth = useSkinWidth ? m_physics.skinWidth : 0;
            Vector3 startPosition = currentPosition.WithY(radius + skinWidth);
            Vector3 spherePosition = positionOffset != null ? startPosition + positionOffset.Value : startPosition;
            float maxDistance = distance + skinWidth;

            bool result = Physics.SphereCast(spherePosition, radius, direction, out hit, maxDistance, GetCollisionMask(), m_advance.queryTrigger);

            if (debug) {
                var hitPoint = result ? hit.point : currentPosition;
                DebugDrawer.DrawSphere(spherePosition, radius, grey2);
                DebugDrawer.DrawLine(spherePosition, hitPoint, result ? Color.green : Color.red);
                if (useSphere)
                    DebugDrawer.DrawSphere(hitPoint, radius, result ? green2 : red2);
                else
                    DebugDrawer.DrawPoint(hitPoint, result ? Color.green : Color.red, radius);
                //DebugDrawer.DrawSphere(hit.point - direction * radius, radius, result ? Color.green : Color.red);
            }

            return result;
        }



        public bool CapsuleCast(Vector3 currentPosition, Vector3 direction, float distance, out RaycastHit capsuleHit, bool useSkinWidth = false, bool debug = false)
        {
            var origin = currentPosition.WithY(m_colliderCenter.y);
            var p1 = GetBottomCapsulePoint(origin);
            var p2 = GetTopCapsulePoint(origin);
            float skinWidth = useSkinWidth ? m_physics.skinWidth : 0;
            float castRadius = m_colliderRadius - skinWidth;
            float maxDistance = distance + skinWidth;

            bool result = Physics.CapsuleCast(p1, p2, castRadius, direction, out capsuleHit, maxDistance, GetCollisionMask(), m_advance.queryTrigger);

            if (debug) {
                var hitPos = direction * (capsuleHit.distance - (m_colliderRadius + skinWidth));
                //DebugDrawer.DrawCapsule(p1 + hitPos, p2 + hitPos, castRadius, result ? Color.green : Color.white);
                Debug.DrawRay(p1, direction * (capsuleHit.distance - (m_colliderRadius + skinWidth)), result ? Color.green : Color.grey);
                Debug.DrawRay(p2, direction * (capsuleHit.distance - (m_colliderRadius + skinWidth)), result ? Color.green : Color.grey);
            }

            return result;
        }




        public bool CheckCapsule(Vector3 currentPosition, Vector3? offsetPosition = null)
        {
            Vector3 offset = offsetPosition != null ? offsetPosition.Value : Vector3.zero;
            return Physics.CheckCapsule(GetCapsulePoint(currentPosition, m_transform.up) + offset,
                                        GetCapsulePoint(currentPosition, -m_transform.up) + offset,
                                        m_colliderRadius + m_physics.skinWidth,
                                        GetCollisionMask(), m_advance.queryTrigger);

        }


        private bool ComputePenetration(Vector3 currentPosition, Vector3 positionOffset,
                                        Collider collision, Vector3 colliderPosition, Quaternion colliderRotation,
                                        out Vector3 direction, out float distance)
        {
            if (collision == characterCollider) {
                direction = Vector3.one;
                distance = 0.0f;
                return false;
            }

            bool result = Physics.ComputePenetration(characterCollider, currentPosition + positionOffset, Quaternion.identity,
                                                     collision, colliderPosition, colliderRotation,
                                                     out direction, out distance);

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










        // Scale the capsule collider to 'mlp' of the initial value
        protected void ScaleCapsule(float scale)
        {
            scale = Mathf.Abs(scale);
            if (characterCollider.height < m_colliderHeight * scale || characterCollider.height > m_colliderHeight * scale) {
                characterCollider.height = Mathf.MoveTowards(characterCollider.height, m_colliderHeight * scale, Time.deltaTime * 4);
                characterCollider.center = Vector3.MoveTowards(characterCollider.center, m_colliderCenter * scale, Time.deltaTime * 2);
            }
        }


        public virtual float GetColliderHeightAdjustment()
        {
            return characterCollider.height;
        }





        // Rotate a rigidbody around a point and axis by angle
        public void RigidbodyRotateAround(Vector3 point, Vector3 axis, float angle)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            Vector3 d = transform.position - point;
            m_rigidbody.MovePosition(point + rotation * d);
            m_rigidbody.MoveRotation(rotation * transform.rotation);
        }




        public Vector3 GetCapsulePoint(Vector3 origin, Vector3 direction)
        {
            var pointsDist = m_colliderHeight - (m_colliderRadius * 2f);
            return origin + (direction * (pointsDist * .5f));
        }

        public Vector3 GetBottomCapsulePoint(Vector3 origin, Vector3? offsetPosition = null)
        {
            var offset = offsetPosition != null ? offsetPosition.Value : Vector3.zero;
            var pointsDist = m_colliderHeight - (m_colliderRadius * 2f);
            return origin + (Vector3.down * (pointsDist * .5f)) + offset;
        }

        public Vector3 GetTopCapsulePoint(Vector3 origin, Vector3? offsetPosition = null)
        {
            var offset = offsetPosition != null ? offsetPosition.Value : Vector3.zero;
            var pointsDist = m_colliderHeight - (m_colliderRadius * 2f);
            return origin + (Vector3.up * (pointsDist * .5f)) + offset;
        }








        #endregion












    }
}