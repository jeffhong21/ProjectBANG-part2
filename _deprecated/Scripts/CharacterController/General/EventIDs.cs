namespace CharacterController
{
    using UnityEngine;
    using System.Collections.Generic;

    public static class EventIDs
    {
        //  Can be used to disable input.
        //EventHandler.ExecuteEvent(m_Character, "OnEnableGameplayInput", false);
        public static readonly string OnEnableGameplayInput = "OnEnableGameplayInput";


        public static readonly string OnCharacterActionActive = "OnCharacterActionActive";
        public static readonly string OnAimActionStart = "OnAimActionStart";
        public static readonly string OnItemActionActive = "OnItemActionActive";
        //
        //  Inventory events
        //
        public static readonly string OnInventoryUseItem = "OnInventoryUseItem";
        public static readonly string OnInventoryEquipItem = "OnInventoryEquipItem";
        public static readonly string OnInventoryUnequipItem = "OnInventoryUnequipItem";
        public static readonly string OnInventoryDropItem = "OnInventoryDropItem";
        public static readonly string OnInventoryAddItem = "OnInventoryAddItem";
        public static readonly string OnInventoryRemoveItem = "OnInventoryRemoveItem";
        public static readonly string OnInventoryPickupItem = "OnInventoryPickupItem";

        //
        //  Item events.
        //
        public static readonly string OnUseItem = "OnUseItem";
        public static readonly string OnEquipItem = "OnEquipItem";
        public static readonly string OnUnequipItem = "OnUnequipItem";
        public static readonly string OnDropItem = "OnDropItem";
        public static readonly string OnPickupItem = "OnPickupItem";

        //
        //  Animator events.
        //  
        public static readonly string OnAnimatorUseItem = "OnAnimatorUseItem";
        public static readonly string OnAnimatorEquipItem = "OnAnimatorEquipItem";
        public static readonly string OnAnimatorEquipItemComplete = "OnAnimatorEquipItemComplete";
        public static readonly string OnAnimatorUnequipItem = "OnAnimatorUnequipItem";
        public static readonly string OnAnimatorUnequipItemComplete = "OnAnimatorUnequipItemComplete";
        public static readonly string OnAnimatorDropItem = "OnAnimatorDropItem";
        public static readonly string OnAnimatorAddItem = "OnAnimatorAddItem";
        public static readonly string OnAnimatorRemoveItem = "OnAnimatorRemoveItem";
        public static readonly string OnAnimatorPickupItem = "OnAnimatorPickupItem";


        public static readonly string OnAnimatorStartJump = "OnAnimatorStartJump";
        public static readonly string OnAnimatorEndJump = "OnAnimatorEndJump";



        public static readonly string OnTakeDamage = "OnTakeDamage";
        public static readonly string OnHeal = "OnHeal";
        public static readonly string OnDeath = "OnDeath";
        public static readonly string OnRagdoll = "OnRagdoll";







    }





}