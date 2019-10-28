namespace CharacterController
{
    using UnityEngine;
    using System.Collections;

    [RequireComponent(typeof(Collider))]
    public class Hitbox : MonoBehaviour
    {


        public Collider hitbox;
        [Min(0)]
        public int damageMultiplier = 1;




        private void OnValidate()
        {
            if (hitbox != null)
            {
                if(!(hitbox is BoxCollider) || !(hitbox is SphereCollider) || !(hitbox is CapsuleCollider)) {

                }
            }


        }

    }

}
