
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour {

    // View refs
    public TMP_Dropdown renderTypeDropdown;
    public Slider mouseSensSlider;
    public TMP_InputField mouseSensInput;

    public FileManagerMain main;

    bool refuseCallbacks = false;

    void Start() {
        refuseCallbacks = true;
        renderTypeDropdown.value = (int)SettingsManager.renderMode;
        mouseSensSlider.value = SettingsManager.mouseSensitivity;
        mouseSensInput.text = SettingsManager.mouseSensitivity.ToString();
        refuseCallbacks = false;
    }

    public void OnClickDone() {

        SettingsManager.SaveToFile();

        main.OpenHome();

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

}