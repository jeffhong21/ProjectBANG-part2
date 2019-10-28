using UnityEngine;



namespace CharacterController
{
    using CharacterController.Items;

    [CreateAssetMenu(fileName = "ItemType", menuName = "Character Controller/Item Type", order = 1000)]
    public class ItemType : ScriptableObject
    {


        //[SerializeField, DisplayOnly]
        //protected int m_ID = -1;

        protected string description;
        [Tooltip("The generic name for the item.  This is used for animations and to categorizing the item.")]
        public string itemName;
        [Tooltip("The id used in the animator.")]
        public ItemTypeID itemID = ItemTypeID.Revolver;
        [Tooltip("")]
        public ItemCategory category = ItemCategory.Weapon;
        [Tooltip("What movement set to use.")]
        public int movementSetID = -1;

        [Tooltip("Max amount inventory can hold."), Min(0)]
        public int capacity = int.MaxValue;
        [Tooltip("Does the item take an additional item slot?")]
        public bool stackable;

        public UseableConsumableItem m_ConsumableItem;


        [Group("GUI")]
        [SerializeField] public Sprite m_icon;
        [Group("GUI")]
        [SerializeField] public Sprite m_crosshairs;


        [Tooltip("The prefab when item is dropped.")]
        public GameObject m_dropPrefab;
        //
        //  Properties
        //


        public string ItemName {
            get { return itemName; }
        }

        public int Capacity{
            get { return capacity; }
            set { capacity = Mathf.Clamp(value, 0, int.MaxValue); }
        }

        public bool Stackable { get { return stackable; } }

        public UseableConsumableItem ConsumableItem{
            get { return m_ConsumableItem; }
            set { m_ConsumableItem = value; }
        }



        private void OnValidate()
        {
            if(itemName == string.Empty) {
                itemName = this.name;
            }
        }



        [System.Serializable]
        public class UseableConsumableItem
        {
            [SerializeField, Tooltip("The type of consumable item the primary item uses.")]
            protected ItemType m_ItemType;
            [SerializeField, Tooltip("The max amount of consumable item type the primary item can hold.")]
            protected int m_capacity = int.MaxValue;


            public ItemType ItemType
            {
                get { return m_ItemType; }
                set { m_ItemType = value; }
            }

            public int Capacity
            {
                get { return m_capacity; }
                set { m_capacity = value; }
            }



            public UseableConsumableItem(ItemType itemType, int capacity)
            {
                m_ItemType = itemType;
                m_capacity = capacity;
            }
        }
    }

}
