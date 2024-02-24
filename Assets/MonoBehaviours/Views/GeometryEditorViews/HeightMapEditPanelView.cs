
using UnityEngine;
using UnityEngine.UI;

public class HeightMapEditPanelView: MonoBehaviour {

    // View Refs
    public Image keepOnTopToggleImage;

    public HeightMapEditMode controller;


    void Start() {

        controller.view = this;

        ChangeToggleColor(keepOnTopToggleImage, HeightMapEditMode.keepHeightsOnTop);

    }

    void ChangeToggleColor(Image image, bool toggle) {

        if (toggle) {
            image.color = Color.white;

        }
        else {
            image.color = new Color(0.3137255f, 0.3137255f, 0.3137255f);

        }

    }

    public void OnClickKeepVerticiesOnTopToggle() {

        HeightMapEditMode.keepHeightsOnTop = !HeightMapEditMode.keepHeightsOnTop;

        ChangeToggleColor(keepOnTopToggleImage, HeightMapEditMode.keepHeightsOnTop);

    }


}

