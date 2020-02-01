namespace CharacterController
{
    using UnityEngine;
    using System.Collections;

    public class DamageReciever : MonoBehaviour
    {
        [SerializeField]
        protected float m_DamageMultiplier = 1;
        [SerializeField]
        protected HumanBone m_Bone;

        protected Collider m_Collider;
        protected Rigidbody m_Rigidbody;
        protected Animator m_Animator;
        protected GameObject m_Character;
        protected GameObject m_GameObject;


		private void Awake()
		{
            m_Collider = GetComponent<Collider>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Animator = GetComponentInParent<Animator>();
            m_Character = m_Animator.gameObject;
            m_GameObject = gameObject;

            m_DamageMultiplier = Random.Range(0.5f, 2);


		}



		public virtual void TakeDamage(float amount, Vector3 position, Vector3 force, GameObject attacker)
        {
            //Debug.LogFormat("{0} got hit in the {1}.", m_Character.name, m_GameObject.name);

            amount *= m_DamageMultiplier;
            EventHandler.ExecuteEvent(m_Character, EventIDs.OnTakeDamage, amount, position, force, attacker);


            //m_Rigidbody.AddForceAtPosition(hitDirection.normalized * 10, hitLocation, ForceMode.Impulse);

        }



    }

}
