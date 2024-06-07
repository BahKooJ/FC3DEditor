

using UnityEngine;
using UnityEngine.UI;

public class EscMenuViewSettingsView : MonoBehaviour {

    // - View Refs -
    public Toggle showShaderToggle;
    public Toggle showTransparencyToggle;
    public Toggle showAnimationsToggle;

    // - Parameters -
    public Main main;

    bool refuseCallback = true;

    void OnEnable() {

        refuseCallback = true;

        showShaderToggle.isOn = SettingsManager.showShaders;
        showTransparencyToggle.isOn = SettingsManager.showTransparency;
        showAnimationsToggle.isOn = SettingsManager.showAnimations;

        refuseCallback = false;

    }

    public void OnToggleShowShaders() {

        if (refuseCallback) return;

        SettingsManager.showShaders = showShaderToggle.isOn;
        main.RefreshLevel();
    }

    public void OnToggleShowTransparency() {

        if (refuseCallback) return;

        SettingsManager.showTransparency = showTransparencyToggle.isOn;
        SettingsManager.clipBlack = showTransparencyToggle.isOn;
        main.RefreshLevel();

    }

    public void OnToggleShowAnimations() {

        if (refuseCallback) return;

        SettingsManager.showAnimations = showAnimationsToggle.isOn;
        main.RefreshLevel();

    }

}