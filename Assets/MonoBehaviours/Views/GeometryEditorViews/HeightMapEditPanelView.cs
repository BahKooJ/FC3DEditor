
using UnityEngine;
using UnityEngine.UI;

public class HeightMapEditPanelView: MonoBehaviour {

    // View Refs
    public Image keepOnTopToggleImage;

    //Prefabs
    public GameObject debugTilePanel;

    public HeightMapEditMode controller;

    public GameObject debugTilePanelView = null;

    void Start() {

        controller.view = this;

        if (Main.debug) {

            debugTilePanelView = Instantiate(debugTilePanel);

            debugTilePanelView.GetComponent<DebugTilePanelView>().controller = controller;

            debugTilePanelView.transform.SetParent(transform.parent, false);

        }

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

