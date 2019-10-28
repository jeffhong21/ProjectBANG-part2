namespace CharacterController
{
    using System;

    [Serializable]
    public struct InventorySlot
    {
        public Item item;
        public int quantity;
        public bool isActive;
    }

}
