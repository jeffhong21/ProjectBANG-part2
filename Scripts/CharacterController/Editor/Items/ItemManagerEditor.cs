namespace CharacterController
{
    using System;
    using UnityEngine;
    using UnityEditor;


    [CustomEditor(typeof(ItemManager))]
    public class ItemManagerEditor : Editor
    {
        private static readonly string[] m_DontIncude = new string[] { "m_Script" };
        private static readonly string m_SearchFilter = "t:ItemType";

        private ItemManager m_ItemManager;

        private SerializedProperty m_Items;




        private void OnEnable()
        {
            if (target == null) return;
            m_ItemManager = (ItemManager)target;

            m_Items = serializedObject.FindProperty("m_Items");

        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, m_DontIncude);



            if(GUILayout.Button(new GUIContent("Initialize ItemType Manager"))){
                //InitializeItemManager();
                m_ItemManager.Initialize();
            }


            serializedObject.ApplyModifiedProperties();
        }


        private void InitializeItemManager(string[] searchInFolders = null)
        {
            string[] guids = AssetDatabase.FindAssets(m_SearchFilter, searchInFolders);
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                ItemType asset = AssetDatabase.LoadAssetAtPath<ItemType>(path);
                Debug.LogFormat("Asset: {1}\nPath: {0}", path, asset);

                //m_Items.InsertArrayElementAtIndex(i);
                //serializedObject.ApplyModifiedProperties();

                //m_Items.GetArrayElementAtIndex(i).objectReferenceValue = asset;

                //Debug.LogFormat("{0}", m_Items.GetArrayElementAtIndex(i).name);
            }
        }
    }

}
