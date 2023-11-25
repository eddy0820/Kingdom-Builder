using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputSystemExensions
{
    public static string GetKeyCodeAsBindingPath(KeyCode keyCode)
    {
        string bindindPath = $"<Keyboard>/";
        string keyCodeString = keyCode.ToString().ToLower();

        if(keyCode >= KeyCode.Alpha0 && keyCode <= KeyCode.Alpha9)
            keyCodeString = keyCodeString.Replace("alpha", "");

        if(Enum.IsDefined(typeof(LowerCaseFirstWordKeyCodeEnum), keyCode))
            keyCodeString = keyCodeString.Replace(keyCodeString[0], char.ToLower(keyCodeString[0]));

        if(keyCode is KeyCode.Return)
            keyCodeString = "enter";

        if(keyCode >= KeyCode.Mouse0 && keyCode <= KeyCode.Mouse2)
            bindindPath = $"<Mouse>/";

        if(keyCode is KeyCode.Mouse0)
            keyCodeString = "leftButton";
        else if(keyCode is KeyCode.Mouse1)
            keyCodeString = "rightButton";
        else if(keyCode is KeyCode.Mouse2)
            keyCodeString = "middleButton";
        
        return bindindPath + keyCodeString;
    }

    public static KeyCode GetKeyCodeFromBindingPath(string bindingPath)
    {
        string[] bindingPathParts = bindingPath.Split('/');
        string keyCodeString = bindingPathParts[bindingPathParts.Length - 1];

        if(keyCodeString is "leftButton")
            return KeyCode.Mouse0;
        else if(keyCodeString is "rightButton")
            return KeyCode.Mouse1;
        else if(keyCodeString is "middleButton")
            return KeyCode.Mouse2;

        if(keyCodeString is "enter")
            keyCodeString = "return";

        if(Enum.TryParse(keyCodeString, true, out LowerCaseFirstWordKeyCodeEnum keyCode))
            return (KeyCode)keyCode;

        if(Enum.TryParse(keyCodeString, true, out KeyCode keyCode2))
            return keyCode2;

        Debug.LogError($"Could not parse {keyCodeString} to KeyCode");

        return KeyCode.None;
    }

    private enum LowerCaseFirstWordKeyCodeEnum
    {
        LeftAlt = 308,
        RightAlt = 307,
        DownArrow = 274,
        LeftArrow = 276,
        RightArrow = 275,
        UpArrow = 273,
        Backspace = 8,
        LeftCtrl = 306,
        RightCtrl = 305,
        Escape = 27,
        F1 = 282,
        F2 = 283,
        F3 = 284,
        F4 = 285,
        F5 = 286,
        F6 = 287,
        F7 = 288,
        F8 = 289,
        F9 = 290,
        F10 = 291,
        F11 = 292,
        F12 = 293,
        LeftShift = 304,
        RightShift = 303,
        Space = 32,
        Tab = 9,
    }
}
