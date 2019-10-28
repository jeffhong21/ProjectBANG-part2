namespace CharacterController
{
    using UnityEngine;
    using System.Collections.Generic;

    public class HealthPickup : ItemPickup
    {

        [Header("--  Health Pickup Settings --")]
        [SerializeField]
        protected float healAmount;



		protected override bool ObjectPickup(Collider other)
		{
            Health health = other.GetComponent<Health>();

            if (health == null) 
                return false;
            
            health.Heal(healAmount);


            return true;
		}
	}
}