namespace CharacterController
{
    using UnityEngine;



    public class Vault : CharacterAction
    {
        public override int ActionID { get { return m_actionID = ActionTypeID.Vault; } set { m_actionID = value; } }

        [Tooltip("Maximum height."), Range(0, 90)]
        [SerializeField] protected float angleThreshold = 30f;
        [Tooltip("Max distance to start action.")]
        [SerializeField] protected float startDistance = 2f;
        [Tooltip("Minimum height to check if action can start.")]
        [SerializeField] protected float minHeight = 0.4f;
        [Tooltip("Maximum height.")]
        [SerializeField] protected float maxHeight = 2f;
        [Tooltip("Layers to check against.")]
        [SerializeField] protected LayerMask detectLayers;

        public ActionStateBehavior stateBehavior;

        protected float platformHeight;
        protected RaycastHit objectHit, heightHit;

        protected bool cachedIsKinamatic;

        //  Where to start the vertical raycast.
        protected Vector3 heightCheck;

        protected float objectAngle;

        protected Vector3 rayOrigin;
        protected Vector3 objectNormal;
        protected Vector3 platformEdge;
        protected Vector3 startReach, endReach;
        protected Vector3 startPosition, endPosition;

        protected Vector3 m_verticalVelocity;

        private float distance;
        private Vector3 verticalPosition, fwdPosition;
        private bool apexReached;
        private float currentTime, totalTime;
        protected float checkHeight = 0.4f;

        


        #region Character Action Methods

        protected virtual void Start()
        {
            detectLayers = m_layers.SolidLayers;
        }


        public override bool CanStartAction()
        {
            if (!base.CanStartAction()) return false;


            //rayOrigin = m_transform.position + (Vector3.up * checkHeight) + (m_transform.forward * (m_collider.radius - 0.1f));

            //  Get cast origin.
            checkHeight = m_collider.radius;
            rayOrigin = m_transform.position + (Vector3.up * checkHeight);

            if (Physics.Raycast(rayOrigin, m_transform.forward, out objectHit, startDistance, detectLayers))
            {
                //  Check if we meet the angle threshold.
                float angle = Vector3.Angle(m_transform.forward, -objectHit.normal);
                if (Mathf.Abs(angle) < angleThreshold)
                {
                    distance = Vector3.Distance(rayOrigin, objectHit.point);
                    return CheckHeightRequirement(objectHit.point);
                }
                    
            }

            return false;
        }


        /// <summary>
        /// Check if the detected object meets the height requirements.
        /// </summary>
        /// <param name="hitPoint"></param>
        /// <returns>Returns true if hit object meets the height requirement </returns>
        protected bool CheckHeightRequirement( Vector3 hitPoint )
        {
            heightCheck = hitPoint + Vector3.up * (maxHeight - checkHeight + 0.01f);
            float radius = m_collider.radius * 0.75f;

            if (Physics.SphereCast(heightCheck, radius, Vector3.down, out heightHit, maxHeight, detectLayers)) {
                //  If max height is 2m and distance is 0.4m, than the platform height is 1.6m.
                platformHeight = maxHeight - heightHit.distance;
                if (platformHeight >= minHeight) {
                    
                    return true;
                }
            }

            platformHeight = 0;
            heightCheck = Vector3.zero;
            return false;
        }


        protected Vector3 GetNormal(Vector3 normal)
        {
            var rhs = Vector3.Cross(normal, Vector3.up);
            var orthoNormal = Vector3.Cross(rhs, Vector3.down);

            if (m_Debug) Debug.DrawRay(objectHit.point, normal, Color.red, 2);
            if (m_Debug) Debug.DrawRay(objectHit.point, orthoNormal, Color.blue, 2);

            return orthoNormal;
        }


        protected Vector3 GetEndPosition(int depthCheck = 4)
        {
            depthCheck = Mathf.Clamp(depthCheck, 2, 5);
            heightCheck = objectHit.point + Vector3.up * (maxHeight - checkHeight + 0.01f);
            RaycastHit hit;
            for (int i = 1; i < depthCheck + 1; i++) {
                Vector3 raycastPosition = heightCheck + -objectNormal * (m_collider.radius * i);
                if (Physics.Raycast(raycastPosition, Vector3.down, out hit, maxHeight, detectLayers))
                {
                    if (m_Debug)
                        Debug.DrawRay(raycastPosition, Vector3.down * maxHeight, Color.green, 1);

                    if (i > 2){
                        if(Mathf.Abs(hit.point.y - endPosition.y) > m_collider.radius) {
                            RaycastHit depthHit;
                            if (Physics.Raycast(raycastPosition + -objectNormal * m_collider.radius, -objectNormal, out depthHit, maxHeight, detectLayers)) {
                                endPosition = depthHit.point;
                                continue;
                            }
                        }
                        if (hit.point == endPosition) {
                            if (Mathf.Abs(hit.point.y - endPosition.y) > m_collider.radius) {
                                endPosition += -objectNormal * (m_collider.radius * 1.0f);
                            }
                            break;
                        }
                            
                    }
                    endPosition = hit.point;
                }

            }
            return endPosition;
        }


