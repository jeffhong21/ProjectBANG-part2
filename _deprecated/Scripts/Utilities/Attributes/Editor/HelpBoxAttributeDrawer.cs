using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
public class HelpBoxAttributeDrawer : DecoratorDrawer
{
    //HelpBoxAttribute helpBoxAttribute
    //{
    //    get { return (HelpBoxAttribute)attribute; }
    //}

    private readonly int heightOffset = 4;

    public override float GetHeight()
    {
        if (!(attribute is HelpBoxAttribute helpBoxAttribute))
            return base.GetHeight();
        GUIStyle helpBoxStyle = (GUI.skin != null) ? GUI.skin.GetStyle("helpbox") : null;
        if (helpBoxStyle == null)
            return base.GetHeight();
        return Mathf.Max(40f, helpBoxStyle.CalcHeight(new GUIContent(helpBoxAttribute.text), EditorGUIUtility.currentViewWidth) + 4);
    }

    public override void OnGUI(Rect position)
    {
        if (!(attribute is HelpBoxAttribute helpBoxAttribute)) return;

        position.y = position.y + heightOffset;
        position.height = position.height - (2 * heightOffset);
        EditorGUI.HelpBox(position, helpBoxAttribute.text, GetMessageType(helpBoxAttribute.messageType));
    }


    private MessageType GetMessageType(HelpBoxMessageType helpBoxMessageType)
    {
        switch (helpBoxMessageType)
        {
            case HelpBoxMessageType.Info: return MessageType.Info;
            case HelpBoxMessageType.Warning: return MessageType.Warning;
            case HelpBoxMessageType.Error: return MessageType.Error;
            default: return MessageType.None;
        }
    }
}