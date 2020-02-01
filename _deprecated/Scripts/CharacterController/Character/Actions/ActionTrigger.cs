namespace CharacterController
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(BoxCollider))]
    public class ActionTrigger : MonoBehaviour
    {
        
        public string actionName;


        [Header("-- Debug --")]
        [SerializeField]
        protected Color m_ColliderColor = new Color(0.2f, 0.8f, 0.8f, 0.8f);
        protected BoxCollider m_Collider;





		private void Start()
		{
            Initialize();
		}


		public void Initialize()
		{
            m_Collider = GetComponent<BoxCollider>();
            m_Collider.isTrigger = true;

		}


		public void OnTriggerEnter(Collider other)
		{
            if(other.GetComponent<RigidbodyCharacterController>()){
                var charLocomotion = other.GetComponent<RigidbodyCharacterController>();
                var animator = other.GetComponent<Animator>();

                animator.Play(actionName);
            }
		}




		private void OnDrawGizmos()
		{
            if(m_Collider == null)
                m_Collider = GetComponent<BoxCollider>();
            
            if(m_Collider != null){
                Gizmos.color = m_ColliderColor;
                Gizmos.DrawCube(m_Collider.center, m_Collider.size); 
            }



		}


	}

}
