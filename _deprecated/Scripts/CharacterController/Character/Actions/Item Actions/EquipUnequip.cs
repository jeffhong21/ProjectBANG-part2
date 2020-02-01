namespace CharacterController
{
    using UnityEngine;
    using System.Collections;

    public class EquipUnequip : ItemAction
    {
        //public override int ItemStateID
        //{
        //    get { return m_ItemStateID = ItemActionID.Aim; }
        //    set { m_ItemStateID = value; }
        //}

        protected enum state { inactive, equipping, unequipping, switching }

        [SerializeField] [DisplayOnly]
        protected int m_equipStateID = ItemActionID.Equip;
        [SerializeField] [DisplayOnly]
        protected int m_unequipStateID = ItemActionID.Unequip;

        [SerializeField] [DisplayOnly]
        private Item m_currentItem;
        [SerializeField] [DisplayOnly]
        private Item m_nextItem;

        private int m_index = -1;
        private bool m_isSwitching; 

        protected state m_currentState = state.inactive;






        public void StartEquipUnequipAction(int index)
        {
            if (index > m_inventory.SlotCount || index < 0) return;


            if( !m_isSwitching) {
                m_index = index;


                StartAction();
            }


        }


        protected override void ActionStarted()
        {
            m_isSwitching = true;

            m_currentItem = m_inventory.EquippedItem;
            m_nextItem = m_inventory.GetItem(m_index);

            EventHandler.RegisterEvent(m_gameObject, "OnItemUnequip", OnItemUnequip);
            EventHandler.RegisterEvent(m_gameObject, "OnItemEquip", OnItemEquip);


            //if (m_currentItem == null && m_nextItem != null)
            //    m_currentState = state.equipping;
            //else if (m_currentItem != null && m_nextItem == null)
            //    m_currentState = state.unequipping;
            //else if (m_currentItem != null && m_nextItem != null)
            //    m_currentState = state.switching;
            //else
            //    m_currentState = state.inactive;


            m_animatorMonitor.SetItemStateIndex(ItemActionID.Equip);
            m_animatorMonitor.SetItemStateIndex(0);
        }


        //public override string GetDestinationState(int layer)
        //{
        //    if (layer == 3) {
        //        if (m_currentState == state.unequipping || m_currentState == state.switching) {
        //            return m_currentItem.unequipStateName;
        //        }
        //        if (m_currentState == state.equipping) {
        //            return m_nextItem.equipStateName;
        //        }
        //    }

        //    return "";
        //}


        public override bool CanStopAction()
        {
            if (Time.time > m_ActionStartTime + 2.5f) return true;

            return m_currentState == state.inactive;
        }


        protected override void ActionStopped()
        {
            EventHandler.UnregisterEvent(m_gameObject, "OnItemUnequip", OnItemUnequip);
            EventHandler.UnregisterEvent(m_gameObject, "OnItemEquip", OnItemEquip);

            m_index = -1;
            m_currentState = state.inactive;
        }



        protected bool GetItemDestinationState(string stateName, int layer, out int hash)
        {
            string layerName = m_animator.GetLayerName(layer);
            string destinationStateName = layerName + "." + stateName;
            hash = Animator.StringToHash(destinationStateName);
            if (m_animator.HasState(layer, hash)) {
                return true;
            }
            return false;
        }



        protected void OnItemUnequip()
        {
            m_currentItem = null;
        }


        protected void OnItemEquip()
        {
            Debug.LogFormat("<b><color=blue>** OnItemEquip **</color></b> Animation event has been called.");
            m_currentItem = m_nextItem;
            m_nextItem = null;
        }


        protected IEnumerator SwapItems(GameObject currentItem, GameObject nextItem)
        {
            float elapsedTime = 0;
            while(currentItem != nextItem || elapsedTime > 2) {
                elapsedTime += m_deltaTime;
                yield return null;
            }

            if(nextItem == null) {

            }

        }



    }

}












//protected void OnItemUnequip()
//{
//    Debug.LogFormat("<b><color=magenta>**OnItemUnequip</color></b> Animation event has been called.  Next Item is <b>{0}</b>", m_nextItem);


//    if (m_inventory.EquippedItem != null) {
//        m_inventory.UnequipCurrentItem();
//    }

//    if (m_nextItem != null) {
//        //  Call play equip state.
//        bool changingStates = m_animatorMonitor.ChangeAnimatorState(this, m_nextItem.equipStateName, 0.25f, m_itemLayer);
//        if (changingStates) {
//            //  Set state to equipping.
//            m_currentState = state.equipping;
//            return;
//        }
//        //if(GetItemDestinationState(m_nextItem.equipStateName, m_itemLayer, out int hash)) {
//        //    m_animator.CrossFade(hash, 0.2f, m_itemLayer, 0);
//        //}


