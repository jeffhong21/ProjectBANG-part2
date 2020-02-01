namespace CharacterController
{
    using UnityEngine;


    public class Aim : ItemAction
    {

        public override int ItemStateID {
            get { return m_ItemStateID = ItemActionID.Aim; }
            set { m_ItemStateID = value; }
        }

        protected int movementsetID;



        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if (!base.CanStartAction()) return false;

            if(m_isActive || !m_controller.isGrounded) {
                return false;
            }

            return true;
        }


        protected override void ActionStarted()
        {
            //movementsetID = m_inventory.EquippedItem == null ? 0 : m_inventory.EquippedItem.movementSetID;
            ////m_animatorMonitor.SetItemID(GetItemID(), m_ItemStateID);
            //m_controller.Aiming = true;
            //m_animator.SetBool(HashID.Aiming, m_controller.Aiming);

            //m_animatorMonitor.SetActionID(m_actionID);
            //m_animatorMonitor.SetMovementSetID(movementsetID);

            Debug.LogFormat("<b>Aiming with {0}</b>.", m_inventory.EquippedItem);

            m_controller.Aiming = true;
            m_animatorMonitor.SetAiming(true);

            EventHandler.ExecuteEvent(m_gameObject, EventIDs.OnAimActionStart, m_controller.Aiming);

            //CameraController.Instance.SetCameraState("AIM");
        }


        protected override void ActionStopped()
        {
            //CameraController.Instance.SetCameraState("DEFAULT");
            Debug.LogFormat("<b>Exiting aiming state with {0}</b>.", m_inventory.EquippedItem);
            m_controller.Aiming = false;
            m_animatorMonitor.SetAiming(false);



            EventHandler.ExecuteEvent(m_gameObject, EventIDs.OnAimActionStart, m_controller.Aiming);
        }





        public override bool IsConcurrentAction()
        {
            return true;
        }





    }

}
