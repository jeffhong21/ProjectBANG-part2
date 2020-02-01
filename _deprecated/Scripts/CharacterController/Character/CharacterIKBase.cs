using System.Collections;
using System.Collections.Generic;
using CharacterController.CharacterInput;
using UnityEngine;

namespace CharacterController
{
    [DisallowMultipleComponent]
    public abstract class CharacterIKBase : MonoBehaviour
    {
        protected const float tinyOffset = .0001f;

        protected RigidbodyCharacterController m_controller;
        protected Animator m_animator;
        protected Transform m_transform;
        protected GameObject m_gameObject;


        protected Vector3 m_pivotPosition;
        protected Vector3 m_leftFootPosition;
        protected Vector3 m_rightFootPosition;

        [SerializeField]
        protected Transform m_lookTarget;

        public virtual Transform lookTarget
        {
            get {
                return m_lookTarget;
            }
            set {
                m_lookTarget = value;
                //if(lookatIK != null) lookatIK.solver.target = m_lookTarget;
            }
        }

        //  ---  Abstract methods    --------------------

        /// <summary>
        /// This method is called in the Start method.  Use it to initialize any character parameters.
        /// </summary>
        protected abstract void Initialize();






        private void Awake()
        {
            if(m_animator == null) m_animator = GetComponent<Animator>();
            m_controller = GetComponent<RigidbodyCharacterController>();
            m_transform = transform;
            m_gameObject = gameObject;

            //Initialize();
        }


        protected void Start()
        {
            Initialize();
        }

        //protected virtual void OnAnimatorIK(int layerIndex)
        //{
        //    m_pivotPosition = GetPivotPosition();
        //}





        protected Vector3 GetPivotPosition()
        {
            m_animator.stabilizeFeet = true;

            Vector3 pivotPosition = m_animator.pivotPosition;

            m_leftFootPosition = GetFootPosition(AvatarIKGoal.LeftFoot);
            m_rightFootPosition = GetFootPosition(AvatarIKGoal.RightFoot);
            float leftHeight = MathUtil.Round(Mathf.Abs(m_leftFootPosition.y));
            float rightHeight = MathUtil.Round(Mathf.Abs(m_rightFootPosition.y));

            float threshold = 0.1f;
            float pivotDifference = Mathf.Abs(0.5f - m_animator.pivotWeight);
            float t = Mathf.Clamp(pivotDifference, 0, 0.5f) / 0.5f;
            //  1 means feet are not pivot.
            float feetPivotActive = 1;

            //  Both feet are grouned.
            if ((leftHeight < threshold && rightHeight < threshold) || (leftHeight > threshold && rightHeight > threshold) && pivotDifference < 5f)
            {
                t = Time.deltaTime;
                feetPivotActive = Mathf.Clamp01(feetPivotActive + t);
                pivotPosition = m_transform.position;
            }
            //  If one leg is raised and one is planted.
            else if ((leftHeight < tinyOffset && rightHeight > 0) || rightHeight < tinyOffset && leftHeight > 0)
            {
                t = t * t * t * (t * (6f * t - 15f) + 10f);
                feetPivotActive = Mathf.Lerp(0f, 1f, t);

                m_animator.feetPivotActive = feetPivotActive;
                pivotPosition = m_animator.pivotPosition;
            }


            pivotPosition.y = m_transform.position.y;


            return pivotPosition;
        }


        protected Vector3 GetFootPosition(AvatarIKGoal foot, bool worldPos = false)
        {
            if (foot == AvatarIKGoal.LeftFoot || foot == AvatarIKGoal.RightFoot)
            {
                Vector3 footPos = m_animator.GetIKPosition(foot);
                Quaternion footRot = m_animator.GetIKRotation(foot);
                float botFootHeight = foot == AvatarIKGoal.LeftFoot ? m_animator.leftFeetBottomHeight : m_animator.rightFeetBottomHeight;
                Vector3 footHeight = new Vector3(0, -botFootHeight, 0);


                footPos += footRot * footHeight;

                return !worldPos ? footPos : m_transform.InverseTransformPoint(footPos);
            }

            return Vector3.zero;
        }



        /// <summary>
        /// Create Transforms to be used as targets for IK.
        /// </summary>
        /// <param name="effectorName"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="hideFlag"></param>
        /// <returns></returns>
        protected Transform CreateIKEffectors(string effectorName, Vector3 position, Quaternion rotation, bool hideFlag = false)
        {
            Transform effector = new GameObject(effectorName).transform;
            effector.position = position;
            effector.rotation = rotation;
            effector.parent = transform;

            if(hideFlag) effector.hideFlags = HideFlags.HideInHierarchy;
            return effector;
        }
    }

}
