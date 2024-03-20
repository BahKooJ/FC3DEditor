
using TMPro;
using UnityEngine;

public class SettingsView : MonoBehaviour {

    // View refs
    public TMP_Dropdown renderTypeDropdown;

    public FileManagerMain main;

    void Start() {
        renderTypeDropdown.value = (int)SettingsManager.renderMode;
    }

    public void OnClickDone() {

        SettingsManager.SaveToFile();

        main.OpenHome();

    }

    public void OnChangeRenderType() {
        SettingsManager.renderMode = (RenderType)renderTypeDropdown.value;
    }

}