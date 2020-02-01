using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public interface IInventory
	{
        //public delegate void OnInventoryAddItem(Item item);
        //public delegate void OnInventoryRemoveItem(Item item, int slotID);
        //public delegate void OnInventoryEquipItem(Item item);
        //public delegate void OnInventoryUnequipItem(Item item);
        //public delegate void OnInventoryUseItem(ItemType itemType, int remaining);

        //public event OnInventoryUseItem onInventoryUseItem = delegate { };
        //public event OnInventoryEquipItem onInventoryEquipItem = delegate { };
        //public event OnInventoryUnequipItem onInventoryUnequipItem = delegate { };
        //public event OnInventoryAddItem onInventoryAddItem = delegate { };
        //public event OnInventoryRemoveItem onInventoryRemoveItem = delegate { };


        bool AddItem(ItemType itemType, float amount);
        Item RemoveItem(int slotIndex);
        Item RemoveItem(ItemType itemType);
        bool PickupItem(ItemType itemType, int amount);
        Item DropItem(int slotIndex);
        Item DropItem(ItemType itemType, int slotIndex);
        Item EquipItem(int slotIndex);
        Item EquipItem(bool next = true);
        Item EquipItem(ItemType itemType);
        void UnequipItem();
        void UseItem(ItemType itemType, int count = 1);
        void UseItem(int count = 1);
        Item GetItem(int slotIndex);

    }



	public class Inventory : MonoBehaviour
	{



        [Min(0), Tooltip("How many available item slots.")]
        [SerializeField] protected int m_slotCount;

        [SerializeField] protected InventorySlot[] m_inventorySlots;

        protected Item m_activeItem;

        protected Item m_previousItem;

        protected Item m_nextItem;

        private bool m_canAdd
        {
            get {
                for (int i = 0; i < m_inventorySlots.Length; i++) {
                    if (m_inventorySlots[i].item == null) {
                        return true;
                    }
                }
                return false;
            }
        }

        //private int nextFreeSlot
        //{
        //    get {

        //    }
        //}


        protected Dictionary<ItemType, Item> m_inventoryItems;
        protected Dictionary<ItemType, int> m_itemCount;

        protected Transform m_transform;
        protected Animator m_animator;


        private void Awake()
        {


            m_transform = transform;
        }








        [Serializable]
        public class InventorySlot
        {
            public ItemType item;
            public int quantity;
            public bool isActive;
        }


    }




}
