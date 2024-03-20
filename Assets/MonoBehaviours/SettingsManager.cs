
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public abstract class SettingsManager {

    public static Dictionary<string, string> keyBinds = new();

    public static RenderType renderMode = RenderType.Smooth;
    public static bool showTransparency = true;
    public static bool clipBlack = true;

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

                    foreach (var i in Enumerable.Range(0,values.Count / 2)) {

                        var offset = i * 2;

                        keyBinds.Add(values[offset], values[offset + 1]);

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
            total += "\"" + binds.Key + "\":\"" + binds.Value + "\"\n";
        }

        total += "}\n";

        total += "RenderMode = \"" + renderMode.ToString() + "\";";

        File.WriteAllText("Settings.txt", total);

    }

}

public abstract class Controls {

    public static bool OnDown(string input) {

        try {

            var keyString = SettingsManager.keyBinds[input];

            if (keyString[0] == '#') {

                return Input.GetMouseButtonDown(Int32.Parse(keyString[1].ToString()));

            }
            else {

                var keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), keyString, true);

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

            var keyString = SettingsManager.keyBinds[input];

            if (keyString[0] == '#') {

                return Input.GetMouseButtonUp(Int32.Parse(keyString[1].ToString()));

            }
            else {

                var keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), keyString, true);

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

            var keyString = SettingsManager.keyBinds[input];

            if (keyString[0] == '#') {

                return Input.GetMouseButton(Int32.Parse(keyString[1].ToString()));

            }
            else {

                var keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), keyString, true);

                return Input.GetKey(keyCode);

            }

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