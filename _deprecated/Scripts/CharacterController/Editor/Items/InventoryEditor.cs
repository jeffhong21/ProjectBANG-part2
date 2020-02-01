namespace CharacterController
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;
    using System;
    using System.IO;


    [CustomEditor(typeof(Inventory))]
    public class InventoryEditor : Editor
    {
        private static readonly string[] m_DontIncude = { "m_Script", "m_DefaultLoadout" };

        private static int m_intFieldWidth = 36;


        private Inventory m_Inventory;
        private ReorderableList m_DefaultLoadoutList;
        private ReorderableList m_InventorySlotsList;


        private SerializedProperty m_Script;
        private SerializedProperty m_DefaultLoadout;
        private SerializedProperty m_slotCount;
        private SerializedProperty m_inventorySlots;

        private SerializedProperty m_debug;
        private SerializedProperty m_equippedItem;
        private SerializedProperty m_previouslyEquippedItem;
        private SerializedProperty m_nextActiveSlot;


        string itemTypeCount = "";

        private void OnEnable()
        {
            if (target == null) return;
            m_Inventory = (Inventory)target;

            m_Script = serializedObject.FindProperty("m_Script");
            m_DefaultLoadout = serializedObject.FindProperty("m_DefaultLoadout");
            m_slotCount = serializedObject.FindProperty("m_slotCount");
            m_inventorySlots = serializedObject.FindProperty("m_inventorySlots");
            //m_inventorySlots.arraySize =

            m_debug = serializedObject.FindProperty("m_debug");
            m_equippedItem = serializedObject.FindProperty("m_equippedItem");
            m_previouslyEquippedItem = serializedObject.FindProperty("m_previouslyEquippedItem");
            m_nextActiveSlot = serializedObject.FindProperty("m_nextActiveSlot");

            m_DefaultLoadoutList = new ReorderableList(serializedObject, m_DefaultLoadout, true, true, true, true);
            m_InventorySlotsList = new ReorderableList(serializedObject, m_inventorySlots, true, false, false, false);

        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            InspectorUtility.PropertyField(serializedObject.FindProperty("m_Script"));

            EditorGUILayout.PropertyField(m_slotCount);

            //EditorGUI.indentLevel++;
            m_DefaultLoadout.isExpanded = EditorGUILayout.Foldout(m_DefaultLoadout.isExpanded, m_DefaultLoadout.displayName);
            if (m_DefaultLoadout.isExpanded) DrawReorderableList(m_DefaultLoadoutList);
            //EditorGUI.indentLevel--;


            //EditorGUI.indentLevel++;
            m_inventorySlots.isExpanded = EditorGUILayout.Foldout(m_inventorySlots.isExpanded, m_inventorySlots.displayName);
            if (m_inventorySlots.isExpanded) {
                DrawInventorySlots();
            }
            //EditorGUI.indentLevel--;





            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_debug);
            if (m_debug.boolValue)
            {
                GUI.enabled = false;
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_equippedItem);
                EditorGUILayout.PropertyField(m_previouslyEquippedItem);
                EditorGUILayout.PropertyField(m_nextActiveSlot);
                EditorGUI.indentLevel--;
                GUI.enabled = true;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_rightHandSocket"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_animator"));
            //DrawPropertiesExcluding(serializedObject, m_DontIncude);

            serializedObject.ApplyModifiedProperties();
        }



        private void DrawInventorySlots()
        {
            m_InventorySlotsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2;
                SerializedProperty element = m_InventorySlotsList.serializedProperty.GetArrayElementAtIndex(index);
                SerializedProperty item = element.FindPropertyRelative("item");
                SerializedProperty quantity = element.FindPropertyRelative("quantity");
                //SerializedProperty isActive = property.FindPropertyRelative("isActive");

                GUI.enabled = item.objectReferenceValue != null;
                Rect elementRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                //elementRect.x += rect.width * 0.4f;
                elementRect.width = rect.width * 0.36f;
                EditorGUI.ObjectField(elementRect, item.objectReferenceValue, typeof(Item), true);

                elementRect.width = m_intFieldWidth;
                elementRect.x = rect.width - (elementRect.width * 0.5f);
                quantity.intValue = EditorGUI.IntField(elementRect, quantity.intValue);

                GUI.enabled = true;
            };

            m_InventorySlotsList.drawHeaderCallback = (Rect rect) =>
            {
                //m_DefaultLoadout.isExpanded = EditorGUI.ToggleLeft(rect, "ItemType", m_DefaultLoadout.isExpanded);
                rect.x += 12;
                EditorGUI.LabelField(rect, "Item");
                rect.x = rect.width - m_intFieldWidth;
                EditorGUI.LabelField(rect, "Quantity");

            };

            m_InventorySlotsList.DoLayoutList();
        }




        private void DrawReorderableList(ReorderableList list)
        {

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2;
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                SerializedProperty m_Item = element.FindPropertyRelative("m_Item");
                SerializedProperty m_Amount = element.FindPropertyRelative("m_Amount");
                SerializedProperty m_Equip = element.FindPropertyRelative("m_Equip");
                ItemType itemType = (ItemType)m_Item.objectReferenceValue;

                Rect elementRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

                //  ItemType Scriptableobject
                elementRect.width = rect.width * 0.95f - m_intFieldWidth - 4;
                EditorGUI.ObjectField(elementRect, m_Item, GUIContent.none);

                //  Amount field.
                elementRect.width = m_intFieldWidth;
                elementRect.x = rect.width - (elementRect.width * 0.5f);
                EditorGUI.PropertyField(elementRect, m_Amount, GUIContent.none);


                ////  Toggle Enable
                //elementRect.x = rect.width - elementRect.width;
                ////elementRect.width = m_intFieldWidth;
                //EditorGUI.PropertyField(elementRect, m_Equip, GUIContent.none);
                //if (m_Equip.boolValue) {
                //    if (index < list.list.Count - 1) {
                //        for (int i = index + 1; i < list.list.Count; i++) {
                //            SerializedProperty nextElement = list.serializedProperty.GetArrayElementAtIndex(i);
                //            SerializedProperty nextElementEquip = nextElement.FindPropertyRelative("m_Equip");
                //            nextElementEquip.boolValue = false;
                //        }
                //    }
                //}
                ////EditorGUI.LabelField(rect, m_Inventory.DefaultLoadout[index].GetType().Name);
                //DrawDefaultLoadoutElement(elementRect, element);

                ////elementObj.ApplyModifiedProperties();
            };

            list.drawHeaderCallback = (Rect rect) =>
            {
                //m_DefaultLoadout.isExpanded = EditorGUI.ToggleLeft(rect, "ItemType", m_DefaultLoadout.isExpanded);
                rect.x += 12;
                EditorGUI.LabelField(rect, "Items");
                rect.x = rect.width - m_intFieldWidth;
                EditorGUI.LabelField(rect, "Amount");

            };


            //list.onSelectCallback = (ReorderableList l) =>
            //{

            //};


            list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
            {
                var menu = new GenericMenu();
                var itemGuids = AssetDatabase.FindAssets("t:ItemType");
                for (int i = 0; i < itemGuids.Length; i++)
                {
                    var itemPath = AssetDatabase.GUIDToAssetPath(itemGuids[i]);
                    menu.AddItem(new GUIContent(Path.GetFileNameWithoutExtension(itemPath)), false, () => AddItemToDefaultLoadout(itemPath));
                }
                menu.ShowAsContext();
            };

            list.onRemoveCallback = (ReorderableList l) =>
            {
                SerializedProperty serializedList = m_DefaultLoadoutList.serializedProperty;
                serializedList.DeleteArrayElementAtIndex(l.index);

                serializedObject.ApplyModifiedProperties();
            };


            list.DoLayoutList();
        }

        private void AddItemToDefaultLoadout(string itemPath)
        {
            ItemType item = (ItemType)AssetDatabase.LoadAssetAtPath(itemPath, typeof(ItemType));

            int index = m_DefaultLoadoutList.count;

            SerializedProperty serializedList = m_DefaultLoadoutList.serializedProperty;
            //  You have to ApplyModifiedProperties after inserting a new array element otherwise the changes don't get reflected right away.
            serializedList.InsertArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();

            SerializedProperty arrayElement = serializedList.GetArrayElementAtIndex(index);
            SerializedProperty itemProperty = arrayElement.FindPropertyRelative("m_Item");
            itemProperty.objectReferenceValue = item;

            serializedObject.ApplyModifiedProperties();
        }




    }

}

