
using System;
using System.Linq;
using TMPro;
using UnityEngine;

class ControlRebindItem : MonoBehaviour {

    public string controlStringKey;
    public TMP_Text keyText;

    bool listen = false;

    void Start() {

        if (SettingsManager.keyBinds.Count == 0) {
            SettingsManager.ParseSettings();
        }

        SetKeyText();

    }

    void Update() {

        if (listen) {

            if (Input.anyKeyDown) {

                var key = TestKey();
                var mouse = TestMouse();

                if (mouse != null) {
                    SettingsManager.keyBinds[controlStringKey] = "#" + mouse.ToString();
                    FinishListening();
                }
                else if (key != null) {
                    SettingsManager.keyBinds[controlStringKey] = key.ToString();
                    FinishListening();
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

        if (SettingsManager.keyBinds[controlStringKey][0] == '#') {
            keyText.text = MouseName(SettingsManager.keyBinds[controlStringKey][1]);
        } else {
            keyText.text = SettingsManager.keyBinds[controlStringKey];
        }

    }

    string MouseName(char button) {
        return "Mouse " + button;
    }

    public void OnClickListen() {
        listen = true;
        keyText.text = "Press key...";
    }

}