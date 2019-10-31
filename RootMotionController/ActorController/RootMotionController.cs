using System;
using System.Collections.Generic;
using UnityEngine;

namespace JH.RootMotionController
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public partial class RootMotionController : MonoBehaviour
    {
        private const float k_collisionOffset = 0.01f;
        private readonly float k_wallAngle = 85f;
        private readonly float k_gravity = -9.81f;

        [Tooltip("Movement settings.")]
        [SerializeField] private MovementSettings m_motor = new MovementSettings();
        [Tooltip("Animation settings.")]
        [SerializeField] private AnimationSettings m_animation = new AnimationSettings();
        [Tooltip("Physics settings.")]
        [SerializeField] private PhysicsSettings m_physics = new PhysicsSettings();
        [Tooltip("Collision settings.")]
        [SerializeField] private CollisionSettings m_collision = new CollisionSettings();
        [Tooltip("Advanced settings.")]
        [SerializeField] private AdvanceSettings m_advance = new AdvanceSettings();
        [Tooltip("Debug settings.")]
        [SerializeField] public DebugSettings debugMode = new DebugSettings();

        private CollisionSettings.ColliderSettings m_collider { get { return m_collision.colliderSettings; } }

        private Vector3 _gravity;
        public Vector3 gravity{
            get {
                _gravity.Set(0, k_gravity * m_physics.gravityModifier, 0);
                return _gravity;
            }
        }


        /// <summary>
        /// Returns movement settings turning speed as radians.
        /// </summary>
        private float rotationSpeed { get => m_motor.turningSpeed * Mathf.Deg2Rad; }

        public float colliderRadius{
            get { return m_actorCollider.radius * m_transform.lossyScale.x; }
            set { m_actorCollider.radius = value * m_transform.lossyScale.x; }
        }

        public LayerMask collisionMask { get { return m_collision.collisionsMask; } }
        public float deltaTime { get; private set; }
        public Animator animator { get => m_animator; }
        public CapsuleCollider actorCollider { get => m_actorCollider; }




        private float m_spherecastRadius = 0.1f;
        private float m_deltaTime;
        private float m_fixedDeltaTime;
        private Animator m_animator;
        private Rigidbody m_rigidbody;
        private CapsuleCollider m_actorCollider;
        private GameObject m_gameObject;
        private Transform m_transform;
        /// <summary>Make sure update is called once.</summary>
        private bool frameUpdated;






        private void InitializeRootMotionController()
        {
            //  Animator settings.
            m_animator.applyRootMotion = true;
            m_animator.updateMode = m_advance.animatorUpdateMode;

            //  Rigidbody settings.
            m_rigidbody.mass = m_physics.mass;
            m_rigidbody.drag = 0;
            m_rigidbody.useGravity = m_physics.useGravity;
            m_rigidbody.collisionDetectionMode = m_advance.collisionDetectionMode;
            m_rigidbody.isKinematic = m_advance.isKinematic;
            m_rigidbody.interpolation = m_advance.rigidbodyInterpolation;
            m_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            //  Collider settings.
            m_actorCollider.center = m_collider.center;
            m_actorCollider.height = m_collider.height;
            m_actorCollider.radius = m_collider.radius;
            m_actorCollider.sharedMaterial = m_collider.physicMaterial;

            lastPosition = m_transform.position;
        }


        private void Awake()
        {
            m_deltaTime = Time.deltaTime;
            m_fixedDeltaTime = Time.fixedDeltaTime;
            m_animator = GetComponent<Animator>();
            m_rigidbody = this.GetOrAddComponent<Rigidbody>();
            m_actorCollider = this.GetOrAddComponent<CapsuleCollider>();
            m_gameObject = gameObject;
            m_transform = transform;

            InitializeRootMotionController();
        }




        private void FixedUpdate()
        {
            UpdateMovement(m_fixedDeltaTime);
            frameUpdated = true;
        }


        private void Update()
        {
            if (!frameUpdated) {
                UpdateMovement(m_deltaTime);
                frameUpdated = true;
            }
                
        }

        private void LateUpdate()
        {
            if (frameUpdated) {
                frameUpdated = false;
            }
        }

        private void UpdateMovement(float time)
        {
            if (!isGrounded) m_airTime += time;
            else m_airTime = 0;
        }










        private void OnDrawGizmos()
        {
            if (Application.isPlaying) {
                if (debugMode.showMotionVectors) {
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

                    if (inputVector != Vector3.zero) {
                        DebugDrawer.DrawArrow(position.WithY(drawHeight), m_transform.rotation * inputVector, debugMode.options.inputDirectionColor);
                    }

                    if (velocityVector != Vector3.zero) {
                        DebugDrawer.DrawArrow(position.WithY(drawHeight + (1 * drawStep)), velocityVector, debugMode.options.velocityColor);
                    }
                    if (m_rigidbody.velocity != Vector3.zero) {
                        DebugDrawer.DrawArrow(position.WithY(drawHeight + (1 * drawStep) + drawSubstep), m_rigidbody.velocity, debugMode.options.velocityColor.Darker());
                    }
                }
            }

        }



        GUIStyle guiStyle = new GUIStyle();
        Rect rect = new Rect();
        private void OnGUI()
        {
            if (animator == null) return;
            guiStyle.normal.textColor = Color.black;
            guiStyle.fontStyle = FontStyle.Bold;
            rect.width = Screen.width * 0.25f;
            rect.x = (Screen.width * 0.5f) - (rect.width * 0.5f) + 36;
            rect.y = (Screen.height * 0.5f) - (rect.height * 0.5f) + 18;
            rect.height = 16 + rect.y;
            GUI.Label(rect, "pivotWeight: " + animator.pivotWeight.ToString(), guiStyle);
            rect.y += rect.height = 16;
            rect.height += rect.height;
            GUI.Label(rect, "feetPivotActive: " + animator.feetPivotActive.ToString(), guiStyle);


        }


    }
}
