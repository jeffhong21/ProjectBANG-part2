namespace CharacterController
{
    using UnityEngine;


    public class Shoot : Use
    {

        protected ShootableWeapon m_ShootableWeapon;

        //
        // Methods
        //
        public override bool CanStartAction()
        {
            if (base.CanStartAction() && m_controller.Aiming)
                return m_inventory.EquippedItem != null;
            return false;
        }


        protected override void ActionStarted()
        {
            //m_inventory.EquippedItem.TryUse();
            //if (CameraController.Instance != null) {
            //    m_ShootableWeapon = (ShootableWeapon)m_inventory.GetCurrentItem(m_inventory.EquippedItemType);
            //    Ray ray = CameraController.Instance.Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            //    RaycastHit hit;
            //    if (Physics.Raycast(ray.origin, ray.direction, out hit, 50f, m_ShootableWeapon.ImpactLayers)) {
            //        m_ShootableWeapon.SetFireAtPoint(hit.point);
            //        m_inventory.UseItem(m_inventory.EquippedItemType, 1);
            //        return;
            //    }
            //}
            //m_inventory.UseItem(m_inventory.EquippedItemType, 1);
            //Debug.Log("Shoot Action raycast did not hit anything to provide gun a cheat target.");
        }

        //protected override void ActionStopped()
        //{
        //    //Debug.Log("Shooting action done");
        //}
    }

}