namespace CharacterController
{
    using UnityEngine;
    using System.Collections.Generic;

    public class ItemInventorySlot : MonoBehaviour
    {
        public enum ItemParentBone {Hips, Spine, Chest, UpperChest, LeftUpperLeg, RightUpperLeg }

        [SerializeField]
        protected int m_InventorySlotID;
        [SerializeField]
        protected Transform m_ItemSlotHolder;
        [SerializeField]
        protected Transform m_ItemSlotHolster;
        [SerializeField]
        protected ItemParentBone m_ParentBone = ItemParentBone.Hips;

        private Animator m_Animator;


        public int InventorySlotID{
            get { return m_InventorySlotID; }
        }

        public Transform ItemSlotHolder{
            get { return m_ItemSlotHolder; }
        }

        public Transform ItemSlotHolster{
            get { return m_ItemSlotHolster; }
        }

        public ItemParentBone ParentBone{
            get { return m_ParentBone; }
            set { m_ParentBone = value; }
        }






	}
}