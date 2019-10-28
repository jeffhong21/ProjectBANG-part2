using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DebugUI
{
    public static class DebugUI
    {
        private const string seperator = "------------\n";
        private const int msgCapacity = 2000;
        private const RichTextColor defaultColor = RichTextColor.White;

        private static StringBuilder propertyMessages = new StringBuilder(2000);
        private static Dictionary<object, Dictionary<string, string>> propertyLogs;

        public static bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null;

        public struct DebugMessage
        {
            public string property;
            public string message;
            public RichTextColor propertyColor;
            public RichTextColor messageColor;
        }


        //public static void Log<T>(T value, string field, RichTextColor textColor) where T : class
        //{
        //    string fieldName = typeof(T).GetField(field).Name;
        //    object fieldValue = typeof(T).GetField(field).GetValue(null);
        //    Log(value, fieldName, fieldValue.ToString(), textColor, textColor);
        //}


        //
        //  Add seperators to the end of the message
        //
        public static void Log<T, W>(T value, W property, string label, RichTextColor textColor = default) where T : class
        {
            if(IsNullable(typeof(W)) )
                Log(value, label, property == null ? "null" : property.ToString(), textColor, textColor);
            else
                Log(value, label, property.ToString(), textColor, textColor);
        }

        public static void Log<T>(T value, string property, string label, RichTextColor textColor = default) where T : class
        {
            Log(value, label, property, textColor, textColor);
        }

        public static void Log<T, W>(T value, string property, W message, RichTextColor textColor = default) where T : class
        {
            Log(value, property, message.ToString(), textColor, textColor);
        }

        //public static void Log<T, W>(T value, string property, W message, RichTextColor propertyColor = default, RichTextColor messageColor = default) where T : class
        //{
        //    Log(value, property, message.ToString(), propertyColor, messageColor);
        //}




        /// <summary>
        /// Logs the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <param name="message"></param>
        /// <param name="propertyColor"></param>
        /// <param name="messageColor"></param>
        public static void Log<T>(T value, string property, string message, RichTextColor propertyColor = default, RichTextColor messageColor = default) where T : class
        {
            if (!DebugUIView.Instance.enabled)
                return;

            if (propertyLogs == null) propertyLogs = new Dictionary<object, Dictionary<string, string>>();
            //  Add the context object if not in the dictionary.
            if (!propertyLogs.ContainsKey(value))
            {
                propertyLogs.Add(value, new Dictionary<string, string>());
            }


            property = SetTextColor(property, (propertyColor == default) ? defaultColor : propertyColor);
            message = SetTextColor(message, (propertyColor == default) ? defaultColor : messageColor);



            //  If the context object already contains the property, update the value.
            if (propertyLogs[value].ContainsKey(property))
            {
                propertyLogs[value][property] = message;
            }
            //  If not add the value to the property.
            else
            {
                propertyLogs[value].Add(property, message);
            }
        }





        public static string WritePropertyMessages()
        {
            if (propertyMessages == null) propertyMessages = new StringBuilder(msgCapacity);
            propertyMessages.Clear();

            foreach (var log in propertyLogs)
            {
                propertyMessages.AppendFormat("<color={0}>-- {1} -- </color>\n",GetHexValue(defaultColor), SetTextBold(log.Key.GetType().Name) );

                foreach (var property in log.Value)
                {
                    propertyMessages.AppendFormat(" {0}: {1}\n", SetTextBold(property.Key), SetTextBold(property.Value));
                }
            }


            return propertyMessages.ToString();
        }
       



        public static void Remove<T>(T value, string property) where T : class
        {
            if (propertyLogs == null) return;

            if (propertyLogs.ContainsKey(value)){
                if (propertyLogs[value].ContainsKey(property))
                    propertyLogs[value].Remove(property);
            }
        }



        public static string SetTextBold(string text)
        {
            return "<b>" + text + "</b>";
        }


        public static string SetTextColor(string text, RichTextColor color = RichTextColor.Black)
        {
            return "<color=" + GetHexValue(color) + ">" + text + "</color>";
        }


        public static string GetHexValue(RichTextColor color)
        {
            switch (color)
            {
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