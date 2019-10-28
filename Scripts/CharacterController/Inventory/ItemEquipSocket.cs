using UnityEngine;
using System.Collections.Generic;

namespace CharacterController.CharacterInventory
{
    public enum ItemSocketType
    {
        None = -1,
        RightHand = 0,
        LeftHand = 1
    }

    public class ItemEquipSocket : MonoBehaviour
    {
        [SerializeField]
        private ItemSocketType m_itemSocket;


        public ItemSocketType itemSocket { get { return m_itemSocket; } }






        public void Init(Animator animator)
        {
            gameObject.SetActive(true);
            m_itemSocket = ItemSocketType.RightHand;
            transform.localEulerAngles = new Vector3(0, 90, -90);

            //if (transform.IsChildOf(animator.GetBoneTransform(HumanBodyBones.RightHand))) {
            //    m_itemSocket = ItemSocketType.RightHand;
            //    transform.localEulerAngles = new Vector3(0, 90, -90);
            //    return;
            //}
            //if (transform.IsChildOf(animator.GetBoneTransform(HumanBodyBones.LeftHand))) {
            //    m_itemSocket = ItemSocketType.LeftHand;
            //    transform.localEulerAngles = new Vector3(0, -90, -90);
            //    return;
            //}
        }

    }
}