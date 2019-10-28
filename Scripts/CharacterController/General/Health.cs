namespace CharacterController
{
    using UnityEngine;
    using System;
    using System.Collections;

    [DisallowMultipleComponent]
    public class Health : MonoBehaviour
    {
        public delegate void OnTakeDamage(float amount, Vector3 position, Vector3 force, GameObject attacker, Collider hitCollider);
        public delegate void OnHeal(float amount);
        public delegate void OnDeath(Vector3 position, Vector3 force, GameObject attacker);

        //
        // Fields
        // 
        [SerializeField]
        protected bool m_Invincible;
        [SerializeField, DisplayOnly]
        protected float m_CurrentHealth;
        [SerializeField]
        protected float m_MaxHealth = 100f;
        [SerializeField]
        protected GameObject m_DamageEffect;
        [SerializeField]
        protected AudioClip m_DamageSfx;
        [SerializeField]
        protected GameObject m_DeathEffect;
        [SerializeField]
        protected AudioClip m_DeathSfx;
        [SerializeField]
        protected bool m_DeactivateOnDeath;
        [SerializeField]
        protected float m_DeactivateOnDeathDelay = 5f;
        [SerializeField]
        protected GameObject[] m_SpawnedObjectsOnDeath;

        [SerializeField]
        protected LayerMask m_DeathLayer;
        [SerializeField]
        protected float m_TimeInvincibleAfterSpawn;

        protected Rigidbody m_Rigidbody;
        protected Collider m_Collider;
        protected GameObject m_GameObject;
        protected Transform m_Transform;


        //
        // Properties
        // 
        public bool Invincible{
            set { m_Invincible = value; }
        }

        public float MaxHealth{
            get { return m_MaxHealth; }
            set { m_MaxHealth = value; }
        }

        public float CurrentHealth{
            get { return m_CurrentHealth; }
        }



        //
        // Methods
        // 
        protected virtual void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Collider = GetComponent<Collider>();
            m_CurrentHealth = m_MaxHealth;
            m_GameObject = gameObject;
            m_Transform = transform;


        }


		private void OnEnable()
		{
            //EventHandler.RegisterEvent<float, Vector3, Vector3, GameObject>(m_GameObject, EventIDs.OnTakeDamage, TakeDamage);
            //EventHandler.RegisterEvent<Vector3, Vector3, GameObject>(m_GameObject, "Death", Death);
		}


		private void OnDisable()
		{
            //EventHandler.UnregisterEvent<float, Vector3, Vector3, GameObject>(m_GameObject, EventIDs.OnTakeDamage, TakeDamage);
            //EventHandler.UnregisterEvent<Vector3, Vector3, GameObject>(m_GameObject, "Death", Death);
		}



		public void SetHealth(float value)
        {
            m_CurrentHealth = Mathf.Clamp(value, 0, m_MaxHealth);
        }




        public virtual void TakeDamage(float amount, Vector3 position, Vector3 force, GameObject attacker)
        {
            if (m_Invincible) return;

            if(m_CurrentHealth > 0)
            {
                //  Change health amount.
                m_CurrentHealth -= amount;
                SpawnParticles(m_DamageEffect, position, force);
                if (m_CurrentHealth <= 0)
                {
                    Die(position, force, attacker);
                }

                Debug.LogFormat("{0} hit {1}", attacker.name, gameObject.name);
            }
        }


        public virtual void TakeDamage(float amount, Vector3 position, Vector3 force, GameObject attacker, GameObject hitGameObject)
        {
            if (m_Invincible) return;


            if (m_CurrentHealth > 0)
            {
                EventHandler.ExecuteEvent(gameObject, EventIDs.OnTakeDamage, amount, position, force, attacker);
                //  Change health amount.
                m_CurrentHealth -= amount;
                SpawnParticles(m_DamageEffect, position);
                if (m_CurrentHealth <= 0)
                {
                    Die(position, force, attacker);
                }
            }
        }



        public bool IsAlive(){
            return m_CurrentHealth >= 0;
        }


        public void InstantDealth(){
            m_CurrentHealth = 0;
        }


        public virtual void Heal(float amount)
        {
            if(m_CurrentHealth + amount > m_MaxHealth){
                m_CurrentHealth = m_MaxHealth;
                EventHandler.ExecuteEvent(m_GameObject, EventIDs.OnHeal, amount);

                Debug.LogFormat("-- {0} recieved {1} health.", m_GameObject.name, amount);
            }
        }


        protected virtual void Die(Vector3 position, Vector3 force, GameObject attacker)
        {
            //Debug.LogFormat("{0} killed by {1}", m_GameObject.name, attacker.name);
            //  Deactivate gameobject on death.


        }



        protected IEnumerator FlashDamageColor()
        {
            Material material = m_GameObject.GetComponentInChildren<Renderer>().material;
            Color initialColor = material.color;
            Color flashColor = Color.red;

            float flashSpeed = 1;
            float timer = 0;
            float changeSpeed = 0.5f;
            while(timer < changeSpeed){
                material.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(timer * flashSpeed, 1));
                timer += Time.deltaTime;
                yield return null;
            }

        }





        protected void SpawnParticles(GameObject particleObject, Vector3 position)
        {
            //var go = Instantiate(particleObject, position, Quaternion.FromToRotation(m_Transform.forward + position, direction));
            var go = Instantiate(particleObject, position, Quaternion.FromToRotation(m_Transform.forward, position));
            var ps = go.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + 1);
        }

        protected void SpawnParticles(GameObject particleObject, Vector3 position, Vector3 direction)
        {
            //var go = Instantiate(particleObject, position, Quaternion.FromToRotation(m_Transform.forward + position, direction));
            var go = Instantiate(particleObject, position, Quaternion.FromToRotation(m_Transform.forward, direction));
            var ps = go.GetComponentInChildren<ParticleSystem>();
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + 1);
        }
    }

}
