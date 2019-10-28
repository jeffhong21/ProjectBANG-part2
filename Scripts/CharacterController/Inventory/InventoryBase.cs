using UnityEngine;
using System;
using System.Collections.Generic;



namespace CharacterController
{
    public abstract class InventoryBase : MonoBehaviour
    {
        #region Events
        //public delegate void OnInventoryUseItem(ItemType itemType, float remaining);
        //public delegate void OnInventoryPickupItem(Item item, float count, bool immediatePickup, bool forceEquip);
        //public delegate void OnInventoryEquipItem(Item item);
        //public delegate void OnInventoryUnequipItem(Item item);
        //public delegate void OnInventoryAddItem(Item item);
        //public delegate void OnInventoryRemoveItem(Item item, int slotID);

        //public event OnInventoryUseItem InventoryUseItem = delegate { };
        //public event OnInventoryPickupItem InventoryPickupItem = delegate { };
        //public event OnInventoryEquipItem InventoryEquipItem = delegate { };
        //public event OnInventoryUnequipItem InventoryUnequipItem = delegate { };
        //public event OnInventoryAddItem InventoryAddItem = delegate { };
        //public event OnInventoryRemoveItem InventoryRemoveItem = delegate{ };
        #endregion




        //  All the items parented to the character.
        protected List<Item> m_AllItems = new List<Item>();
        //  All the itemTypes parented to the character.
        protected List<ItemType> m_AllItemTypes = new List<ItemType>();

        [Tooltip("The ItemTypes that the character starts out with.")]
        [SerializeField] protected ItemAmount[] m_DefaultLoadout = new ItemAmount[0];



        //  When ever an item is added, the item will be mapped here.
        protected Dictionary<ItemType, Item> m_ItemTypeItemMap = new Dictionary<ItemType, Item>();
        protected Dictionary<ItemType, float> m_ItemTypeCount = new Dictionary<ItemType, float>();





        protected AnimatorMonitor m_animatorMonitor;
        [SerializeField]
        protected Animator m_animator;
        protected GameObject m_gameObject;


        //
        //  Properties
        //  


        public bool IsSwitching { get; protected set; }




        protected virtual void Awake()
        {
            m_gameObject = gameObject;
            m_animator = GetComponent<Animator>();
            m_animatorMonitor = GetComponent<AnimatorMonitor>();


        }










        /// <summary>
        /// Adds the specified amount of ItemType to the inventory.
        /// </summary>
        /// <param name="itemType"> The ItemType to add. </param>
        /// <param name="count"> The amount of itemType to add. </param>
        /// <returns> Returns true if the ItemType was pickedup.</returns>
        protected bool InternalPickupItemType( ItemType itemType, float count )
        {
            float existingAmount;
            if (!m_ItemTypeCount.TryGetValue(itemType, out existingAmount)) {
                //  ItemType doesn't have an item count
                m_ItemTypeCount.Add(itemType, Mathf.Min(count, itemType.Capacity));
            } else {
                //  The itemType will not be pickedup if at capacity.
                if (existingAmount >= itemType.Capacity)
                    return false;
                //  Adding the ItemType amount.
                m_ItemTypeCount[itemType] = Mathf.Clamp(existingAmount + count, 0, itemType.Capacity);

            }

            var item = GetItem(itemType);
            if (!m_AllItems.Contains(item)) {
                m_AllItems.Add(item);
            }
            if (!m_AllItemTypes.Contains(itemType)) {
                m_AllItemTypes.Add(itemType);
            }

            return true;
        }


        protected Item InternalRemoveItemType( ItemType itemType )
        {
            float existingAmount;
            if (!m_ItemTypeCount.TryGetValue(itemType, out existingAmount))
                return null;

            Item item;
            if (!m_ItemTypeItemMap.TryGetValue(itemType, out item)) {
                //  Remove the item.  ItemType does not correspond to the item, so it should be removed completely.
                m_ItemTypeCount[itemType] = 0;
                return null;
            }

            //  Remove a single item.
            m_ItemTypeCount[itemType] = Mathf.Clamp(existingAmount - 1, 0, itemType.Capacity);

            return item;
        }


