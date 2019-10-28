//namespace CharacterController
//{
//    using UnityEngine;


//    public class EquipWeaponCycle : CharacterAction
//    {
//        [Header("--  Equip Weapon Action Settings --")]
//        [SerializeField]
//        protected const int m_EquipStateID = 3;
//        [SerializeField]
//        protected const int m_UnequipStateID = 4;
//        [SerializeField]
//        protected ItemType m_Item;
//        [SerializeField]
//        protected string m_ItemName;
//        [SerializeField]
//        protected int m_LayerIndex;
//        [Header("--  States --")]
//        [SerializeField]
//        protected bool m_IsEquipping;
//        [SerializeField]
//        protected bool m_IsUnequipping;
//        [SerializeField]
//        protected bool m_IsSwitching;

//        //
//        // Methods
//        //
//        protected virtual void Start()
//        {
//            m_LayerIndex = m_animatorMonitor.UpperBodyLayerIndex;
//        }

//		//public override void StartAction()
//		//{
//		//    m_IsActive = true;
//		//    ActionStarted();
//		//    EventHandler.ExecuteEvent(m_gameObject, "OnCharacterActionActive", this, true);
//		//    //m_animator.CrossFade(Animator.StringToHash(GetDestinationState(m_LayerIndex)), m_TransitionDuration, m_LayerIndex);
//		//}

//		//public override void StopAction()
//		//{
//		//    m_IsActive = false;
//		//    ActionStopped();
//		//    EventHandler.ExecuteEvent(m_gameObject, "OnCharacterActionActive", this, false);
//		//}





//        public override bool CanStartAction()
//        {

//            if (Input.GetKeyDown(KeyCode.Q))
//            {
//                if (!m_IsActive)
//                {
//                    return true;
//                }
//            }
//            return false;
//		}

//		protected override void ActionStarted()
//        {
//            m_inventory.SwitchItem(true);
//            //m_animatorMonitor.SetItemID(m_inventory.CurrentItemID);

//            //Debug.LogFormat("Starting EquipUnequip");
//            //var currentItem = m_inventory.EquippedItemType;
//            //var nextItem = m_inventory.GetNextItem(true);
//            ////  If no item is equipped, than equip next item.
//            //if (currentItem == null && nextItem != null)
//            //{
//            //    //Debug.LogFormat("Equipping {0}", nextItem.name);
//            //    m_IsSwitching = false;
//            //    m_animatorMonitor.SetItemID(m_inventory.GetItem(nextItem).ItemID);
//            //    //m_animatorMonitor.SetItemStateIndex(m_EquipStateID);

//            //    m_inventory.EquipItem(nextItem);
//            //}
//            ////  Switching Items
//            //else if (currentItem != null && nextItem != null)
//            //{
//            //    //Debug.LogFormat("{0} is switching to {1}", currentItem.name, nextItem.name);
//            //    m_IsSwitching = true;
//            //    m_animatorMonitor.SetItemID(m_inventory.GetItem(nextItem).ItemID);
//            //    //m_animatorMonitor.SetItemStateIndex(m_EquipStateID);

//            //    m_inventory.SwitchItem(true);
//            //}
//            ////  Unequipping.
//            //else if (nextItem == null)
//            //{
//            //    //Debug.LogFormat("Unequipping {0}", currentItem.name);
//            //    m_inventory.UnequipCurrentItem();
//            //    m_IsSwitching = false;
//            //    m_animatorMonitor.SetItemID(m_inventory.GetItem(currentItem).ItemID);
//            //    //m_animatorMonitor.SetItemStateIndex(m_UnequipStateID);
//            //}
//            //else
//            //{

//            //}
//        }





//        public override bool CanStopAction()
//        {
//            if (m_IsActive)
//            {
//                m_TransitionInfo = m_animator.GetAnimatorTransitionInfo(m_LayerIndex);
//                //Debug.LogFormat("Switching transition duration: {0}", duration);
//                if (GetNormalizedTime() > 1 - m_TransitionInfo.duration)
//                {
//                    return true;
//                }
//            }

//            return false;
//        }


//        protected override void ActionStopped()
//        {
//            //Debug.LogFormat("Done Equipping Unequipping.");
//            //m_animatorMonitor.SetItemStateIndex(0);
//            //m_IsSwitching = false;



//            //if(m_inventory.EquippedItemType != null){
//            //    var itemObject = m_inventory.GetItem(m_inventory.EquippedItemType);
//            //    if (itemObject != null)
//            //        m_animatorMonitor.SetMovementSetID(m_inventory.GetItem(m_inventory.EquippedItemType).MovementSetID);
//            //}
//        }

//        //public override void ActionWillStart(CharacterAction nextAction)
//        //{
//        //    if(nextAction.CanStartAction()){
//        //        nextAction.StartAction();
//        //    }
//        //}



//        public override string GetDestinationState(int layer)
//        {
//            //var currentItem = m_inventory.GetCurrentItem();
//            //var nextItem = m_inventory.GetNextItem(true);

//            //if (currentItem == null){
//            //    if (nextItem == null)
//            //        return null;
//            //    return m_inventory.GetItem(nextItem).ItemAnimName;
//            //    //return null;
//            //}
//            //return currentItem.ItemAnimName;
//            return "";
//        }


//        public override float GetNormalizedTime()
//        {
//            return m_animator.GetCurrentAnimatorStateInfo(m_LayerIndex).normalizedTime % 1; ;
//        }


//        protected int GetItemID()
//        {
//            var itemObject = m_inventory.GetCurrentItem();
//            if (itemObject == null)
//                return 0;

//            var itemID = itemObject.ItemID;
//            return itemID;
//        }




//    }

//}
