using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorGenerator
{
    [Serializable]
    public class SkinMaterialSet
    {
        public string setId;

        public Renderer mesh;

        public Material[] materials;


        public SkinMaterialSet()
        {
            setId = "Generic";
            mesh = null;
            materials = new Material[0];
        }

        public SkinMaterialSet(MeshRenderer meshRenderer)
        {
            setId = "Generic";
            mesh = meshRenderer;
            materials = new Material[0];
        }

        public SkinMaterialSet(string id, MeshRenderer meshRenderer)
        {
            setId = id;
            mesh = meshRenderer;
            materials = new Material[0];
        }

        public SkinMaterialSet(string id, MeshRenderer meshRenderer, Material[] mat)
        {
            setId = id;
            mesh = meshRenderer;
            materials = mat;
        }




    }

}
