

using UnityEngine;
using UnityEngine.UI;

public class EscMenuViewSettingsView : MonoBehaviour {

    // - View Refs -
    public Toggle showShaderToggle;
    public Toggle showTransparencyToggle;
    public Toggle showAnimationsToggle;
    public Toggle renderDirectionalLight;
    public Slider lightDirectionX;
    public Slider lightDirectionY;

    // - Parameters -
    public Main main;

    bool refuseCallback = true;

    void OnEnable() {

        refuseCallback = true;

        showShaderToggle.isOn = SettingsManager.showShaders;
        showTransparencyToggle.isOn = SettingsManager.showTransparency;
        showAnimationsToggle.isOn = SettingsManager.showAnimations;
        renderDirectionalLight.isOn = SettingsManager.renderDirectionalLight;
        lightDirectionX.value = SettingsManager.lightDirectionX;
        lightDirectionY.value = SettingsManager.lightDirectionY;

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

    public void OnToggleRenderDirectionalLight() {

        if (refuseCallback) return;

        SettingsManager.renderDirectionalLight = renderDirectionalLight.isOn;
        main.ChangeRenderedLevelMeshes();


    }

    public void OnDirectionXSliderChange() {

        if (refuseCallback) return;

        SettingsManager.lightDirectionX = lightDirectionX.value;
        var angle = main.worldLight.transform.eulerAngles;
        angle.x = SettingsManager.lightDirectionX;
        main.worldLight.transform.eulerAngles = angle;


    }

    public void OnDirectionYSliderChange() {

        if (refuseCallback) return;

        SettingsManager.lightDirectionY = lightDirectionY.value;
        var angle = main.worldLight.transform.eulerAngles;
        angle.y = SettingsManager.lightDirectionY;
        main.worldLight.transform.eulerAngles = angle;

    }

}