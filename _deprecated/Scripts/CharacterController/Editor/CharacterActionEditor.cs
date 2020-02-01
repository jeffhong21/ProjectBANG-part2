namespace CharacterController
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;



    [CustomEditor(typeof(CharacterAction), true)]
    public class CharacterActionEditor : Editor
    {


        private static readonly string[] m_DontIncude = { "m_Script", "m_startType", "m_stopType", "m_inputKey" };

        private CharacterAction m_action;
        private GUIContent m_ActionSettingsHeader;
        private GUIStyle m_HeaderStyle;

        SerializedProperty m_animatorMotion, m_executeOnStart, m_executeOnStop;
        SerializedProperty enableDebug;


        private void OnEnable()
        {
            if (target == null) return;
            m_action = (CharacterAction)target;
            m_action.hideFlags = HideFlags.HideInInspector;

            if(m_ActionSettingsHeader == null){
                m_ActionSettingsHeader = new GUIContent()
                {
                    text = "-- " + m_action.GetType().Name + " Settings --",
                    tooltip = "Settings for " + m_action.GetType().Name
                };
            }
            if(m_HeaderStyle == null){
                m_HeaderStyle = new GUIStyle()
                {
                    font = new GUIStyle(EditorStyles.label).font,
                    fontStyle = FontStyle.Bold,
                    //fontSize = 12,
                };
            }


            m_animatorMotion = serializedObject.FindProperty("m_animatorMotion");
            m_executeOnStart = serializedObject.FindProperty("m_executeOnStart");
            m_executeOnStop = serializedObject.FindProperty("m_executeOnStop");
            enableDebug = serializedObject.FindProperty("m_Debug");
        }



        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //  Header

            InspectorUtility.ScriptPropertyField(serializedObject);
            //GUI.enabled = false;
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            //GUI.enabled = true;
            InspectorUtility.LabelField(m_ActionSettingsHeader);

            InspectorUtility.PropertyField(serializedObject.FindProperty("m_stateName"));
            InspectorUtility.PropertyField(serializedObject.FindProperty("m_TransitionDuration"));
            InspectorUtility.PropertyField(serializedObject.FindProperty("m_SpeedMultiplier"));
            InspectorUtility.PropertyField(serializedObject.FindProperty("m_actionID"));
            //InspectorUtility.PropertyField(serializedObject.FindProperty("m_startType"));
            //InspectorUtility.PropertyField(serializedObject.FindProperty("m_stopType"));
            SerializedProperty inputNames = serializedObject.FindProperty("m_InputNames");
            inputNames.isExpanded = true;

            //if (m_action.StartType == ActionStartType.ButtonDown ||
            //   m_action.StartType == ActionStartType.DoublePress ||
            //   m_action.StopType == ActionStopType.ButtonToggle ||
            //   m_action.StopType == ActionStopType.ButtonUp
            //  ) {
            //    //EditorGUILayout.Space();
            //    SerializedProperty inputNames = serializedObject.FindProperty("m_InputNames");
            //    inputNames.isExpanded = true;
            //    //EditorGUI.indentLevel++;
            //    //if(inputNames.isExpanded){
            //    //    if(inputNames.arraySize == 0){
            //    //        inputNames.InsertArrayElementAtIndex(0);
            //    //    }
            //    //    else{
            //    //        for (int index = 0; index < inputNames.arraySize; index++)
            //    //        {
            //    //            EditorGUILayout.BeginHorizontal();
            //    //            //inputNames.GetArrayElementAtIndex(index).stringValue = EditorGUILayout.TextField(inputNames.GetArrayElementAtIndex(index).stringValue);
            //    //            EditorGUILayout.PropertyField(inputNames.GetArrayElementAtIndex(index), GUIContent.none);
            //    //            if (index == inputNames.arraySize - 1){
            //    //                if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(28))){
            //    //                    inputNames.InsertArrayElementAtIndex(index);
            //    //                }
            //    //            }
            //    //            if (inputNames.arraySize > 1){
            //    //                if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(28))){
            //    //                    inputNames.DeleteArrayElementAtIndex(index);
            //    //                }
            //    //            }
            //    //            EditorGUILayout.EndHorizontal();
            //    //        }
            //    //    }
            //    //}
            //    //EditorGUI.indentLevel--;
            //}

            //  Draw animator motion.
            InspectorUtility.PropertyField(m_animatorMotion);
            //  Draw on start Audio and PArticle effects
            DrawExecuteEffects(ref m_action.executeOnStart, new GUIContent("Play On Start", "Execute effects when action start."), new string[2] { "m_StartEffect", "m_StartAudioClips" });
            DrawExecuteEffects(ref m_action.executeOnStop, new GUIContent("Play On Stop", "Execute effects when action stops."), new string[2]{ "m_EndEffect", "m_StopAudioClips" } );
            EditorGUILayout.Space();

            //  Draw properties.
            DrawPropertiesExcluding(serializedObject, m_DontIncude);



            //  -----
            //  Debugging
            //  -----
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                InspectorUtility.PropertyField(enableDebug, true);
            }
            EditorGUILayout.EndVertical();





            serializedObject.ApplyModifiedProperties();
        }


        //new GUIContent("On Start", "Execute effects on start.")


        private void DrawExecuteEffects(ref bool boolValue, GUIContent label, string[] properties = null)
        {
            if(properties != null) {
                boolValue = EditorGUILayout.ToggleLeft(label, boolValue);
                if (boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < properties.Length; i++) {
                        InspectorUtility.PropertyField(serializedObject.FindProperty(properties[i]), true);
                    }
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }
            }

        }



    }

}