//    }

//    m_currentState = state.inactive;
//}


//protected void OnItemEquip()
//{
//    Debug.LogFormat("<b><color=blue>** OnItemEquip **</color></b> Animation event has been called.");

//    m_currentItem = m_inventory.EquipItem(m_index);

//    //if(m_currentItem != null) {
//    //    m_animatorMonitor.ChangeAnimatorState(this, m_currentItem.idleStateName, 0.2f, m_animator.GetLayerIndex(""));
//    //}

//    //Debug.Break();
//}



//protected void OnItemUnequipComplete()
//{
//    Debug.LogFormat("<b><color=magenta>** OnItemUnequip Animation event has been called</color></b>");
//    //Debug.Break();

//    m_currentState = m_nextItem != null ? state.equipping : state.inactive;
//    if (m_currentState == state.inactive)
//        StopAction();
//}
//protected void OnItemEquipComplete()
//{
//    Debug.LogFormat("<b><color=blue>** OnItemEquipComplete **</color></b> Animation event has been called</color></b>");
//    //Debug.Break();

//    m_currentState = state.inactive;
//    StopAction();
//}





//protected override void ActionStarted()
//{
//    if (m_nextItem != null) {
//        //m_nextItem.SetActive(true);

//        //var stateName = m_animator.GetLayerName(0) + ".Movement.Rifle Movement";
//        ////m_animator.CrossFade(stateName, 0.2f, 0, 0);

//        //if (m_currentItem != null) {
//        //    //  Unequip
//        //    if (itemToEquip == null) {

//        //    }

//        //    else {

//        //    }
//        //}
//        //m_currentItem = m_nextItem;
//        //m_animatorMonitor.SetItemID(m_nextItem.AnimatorItemID);
//    }
//    else {
//        if (m_currentItem != null)
//            m_currentItem.SetActive(true);
//        m_currentItem = null;
//        m_animatorMonitor.SetItemID(0);
//    }



//    m_animatorMonitor.SetMovementSetID(m_currentItem == null ? 0 : m_currentItem.AnimatorMovementSetID);






//    //Item currentItem = m_inventory.EquippedItem;
//    //Item nextItem;

//    ////
//    //if (itemSlotIndex > -1)
//    //{
//    //    nextItem = m_inventory.GetItem(itemSlotIndex);  //  DONOT chanmge what next item is.  We need to know the correct animatorID.
//    //    //  If next item is null, do nothing.
//    //    if (nextItem == null) {
//    //        if (m_Debug) Debug.LogFormat("Item slot {0} contains nothing.", itemSlotIndex);
//    //        return;
//    //    }

//    //    //  Switch the items.  Inform the inventory that it is switching.
//    //    if (currentItem != nextItem)
//    //    {
//    //        //  We want to exit the items statemachine.  We do this by unequipping the current item.
//    //        if(currentItem != null) {
//    //            m_inventory.UnequipCurrentItem();
//    //        }
//    //        //  No item is equipped.  Equip the next item.
//    //        else{
//    //            if (m_Debug) Debug.LogFormat("Equipping {0} at slot index {1} | Current: {2}", nextItem, itemSlotIndex, currentItem);
//    //            m_inventory.EquipItem(itemSlotIndex);
//    //        }

//    //    }
//    //    //  Next item is the same as the current item.  unequip current item off.  
//    //    else {

//    //        if (m_Debug) Debug.LogFormat("Item slot {0} is currently equipped item.", itemSlotIndex);
//    //        nextItem = null;
//    //        m_inventory.UnequipCurrentItem();
//    //    }
//    //}
//    ////  Alpha keycode wasn't pressed.  So switch to next or prev.
//    //else {
//    //    nextItem = m_inventory.EquipNextItem(equipNext);
//    //    //equipNext = !equipNext;
//    //}





//    ////  Unequipping current item.  Play unequip animation for current item.
//    //if(currentItem != null && nextItem == null) {
//    //    m_animatorMonitor.SetItemID(currentItem.AnimatorItemID);
//    //    m_animatorMonitor.SetItemStateIndex(ItemActionID.Unequip);
//    //}
//    ////  Currently has no items equipped.  Play next item equip animation.
//    //else if (currentItem == null && nextItem != null) {
//    //    m_animatorMonitor.SetItemID(nextItem.AnimatorItemID);
//    //    m_animatorMonitor.SetItemStateIndex(ItemActionID.Equip);
//    //}
//    //else {
//    //    m_animatorMonitor.SetItemID(0);
//    //    m_animatorMonitor.SetItemStateIndex(0);
//    //}

//    //m_animatorMonitor.SetItemStateChange();

//    Debug.LogFormat("<b>[{0}]</b> ActionStarted", GetType().Name);
//}