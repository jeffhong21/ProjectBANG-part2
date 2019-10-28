using System;
using System.Collections.Generic;
using UnityEngine;


namespace Inventory
{


    public class Item : MonoBehaviour
    {

        [Serializable]
        public class OffsetSettings
        {
            public Vector3 localSpawnPosition;
            public Vector3 localSpawnRotation;
        }


        [SerializeField]
        protected ItemType m_itemType;



        


        public int itemId
        {
            get {
                if (m_itemType == null) return -1;
                return m_itemType.id;
            }
        }



        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public void Initialize(Inventory inventory)
        {

        }


        public void SetActive(bool active)
        {

        }


    }

}
