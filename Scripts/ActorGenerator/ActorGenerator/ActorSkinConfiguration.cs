using UnityEngine;
using System.Collections.Generic;
using ActorSkins.Attachments;
using ActorGenerator;

namespace ActorSkins
{
    /// <summary>
    /// Contains all the information for a specific actor.
    /// </summary>
    public class ActorSkinConfiguration : ScriptableObject
    {
        [SerializeField]
        private string m_configId;
        [SerializeField]
        private Transform m_skinObject;
        [SerializeField]
        private SkinMaterialSet m_materials;

        [SerializeField]
        private AttachmentGroup m_headAttachments;
        [SerializeField]
        private AttachmentGroup m_backAttachments;
        [SerializeField]
        private AttachmentGroup m_hipAttachments;



        public string configId{
            get { return m_configId == string.Empty ? this.name : m_configId; }
            private set { m_configId = value; }
        }



        public bool SetSkinConfig(Transform target)
        {
            var skinnedMeshRenderer = target.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer) {

                return true;
            }

            return false;
        }

    }

}
