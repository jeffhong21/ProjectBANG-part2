using System;
using System.Collections.Generic;
using UnityEngine;



namespace CharacterController.CharacterInput
{
    public partial class PlayerInput
    {

        [Serializable]
        private class InventoryHandler
        {
            public ItemAction useAction { get; private set; }
            public ItemAction aimAction { get; private set; }
            public EquipUnequip equipUnequipAction { get; private set; }

            public InventoryHandler(RigidbodyCharacterController controller)
            {
                Initialize(controller);
            }

            public void Initialize(RigidbodyCharacterController controller)
            {
                useAction = controller.GetAction<Use>();
                aimAction = controller.GetAction<Aim>();
                equipUnequipAction = controller.GetAction<EquipUnequip>();
            }
        }

        private bool m_inventoryInitialized;
        private InventoryHandler m_inventoryHandler;
        private Animator m_animator;



        private void HandleItemActions()
        {
            if (m_inventoryHandler == null) m_inventoryHandler = new InventoryHandler(m_controller);
            if (m_animator == null) m_animator = m_controller.animator;


            if (m_inventory != null) {

                if (ReloadAction(m_inventory.EquippedItem, KeyCode.R)) return;

                //if (AimAction()) return;

                if (ShootAction(m_inventory.EquippedItem, KeyCode.Mouse0)) return;

                if (EquipUnequipAction()) return;



                //for (int number = 1; number < m_inventory.SlotCount + 1; number++) {
                //    if (Input.GetKeyDown(number.ToString())) {
                //        Debug.LogFormat("Inventory Item Slot {0} is {1}", number - 1, m_inventory.GetItem(number - 1));
                //    }
                //}
            }
        }




        private bool EquipUnequipAction()
        {
            if (m_inventoryHandler.equipUnequipAction != null)
            {
                m_inventoryHandler.equipUnequipAction.StartAction();
                return true;

                for (int number = 1; number < m_inventory.SlotCount + 1; number++) {
                    if (Input.GetKeyDown(number.ToString())) {
                        m_inventoryHandler.equipUnequipAction.StartEquipUnequipAction(number - 1);
                        return true;
                    }
                }
            }

            return false;
        }

        private bool ReloadAction(Item item, KeyCode key)
        {
            if (Input.GetKeyDown(key)) {
                var destinationState = m_animator.GetLayerName(1) + "." + item.ItemType.itemName + ".Reload";
                var hash = Animator.StringToHash(destinationState);
                int layerIndex = 3;
                if (m_animator.HasState(layerIndex, hash)) {
                    m_animator.CrossFade(hash, 0.1f, layerIndex, 0);
                    return true;
                }
                else {
                    Debug.LogFormat("<b>\"{0}\"</b> state does not exist.", destinationState);
                }
            }
            return false;
        }

        private bool ShootAction(Item item, KeyCode key)
        {
            if (Input.GetKeyDown(key)) {
                var destinationState = m_animator.GetLayerName(1) + "." + item.ItemType.itemName + ".Shoot";
                var hash = Animator.StringToHash(destinationState);
                int layerIndex = 3;
                if (m_animator.HasState(layerIndex, hash)) {
                    m_animator.CrossFade(hash, 0.1f, layerIndex, 0);
                    return true;
                }
                else {
                    Debug.LogFormat("<b>\"{0}\"</b> state does not exist.", destinationState);
                }
            }
            return false;
        }


        //private bool AimAction()
        //{
        //    if (m_inventoryHandler.aimAction != null) {
        //        if (Input.GetKeyDown(KeyCode.Mouse1)) {
        //            if (m_inventoryHandler.aimAction.IsActive)
        //                m_inventoryHandler.aimAction.StopAction();
        //            else
        //                m_inventoryHandler.aimAction.StartAction();
        //            return true;
        //        }
        //    }

        //    return false;
        //}

    }

}



