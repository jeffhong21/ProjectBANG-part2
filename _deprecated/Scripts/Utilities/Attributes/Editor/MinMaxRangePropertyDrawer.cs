using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
public class MinMaxRangePropertyDrawer : PropertyDrawer
{

    private float GetIndentOffset(Rect rect)
    {
        float width = rect.width - EditorGUI.IndentedRect(rect).width;
        return width;
    }

    private float GetIndentOffset(float width)
    {
        int indentLevel = EditorGUI.indentLevel;
        float positionOffset = EditorGUI.indentLevel > 0 ? width * (width / (width * indentLevel)) * indentLevel : indentLevel;

        return positionOffset;
    }




    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        int originalIndentLevel = EditorGUI.indentLevel;
        EditorGUI.BeginProperty(position, label, property);

        EditorGUILayout.Space();
        //EditorGUILayout.LabelField(property.displayName, EditorStyles.boldLabel);

        //  From NaughtyAttributes
        //MinMaxRangeAttribute minMaxSliderAttribute = PropertyUtility.GetAttribute<MinMaxRangeAttribute>(property);
        MinMaxRangeAttribute minMaxSliderAttribute = this.attribute as MinMaxRangeAttribute;


        if (property.propertyType == SerializedPropertyType.Vector2)
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            //  If indent level is zero, value is infinity.

            //EditorGUI.indentLevel = 0;
            int indentLevel = EditorGUI.indentLevel > 0 ? EditorGUI.indentLevel : 0;
            float labelWidth = EditorGUIUtility.labelWidth;
            float floatFieldWidth = EditorGUIUtility.fieldWidth;
            float positionOffset = indentLevel > 0 ? floatFieldWidth * (floatFieldWidth / (floatFieldWidth * indentLevel)) * indentLevel : indentLevel;
            float sliderWidth = controlRect.width - labelWidth -  (2 * floatFieldWidth) - indentLevel;
            float sliderPadding = 5f + indentLevel;

            Rect labelRect = new Rect(
                controlRect.x,
                controlRect.y,
                labelWidth - positionOffset,
                controlRect.height);

            Rect sliderRect = new Rect(
                controlRect.x + labelWidth + floatFieldWidth + sliderPadding - positionOffset,
                controlRect.y,
                sliderWidth - (2f * sliderPadding) + positionOffset,
                controlRect.height);
                
            Rect minFloatFieldRect = new Rect(
                controlRect.x + labelWidth - positionOffset,
                controlRect.y,
                floatFieldWidth + positionOffset - indentLevel,
                controlRect.height);

            Rect maxFloatFieldRect = new Rect(
                controlRect.x + labelWidth + floatFieldWidth + sliderWidth - positionOffset + indentLevel,
                controlRect.y,
                floatFieldWidth + positionOffset - indentLevel,
                controlRect.height);

            // Draw the label
            EditorGUI.LabelField(labelRect, property.displayName);
            //EditorGUI.LabelField(labelRect, string.Format("{0} | {1} | {2}", property.displayName, floatFieldWidth, floatFieldWidth + positionOffset) );

            // Draw the slider
            EditorGUI.BeginChangeCheck();

            Vector2 sliderValue = property.vector2Value;
            EditorGUI.MinMaxSlider(sliderRect, ref sliderValue.x, ref sliderValue.y, minMaxSliderAttribute.MinValue, minMaxSliderAttribute.MaxValue);

            sliderValue.x = EditorGUI.FloatField(minFloatFieldRect, sliderValue.x);
            sliderValue.x = Mathf.Clamp(sliderValue.x, minMaxSliderAttribute.MinValue, Mathf.Min(minMaxSliderAttribute.MaxValue, sliderValue.y));
            sliderValue.x = (float)Math.Round(sliderValue.x, minMaxSliderAttribute.Round);

            sliderValue.y = EditorGUI.FloatField(maxFloatFieldRect, sliderValue.y);
            sliderValue.y = Mathf.Clamp(sliderValue.y, Mathf.Max(minMaxSliderAttribute.MinValue, sliderValue.x), minMaxSliderAttribute.MaxValue);
            sliderValue.y = (float)Math.Round(sliderValue.y, minMaxSliderAttribute.Round);

            if (EditorGUI.EndChangeCheck())
            {
                property.vector2Value = sliderValue;
            }
        }
        else
        {
            string warning = minMaxSliderAttribute.GetType().Name + " can be used only on Vector2 fields";
            EditorGUILayout.HelpBox(warning, MessageType.Warning);
            EditorGUILayout.PropertyField(property, true);
        }

        EditorGUI.EndProperty();
        EditorGUI.indentLevel = originalIndentLevel;

    }


}
