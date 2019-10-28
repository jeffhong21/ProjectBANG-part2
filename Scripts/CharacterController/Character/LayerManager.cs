namespace CharacterController
{
    using UnityEngine;
    using System.Collections.Generic;

    [DisallowMultipleComponent]
    public class LayerManager : MonoBehaviour
    {

        public const int Solid = 27;
        public const int VisualEffects = 28;
        public const int CharacterCollider = 29;
        public const int Agent = 30;
        public const int Player = 31;
        //  
        // Fields
        //
        [SerializeField]
        protected LayerMask m_EnemyLayer;
        [SerializeField]
        protected LayerMask m_InvisibleLayer;
        [SerializeField]
        protected LayerMask m_SolidLayers;



        public LayerMask EnemyLayer
        {
            get { return m_EnemyLayer; }
            set { m_EnemyLayer = value; }
        }

        public LayerMask InvisibleLayer
        {
            get { return m_InvisibleLayer; }
            set { m_InvisibleLayer = value; }
        }

        public LayerMask SolidLayers
        {
            get { return m_SolidLayers; }
            set { m_SolidLayers = value; }
        }



        private static Dictionary<Collider, HashSet<Collider>> collisions = new Dictionary<Collider, HashSet<Collider>>();


        public static void IgnoreCollision(Collider mainCollider, Collider otherCollider)
        {
            Physics.IgnoreCollision(mainCollider, otherCollider, true);

            if(collisions.ContainsKey(mainCollider)){
                if(collisions[mainCollider].Contains(otherCollider) == false)
                    collisions[mainCollider].Add(otherCollider);
            }
            else{
                collisions.Add(mainCollider, new HashSet<Collider>());
                collisions[mainCollider].Add(otherCollider);
            }

        }

        public static void RevertCollision(Collider mainCollider)
        {
            if (collisions.ContainsKey(mainCollider))
            {
                foreach (Collider collider in collisions[mainCollider])
                {
                    Physics.IgnoreCollision(mainCollider, collider, false);
                }
            }
        }



        public static void UpdateLayers()
        {

        }
	}

}

