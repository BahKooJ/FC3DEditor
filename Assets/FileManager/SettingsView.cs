
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour {

    static float width = 1260f;
    static float height = 600f;

    // View refs
    public TMP_Dropdown renderTypeDropdown;
    public Slider mouseSensSlider;
    public TMP_InputField mouseSensInput;
    public Slider fovSlider;
    public TMP_InputField fovInput;
    public Slider uiScaleSlider;
    public TMP_InputField uiScaleInput;

    public FileManagerMain main;

    bool refuseCallbacks = false;

    void Start() {
        refuseCallbacks = true;
        renderTypeDropdown.value = (int)SettingsManager.renderMode;
        mouseSensSlider.value = SettingsManager.mouseSensitivity;
        mouseSensInput.text = SettingsManager.mouseSensitivity.ToString();
        fovSlider.value = SettingsManager.fov;
        fovInput.text = SettingsManager.fov.ToString();
        uiScaleSlider.value = SettingsManager.uiScale;
        uiScaleInput.text = SettingsManager.uiScale.ToString();
        refuseCallbacks = false;

        Resize();

        preScreenWidth = Screen.width;
        preScreenHeight = Screen.height;


    }

    float preScreenWidth = 0f;
    float preScreenHeight = 0f;

    public void Update() {

        if (preScreenWidth != Screen.width || preScreenHeight != Screen.height) {
            Resize();
            preScreenWidth = Screen.width;
            preScreenHeight = Screen.height;
        }

    }

    void Resize() {

        if (main == null) {

            var widthScale = (Screen.width / Main.uiScaleFactor) / width;

            var rectTrans = (RectTransform)transform;

            transform.localScale = new Vector3(widthScale, widthScale, 1);
            rectTrans.sizeDelta = new Vector2(width, (Screen.height / Main.uiScaleFactor) / widthScale);

        }
        else {

            var widthScale = Screen.width / width;

            var rectTrans = (RectTransform)transform;

            transform.localScale = new Vector3(widthScale, widthScale, 1);
            rectTrans.sizeDelta = new Vector2(width, Screen.height / widthScale);

        }

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

    public void OnChangeUIScaleSlider() {

        if (refuseCallbacks) { return; }

        var sliderValue = uiScaleSlider.value;

        sliderValue = (float)Math.Round(sliderValue, 2);

        SettingsManager.uiScale = sliderValue;
        uiScaleInput.text = sliderValue.ToString();

    }

    public void OnFinishUIScaleType() {

        if (refuseCallbacks) { return; }

        try {

            var value = Single.Parse(uiScaleInput.text);

            SettingsManager.uiScale = value;
            uiScaleSlider.value = value;

        }
        catch {

            OnChangeUIScaleSlider();

        }

    }

}