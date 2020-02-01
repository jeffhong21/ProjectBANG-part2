using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using RootMotion.FinalIK;


namespace CharacterController.ccEditor
{
    [CustomEditor(typeof(FinalIKController), true)]
    public class FinalIKControllerEditor : Editor
    {
        private static readonly string[] k_dontInclude = { "m_Script" };

        FinalIKController finalIkController;
        Editor finalIKEditor;
        Rect inspectorRect;

        SerializedProperty fullbodyIK, grounderIK, lookatIK, aimIK;

        private void OnEnable()
        {
            if (target == null) return;
            finalIkController = (FinalIKController)target;

            fullbodyIK = serializedObject.FindProperty("fullbodyIK");
            grounderIK = serializedObject.FindProperty("grounderIK");
            lookatIK = serializedObject.FindProperty("lookatIK");
            aimIK = serializedObject.FindProperty("aimIK");
        }



        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //if(finalIkController == null) {

            //    return;
            //}
            //  Header
            InspectorUtility.ScriptPropertyField(serializedObject);

            //  Draw Default Inspector.
            DrawPropertiesExcluding(serializedObject, k_dontInclude);


            //  Draw final ik inspector.

            //if (finalIkController.lookatIK != null && finalIkController.lookatIK.enabled)
            //    DrawFinalIKInspector(finalIkController.lookatIK);

            serializedObject.ApplyModifiedProperties();
        }


        private void DrawFinalIKHeader(IK finalIK)
        {

        }


        private void DrawHeader(Component finalIK)
        {
            
        }


        private void AddFinalIKComponent<T>(T component)
        {

        }


        private void DrawFinalIKInspector(IK finalIK)
        {
            DrawInspector(finalIK);
        }

        private void DrawFinalIKInspector(Grounder finalIK)
        {
            DrawInspector(finalIK);
        }


        private void DrawInspector(MonoBehaviour monobehavior, string label = "")
        {
            if (monobehavior != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUI.indentLevel++;

                    finalIKEditor = CreateEditor(monobehavior);
                    if (label != string.Empty) {
                        InspectorUtility.LabelField(label);
                    }
                    finalIKEditor.OnInspectorGUI();

                    EditorGUILayout.Space();
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }
        }

    }

}
