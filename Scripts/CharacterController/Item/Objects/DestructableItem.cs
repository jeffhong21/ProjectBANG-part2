namespace CharacterController.Items
{
    using UnityEngine;

    /// <summary>
    /// Default class for Items.
    /// </summary>
    public class DestructableItem : MonoBehaviour
    {


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

        protected Collider m_Collider;

        protected Transform m_Transform;



        protected virtual void Awake()
        {

        }



        protected virtual void Collide(Transform collisionTransform, Vector3 collisionPoint, Vector3 collisionPointNormal, bool destroy)
        {

            var damagableObject = collisionTransform.GetComponentInParent<Health>();
            var hitObject = collisionTransform.gameObject;
            Vector3 hitDirection = collisionTransform.position - m_Transform.position;
            Vector3 force = hitDirection.normalized * m_ImpactForce;
            Rigidbody rigb = collisionTransform.GetComponent<Rigidbody>();



            if (damagableObject is CharacterHealth)
            {
                damagableObject.TakeDamage(m_DamageAmount, collisionPoint, force, this.gameObject, hitObject);
            }
            else if (damagableObject is Health)
            {
                damagableObject.TakeDamage(m_DamageAmount, collisionPoint, force, this.gameObject);
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




            //m_HasCollided = true;
            //if (destroy) DestroyProjectile();
        }





        protected void OnCollisionEnter(Collision collision)
        {
            //m_Velocity = 0;
            ContactPoint contact = collision.contacts[0];
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 position = contact.point;

            Collide(collision.transform, position, contact.normal, true);

        }




    }

}
