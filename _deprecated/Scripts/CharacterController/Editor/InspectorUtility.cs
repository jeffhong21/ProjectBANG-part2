using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public static class InspectorUtility
{
    private static GUIStyle labelFieldStyle;
    private static GUIStyle labelItalicStyle;

    private static GUIStyle foldoutStyle;


    public static GUIStyle HeaderStyleBlue
    {
        get {
            return Style(new Color(0, 0.5f, 1f, 0.3f));
        }
    }

    public static GUIStyle Style(Color color, int offset = -2)
    {

        GUIStyle currentStyle = new GUIStyle(GUI.skin.box)
        {
            border = new RectOffset(offset, offset, offset, offset)
        };

        Color[] pix = new Color[1];
        pix[0] = color;
        Texture2D bg = new Texture2D(1, 1);
        bg.SetPixels(pix);
        bg.Apply();


        currentStyle.normal.background = bg;
        return currentStyle;
    }

    static InspectorUtility()
    {
        labelFieldStyle = new GUIStyle()
        {
            font = new GUIStyle(EditorStyles.label).font,
            fontStyle = FontStyle.Bold,
            fontSize = 11,
        };

        labelItalicStyle = new GUIStyle()
        {
            font = new GUIStyle(EditorStyles.label).font,
            fontStyle = FontStyle.Italic,
            fontSize = 11,
        };

        foldoutStyle = new GUIStyle("ShurikenModuleTitle") // ShurikenModuleTitle
        {
            font = new GUIStyle(EditorStyles.label).font,
            fontStyle = FontStyle.Normal,
            fontSize = 11,
            border = new RectOffset(15, 7, 4, 4),
            //fixedHeight = 22,
            contentOffset = new Vector2(20f, -2f),
        };
        //foldoutStyle = new GUIStyle(EditorStyles.foldoutHeader) // EditorStyles.foldoutHeader
        //{
        //    font = new GUIStyle(EditorStyles.label).font,
        //    fontStyle = FontStyle.Bold,
        //    fontSize = 11,
        //    border = new RectOffset(15, 7, 4, 4),
        //    //fixedHeight = 22,
        //    contentOffset = new Vector2(20f, -2f),
            
        //};

    }


    public static void LabelField(string text, int fontSize = 11, FontStyle fontStyle = FontStyle.Bold){
        labelFieldStyle.fontSize = fontSize;
        labelFieldStyle.fontStyle = fontStyle;
        EditorGUILayout.LabelField(text, labelFieldStyle);

    }

    public static void LabelField(GUIContent content, int fontSize = 11, FontStyle fontStyle = FontStyle.Bold)
    {
        labelFieldStyle.fontSize = fontSize;
        labelFieldStyle.fontStyle = fontStyle;
        EditorGUILayout.LabelField(content, labelFieldStyle);
    }

    public static bool PropertyField(SerializedProperty property, bool includeChildren = false)
    {
        if(property == null){
            EditorGUILayout.HelpBox("Property " + property.name + " does not exist", MessageType.Error);
            return false;
        }

        if(property.name == "m_Script")
        {
            ScriptPropertyField(property.serializedObject);
        }

        else if(property.isArray && includeChildren)
        {
            if(property.propertyType == SerializedPropertyType.String){
                EditorGUILayout.PropertyField(property, false);
                return true;
            }
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.displayName);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(property.FindPropertyRelative("Array.size"));
                for (int index = 0; index < property.arraySize; index++)
                {
                    EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(index));
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            return true;
        }
        else{
            EditorGUILayout.PropertyField(property, includeChildren);
        }
        return true;
    }


    public static void ScriptPropertyField(SerializedObject serializedObject, bool enable = false, int? bottomSpacing = null)
    {
        if(serializedObject != null)
        {
            EditorGUILayout.Space();
            GUI.enabled = enable;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            GUI.enabled = true;
            if(bottomSpacing != null) {
                float spacing = Mathf.Clamp(bottomSpacing.Value, 0, EditorGUIUtility.singleLineHeight * 4);
                spacing = Mathf.FloorToInt(spacing);
                GUILayout.Space(spacing);
            }
            else {
                GUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
            }

        }
        else {
            EditorGUILayout.HelpBox("SeriializedObject \"" + serializedObject + "\" is not valid.", MessageType.Error);
        }
    }


    public static bool Foldout(bool display, string title, int fontSize, FontStyle fontStyle = FontStyle.Normal)
    {
        foldoutStyle.fontSize = Mathf.Clamp(fontSize, 8, 24);
        foldoutStyle.fontStyle = fontStyle;

        var rect = GUILayoutUtility.GetRect(16f, 22f, foldoutStyle);
        GUI.Box(rect, title, foldoutStyle);

        var e = Event.current;

        var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
        if (e.type == EventType.Repaint)
        {
            EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
        }

        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            display = !display;
            e.Use();
        }
        EditorGUILayout.Space();
        return display;
    }


    public static bool Foldout(bool display, string title)
    {
        //EditorGUILayout.Space();
        var rect = GUILayoutUtility.GetRect(16f, 22f, foldoutStyle);
        GUI.Box(rect, title, foldoutStyle);

        var e = Event.current;

        var toggleRect = new Rect(rect.x + 4f, rect.y, 13f, 13f);
        if (e.type == EventType.Repaint) {
            EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
        }

        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            display = !display;
            e.Use();
        }
        //EditorGUILayout.Space();
        return display;
    }







    #region Reorderable List

    public static void DrawReorderableList(ReorderableList list)
    {
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 1.0f;
            rect.x += 10.0f;
            rect.width -= 10.0f;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, 0.0f), element, true);
        };


        list.drawHeaderCallback = (Rect rect) =>
        {
            //rect.x += 12;
            EditorGUI.LabelField(rect, string.Format("{0}: {1}", list.serializedProperty.displayName, list.serializedProperty.arraySize), EditorStyles.label);
        };

        list.elementHeightCallback = (int index) =>
        {
            return EditorGUI.GetPropertyHeight(list.serializedProperty.GetArrayElementAtIndex(index)) + 4.0f;
        };

        list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
        {
            l.serializedProperty.InsertArrayElementAtIndex(l.count);
        };


        list.onRemoveCallback = (ReorderableList l) =>
        {
            l.serializedProperty.DeleteArrayElementAtIndex(l.index);
        };
    }




    private static void DrawReorderableList(SerializedObject serializedObject, SerializedProperty property)
    {
        ReorderableList list = new ReorderableList(serializedObject, property, true, true, true, true)
        {
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = property.GetArrayElementAtIndex(index);
                rect.y += 1.0f;
                rect.x += 10.0f;
                rect.width -= 10.0f;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, 0.0f), element, true);
            },


            drawHeaderCallback = (Rect rect) =>
            {
                //rect.x += 12;
                EditorGUI.LabelField(rect, string.Format("{0}: {1}", property.displayName, property.arraySize), EditorStyles.label);
            },

            elementHeightCallback = (int index) =>
            {
                return EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(index)) + 4.0f;
            },

            onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
            {
                l.serializedProperty.InsertArrayElementAtIndex(l.index);
            },


            onRemoveCallback = (ReorderableList l) =>
            {
                l.serializedProperty.DeleteArrayElementAtIndex(l.index);
            }
        };

    }


    #endregion

}
