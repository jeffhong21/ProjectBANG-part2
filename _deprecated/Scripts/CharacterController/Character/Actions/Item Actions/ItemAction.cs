namespace CharacterController
{
    using UnityEngine;
    using System.Collections;

    public class ItemAction : CharacterAction
    {
        protected int m_itemLayer = 3;

        [SerializeField, HideInInspector]
        protected int m_ItemStateID = -1;


        public virtual int ItemStateID { get { return m_ItemStateID; } set { m_ItemStateID = Mathf.Clamp(value, -1, int.MaxValue); } }


        protected override void OnValidate()
        {
            base.OnValidate();

            m_ItemStateID = ItemStateID >= 0 ? ItemStateID : -1;
            //if (string.IsNullOrEmpty(m_stateName)) m_stateName = GetType().Name;
        }

    }

}
