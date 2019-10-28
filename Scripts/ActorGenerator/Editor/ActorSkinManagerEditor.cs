using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ActorSkins
{
    public class ActorSkinManagerEditor : EditorWindow
    {


        private Vector2 m_minSize = new Vector2(200, 420);
        private GUIContent m_title = new GUIContent("Actor Skin Manager");

        private Editor m_previewWindow;


        //[MenuItem("Tools/ActorSkin Toolkit/Manager", false, 100)]
        public static void ShowWindow()
        {


            var window = GetWindow<ActorSkinManagerEditor>();
            window.minSize = window.m_minSize;
            window.titleContent = window.m_title;

            window.ShowUtility();
        }


        GameObject target = null;
        Transform rootBone;

        private void OnGUI()
        {

            using (new GUILayout.VerticalScope("box")) {


                target = EditorGUILayout.ObjectField(target, typeof(GameObject), true) as GameObject;
                rootBone = EditorGUILayout.ObjectField(rootBone, typeof(Transform), true) as Transform;

                if (GUILayout.Button("Add Skin Mesh")) {
                    var selection = Selection.gameObjects;
                    if(selection.Length > 0 && selection != null) {
                        ActorSkinManager.AddActorSkinRenderer(target, selection, rootBone);

                        //foreach (var go in selection) {
                        //    ActorSkinManager.DebugActorSkin(go);
                        //}
                    }
                }





            }
        }





        private void AddSkinMeshRenderer()
        {

        }


    }
}
