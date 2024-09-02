

using FCopParser;
using TMPro;
using UnityEngine;

public class SoundEffectPropertiesView : MonoBehaviour {

    // - Unity Refs -
    public TMP_Text dataIDText;
    public TMP_Text groupIDText;
    public TMP_Text nameText;
    public TMP_Text scriptingIDText;
    public TMP_Text doesLoopText;

    // - Parameters -
    public FCopAudio audioAsset;

    private void Start() {
        
        dataIDText.text = "Data ID: " + audioAsset.DataID.ToString();
        groupIDText.text = "Group ID: " + audioAsset.groupID.ToString();
        nameText.text = audioAsset.name;
        scriptingIDText.text = "Scripting ID: " + audioAsset.scriptingID.ToString();
        doesLoopText.text = "Does Loop: " + audioAsset.isLoop.ToString();

    }

}