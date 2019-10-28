using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSkins
{

    public class SkinObject : MonoBehaviour, ISkinObject
    {



        [SerializeField]
        private Renderer m_mesh;



        public Renderer mesh { get => m_mesh; }
    }
}
