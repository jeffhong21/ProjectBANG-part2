﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace JH.RootMotionController
{

    public partial class RootMotionController
    {

        public Vector3 position { get => m_transform.position; }
        public Vector3 forward { get => m_transform.forward; }
        public Vector3 up { get => m_transform.up; }
        public Vector3 down { get => -m_transform.up; }




        [Serializable]
        private class MovementSettings
        {
            /// <summary> Scaler multiplier added to root motion.</summary>
            [Tooltip("Scaler multiplier added to root motion.")]
            public float rootMotionScale = 1;

            //  -----  Movement Options  -----
            /// <summary> #NOSUMMARY</summary>
            [Tooltip("#NOTOOLTIP")]
            public float forwardAcceleration = 0.2F;
            /// <summary> #NOSUMMARY</summary>
            [Tooltip("#NOTOOLTIP")]
            public float lateralAcceleration = 0.28F;
            /// <summary> #NOSUMMARY</summary>
            [Tooltip("#NOTOOLTIP")]
            public float moveDamping = 0.28F;
            /// <summary>#NOSUMMARY</summary>
            [Tooltip("#NOTOOLTIP")]
            public float maxSpeed = 10;

            //  -----  Rotation Options  -----
            /// <summary>   </summary>
            [Tooltip("The degrees per second that the character can turn.")]
            public float turningSpeed = 300f;
            /// <summary> Actor turn speed.  </summary>
            [Tooltip("Turn speed scale while not moving."), Min(0)]
            public float standingTurnSpeedScale = 0.25f;

            //  -----  Airborne Options  -----
            [Tooltip("Actor turn speed while not moving."), Min(0)]
            public float airTurnSpeedScale = 0.1f;
            public AnimationCurve airborneMoveCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

            //  -----  Other Options  -----
            /// <summary> "Multiplier to ground speed when moving backwards.  </summary>
            [Tooltip("Multiplier to ground speed when moving backwards."), Range(0, 2)]
            public float backwardsMultiplier = 0.7f;
            /// <summary> Movement speed required to start movement.  </summary>
            [Tooltip("Movement speed required to start movement."), Range(0, 0.8f)]
            public float moveThreshold = 0.2f;
        }


        [Serializable]
        private class AnimationSettings
        {
            /// <summary> The minimum angle required to trigger a turnaround while the actor is moving.  </summary>
            [Tooltip("The minimum angle required to trigger a turnaround while the actor is moving.")]
            public float turnAroundAngle = 160f;
            /// <summary> The minimum angle required to trigger a turnaround while the actor is stationary.  </summary>
            [Tooltip("The minimum angle required to trigger a turnaround while the actor is stationary.")]
            public float standingTurnAroundAngle = 89f;
            /// <summary>#NOSUMMARY</summary>
            [Tooltip("#NOTOOLTIP")]
            public float maxFallSpeed = -100;
            [Tooltip("The name of the state that should be activated when the character is moving.")]
            public string moveStateName = "Movement";
            [Tooltip("The name of the state that should be activated when the character is airborne.")]
            public string airborneStateName = "Airborne";
        }


        [Serializable]
        private class PhysicsSettings
        {
            [Tooltip("The mass of the character.  Is used to calculate how much force to add when colliding with another rigidbody.")]
            public float mass = 100;
            [Tooltip("#NotYetImplemented"), Min(0)]
            public float skinWidth = 0.08f;
            [Tooltip("#NotYetImplemented"), Min(0)]
            public float slopeLimit = 45f;
            [Tooltip("Max step height.  Should be lower than collider radius for smooth step transition."), Min(0)]
            public float maxStepHeight = 0.3f;
            [Tooltip("Use built in gravity or custom gravity.")]
            public bool useGravity;
            [Tooltip("Gravity modifier."), Range(0.01f, 4)]
            public float gravityModifier = 1f;
            [Tooltip("Maximum downward velocity the character can reach when falling.")]
            public float terminalVelocity = 10;
            [Tooltip("#NotYetImplemented")]
            public float groundStickiness = 2f;
            [Tooltip("#NotYetImplemented")]
            public float airborneThreshold = 0.3f;

        }


        [Serializable]
        private class CollisionSettings
        {
            [Serializable]
            public class ColliderSettings
            {
                [Tooltip("Center of the character collider.")]
                public Vector3 center = new Vector3(0, 0.9f, 0);
                [Tooltip("Height of the character collider."), Min(0)]
                public float height = 1.8f;
                [Tooltip("Radius of the character collider."), Min(0)]
                public float radius = 0.4f;
                [Tooltip("Characters physics material.")]
                public PhysicMaterial physicMaterial;
                [Tooltip("Colliders to ignore when determing collision detection.")]
                public Collider[] ignoreColliders = new Collider[0];
            }
            [Tooltip("Character capsule collider settings.")]
            public ColliderSettings colliderSettings = new ColliderSettings();
            [Tooltip("What layer to detect collisions.")]
            public LayerMask collisionsMask = LayerMask.NameToLayer("Ground");
            //[Tooltip("Should character check for collisions in horizontal space.")]
            //public bool detectHorizontalCollisions = true;
            //[Tooltip("Should character check for collisions in vertical space.")]
            //public bool detectVerticalCollisions = false;
            [Tooltip("Max collision count when detecting collisions.")]
            public int maxCollisionsCount = 64;

        }



        [Serializable]
        private class AdvanceSettings
        {
            [Tooltip("Actor's root bone transform.")]
            public Transform rootTransform;
            [Tooltip("Offset value of the root bone transform.")]
            public Vector3 rootTransformOffset;
            [Tooltip("Animator's update mode.")]
            public AnimatorUpdateMode animatorUpdateMode = AnimatorUpdateMode.AnimatePhysics;
            [Tooltip("Rigidbody's collision detection mode.")]
            public CollisionDetectionMode collisionDetectionMode = CollisionDetectionMode.Discrete;
            [Tooltip("Rigidbody's interpolation mode.")]
            public RigidbodyInterpolation rigidbodyInterpolation = RigidbodyInterpolation.Interpolate;
            [Tooltip("Trigger querry when detecting collisions.")]
            public QueryTriggerInteraction queryTrigger = QueryTriggerInteraction.Ignore;
            [Tooltip("Actors local time scale."), Range(0, 4)]
            public float timeScale = 1;
            [Tooltip("Is rigidbody kinematic.")]
            public bool isKinematic;
        }


        [Serializable]
        public class DebugSettings
        {
            [Tooltip("Display debug parameters.")]
            public bool showDisplaySettings;
            [Tooltip("Display velocity vectors, move direction vectors, etc.")]
            public bool showMotionVectors;
            [Tooltip("Display ground check gizmos.")]
            public bool showGroundCheck;
            [Tooltip("Display collision detections.")]
            public bool showCollisions;
            [Tooltip("Default debug color options")]
            public DebugOptions options = new DebugOptions();
            /// <summary>
            /// Lock the input values to observe the legs.
            /// </summary>
            public bool lockInputValues { get; set; }
            /// <summary>
            /// Cached inputVector when locked.
            /// </summary>
            public Vector3 cachedInputValues { get; set; }

            [Serializable]
            public class DebugOptions
            {
                public Color forwardDirectionColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
                public Color targetDirectionColor = new Color(0.5f, 0.8f, 0.0f, 1.0f);
                public Color moveDirectionColor = new Color(0.0f, 0.5f, 1.0f, 1.0f);
                public Color inputDirectionColor = new Color(0.0f, 0.25f, 1.0f, 0.5f);
                public Color velocityColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
                public Color lookDirectionColor = new Color(0.0f, 1.0f, 1.0f, 1.0f);
                public float arrowWidth = 0.3f;
                public float arrowTip { get { return arrowWidth * 0.5f; } }
            }

            public void LockInputValues(bool lockInput, Vector3 inputVector)
            {
                if (lockInput) {
                    cachedInputValues = inputVector;
                }
                else {
                    cachedInputValues = Vector3.zero;
                }
                lockInputValues = lockInput;
            }
        }



        [Serializable]
        public struct GroundInfo
        {
            public Vector3 point;
            public Vector3 normal;
            public float distance;
            public float angle;

            public GroundInfo(RaycastHit hit)
            {
                point = hit.point;
                normal = hit.normal;
                distance = hit.distance;
                angle = Vector3.Angle(hit.normal, Vector3.up);
            }


            public void Update(Vector3 point, Vector3 normal, float distance, float angle)
            {
                this.point = point;
                this.normal = normal;
                this.distance = distance;
                this.angle = angle;
            }
        }



    }
}
