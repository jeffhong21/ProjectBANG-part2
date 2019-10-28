namespace CharacterController
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Animations;

    using System;

    [CustomPropertyDrawer(typeof(AnimatorStateData))]
    public class AnimatorStateDataPropertyDrawer : PropertyDrawer
    {

        //private float GetIndentOffset(Rect rect)
        //{
        //    float width = rect.width - EditorGUI.IndentedRect(rect).width;
        //    return width;
        //}

        //private float GetIndentOffset(float width)
        //{
        //    int indentLevel = EditorGUI.indentLevel;
        //    float positionOffset = EditorGUI.indentLevel > 0 ? width * (width / (width * indentLevel)) * indentLevel : indentLevel;

        //    return positionOffset;
        //}
        SerializedProperty stateName;
        SerializedProperty transitionDuration;
        SerializedProperty speedMultiplier;
        SerializedProperty nameHash;
        Animator animator;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            EditorGUI.BeginProperty(position, label, property);

            Rect rect = position;
            animator = GetAnimatorController(property);
            stateName = property.FindPropertyRelative("stateName");
            transitionDuration = property.FindPropertyRelative("transitionDuration");
            speedMultiplier = property.FindPropertyRelative("speedMultiplier");
            nameHash = property.FindPropertyRelative("nameHash");


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(stateName);

            if(animator)
            {
                if(string.IsNullOrEmpty(stateName.stringValue) == false)
                {
                    nameHash.intValue = Animator.StringToHash(stateName.stringValue);
                    bool stateExist = false;
                    for (int index = 0; index < animator.layerCount; index++)
                    {
                        if (animator.HasState(index, nameHash.intValue))
                        {
                            stateExist = true;
                        }
                    }
                    if (stateExist == false)
                    {
                        EditorGUILayout.HelpBox(string.Format("{0} does not exist.>", stateName.stringValue), MessageType.Error);
                        nameHash.intValue = 0;
                    }
                }

            }
            else{
                EditorGUILayout.HelpBox("Missing Aniamtor Component.", MessageType.Warning);
            }

                

            EditorGUILayout.PropertyField(transitionDuration);
            EditorGUILayout.PropertyField(speedMultiplier);
            EditorGUILayout.PropertyField(nameHash);

            EditorGUILayout.EndVertical();
            //EditorGUI.PropertyField(rect, transitionDuration);
            //EditorGUI.PropertyField(rect, speedMultiplier);
            //EditorGUI.PropertyField(rect, nameHash);
            //EditorGUILayout.LabelField(property.displayName, EditorStyles.boldLabel);



            EditorGUI.EndProperty();
        }







        private Animator GetAnimatorController(SerializedProperty property)
        {
            var component = property.serializedObject.targetObject as Component;

            if (component == null){
                Debug.LogException(new InvalidCastException("Couldn't cast targetObject"));
            }

            var anim = component.GetComponent<Animator>();

            if (anim == null){
                var exception = new MissingComponentException("Missing Aniamtor Component");
                Debug.LogException(exception);
                return null;
            }

            return anim;
        }
    }

}
