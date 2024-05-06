
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

class ControlRebindItem : MonoBehaviour {

    public string controlStringKey;
    public TMP_Text keyText;

    bool listen = false;
    List<string> pressedKeys = new();

    void Start() {

        if (SettingsManager.keyBinds.Count == 0) {
            SettingsManager.ParseSettings();
        }

        SetKeyText();

    }

    void Update() {

        if (listen) {

            if (!Input.anyKey && pressedKeys.Count != 0) {

                SettingsManager.keyBinds[controlStringKey] = pressedKeys.ToArray();

                FinishListening();

            }

            if (Input.anyKeyDown) {

                var key = TestKey();
                var mouse = TestMouse();

                if (mouse != null) {

                    pressedKeys.Add("#" + mouse.ToString());

                }
                else if (key != null) {

                    pressedKeys.Add(key.ToString());

                }

            }

        }

    }

    // TODO: This is so bad but I can't find a better way of doing this
    KeyCode? TestKey() {

        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode))) {

            if (Input.GetKeyDown(kcode)) {
                return kcode;
            }

        }

        return null;

    }

    int? TestMouse() {

        foreach (var buttonNum in Enumerable.Range(0,6)) {

            if (Input.GetMouseButtonDown(buttonNum)) {
                return buttonNum;
            }

        }

        return null;

    }

    void FinishListening() {

        listen = false;
        SetKeyText();

    }

    void SetKeyText() {

        var total = "";

        foreach (var bind in SettingsManager.keyBinds[controlStringKey]) {

            if (bind[0] == '#') {
                total += MouseName(bind[1]);
            }
            else {
                total += bind;
            }

            total += " + ";

        }

        total = total.Remove(total.Length - 3, 3);

        keyText.text = total;

    }

    string MouseName(char button) {
        return "Mouse " + button;
    }

    public void OnClickListen() {
        listen = true;
        keyText.text = "Press key...";
    }

}