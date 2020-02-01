using System;
using System.Collections.Generic;
using UnityEngine;

namespace JH.RootMotionController.RootMotionDebug
{
    using RootMotionUI;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Struct)]
    public class DebugDisplayAttribute : Attribute
    {
        public Color color { get; private set; }
        public int priority { get; private set; }

        public DebugDisplayAttribute(int priority = 0)
        {
            color = DebugHUD.defaultColor;
            this.priority = priority;
        }

        public DebugDisplayAttribute(RichTextColor displayColor)
        {
            if (ColorUtility.TryParseHtmlString(GetHexValue(displayColor), out Color c))
                color = c;
            else
                color = DebugHUD.defaultColor;

            this.priority = 0;
        }

        public DebugDisplayAttribute(RichTextColor displayColor, int priority = 0)
        {
            if (ColorUtility.TryParseHtmlString(GetHexValue(displayColor), out Color c))
                color = c;
            else
                color = DebugHUD.defaultColor;

            this.priority = priority;
        }



        public string GetHexValue(RichTextColor color)
        {
            switch (color) {
                case RichTextColor.Aqua:
                    return "#00ffffff";
                case RichTextColor.Black:
                    return "#000000ff";
                case RichTextColor.Blue:
                    return "#0000ffff";
                case RichTextColor.Brown:
                    return "#a52a2aff";
                case RichTextColor.Cyan:
                    return "#00ffffff";
                case RichTextColor.DarkBlue:
                    return "#0000a0ff";
                case RichTextColor.DarkGreen:
                    return "#008000ff";
                case RichTextColor.Magenta:
                    return "#ff00ffff";
                case RichTextColor.Green:
                    return "#00ff00ff";
                case RichTextColor.Grey:
                    return "#808080ff";
                case RichTextColor.LightBlue:
                    return "#add8e6ff";
                case RichTextColor.Lime:
                    return "#00ff00ff";
                case RichTextColor.Maroon:
                    return "#800000ff";
                case RichTextColor.Navy:
                    return "#000080ff";
                case RichTextColor.Olive:
                    return "#808000ff";
                case RichTextColor.Orange:
                    return "#ffa500ff";
                case RichTextColor.Purple:
                    return "#800080ff";
                case RichTextColor.Red:
                    return "#ff0000ff";
                case RichTextColor.Silver:
                    return "#c0c0c0ff";
                case RichTextColor.Teal:
                    return "#008080ff";
                case RichTextColor.White:
                    return "#ffffffff";
                case RichTextColor.Yellow:
                    return "#ffff00ff";
                default:
                    return "#000000ff";
            }
        }

    }

    public enum RichTextColor
    {
        Aqua, Black, Blue, Brown, Cyan, DarkBlue, DarkGreen, Magenta, Green, Grey, LightBlue, Lime,
        Maroon, Navy, Olive, Orange, Purple, Red, Silver, Teal, White, Yellow
    }
}
