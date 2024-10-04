using FCopParser;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapOpenerView : MonoBehaviour {

    // - Unity Refs -
    public Slider widthSlider;
    public TMP_Text widthText;
    public Slider heightSlider;
    public TMP_Text heightText;

    public Image paOptionBackground;
    public Image emptyOptionBackground;
    public Image existingOptionBackground;

    // - Parameters -
    [HideInInspector]
    public FileManagerMain main;

    FCopLevel selectedLevel;

    void Start() {

    }

    void DisableOtherColors() {

        paOptionBackground.color = Main.mainColor;
        emptyOptionBackground.color = Main.mainColor;
        existingOptionBackground.color = Main.mainColor;

    }

    public void OnClickPAOption() {


        selectedLevel = new FCopLevel(File.ReadAllBytes("PA Template.ncfc"));

        DisableOtherColors();

        paOptionBackground.color = Main.selectedColor;

    }

    public void OnClickEmptyOption() {

        // Not supported yet
        return;

        selectedLevel = new FCopLevel(File.ReadAllBytes("Empty Template.ncfc"));

        DisableOtherColors();

        emptyOptionBackground.color = Main.selectedColor;

    }

    public void OnClickExistingOption() {



        OpenFileWindowUtil.OpenFile("MissionFiles", "", file => {

            var level = main.GetFile(file);

            if (level != null) {

                selectedLevel = level;

                DisableOtherColors();

                existingOptionBackground.color = Main.selectedColor;

            }

        });


    }

    public void OnWidthSliderChange() {

        widthText.text = widthSlider.value.ToString();

    }

    public void OnHeightSliderChange() {
        heightText.text = heightSlider.value.ToString();
    }

    public void OnClickOpen() {

        if (selectedLevel == null) {
            return;
        }

        var width = (int)widthSlider.value;
        var height = (int)heightSlider.value;

        selectedLevel.ClearLevelData(width, height);

        FileManagerMain.level = selectedLevel;

        SceneManager.LoadScene("Scenes/LevelEditorScene", LoadSceneMode.Single);

    }

    public void OnClickCanel() {
        Destroy(gameObject);
    }


}
