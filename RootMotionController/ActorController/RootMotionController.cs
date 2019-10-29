using System;
using System.Collections.Generic;
using UnityEngine;

namespace JH.RootMotionController
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public partial class RootMotionController : MonoBehaviour
    {


        private float m_deltaTime;
        private float m_fixedDeltaTime;
        private Animator m_animator;
        private Rigidbody m_rigidbody;
        private CapsuleCollider m_actorCollider;
        private GameObject m_gameObject;
        private Transform m_transform;

        public float deltaTime { get; private set; }
        public Animator animator { get => m_animator; }
        public CapsuleCollider actorCollider { get => m_actorCollider; }
        public Vector3 position { get => m_transform.position; }
        public Vector3 forward { get => m_transform.forward; }
        public Vector3 up { get => m_transform.up; }
        public Vector3 down { get => -m_transform.up; }



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
            m_rigidbody.constraints = m_rigidbody.isKinematic ?
                                      RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ :
                                      RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY| RigidbodyConstraints.FreezeRotationZ;

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





        //private void UpdateLocomotion()
        //{


        //    //velocityVector = m_previousVelocity;
        //    //m_previousVelocity = (m_animator.deltaPosition * m_motor.rootMotionScale) / m_deltaTime;
        //    //m_previousVelocity = m_animator.velocity;


        //    if (inputVector.sqrMagnitude > 1)
        //        inputVector.Normalize();
        //    m_inputMagnitude = inputVector.magnitude;


        //    //SetMovement();
        //    //CheckGround();
        //    //CheckMovement();
        //    //UpdateRotation();
        //    //UpdateMovement();
        //    //UpdateAnimator();
        //}









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

                    if (inputDirection != Vector3.zero) {
                        DebugDrawer.DrawArrow(position.WithY(drawHeight), m_transform.rotation * inputDirection, debugMode.options.inputDirectionColor);
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
