
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public abstract class SettingsManager {

    public static Dictionary<string, string[]> keyBinds = new();
    public static float mouseSensitivity = 3f;
    public static float fov = 90f;
    public static RenderType renderMode = RenderType.Smooth;
    public static bool showShaders = true;
    public static bool showTransparency = true;
    public static bool clipBlack = true;
    public static bool showAnimations = true;

    static public void ParseSettings() {

        var fileContents = File.ReadAllText("Settings.txt");

        string property = "";
        bool set = false;
        bool isDictionary = false;

        bool insideData = false;
        List<string> values = new();

        void CloseSet() {

            if (isDictionary) {

                if (property == "Inputs") {

                    var offset = 0;

                    while (offset < values.Count) {

                        var bindCount = Int32.Parse(values[offset + 1]);

                        keyBinds.Add(values[offset], values.GetRange(offset + 2, bindCount).ToArray());

                        offset += bindCount + 2;

                    }

                }

            } 
            else {

                if (property == "RenderMode") {

                    switch (values[0]) {
                        case "Smooth":
                            renderMode = RenderType.Smooth;
                            break;
                        case "Pixelated":
                            renderMode = RenderType.Pixelated;
                            break;
                    }

                }
                if (property == "MouseSensitivity") {
                    mouseSensitivity = Single.Parse(values[0]);
                }
                if (property == "FOV") {
                    fov = Single.Parse(values[0]);
                }

            }

            property = "";
            set = false;
            isDictionary = false;
            insideData = false;
            values.Clear();

        }

        foreach (var c in fileContents) {

            if (set) {

                if (c == '{') {
                    isDictionary = true;
                    continue;
                }

                if (c == '\"') {
                    insideData = !insideData;

                    if (insideData) {
                        values.Add("");
                    }

                    continue;
                }

                if (c == '}') {

                    CloseSet();
                    continue;
                }

                if (c == ';') {
                    CloseSet();
                    continue;
                }

                if (insideData) {

                    values[values.Count - 1] = values.Last() + c;

                    continue;
                }

                continue;
            }

            if (c == '=') {
                set = true;
                continue;
            }

            if (c != ' ' && c != '\n') {
                property += c;
            }

        }


    }

    static public void SaveToFile() {

        var total = "";

        total += "Inputs = { \n";

        foreach (var binds in keyBinds) {
            total += "\"" + binds.Key + "\":\"" + binds.Value.Count().ToString() + "\" ";

            foreach (var key in binds.Value) {
                total += "\"" + key + "\"";
            }

            total += "\n";

        }

        total += "}\n";

        total += "RenderMode = \"" + renderMode.ToString() + "\";\n";
        total += "MouseSensitivity = \"" + mouseSensitivity.ToString() + "\";\n";
        total += "FOV = \"" + fov.ToString() + "\";";

        File.WriteAllText("Settings.txt", total);

    }

}

public abstract class Controls {

    static bool IsSingleKeyDown(string input) {

        if (input[0] == '#') {

            return Input.GetMouseButton(Int32.Parse(input[1].ToString()));

        }
        else {

            var keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), input, true);

            return Input.GetKey(keyCode);

        }

    }

    public static bool OnDown(string input) {

        try {

            var keyStrings = SettingsManager.keyBinds[input];

            // Tests to see if the modifiers are held down (if applicable)
            if (keyStrings.Length > 1) {

                foreach (var key in keyStrings.SkipLast(1)) {

                    if (!IsSingleKeyDown(key)) {
                        return false;
                    }

                }

            }

            if (keyStrings.Last()[0] == '#') {

                return Input.GetMouseButtonDown(Int32.Parse(keyStrings.Last()[1].ToString()));

            }
            else {

                var keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), keyStrings.Last(), true);

                return Input.GetKeyDown(keyCode);

            }

        } 
        catch {
            Debug.Log("Invalid KeyBind: " + input);
            return false;
        }

    }

    public static bool OnUp(string input) {

        try {

            var keyStrings = SettingsManager.keyBinds[input];

            if (keyStrings.Length > 1) {
                Debug.Log("Modifiers will not work on OnUp! Input: " + input);
            }
            
            if (keyStrings.Last()[0] == '#') {

                return Input.GetMouseButtonUp(Int32.Parse(keyStrings.Last()[1].ToString()));

            }
            else {

                var keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), keyStrings.Last(), true);

                return Input.GetKeyUp(keyCode);

            }

        }
        catch {
            Debug.Log("Invalid KeyBind: " + input);
            return false;
        }

    }

    public static bool IsDown(string input) {

        try {

            var keyStrings = SettingsManager.keyBinds[input];

            foreach (var key in keyStrings) {

                if (!IsSingleKeyDown(key)) {
                    return false;
                }

            }
            return true;

        }
        catch {
            Debug.Log("Invalid KeyBind: " + input);
            return false;
        }

    }

}

public enum RenderType {

    Smooth = 0,
    Pixelated = 1

}