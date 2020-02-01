namespace CharacterController
{
    using System;
    using System.Collections;
    using UnityEngine;

    //[RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        //
        // Fields
        //
        [SerializeField]
        protected float m_DamageAmount = 10f;
        [SerializeField]
        protected float m_ImpactForce = 50f;
        [SerializeField]
        protected GameObject m_Explosion;
        [SerializeField]
        protected GameObject m_DefaultDecal;
        [SerializeField]
        protected GameObject m_DefaultDust;
        [Header("Projectile")]
        [SerializeField]
        protected float m_Velocity = 25;
        [SerializeField]
        protected float m_Lifespan = 1;
        [SerializeField]
        protected bool m_DestroyOnCollision = true;
        [SerializeField]
        private LayerMask m_LayerMask;


        private bool m_Initialized;

        private bool m_HasCollided;
        private float m_CurrentLifespan;
        private float m_ProjectileSize = 0.18f;                    //  If size is too small, sometimes physics doesn't register it.
        private Vector3 m_Target;


        [Header("--  Debug Settings --")]
        [SerializeField] private DebugProjectile DebugSettings = new DebugProjectile();


        private float m_DeltaTime;
        [SerializeField, DisplayOnly]
        private GameObject m_Originator;
        protected SphereCollider m_Collider;
        private Rigidbody m_Rigidbody;
        private GameObject m_GameObject;
        protected Transform m_Transform;



        [Serializable]
        public class DebugProjectile
        {
            public bool debugLogHitObject;
            public bool drawGizmos;
            public Color gizmoColor = new Color(1, 0.92f, 0.016f, 0.5f);
        }

        //
        // Methods
        //
        protected virtual void Awake()
        {
            m_Collider = GetComponent<SphereCollider>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_GameObject = gameObject;
            m_Transform = transform;
            m_DeltaTime = Time.deltaTime;

            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.useGravity = false;



            m_Originator = null;
            gameObject.SetActive(false);
            m_HasCollided = false;
            m_Initialized = false;

        }


        public void Initialize(float damage, Vector3 direction, Vector3 target, GameObject originator)
        {
            m_Target = target;
            Initialize(damage, direction, originator);
        }


        public void Initialize(float damage, Vector3 direction, GameObject originator)
        {
            m_DamageAmount = damage;
            m_Originator = originator;

            if (direction != Vector3.zero)
                m_Transform.rotation = Quaternion.LookRotation(direction);
            else
                m_Transform.rotation = Quaternion.LookRotation(m_Transform.forward);


            m_GameObject.SetActive(true);
            m_HasCollided = false;
            m_Initialized = true;
        }




        protected virtual void Update()
        {
            if(!m_Initialized) return;

            m_Transform.position += m_Transform.forward * (m_Velocity * m_DeltaTime);

            m_CurrentLifespan += Time.deltaTime;
            if (m_CurrentLifespan > m_Lifespan || m_Transform.position == m_Target){
                DestroyProjectile();
            }

        }


		protected void OnCollisionEnter(Collision collision)
		{
            m_Velocity = 0;
            ContactPoint contact = collision.contacts[0];
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 position = contact.point;

            Collide(collision.transform, position, contact.normal, m_DestroyOnCollision);

		}



		protected virtual void Collide(Transform collisionTransform, Vector3 collisionPoint, Vector3 collisionPointNormal, bool destroy)
        {
            if (DebugSettings.debugLogHitObject) Debug.LogFormat("{0} hit {1}", m_Originator.name, collisionTransform.gameObject.name);

            var damagableObject = collisionTransform.GetComponentInParent<Health>();
            var hitObject = collisionTransform.gameObject;
            Vector3 hitDirection = collisionTransform.position - m_Transform.position;
            Vector3 force = hitDirection.normalized * m_ImpactForce;
            Rigidbody rigb = collisionTransform.GetComponent<Rigidbody>();



            if (damagableObject is CharacterHealth)
            {
                damagableObject.TakeDamage(m_DamageAmount, collisionPoint, force, m_Originator, hitObject);
            }
            else if (damagableObject is Health)
            {
                damagableObject.TakeDamage(m_DamageAmount, collisionPoint, force, m_Originator);
            }
            else
            {
                //ObjectPoolManager.Instance.Spawn(m_DefaultDust, collisionPoint, Quaternion.FromToRotation(m_Transform.forward, collisionPointNormal));
                //ObjectPoolManager.Instance.Spawn(m_DefaultDust, collisionPoint, Quaternion.LookRotation(collisionPointNormal));
            }


            if (rigb && !collisionTransform.gameObject.isStatic)
            {
                rigb.AddForceAtPosition(hitDirection.normalized * m_ImpactForce, collisionPoint, ForceMode.Impulse);
            }




            m_HasCollided = true;
            if (destroy) DestroyProjectile();
        }



        protected void DestroyProjectile()
        {
            m_Initialized = false;

            Destroy(m_GameObject);
        }




		private void OnDrawGizmos()
		{
            if(Application.isPlaying && DebugSettings.drawGizmos){
                Gizmos.color = DebugSettings.gizmoColor;
                Gizmos.DrawSphere(transform.position, m_ProjectileSize);
            }
		}



	}

}