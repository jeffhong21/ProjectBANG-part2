namespace CharacterController
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    [RequireComponent(typeof(Item))]
    public class MeleeWeapon : UseableItem
    {
        [Header("Melee Weapon Properties")]
        [Tooltip(" ")]
        [SerializeField] protected Hitbox[] attackHitboxes;
        [Tooltip(" ")]
        [SerializeField] protected LayerMask impactLayers;
        [Tooltip("Amount of damage when item when in collision.")]
        [SerializeField] protected float damageAmount;
        [Tooltip("Max force applied when in collision.")]
        [SerializeField] protected float impactForce = 5;
        [Tooltip("Max amount of possible collision hits.")]
        [SerializeField] protected int maxCollisionCount = 30;


        protected Collider[] collisionBuffer;




        protected override void Awake()
        {
            base.Awake();

            collisionBuffer = new Collider[maxCollisionCount];


            //if(attackHitboxes == null || attackHitboxes.Length == 0) {
            //    attackHitboxes =  new Hitbox[1] { m_GameObject.AddComponent<BoxCollider>() };
                
            //}
        }




        protected void OnCollisionEnter( Collision collision )
        {
            
        }


         

        protected virtual void OnDrawGizmos()
        {

            

        }



    }
}
