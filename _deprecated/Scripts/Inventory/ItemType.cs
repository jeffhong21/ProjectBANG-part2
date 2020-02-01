using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{

    public enum ItemTypeID
    {
        Generic = 0,
        Revolver = 1,
        Rifle = 2,
        Shotgun = 3,
        MeleeUnarmed = 4,
        MeleeWeapon = 5
    }

    public enum ItemCategory
    {
        Generic,
        Weapon,
        Consumable
    }

    [CreateAssetMenu(fileName = "ItemType", menuName = "Character Controller/Item Type v2", order = 900)]
    public class ItemType : ScriptableObject
    {
        protected string m_itemName;

        protected string m_description;

        protected ItemTypeID m_id = ItemTypeID.Generic;

        //public ItemCategory m_category = ItemCategory.Generic;

        protected int m_capacity;

        protected bool m_stackable;







        public string itemName { get => m_itemName; }

        public int id { get => (int)m_id; }

        public bool stackable { get => m_stackable; }
    }

}
