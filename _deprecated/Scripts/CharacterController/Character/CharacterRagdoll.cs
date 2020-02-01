namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Ragdoll Utility controls switching characters in and out of ragdoll mode. It also enables you to use IK effects on top of ragdoll simulation.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CharacterRagdoll : MonoBehaviour
    {

        //private void ActivateRagdoll(Rigidbody rb, Transform root)
        //{
        //    if (Input.GetKeyDown(KeyCode.R)) EnableRagdoll();
        //    if (Input.GetKeyDown(KeyCode.A)) {
        //        // Move the root of the character to where the pelvis is without moving the ragdoll
        //        Vector3 toPelvis = rb.position - root.position;
        //        root.position += toPelvis;
        //        rb.transform.position -= toPelvis;

        //        DisableRagdoll();
        //    }
        //}



        #region Main Interface



        [Tooltip("How long does it take to blend from ragdoll to animation?")]
        public float ragdollToAnimationTime = 0.2f;
        [Tooltip("If true, IK can be used on top of physical ragdoll simulation.")]
        public bool applyIkOnRagdoll;
        [Tooltip("How much velocity transfer from animation to ragdoll?")]
        public float applyVelocity = 1f;
        [Tooltip("How much angular velocity to transfer from animation to ragdoll?")]
        public float applyAngularVelocity = 1f;

        /// <summary>
        /// Switches to ragdoll.
        /// </summary>
        public void EnableRagdoll()
        {
            if (isRagdoll) return;

            StopAllCoroutines();
            enableRagdollFlag = true;
        }

        /// <summary>
        /// Blends back to animation.
        /// </summary>
        public void DisableRagdoll()
        {
            if (!isRagdoll) return;
            StoreLocalState();
            StopAllCoroutines();
            StartCoroutine(DisableRagdollSmooth());
        }

        #endregion Main Interface

        // The rigidbodies and their associates
        public class Rigidbone
        {
            public Rigidbody r;
            public Transform t;
            public Collider collider;
            public Joint joint;
            public Rigidbody c;
            public bool updateAnchor;
            public Vector3 deltaPosition;
            public Quaternion deltaRotation;
            public float deltaTime;
            public Vector3 lastPosition;
            public Quaternion lastRotation;

            // Constructor
            public Rigidbone( Rigidbody r )
            {
                this.r = r;
                t = r.transform;
                joint = t.GetComponent<Joint>();

                collider = t.GetComponent<Collider>();

                if (joint != null) {
                    c = joint.connectedBody;
                    updateAnchor = c != null;
                }

                lastPosition = t.position;
                lastRotation = t.rotation;
            }

            // Store position and rotation deltas
            public void RecordVelocity()
            {
                deltaPosition = t.position - lastPosition;
                lastPosition = t.position;

                deltaRotation = RootMotion.QuaTools.FromToRotation(lastRotation, t.rotation);
                lastRotation = t.rotation;

                deltaTime = Time.deltaTime;
            }

            // Go to ragdoll
            public void WakeUp( float velocityWeight, float angularVelocityWeight )
            {
                // Joint anchors need to be updated when there are animated bones in between ragdoll bones
                if (updateAnchor) {
                    joint.connectedAnchor = t.InverseTransformPoint(c.position);
                }

                r.isKinematic = false;

                // Transfer velocity from animation
                if (velocityWeight != 0f) {
                    r.velocity = (deltaPosition / deltaTime) * velocityWeight;
                }

                // Transfer angular velocity from animation
                if (angularVelocityWeight != 0f) {
                    float angle = 0f;
                    Vector3 axis = Vector3.zero;
                    deltaRotation.ToAngleAxis(out angle, out axis);
                    angle *= Mathf.Deg2Rad;
                    angle /= deltaTime;
                    axis *= angle * angularVelocityWeight;
                    r.angularVelocity = Vector3.ClampMagnitude(axis, r.maxAngularVelocity);
                }

                r.WakeUp();
            }
        }

        // All child Transforms of the root.
        public class Child
        {
            public Transform t;

            public Vector3 localPosition;
            public Quaternion localRotation;

            // Constructor
            public Child( Transform transform )
            {
                t = transform;
                localPosition = t.localPosition;
                localRotation = t.localRotation;
            }

            // Force to the last stored local state
            public void FixTransform( float weight )
            {
                if (weight <= 0f) return;

                if (weight >= 1f) {
                    t.localPosition = localPosition;
                    t.localRotation = localRotation;
                    return;
                }

                t.localPosition = Vector3.Lerp(t.localPosition, localPosition, weight);
                t.localRotation = Quaternion.Lerp(t.localRotation, localRotation, weight);
            }

            // Remember the local state, that is the local position and rotation of the transform
            public void StoreLocalState()
            {
                localPosition = t.localPosition;
                localRotation = t.localRotation;
            }
        }

        private Animator animator;
        private Rigidbone[] rigidbones = new Rigidbone[0];
        private Child[] children = new Child[0];
        private bool enableRagdollFlag;
        private AnimatorUpdateMode animatorUpdateMode;
        //private bool[] fixTransforms = new bool[0];
        private float ragdollWeight;
        private float ragdollWeightV;
        private bool fixedFrame;
        //private bool[] disabledIKComponents = new bool[0];

        // Find all necessary components and initiate
        private void Start()
        {
            animator = GetComponent<Animator>();

            // Gather all the rigidbodies and their associates
            Rigidbody[] rigidbodies = (Rigidbody[])GetComponentsInChildren<Rigidbody>();
            int firstIndex = rigidbodies[0].gameObject == gameObject ? 1 : 0;

            rigidbones = new Rigidbone[firstIndex == 0 ? rigidbodies.Length : rigidbodies.Length - 1];

            for (int i = 0; i < rigidbones.Length; i++) {
                rigidbones[i] = new Rigidbone(rigidbodies[i + firstIndex]);
            }

            // Find all the child Transforms
            Transform[] C = (Transform[])GetComponentsInChildren<Transform>();
            children = new Child[C.Length - 1];

            for (int i = 0; i < children.Length; i++) {
                children[i] = new Child(C[i + 1]);
            }
        }

        // Smoothly blends out of Ragdoll
        private IEnumerator DisableRagdollSmooth()
        {
            // ...make all rigidbodies kinematic
            for (int i = 0; i < rigidbones.Length; i++) {
                rigidbones[i].r.isKinematic = true;
            }


            // Animator has not updated yet.
            animator.updateMode = animatorUpdateMode;
            animator.enabled = true;

            // Blend back to animation
            while (ragdollWeight > 0f) {
                ragdollWeight = Mathf.SmoothDamp(ragdollWeight, 0f, ref ragdollWeightV, ragdollToAnimationTime);
                if (ragdollWeight < 0.001f) ragdollWeight = 0f;

                yield return null;
            }

            yield return null;
        }



        private void FixedUpdate()
        {
            // When in ragdoll, move the bones to where they were after the last physics simulation, so IK won't screw up the physics
            if (isRagdoll && applyIkOnRagdoll) FixTransforms(1f);

            fixedFrame = true;
        }

        private void LateUpdate()
        {
            // When Mecanim has animated...
            if (animator.updateMode != AnimatorUpdateMode.AnimatePhysics || (animator.updateMode == AnimatorUpdateMode.AnimatePhysics && fixedFrame)) {
                AfterAnimation();
            }

            fixedFrame = false;

            OnFinalPose();
        }



        // When animation has been applied by Mecanim
        private void AfterAnimation()
        {
            if (isRagdoll) {
                // If is ragdoll, no animation has been applied, but we need to remember the pose after the physics step just the same
                StoreLocalState();
            } else {
                // Blending from ragdoll to animation. When ragdollWeight is zero, nothing happens here
                FixTransforms(ragdollWeight);
            }
        }

        // When we have the final pose of the character for this frame
        private void OnFinalPose()
        {
            if (!isRagdoll) RecordVelocities();
            if (enableRagdollFlag) RagdollEnabler();
        }

        // Switching to ragdoll
        private void RagdollEnabler()
        {
            // Remember the last animated pose
            StoreLocalState();


            // Switch Animator update mode to AnimatePhysics, so IK is updated in the fixed time step
            animatorUpdateMode = animator.updateMode;
            animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

            // Disable the Animator so it won't overwrite physics
            animator.enabled = false;

            for (int i = 0; i < rigidbones.Length; i++) rigidbones[i].WakeUp(applyVelocity, applyAngularVelocity);



            ragdollWeight = 1f;
            ragdollWeightV = 0f;

            enableRagdollFlag = false;
        }

        // Is the character currently in ragdoll mode?
        private bool isRagdoll { get { return !rigidbones[0].r.isKinematic && !animator.enabled; } }

        // Store position and rotation deltas for all the rigidbodies
        private void RecordVelocities()
        {
            foreach (Rigidbone r in rigidbones) r.RecordVelocity();
        }



        // Stored the current pose of the character
        private void StoreLocalState()
        {
            foreach (Child c in children) c.StoreLocalState();
        }

        // Interpolate the character to the last stored pose (see StoreLocalState)
        private void FixTransforms( float weight )
        {
            foreach (Child c in children) c.FixTransform(weight);
        }


        //public void UpdateCharacterJoints()
        //{
        //    //  InitializeJointSettings
        //    foreach (var item in m_JointSettings) {
        //        m_JointSettings[item.Key].SetupCharacterJoint(GetBoneTransform(item.Key));
        //    }
        //}

    }





    public class RagdollUtility
    {

        public Dictionary<HumanBodyBones, JointSettings> jointSettings = new Dictionary<HumanBodyBones, JointSettings>()
        {
            { HumanBodyBones.LeftUpperLeg, new JointSettings(new Vector3(0, 1, 0), new Vector3(0, 0, -1), -20, 90, 30, 30) },
            { HumanBodyBones.LeftLowerLeg, new JointSettings(new Vector3(0, 0, -1), new Vector3(0, 1, 0),-140, 0, 10, 30) },

            { HumanBodyBones.RightUpperLeg, new JointSettings(new Vector3(0, 1, 0), new Vector3(0, 0, 1),-20, 90, 30, 30) },
            { HumanBodyBones.RightLowerLeg, new JointSettings(new Vector3(0, 0, -1), new Vector3(0, -1, 0),-140, 0, 10, 30) },

            { HumanBodyBones.Spine, new JointSettings(new Vector3(0, 0, 1), new Vector3(0, -1, 0),-30, 20, 20, 30) },
            { HumanBodyBones.Head, new JointSettings(new Vector3(1, 0, 0),  new Vector3(0, 0, 1),-40, 50, 30, 30) },

            { HumanBodyBones.LeftUpperArm, new JointSettings(new Vector3(0, 1, 0), new Vector3(0, 0, 1), -100, 60, 60, 50) },
            { HumanBodyBones.LeftLowerArm, new JointSettings(new Vector3(0, 1, 0), new Vector3(0, 0, 1), -90, 20, 20, 30) },

            { HumanBodyBones.RightUpperArm, new JointSettings(new Vector3(0, -1, 0), new Vector3(0, 0, -1), -60, 100, 60, 50) },
            { HumanBodyBones.RightLowerArm, new JointSettings(new Vector3(0, -1, 0), new Vector3(0, 0, -1), -20, 90, 20, 30) },
        };



        [Serializable]
        public class JointSettings
        {
            public Vector3 axis;
            public Vector3 swingAxis;
            public SoftJointLimit lowTwistLimit;
            public SoftJointLimit highTwistLimit;
            public SoftJointLimit swing1Limit;
            public SoftJointLimit swing2Limit;

            private CharacterJoint charJoint;


            public JointSettings( float lowTwistLimit, float highTwistLimit, float swing1Limit, float swing2Limit )
            {
                this.lowTwistLimit = new SoftJointLimit() { limit = lowTwistLimit };
                this.highTwistLimit = new SoftJointLimit() { limit = highTwistLimit };
                this.swing1Limit = new SoftJointLimit() { limit = swing1Limit };
                this.swing2Limit = new SoftJointLimit() { limit = swing2Limit };
            }

            public JointSettings( Vector3 axis, Vector3 swingAxis, float lowTwistLimit, float highTwistLimit, float swing1Limit, float swing2Limit )
            {
                this.axis = axis;
                this.swingAxis = swingAxis;
                this.lowTwistLimit = new SoftJointLimit() { limit = lowTwistLimit };
                this.highTwistLimit = new SoftJointLimit() { limit = highTwistLimit };
                this.swing1Limit = new SoftJointLimit() { limit = swing1Limit };
                this.swing2Limit = new SoftJointLimit() { limit = swing2Limit };
            }

            public void SetupCharacterJoint( Transform transform )
            {
                charJoint = transform.GetComponent<CharacterJoint>();
                if (charJoint != null) {
                    charJoint.axis = axis;
                    charJoint.swingAxis = swingAxis;
                    charJoint.lowTwistLimit = lowTwistLimit;
                    charJoint.highTwistLimit = highTwistLimit;
                    charJoint.swing1Limit = swing1Limit;
                    charJoint.swing2Limit = swing2Limit;
                } else {
                    Debug.LogFormat("{0} does not have a character joint.", transform.gameObject);
                }

            }
        }
    }

}
