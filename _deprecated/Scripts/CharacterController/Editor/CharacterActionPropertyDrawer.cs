namespace CharacterController
{
    using UnityEngine;
    using UnityEditor;


    [CustomPropertyDrawer(typeof(CharacterAction))]
    public class CharacterActionPropertyDrawer : PropertyDrawer
    {
        readonly bool k_drawDefault = false;

        
        SerializedObject serializedObject;
        SerializedProperty element;
        SerializedProperty stateName;
        SerializedProperty isActive;
        SerializedProperty priority;
        SerializedProperty actionID;
        SerializedProperty m_startType;
        SerializedProperty m_stopType;

        GUIStyle m_DefaultActionTextStyle = new GUIStyle();
        GUIStyle m_ActiveActionTextStyle = new GUIStyle();

        Color activeActiopn = new Color(0, 1, 0, 0.25f);
        Color inactiveAction = new Color(0, 0, 1, 0.5f);

        float lineHeight;
        Rect rect;
        Rect enabledRect, labelRect, stateRect, startTypeRect, stopTypeRect, inputRect = new Rect();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (k_drawDefault) {
                base.OnGUI(position, property, label);
                return;
            }

            element = property;
            lineHeight = EditorGUIUtility.singleLineHeight;
            rect = position;

            EditorGUI.BeginProperty(position, label, element);
            {
                serializedObject = new SerializedObject(element.objectReferenceValue);
                if (serializedObject != null)
                {
                    CharacterAction action = (CharacterAction)element.objectReferenceValue;
                    isActive = serializedObject.FindProperty("m_isActive");
                    actionID = serializedObject.FindProperty("m_actionID");
                    m_startType = serializedObject.FindProperty("m_startType");
                    m_stopType = serializedObject.FindProperty("m_stopType");

                    var allocWidth = (rect.width - 20) / 5;
                    var enumWidth = Mathf.Clamp(allocWidth, 75, float.MaxValue);
                    rect.y += 2;
                    enabledRect = new Rect(rect.x, rect.y, 20, lineHeight);
                    labelRect = new Rect(rect.x + 20, rect.y, allocWidth + 20, lineHeight);
                    //stateRect = new Rect(rect.x + allocWidth + 15, rect.y, allocWidth, lineHeight);
                    startTypeRect = new Rect( (rect.x + enumWidth) + 36, rect.y, enumWidth, lineHeight);
                    stopTypeRect = new Rect( startTypeRect.x + startTypeRect.width - 4, rect.y, enumWidth, lineHeight);
                    inputRect = new Rect( stopTypeRect.x + stopTypeRect.width - 4, rect.y, allocWidth, lineHeight);

                    if (Application.isPlaying) {
                        if (action.IsActive) {
                            EditorGUI.DrawRect(position, activeActiopn);
                        }
                        //else EditorGUI.DrawRect(position, inactiveAction);
                    }

                    //  Enable toggle
                    action.enabled = EditorGUI.Toggle(enabledRect, action.enabled);
                    //  Action name.
                    EditorGUI.LabelField(labelRect, action.GetType().Name, action.IsActive? m_ActiveActionTextStyle : m_DefaultActionTextStyle);
                    //  StateName

                    //  StartType
                    action.StartType = (ActionStartType)EditorGUI.EnumPopup(startTypeRect, action.StartType);
                    //  StopType
                    action.StopType = (ActionStopType)EditorGUI.EnumPopup(stopTypeRect, action.StopType);
                    //  Input
                    EditorGUI.BeginDisabledGroup( !IsStartStopInput(action) );
                    var m_inputKey = serializedObject.FindProperty("m_inputKey");
                    EditorGUI.PropertyField(inputRect, m_inputKey, GUIContent.none);
                    EditorGUI.EndDisabledGroup();


                    //stateName.stringValue = EditorGUI.TextField(rect, stateName.stringValue);





                    //  Action name.
                    //rect.width = position.width * 0.40f;
                    //if (action.IsActive) {
                    //    EditorGUI.LabelField(rect, string.Format("{0} ({1})", action.GetType().Name, "Active"), m_ActiveActionTextStyle);
                    //}
                    //else {
                    //    EditorGUI.LabelField(rect, action.GetType().Name, m_DefaultActionTextStyle);
                    //}


                    ////  Action state name.
                    //rect.x += position.width * 0.40f;
                    //rect.width = position.width * 0.36f;
                    //stateName.stringValue = EditorGUI.TextField(rect, stateName.stringValue);
                    ////  Action ID
                    ////rect.x = position.width * 0.85f;
                    //rect.x = position.width - 36;
                    //rect.width = 36;
                    //actionID.intValue = EditorGUI.IntField(rect, actionID.intValue);

                    ////  Toggle Enable
                    //rect.x = position.width + 12;
                    //rect.width = 36;
                    //action.enabled = EditorGUI.Toggle(rect, action.enabled);
                    ////isActive.boolValue = EditorGUI.Toggle(rect, isActive.boolValue);
                    ////isActive.boolValue = action.enabled;

                    serializedObject.ApplyModifiedProperties();




                }

            }



            EditorGUI.EndProperty();
        }



        private bool IsStartStopInput(CharacterAction action)
        {
            return action.StartType == ActionStartType.ButtonDown ||
                   action.StartType == ActionStartType.DoublePress ||
                   action.StopType == ActionStopType.ButtonUp ||
                   action.StopType == ActionStopType.ButtonToggle;
        }



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

    }

}
