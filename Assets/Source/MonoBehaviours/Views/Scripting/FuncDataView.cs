

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FCopParser;

public class FuncDataView : MonoBehaviour {

    // - Unity Refs -
    public Toggle repeatToggle;
    public TMP_InputField repeatField;
    public TMP_InputField cooldownField;

    // - Parameters -
    public FCopFunction function;

    bool refuseCallback = false;
    void Start() {
        Refresh();
    }

    public void Refresh() {

        if (function == null) return;

        refuseCallback = true;

        repeatField.text = function.repeatCount.ToString();
        cooldownField.text = function.repeatTimer.ToString();

        repeatField.interactable = function.repeatCount != -1;
        repeatToggle.isOn = function.repeatCount != -1;

        refuseCallback = false;

    }

    public void OnStartType() {

        Main.ignoreAllInputs = true;

    }

    public void OnEndType() {

        Main.ignoreAllInputs = false;

    }

    public void OnCheckRepeat() {

        if (refuseCallback) return;

        if (repeatToggle.isOn) {
            function.repeatCount = 1;
            repeatField.interactable = true;
        }
        else {
            function.repeatCount = -1;
            repeatField.interactable = false;
        }

        repeatField.text = function.repeatCount.ToString();

    }

    public void OnFinishRepeatCount() {

        if (refuseCallback) return;

        try {

            var value = int.Parse(repeatField.text);

            function.repeatCount = value;

        }
        catch {

            repeatField.text = function.repeatCount.ToString();

        }

    }

    public void OnFinishCooldown() {

        if (refuseCallback) return;

        try {

            var value = int.Parse(cooldownField.text);

            function.repeatTimer = value;

        }
        catch {

            cooldownField.text = function.repeatTimer.ToString();

        }


    }

}