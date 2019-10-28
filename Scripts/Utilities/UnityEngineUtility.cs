using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Globalization;


public static class UnityEngineUtility
{


    public static KeyCode ConvertStringToKeyCode(string inputString)
    {
        string input = inputString;
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        //  Convert string to titlecase.  (ThisIsTitleCase)
        input = textInfo.ToTitleCase(input);
        //  Remove whitespaces.
        input = Regex.Replace(input, @"\s+", "");

        KeyCode keycode;
        if(Enum.TryParse(input, out keycode)){
            if(Enum.IsDefined(typeof(KeyCode), keycode)) {
                return keycode;
                //keycode = (KeyCode)Enum.Parse(typeof(KeyCode), input);
            }
        }

        return KeyCode.None;
    }

}
