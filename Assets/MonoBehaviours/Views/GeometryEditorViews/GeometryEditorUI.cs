
using UnityEngine;
using UnityEngine.UI;

public class GeometryEditorUI: MonoBehaviour {

    // View Refs
    public Image keepOnTopToggleImage;

    //Prefabs
    public GameObject debugTilePanel;

    public GeometryEditMode controller;

    public GameObject debugTilePanelView = null;

    void Start() {

        controller.view = this;

        if (Main.debug) {

            debugTilePanelView = Instantiate(debugTilePanel);

            debugTilePanelView.GetComponent<DebugTilePanelView>().controller = controller;

            debugTilePanelView.transform.SetParent(transform.parent, false);

        }

        ChangeToggleColor(keepOnTopToggleImage, GeometryEditMode.keepHeightsOnTop);

    }

    void ChangeToggleColor(Image image, bool toggle) {

        if (toggle) {
            image.color = Color.white;

        }
        else {
            image.color = new Color(0.3137255f, 0.3137255f, 0.3137255f);

        }

    }

    public void OnClickShiftHeightUpButton() {

        controller.ShiftTilesHeightUp();

    }

    public void OnClickShiftHeightDownButton() {

        controller.ShiftTilesHeightDown();

    }

    public void OnClickKeepVerticiesOnTopToggle() {

        GeometryEditMode.keepHeightsOnTop = !GeometryEditMode.keepHeightsOnTop;

        ChangeToggleColor(keepOnTopToggleImage, GeometryEditMode.keepHeightsOnTop);

    }


}

