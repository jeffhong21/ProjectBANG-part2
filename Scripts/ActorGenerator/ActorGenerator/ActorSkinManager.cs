using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ActorSkins
{
    public static class ActorSkinManager
    {





        private static List<ISkinObject> allSkinObjects;





        public static void DebugActorSkin(GameObject gameObject)
        {
            
            if (gameObject == null) {
                Debug.Log("No Gameobject provided.");
                return;
            }
            string debugInfo = "<b>[" + gameObject.name + "]</b>\n";
            var skinMeshRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if(skinMeshRenderer != null) {
                debugInfo += skinMeshRenderer.bounds.ToString() + "\n";
                debugInfo += skinMeshRenderer.sharedMesh.ToString() + "\n";
                debugInfo += skinMeshRenderer.sharedMaterial.ToString() + "\n";
                debugInfo += skinMeshRenderer.bones[0].ToString() + "\n";
            }

            Debug.Log(debugInfo);
        }


        public static void AddActorSkinRenderer(GameObject target, GameObject[] skins, Transform rootBone)
        {
            string debugInfo = "<b>DebugInfo</b>\n";

            var targetAnimator = target.GetComponent<Animator>();
            if (skins != null)
            {
                Transform transform = target.transform;
                foreach (GameObject go in skins){
                    //ActorSkinComponent skin = go.GetComponentInChildren<ActorSkinComponent>();
                    SkinnedMeshRenderer skinMesh = go.GetComponentInChildren<SkinnedMeshRenderer>();
                    if (skinMesh != null) {
                        CreateSkinMeshRenderer(go, ref target, rootBone);
                    }
                    else {
                        debugInfo += string.Format("<b>{0}</b> does not contain skinmeshrenderer.\n", go.name);
                    }
                }
            }

            Debug.Log(debugInfo);
        }







        private static bool IsAvatarValid(Animator animator)
        {
            if (animator == null || !animator.isHuman || !animator.avatar.isValid) return false;
            return true;
        }



        private static void CreateSkinMeshRenderer(GameObject source, ref GameObject target, Transform rootBone)
        {
            GameObject meshObject = new GameObject(source.name);
            meshObject.AddComponent<SkinnedMeshRenderer>();
            //meshObject.AddComponent<ActorSkinComponent>();
            meshObject.transform.SetParent(target.transform);

            var sourceSMR = source.GetComponentInChildren<SkinnedMeshRenderer>();
            var targetSMR = meshObject.GetComponent<SkinnedMeshRenderer>();


            targetSMR.sharedMesh = sourceSMR.sharedMesh;
            targetSMR.sharedMaterial = sourceSMR.sharedMaterial;

            Debug.LogFormat("target: {0} | source: {1}", targetSMR.bones.Length, sourceSMR.bones.Length);

            CopyBones(rootBone, targetSMR, sourceSMR);
            Debug.LogFormat("<b>target: {0} | source: {1}</b>", targetSMR.bones.Length, sourceSMR.bones.Length);

            Debug.Log(source.name + " Shared Materials: " + sourceSMR.sharedMaterials);
            //CopyComponent(source.GetComponentInChildren<SkinnedMeshRenderer>(), meshObject);
        }


        private static void CopyBones(Transform rootBone, SkinnedMeshRenderer targetSkin, SkinnedMeshRenderer source)
        {

            Transform[] newBones = new Transform[source.bones.Length];

            for (int i = 0; i < source.bones.Length; i++) {
                foreach (var newBone in rootBone.GetComponentsInChildren<Transform>()) {
                    if (newBone.name == source.bones[i].name) {
                        newBones[i] = newBone;
                        continue;
                    }
                }
            }
            targetSkin.bones = newBones;
            targetSkin.rootBone = rootBone;
        }



        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            Type type = original.GetType();

            var dst = destination.GetComponent(type) as T;
            if (!dst) dst = destination.AddComponent(type) as T;

            var fields = GetAllFields(type);
            foreach (var field in fields) {
                if (field.IsStatic) continue;
                field.SetValue(dst, field.GetValue(original));
            }

            var props = type.GetProperties();
            foreach (var prop in props) {
                if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(dst, prop.GetValue(original, null), null);
            }

            return dst as T;
        }

        public static IEnumerable<FieldInfo> GetAllFields(System.Type t)
        {
            if (t == null) {
                return Enumerable.Empty<FieldInfo>();
            }

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;
            return t.GetFields(flags).Concat(GetAllFields(t.BaseType));
        }


    }

}
