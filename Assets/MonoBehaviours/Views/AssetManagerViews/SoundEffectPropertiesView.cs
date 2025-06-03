

using FCopParser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SoundEffectPropertiesView : MonoBehaviour {

    // - Unity Refs -
    public TMP_Text dataIDText;
    public TMP_Text nameText;

    public TMP_InputField groupIDInput;
    public TMP_InputField scriptingIDInput;
    public Toggle doesLoopToggle;

    // - Parameters -
    public FCopAudio audioAsset;

    bool refuseCallback = false;
    private void Start() {

        refuseCallback = true;

        dataIDText.text = "Data ID: " + audioAsset.DataID.ToString();
        nameText.text = audioAsset.name;

        groupIDInput.text = audioAsset.groupID.ToString();
        scriptingIDInput.text = audioAsset.scriptingID.ToString();
        doesLoopToggle.isOn = audioAsset.isLoop;

        refuseCallback = false;

    }

    public void OnStartType() {
        Main.ignoreAllInputs = true;
    }

    public void OnStopType() {
        Main.ignoreAllInputs = false;
    }

    public void OnFinishGroupID() {

        if (refuseCallback) return;

        try {

            var value = int.Parse(groupIDInput.text);

            if (value < 0) {
                value = 0;
            }
            if (value > 1024) {
                value = 1024;
            }

            audioAsset.groupID = value;

        }
        catch { }

        groupIDInput.text = audioAsset.groupID.ToString();

    }

    public void OnFinshScriptingID() {

        if (refuseCallback) return;

        try {

            var value = int.Parse(scriptingIDInput.text);

            if (value < 0) {
                value = 0;
            }
            if (value > 1024) {
                value = 1024;
            }

            audioAsset.scriptingID = value;

        }
        catch { }

        scriptingIDInput.text = audioAsset.scriptingID.ToString();

    }

    public void OnToggleLoop() {

        if (refuseCallback) return;

        audioAsset.isLoop = doesLoopToggle.isOn;

    }

}