        protected override void ActionStarted()
        {
            objectNormal = GetNormal(objectHit.normal);

            startPosition = m_transform.position;
            platformEdge = heightHit.point;
            startReach = platformEdge + objectNormal * (m_collider.radius + 0);
            endPosition = GetEndPosition();
            endReach = endPosition;
            endReach.y = platformEdge.y;


            //  Cache variables
            //cachedIsKinamatic = m_rigidbody.isKinematic;
            //m_rigidbody.isKinematic = true;

            m_animator.SetInteger(HashID.ActionID, ActionTypeID.Vault);
            m_animatorMonitor.SetActionIntData(distance > 1 ? 1 : 0);
            //m_animatorMonitor.SetActionFloatData(distance);

            currentTime = 0;
            totalTime = 0.5f;


            StartVault();
        }


        public override bool UpdateRotation()
        {
            //Quaternion rotation = Quaternion.FromToRotation(m_transform.forward, -objectNormal) * m_transform.rotation;
            //m_rigidbody.MoveRotation(Quaternion.Slerp(rotation, m_transform.rotation, m_deltaTime * m_controller.RotationSpeed));
            return false;
        }




        protected void StartVault()
        {
            m_verticalVelocity = Vector3.up * Mathf.Sqrt(2 * (platformHeight + 0.02f) * Mathf.Abs(m_controller.gravity));
            //m_rigidbody.velocity += m_verticalVelocity;
            //m_rigidbody.AddForce(m_verticalVelocity, ForceMode.VelocityChange);

            Vector3 dir = platformEdge - m_transform.position;

            //m_animatorMonitor.MatchTarget(endReach, Quaternion.identity);
        }





        public override bool UpdateMovement()
        {
//            Debug.Log(m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            //currentTime += m_deltaTime;
            //if(currentTime > totalTime) {
            //    currentTime = totalTime;
            //}
            //var perc = currentTime / totalTime;
            //var heightDistance = platformEdge.y - m_transform.position.y;

            //if (apexReached == false) apexReached = heightDistance <= 0f;

            ////Debug.LogFormat("heightDistance: {0}, apexReached: {1}, rootMotion: {2}", heightDistance, apexReached, m_animator.applyRootMotion);
            //if (heightDistance >= 0f && !apexReached) {
            //    //verticalPosition = Vector3.Lerp(m_transform.position, startReach, m_deltaTime * 10);
            //    verticalPosition = Vector3.MoveTowards(m_transform.position, startReach, m_deltaTime * 10);
            //}
            //if (heightDistance <= 0.1f) {
            //    fwdPosition = Vector3.Lerp(m_transform.position, endReach, perc * perc);
            //}


            //m_controller.Velocity = m_controller.RootMotionVelocity;


            //var verticalVelocity = -(2 * height) / (m_verticalVelocity.y * m_verticalVelocity.y);
            //m_verticalVelocity = m_controller.Gravity * m_deltaTime + (Vector3.zero * verticalVelocity);



            //m_rigidbody.MovePosition((verticalPosition + fwdPosition));
            DebugDraw.Sphere(verticalPosition + fwdPosition, 1f, Color.blue);
            
            //if (m_Debug) DebugDraw.Sphere(verticalPosition + fwdPosition, 0.1f, Color.black);

            //if ((endReach - m_transform.position).sqrMagnitude < 0.1f) {
            //    m_rigidbody.isKinematic = cachedIsKinamatic;
            //}

            //Debug.LogFormat("Velocity: {0}", m_controller.Velocity);


            //m_controller.Velocity = velocity;
            //m_animator.MatchTarget(endPosition, Quaternion.identity, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 0), 0.2f, 0.4f);
            return false;
        }




        public override bool CanStopAction()
        {
            if (Time.time < m_ActionStartTime + 0.1f) return false;

            //if (m_animator.isMatchingTarget) return false;

                int layerIndex = 0;
            if (m_animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == 0) {
                m_ExitingAction = true;
            }
            if (m_ExitingAction && m_animator.IsInTransition(layerIndex)) {
                Debug.LogFormat("{1} is exiting. | {0} is the next state.", m_animatorMonitor.GetStateName(m_animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash), this.GetType());
                return true;
            }



            return Time.time > m_ActionStartTime + 2;
        }

        protected override void ActionStopped()
        {
            if (m_animator.isMatchingTarget) m_animator.InterruptMatchTarget();
            //m_rigidbody.isKinematic = cachedIsKinamatic;
            //m_rigidbody.velocity = m_controller.Velocity;

            startPosition = Vector3.zero;
            startReach = Vector3.zero;
            endReach = Vector3.zero;
            endPosition = Vector3.zero;
            platformEdge = Vector3.zero;
            apexReached = false;
        }



        public override string GetDestinationState(int layer)
        {
            if (layer == 0) {
                return "Vault.Vault";
            }

            return "";
        }



        #endregion













        protected virtual void OnDrawGizmos()
        {
            if (Application.isPlaying && m_isActive && m_Debug) {
                //Gizmos.color = Color.green;
                //Gizmos.DrawRay(m_transform.position + (Vector3.up * m_CheckHeight), m_transform.forward * m_MoveToVaultDistance);

                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(startPosition, 0.08f);

                Gizmos.color = Color.cyan;
                GizmosUtils.DrawMarker(startReach, 0.12f, Color.cyan);

                Gizmos.color = Color.cyan;
                GizmosUtils.DrawMarker(endReach, 0.12f, Color.cyan);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(endPosition,m_collider.radius * 0.5f);


                //motionPath.DrawMotionPath();
            }
        }
    }

}

