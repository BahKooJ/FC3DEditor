
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour {

    // View refs
    public TMP_Dropdown renderTypeDropdown;
    public Slider mouseSensSlider;
    public TMP_InputField mouseSensInput;
    public Slider fovSlider;
    public TMP_InputField fovInput;

    public FileManagerMain main;

    bool refuseCallbacks = false;

    void Start() {
        refuseCallbacks = true;
        renderTypeDropdown.value = (int)SettingsManager.renderMode;
        mouseSensSlider.value = SettingsManager.mouseSensitivity;
        mouseSensInput.text = SettingsManager.mouseSensitivity.ToString();
        fovSlider.value = SettingsManager.fov;
        fovInput.text = SettingsManager.fov.ToString();
        refuseCallbacks = false;
    }

    public void OnClickDone() {

        SettingsManager.SaveToFile();

        if (main != null) {
            main.OpenHome();
        } else {
            Destroy(this.gameObject);
        }

    }

    public void OnChangeRenderType() {

        if (refuseCallbacks) { return; }

        SettingsManager.renderMode = (RenderType)renderTypeDropdown.value;

    }

    public void OnChangeMouseSlider() {

        if (refuseCallbacks) { return; }

        var sliderValue = mouseSensSlider.value;

        sliderValue = (float)Math.Round(sliderValue, 2);

        SettingsManager.mouseSensitivity = sliderValue;
        mouseSensInput.text = sliderValue.ToString();

    }

    public void OnFinishMouseType() {

        if (refuseCallbacks) { return; }

        try {

            var value = Single.Parse(mouseSensInput.text);

            SettingsManager.mouseSensitivity = value;
            mouseSensSlider.value = value;

        }
        catch {

            OnChangeMouseSlider();

        }

    }

    public void OnChangeFOVSlider() {

        if (refuseCallbacks) { return; }

        var sliderValue = fovSlider.value;

        sliderValue = (float)Math.Round(sliderValue, 2);

        SettingsManager.fov = sliderValue;
        fovInput.text = sliderValue.ToString();

    }

    public void OnFinishFOVType() {

        if (refuseCallbacks) { return; }

        try {

            var value = Single.Parse(fovInput.text);

            SettingsManager.fov = value;
            fovSlider.value = value;

        } catch {

            OnChangeFOVSlider();

        }

    }

}