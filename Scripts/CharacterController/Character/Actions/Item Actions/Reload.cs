//namespace CharacterController
//{
//    using UnityEngine;


//    public class Reload : CharacterAction
//    {
//        [Header("--  Reload Action Settings --")]
//        [SerializeField]
//        protected const int m_ItemStateID = 2;
//        [SerializeField]
//        protected ItemType m_Item;
//        [SerializeField]
//        protected string m_ItemName;
//        [SerializeField]
//        protected int m_LayerIndex;
//        [Header("--  States --")]
//        [SerializeField]
//        protected bool m_IsReloading;





//		//
//		// Methods
//		//
//        protected virtual void Start()
//        {
//            m_LayerIndex = m_animatorMonitor.UpperBodyLayerIndex;
//        }

//        public override void StartAction()
//        {
//            m_IsActive = true;

//            ActionStarted();
//            EventHandler.ExecuteEvent(m_gameObject, "OnCharacterActionActive", this, true);

//            m_animator.CrossFade(Animator.StringToHash(GetDestinationState(m_LayerIndex)), m_TransitionDuration, m_LayerIndex);
//        }

//        public override void StopAction()
//        {
//            m_IsActive = false;
//            ActionStopped();
//            EventHandler.ExecuteEvent(m_gameObject, "OnCharacterActionActive", this, false);
//        }




//		public override bool CanStartAction()
//		{
//            if (Input.GetKeyDown(KeyCode.R))
//            {
//                Debug.LogFormat("Reloading {0}", m_inventory.EquippedItemType);
//                if (!m_IsActive && m_inventory.GetCurrentItem() != null && !m_IsReloading)
//                {
//                    return true;
//                }
//            }
//            return false;
//		}


//		protected override void ActionStarted()
//        {
//            Debug.LogFormat("Reloading {0}", m_inventory.EquippedItemType);

//            m_ItemName = GetItemName();
//            m_IsReloading = true;
//            m_animatorMonitor.SetItemStateIndex(m_ItemStateID);
//        }



//		public override bool CanStopAction()
//        {
//            if (m_IsActive && m_IsReloading){
//                if (m_animator.GetCurrentAnimatorStateInfo(m_LayerIndex).shortNameHash == m_StateHash){
//                    //Debug.LogFormat("Current Hash: {0} | {1} Hash: {2}", m_animator.GetCurrentAnimatorStateInfo(m_animatorMonitor.UpperBodyLayerIndex).shortNameHash, GetType().Name, m_StateHash);
//                    if (GetNormalizedTime() >= 1 - m_TransitionDuration)
//                    {
//                        return true;
//                    }
//                }
//            }
//            return false;
//        }

//        protected override void ActionStopped()
//        {
//            //m_ItemName = "<Empty>";
//            m_IsReloading = false;
//            m_animatorMonitor.SetItemID(0);

//        }








//        public override string GetDestinationState(int layer)
//        {
//            string fullStateName = string.Format("{0}.{1}.{2}", m_animatorMonitor.UpperBodyLayerName, GetItemName(), "Reload");
//            return fullStateName;
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


//        protected string GetItemName()
//        {
//            var itemObject = m_inventory.GetCurrentItem();
//            if (itemObject == null)
//                return null;

//            var itemName = itemObject.ItemAnimName;
//            return itemName;
//        }
//    }

//}