        /// <summary>
        /// Add the item to the inventory.  Does not add the actual ItemType.  PickupItem does that.
        /// </summary>
        /// <param name="item">Item to add to the inventory.</param>
        /// <returns>True if the item was added to the inventory.</returns>
        protected bool InternalAddItem( Item item, bool immediatelyEquip )
        {
            //  Notify others that an item has been added to the inventory.
            EventHandler.ExecuteEvent(m_gameObject, EventIDs.OnInventoryAddItem, item);

            //  Pickup event should be called when the count is greater than zero.
            //  This allows the item Type to be pickedup before the item has been added.
            float count = 0;
            if ((count + GetItemTypeCount(item.ItemType)) > 0) {
                //item.Pickup();
                EventHandler.ExecuteEvent(m_gameObject, EventIDs.OnInventoryPickupItem, item, count, immediatelyEquip, false);
            }


            return true;
        }


        protected void InternalUseItem( ItemType itemType, float count )
        {
            EventHandler.ExecuteEvent(m_gameObject, EventIDs.OnInventoryUseItem, itemType, count);
        }


        protected void InternalReload( ItemType itemType, float amount )
        {
            throw new NotImplementedException();
        }


        protected void InternalEquipItem(Item item)
        {
            //  Execute the equip event.
            EventHandler.ExecuteEvent(m_gameObject, EventIDs.OnInventoryEquipItem, item);
        }


        protected void InternalUnequipCurrentItem(Item lastEquippedItem)
        {
            EventHandler.ExecuteEvent(m_gameObject, EventIDs.OnInventoryUnequipItem, lastEquippedItem);
        }




        /// <summary>
        /// Gets the item from item map.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public Item GetItem( ItemType itemType )
        {
            Item item;
            m_ItemTypeItemMap.TryGetValue(itemType, out item);
            return item;
        }




        /// <summary>
        /// Get the amount of ItemType from the inventory.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public float GetItemTypeCount( ItemType itemType )
        {
            float count;
            m_ItemTypeCount.TryGetValue(itemType, out count);
            return count;
        }





        /// <summary>
        /// Returns list of all items in inventory.  Used by the editor.
        /// </summary>
        /// <returns></returns>
        public List<Item> GetAllItems() { return m_AllItems; }


        /// <summary>
        /// Returns list of all items in inventory.  Used by the editor.
        /// </summary>
        /// <returns></returns>
        public List<ItemType> GetAllItemTypes() { return m_AllItemTypes; }




        /// <summary>
        /// Adds the item to the inventory and adds the specified amount of ItemType.
        /// It should be assumed that the itemType has already been mapped internally.
        /// </summary>
        /// <param name="itemType"> The ItemType to add. </param>
        /// <param name="count"> The amount of itemType to add. </param>
        /// <param name="immediatePickup"> Should the item be picked up immediately. If false, item will be added with an animation. </param>
        /// <param name="forceEquip"> Should the item be forced to equip. </param>
        /// <param name="notifyOnPickup"> Should other objects be notified the itemType was pickedup? </param>
        /// <returns> Returns true if the ItemType was pickedup.</returns>
        public abstract bool PickupItemType( ItemType itemType, float count, bool immediatePickup, bool forceEquip, bool notifyOnPickup = true );
        public abstract Item RemoveItemType( ItemType itemType, int slotIndex );
        public abstract bool AddItem( Item item, bool immediatelyEquip );
        public abstract Item GetItem( int index ); // { return m_ActiveItems[slotID]; }
        public abstract void Reload( ItemType itemType, float amount );
        public abstract void UseItem( ItemType itemType, float count );







        [Serializable]
        public class ItemAmount
        {
            [SerializeField] protected ItemType m_Item;
            [SerializeField] protected int m_Amount = 1;
            [SerializeField] protected bool m_Equip = true;


            public ItemType ItemType {
                get { return m_Item; }
                private set { m_Item = value; }
            }

            public int Amount {
                get { return Mathf.Clamp(m_Amount, 0, m_Item.Capacity); }
                set { m_Amount = Mathf.Clamp(value, 0, m_Item.Capacity); }
            }


            public bool Equip {
                get { return m_Equip; }
                private set { m_Equip = value; }
            }


            public ItemAmount( ItemType itemType, int amount )
            {
                Initialize(itemType, amount);
            }

            public void Initialize( ItemType itemType, int amount )
            {
                ItemType = itemType;
                Amount = amount;
            }
        }
    }

}
