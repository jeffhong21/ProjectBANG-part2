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
        private Animator m_animator;
        private Rigidbody m_rigidbody;
        private CapsuleCollider m_actorCollider;
        private GameObject m_gameObject;
        private Transform m_transform;


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
        }


        private void Awake()
        {
            m_deltaTime = Time.deltaTime;
            m_animator = GetComponent<Animator>();
            m_rigidbody = this.GetOrAddComponent<Rigidbody>();
            m_actorCollider = this.GetOrAddComponent<CapsuleCollider>();
            m_gameObject = gameObject;
            m_transform = transform;

            InitializeRootMotionController();
        }




        private void Start()
        {
            
        }






        private void OnAnimatorMove()
        {
            AnimatorMove();

//#if UNITY_EDITOR
//            if(debugMode.lockInputValues)
//#endif
        }



        private void FixedUpdate()
        {
            UpdateLocomotion(m_deltaTime);
        }



        private void UpdateLocomotion(float deltaTime)
        {
            m_deltaTime = deltaTime;

            velocityVector = m_previousVelocity;
            m_previousVelocity = (m_animator.deltaPosition * m_motor.rootMotionScale) / m_deltaTime;
            m_previousVelocity = m_animator.velocity;


            if (inputVector.sqrMagnitude > 1)
                inputVector.Normalize();
            m_inputMagnitude = inputVector.magnitude;


            Move();
            CheckGround();
            CheckMovement();
            UpdateRotation();
            UpdateMovement();
            UpdateAnimator();
        }





        private void OnDestroy()
        {
            
        }

    }
}
