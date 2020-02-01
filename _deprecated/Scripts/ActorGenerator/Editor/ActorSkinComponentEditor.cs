using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Linq;

namespace ActorSkins
{
    [CustomEditor(typeof(ActorSkinComponent))]
    public class ActorSkinComponentEditor : Editor
    {

        private ReorderableList list;
        private ActorSkinComponent actorSkin;
        private SerializedProperty actors;
        private SerializedProperty hideInHierarchy;
        private MonoScript script;

        private int index = 0;
        private string[] actorNames;

        private void OnEnable()
        {
            actorSkin = (ActorSkinComponent)target;
            script = MonoScript.FromMonoBehaviour(actorSkin);
            actors = serializedObject.FindProperty("actors");
            hideInHierarchy = serializedObject.FindProperty("hideInHierarchy");

            UpdateActorNames();

            actorSkin.onTransformChildrenChanged -= UpdateActorNames;
            actorSkin.onTransformChildrenChanged += UpdateActorNames;
        }

        private void OnDisable()
        {
            actorSkin.onTransformChildrenChanged -= UpdateActorNames;
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();


            if(actorNames != null || actorNames.Length > 0) {
                actorSkin.current = EditorGUILayout.Popup("Select an Actor", actorSkin.current, actorNames);
                hideInHierarchy.boolValue = EditorGUILayout.Toggle(new GUIContent("HideInHierarchy: "), hideInHierarchy.boolValue);
                //if (GUILayout.Button(new GUIContent("Change Actor"), new GUIStyle(EditorStyles.popup))) { 
                //    actorSkin.ChangeActor(actorSkin.current);
                //}
            }
            else {
                EditorGUILayout.HelpBox("No Skinned Meshes.", MessageType.Warning);
            }


            serializedObject.ApplyModifiedProperties();
        }


        private void UpdateActorNames()
        {
            if (actorSkin == null) return;
            if (actorSkin.actors != null || actorSkin.actors.Length > 0) {
                actorNames = actorSkin.actors.Select(x => x.ToString()).ToArray();
            }
            else {
                actorNames = null;
            }
        }

    }

}
