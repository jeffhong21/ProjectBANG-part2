using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace CharacterController
{
    [CustomEditor(typeof(ActionStateBehavior))]
    public class ActionStateBehaviorEditor : Editor
    {

        private ReorderableList onTimedEvents;

        private bool OnTime;

        private ActionStateBehavior stateBehavior;

        private SerializedProperty enableMatchTarget;


        private List<string[]> eventIDs;
        private List<int> eventIndex;

        private void OnEnable()
        {
            stateBehavior = ((ActionStateBehavior)target);

            onTimedEvents = new ReorderableList(serializedObject, serializedObject.FindProperty("onTimedEvents"), true, true, true, true);

            enableMatchTarget = serializedObject.FindProperty("enableMatchTarget");

            //eventIDs.Clear();
            //eventIndex.Clear();
            //for (int i = 0; i < stateBehavior.onTimedEvents.Length; i++) {
            //    eventIDs.Add(GetAllEventIDs());
            //    eventIndex.Add(0);
            //}

            onTimedEvents.drawElementCallback = drawElementCallback;
            onTimedEvents.drawHeaderCallback = drawHeaderCallback;
            //onTimedEvents.onAddCallback = onAddCallback;
            //onTimedEvents.onRemoveCallback = onRemoveCallback;
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                InspectorUtility.PropertyField(serializedObject.FindProperty("m_Script"));

                DrawAnimationEvents();

                //DrawMatchTargetSettings();

            }

            EditorGUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(target, "Message Behaviour Inspector");
                //EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();
        }


        private void DrawAnimationEvents()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            if (onTimedEvents.count > 0) OnTime = true;
            OnTime = EditorGUILayout.Foldout(OnTime, "On Timed Events (" + onTimedEvents.count + ")");
            EditorGUI.indentLevel--;
            if (OnTime) {
                onTimedEvents.DoLayoutList();
            }

            EditorGUILayout.EndVertical();
        }


        private void DrawMatchTargetSettings()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;

            enableMatchTarget.boolValue = EditorGUILayout.ToggleLeft(enableMatchTarget.displayName, enableMatchTarget.boolValue);
            serializedObject.FindProperty("matchTarget").isExpanded = enableMatchTarget.boolValue;
            if (enableMatchTarget.boolValue) {
                EditorGUI.indentLevel++;
                InspectorUtility.PropertyField(serializedObject.FindProperty("matchTarget"), true);
                
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }




        private void drawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = stateBehavior.onTimedEvents[index];
            rect.y += 2;

            Rect R_0 = new Rect(rect.x, rect.y, 15, EditorGUIUtility.singleLineHeight);
            element.enabled = EditorGUI.Toggle(R_0, element.enabled);

            Rect R_1 = new Rect(rect.x + 15, rect.y, (rect.width / 4) + 15, EditorGUIUtility.singleLineHeight);
            element.animationEvent = EditorGUI.TextField(R_1, element.animationEvent);

            Rect R_3 = new Rect(rect.x + ((rect.width) / 4) + 5 + 30, rect.y, ((rect.width) / 4) - 5 - 5, EditorGUIUtility.singleLineHeight);
            element.eventType = (AnimationEventType)EditorGUI.EnumPopup(R_3, element.eventType);

            Rect R_4 = new Rect(rect.x + ((rect.width) / 4) * 2 + 5 + 25, rect.y, ((rect.width) / 4) - 5 - 15, EditorGUIUtility.singleLineHeight);


            element.time = EditorGUI.FloatField(R_4, element.time);

            if (element.time > 1) element.time = 1;
            if (element.time < 0) element.time = 0;

            Rect R_5 = new Rect(rect.x + ((rect.width) / 4) * 3 + 15, rect.y, ((rect.width) / 4) - 15, EditorGUIUtility.singleLineHeight);
            switch (element.eventType) {
                case AnimationEventType.Bool:
                    element.boolValue = EditorGUI.ToggleLeft(R_5, element.boolValue ? " True" : " False", element.boolValue);
                    break;
                case AnimationEventType.Int:
                    element.intValue = EditorGUI.IntField(R_5, element.intValue);
                    break;
                case AnimationEventType.Float:
                    element.floatValue = EditorGUI.FloatField(R_5, element.floatValue);
                    break;
                case AnimationEventType.String:
                    element.stringValue = EditorGUI.TextField(R_5, element.stringValue);
                    break;
                default:
                    break;
            }

        }


        //private void drawElementCallback2(Rect rect, int index, bool isActive, bool isFocused)
        //{
        //    var element = stateBehavior.onTimedEvents[index];
        //    rect.y += 2;

        //    Rect R_0 = new Rect(rect.x, rect.y, 15, EditorGUIUtility.singleLineHeight);
        //    element.enabled = EditorGUI.Toggle(R_0, element.enabled);

        //    Rect R_1 = new Rect(rect.x + 15, rect.y, (rect.width / 4) + 15, EditorGUIUtility.singleLineHeight);
        //    eventIndex[index] = EditorGUI.Popup(R_1, eventIndex[index], eventIDs[index]);
        //    element.animationEvent = eventIDs[index][eventIndex[index]];

        //    Rect R_3 = new Rect(rect.x + ((rect.width) / 4) + 5 + 30, rect.y, ((rect.width) / 4) - 5 - 5, EditorGUIUtility.singleLineHeight);
        //    element.eventType = (AnimationEventType)EditorGUI.EnumPopup(R_3, element.eventType);

        //    Rect R_4 = new Rect(rect.x + ((rect.width) / 4) * 2 + 5 + 25, rect.y, ((rect.width) / 4) - 5 - 15, EditorGUIUtility.singleLineHeight);

        //    element.time = EditorGUI.FloatField(R_4, element.time);

        //    if (element.time > 1) element.time = 1;
        //    if (element.time < 0) element.time = 0;

        //    Rect R_5 = new Rect(rect.x + ((rect.width) / 4) * 3 + 15, rect.y, ((rect.width) / 4) - 15, EditorGUIUtility.singleLineHeight);
        //    switch (element.eventType) {
        //        case AnimationEventType.Bool:
        //            element.boolValue = EditorGUI.ToggleLeft(R_5, element.boolValue ? " True" : " False", element.boolValue);
        //            break;
        //        case AnimationEventType.Int:
        //            element.intValue = EditorGUI.IntField(R_5, element.intValue);
        //            break;
        //        case AnimationEventType.Float:
        //            element.floatValue = EditorGUI.FloatField(R_5, element.floatValue);
        //            break;
        //        case AnimationEventType.String:
        //            element.stringValue = EditorGUI.TextField(R_5, element.stringValue);
        //            break;
        //        default:
        //            break;
        //    }

        //}


        private void drawHeaderCallback(Rect rect)
        {
            Rect R_1 = new Rect(rect.x + 10, rect.y, (rect.width / 4) + 30, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_1, "Message");

            Rect R_3 = new Rect(rect.x + 10 + ((rect.width) / 4) + 5 + 30, rect.y, ((rect.width) / 4) - 5 - 15, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_3, "Type");
            Rect R_4 = new Rect(rect.x + 10 + ((rect.width) / 4) * 2 + 5 + 20, rect.y, ((rect.width) / 4) - 5, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(R_4, "Time");
            Rect R_5 = new Rect(rect.x + ((rect.width) / 4) * 3 + 5 + 10, rect.y, ((rect.width) / 4) - 5, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_5, "Value");

        }



        //private void onRemoveCallback(ReorderableList list)
        //{
        //    eventIDs.RemoveAt(list.index);
        //    eventIndex.RemoveAt(list.index);
        //}

        //private void onAddCallback(ReorderableList list)
        //{
        //    eventIDs.Add(GetAllEventIDs());
        //    eventIndex.Add(0);
        //}





        private string[] GetAllEventIDs()
        {

            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            Type t = typeof(EventIDs);
            FieldInfo[] fields = t.GetFields(bindingFlags);
            string[] ids = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++) {
                ids[i] = fields[i].GetValue(null).ToString();
            }

            return ids;
        }

    }

}
