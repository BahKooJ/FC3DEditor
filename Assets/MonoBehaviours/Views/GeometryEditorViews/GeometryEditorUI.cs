
using FCopParser;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GeometryEditorUI: MonoBehaviour {

    public GeometryEditMode controller;

    public GameObject rotateLeftButton;
    public GameObject rotateRightButton;
    public GameObject shiftHeightUpButton;
    public GameObject shiftHeightDownButton;
    public GameObject graphicsPropertiesButton;
    public GameObject showTilesButton;

    public GameObject graphicsPropertiesView;

    public GameObject activeGraphicsPropertiesView = null;

    void Start() {

        controller.view = this;

        foreach (Object obj in transform) {

            switch (obj.GameObject().name) {

                case "Rotate Left Button":
                    rotateLeftButton = obj.GameObject();
                    break;
                case "Rotate Right Button":
                    rotateRightButton = obj.GameObject();
                    break;
                case "Shift Height Up Button":
                    shiftHeightUpButton = obj.GameObject();
                    break;
                case "Shift Height Down Button":
                    shiftHeightDownButton = obj.GameObject();
                    break;
                case "Graphics Properties Button":
                    graphicsPropertiesButton = obj.GameObject();
                    break;
                case "Show Tiles Button":
                    showTilesButton = obj.GameObject();
                    break;

            }

        }

    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.M)) {

            if (activeGraphicsPropertiesView != null) {
                CloseGraphicsPropertyView();
            }

        }

    }

    public void OnClickGraphicsProperitesButton() {

        if (controller.selectedTiles.Count == 0) { return; }

        if (activeGraphicsPropertiesView != null) {
            CloseGraphicsPropertyView();
        } else {

            activeGraphicsPropertiesView = Instantiate(graphicsPropertiesView);

            activeGraphicsPropertiesView.GetComponent<GraphicsPropertiesView>().controller = controller;

            activeGraphicsPropertiesView.transform.SetParent(transform.parent, false);

        }

    }

    public void OnClickRotateLeftButton() {

        controller.RotateTileLeft();

    }

    public void OnClickRotateRightButton() {

        controller.RotateTileRight();

    }

    public void OnClickShiftHeightUpButton() {

        controller.ShiftTilesHeightUp();

    }

    public void OnClickShiftHeightDownButton() {

        controller.ShiftTilesHeightDown();

    }

    void CloseGraphicsPropertyView() {
        Destroy(activeGraphicsPropertiesView);
        controller.selectedSection.RefreshMesh();
    }

    public void OnClickDuplicateTileGraphicsButton() {

        controller.DuplicateTileGraphics();

    }

}

