using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;


[CustomEditor(typeof(ObjectPool))]
public class ObjectPoolManagerEditor : Editor
{

    private ObjectPool objectPool;
    private ReorderableList list;
    private SerializedProperty listProperty;


    private SerializedProperty showInstanceID;
    private GUIContent showInstanceIDContent;
    private GUIStyle debugTextStyle;

    private bool _debugList = true;


    private void OnEnable()
    {
        if (target == null) return;
        objectPool = (ObjectPool)target;

        listProperty = serializedObject.FindProperty("m_Pools");
        list = new ReorderableList(serializedObject, listProperty, true, true, true, true);


        showInstanceID = serializedObject.FindProperty("_showInstanceID");

        if(showInstanceIDContent == null) showInstanceIDContent = new GUIContent()
        {
            text = "Show InstanceID",
            tooltip = "Display the instance ID of the pooled prefab."
        };


        if (debugTextStyle == null) debugTextStyle = new GUIStyle() { richText = true };

    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        int originalIndentLevel = EditorGUI.indentLevel;

        InspectorUtility.PropertyField(serializedObject.FindProperty("m_Script"));

        EditorGUILayout.Space();
        DrawReorderableList();

        InspectorUtility.LabelField("Options");

        //  Group Objects to new scene
        ObjectPool.GroupObjectsToNewScene = EditorGUILayout.ToggleLeft(" Group objects to new scene.", ObjectPool.GroupObjectsToNewScene);
        EditorGUI.indentLevel++;
        GUI.enabled = ObjectPool.GroupObjectsToNewScene;
        string poolSceneName = string.IsNullOrWhiteSpace(ObjectPool.PoolSceneName) ? "Object Pool Scene" : ObjectPool.PoolSceneName;
        ObjectPool.PoolSceneName = EditorGUILayout.TextField(poolSceneName);
        GUI.enabled = true;
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();
        //  Display InstanceID
        objectPool.DebugMode = EditorGUILayout.ToggleLeft("Debug Mode", objectPool.DebugMode);

        if (objectPool.DebugMode)
        {
            EditorGUI.indentLevel++;

            showInstanceID.boolValue = EditorGUILayout.ToggleLeft(showInstanceIDContent, showInstanceID.boolValue);

            _debugList = EditorGUILayout.ToggleLeft("Debug ReorderableList", _debugList);
            EditorGUI.indentLevel--;
        }


        //debugMode = EditorGUILayout.ToggleLeft("Debug Mode", debugMode);
        //if(debugMode)
        //{
        //    EditorGUI.indentLevel++;
        //    showInstanceID = EditorGUILayout.ToggleLeft(showInstanceIDContent, showInstanceID);

        //    EditorGUI.indentLevel--;
        //}



        EditorGUI.indentLevel = originalIndentLevel;
        serializedObject.ApplyModifiedProperties();
    }



    private void DrawReorderableList()
    {
        if (list == null)
            list = new ReorderableList(serializedObject, listProperty, true, true, true, true);

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            DrawElement(rect, index, isActive, isFocused);
        };

        list.drawHeaderCallback = (Rect rect) =>
        {
            DrawHeader(rect);
        };

        list.elementHeightCallback = (int index) =>
        {
            if (objectPool.DebugMode)
                return (showInstanceID.boolValue ? EditorGUIUtility.singleLineHeight * 2: EditorGUIUtility.singleLineHeight) + 4.0f;
            return EditorGUIUtility.singleLineHeight + 4.0f;
        };

        list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
        {
            bool insertAtIndex = false;  // l.index > -1;
            int index = insertAtIndex ? l.index : l.count;
            l.serializedProperty.InsertArrayElementAtIndex(index);
            var property = listProperty.GetArrayElementAtIndex(index);
            var prefab = property.FindPropertyRelative("Prefab");
            prefab.objectReferenceValue = null;
            var count = property.FindPropertyRelative("Count");
            count.intValue = 0;
            var instanceID = property.FindPropertyRelative("InstanceID");
            instanceID.intValue = 0;
        };

        list.onRemoveCallback = (ReorderableList l) =>
        {
            l.serializedProperty.DeleteArrayElementAtIndex(l.index);
        };

        list.DoLayoutList();

    }



    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        //var e = Event.current;
        //if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        //{
        //    e.Use();
        //}
        rect.y += 2;
        rect.height =  EditorGUIUtility.singleLineHeight;



        var property = listProperty.GetArrayElementAtIndex(index);
        var prefab = property.FindPropertyRelative("Prefab");
        var count = property.FindPropertyRelative("Count");
        var instanceID = property.FindPropertyRelative("InstanceID");


        float padding = 4;
        float elementRectStart = rect.x;
        float fieldWidth = EditorGUIUtility.fieldWidth * 1.5f;
        float objectValueWidth = rect.width - fieldWidth - padding;

        //  Draw prefab object value field.
        rect.x += padding;
        rect.width = objectValueWidth;
        prefab.objectReferenceValue = EditorGUI.ObjectField(rect, GUIContent.none, prefab.objectReferenceValue, typeof(GameObject), false);

        //  Draw prefab pool count.
        rect.x += rect.width + padding;
        rect.width = fieldWidth - padding;
        count.intValue = EditorGUI.IntField(rect, GUIContent.none, count.intValue);


        if(objectPool.DebugMode)
        {
            rect.x = elementRectStart + padding;
            rect.y += rect.height + 2;
            rect.height = EditorGUIUtility.singleLineHeight;

            if (showInstanceID.boolValue)
            {
                rect.x += 12;
                rect.width = objectValueWidth / 3;

                string instanceIDstring = string.Format("InstanceID: <color={1}><i>{0}</i></color>",
                    instanceID.intValue.ToString(),
                    isFocused ? "black" : "grey"
                    );
                EditorGUI.LabelField(rect, instanceIDstring, debugTextStyle);



                if (_debugList)
                {
                    rect.x += rect.width + padding;
                    rect.width = fieldWidth - padding;
                    EditorGUI.LabelField(rect,
                        string.Format("Active: <color={1}>{0}</color> | IsFocused: <color={3}>{2}</color>",
                        isActive, "blue",
                        isFocused, "blue"
                        ),
                        debugTextStyle);
                }
            }


        }

    }



    private void DrawHeader(Rect rect)
    {
        float fieldWidth = EditorGUIUtility.fieldWidth * 1.5f;
        float padding = 4;
        float headerPadding = 12;
        rect.x += headerPadding + padding;
        rect.y += 2;
        rect.width -= fieldWidth - padding;
        rect.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.LabelField(rect, "Preloaded Prefab");

        rect.x += rect.width - headerPadding - padding;
        rect.width = fieldWidth - padding;
        EditorGUI.LabelField(rect, "Count");

    }




}
