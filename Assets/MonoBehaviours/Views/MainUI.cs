
using Unity.VisualScripting;
using UnityEngine;

class MainUI: MonoBehaviour {

    public Main controller;

    public GameObject rotateLeftButton;
    public GameObject rotateRightButton;
    public GameObject shiftHeightUpButton;
    public GameObject shiftHeightDownButton;
    public GameObject addTilePresetButton;
    public GameObject tilePresets;
    public GameObject graphicsPropertiesButton;
    public GameObject showTilesButton;

    public GameObject graphicsPropertiesView;

    public GameObject activeGraphicsPropertiesView = null;

    void Start() {

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
                case "Add Tile Preset Button":
                    addTilePresetButton = obj.GameObject();
                    break;
                case "Tile Presets":
                    tilePresets = obj.GameObject();
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

    public void OnClickGraphicsProperitesButton() {

        if (controller.selectedTile == null) { return; }

        if (activeGraphicsPropertiesView != null) {
            Destroy(activeGraphicsPropertiesView);
        } else {

            activeGraphicsPropertiesView = Instantiate(graphicsPropertiesView);

            activeGraphicsPropertiesView.GetComponent<GraphicsPropertiesView>().controller = controller;

            activeGraphicsPropertiesView.transform.SetParent(transform.parent, false);

        }

    }

}

