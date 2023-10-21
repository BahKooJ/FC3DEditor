
using UnityEngine;

public class GeometryEditorUI: MonoBehaviour {

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

    }

    public void OnClickShiftHeightUpButton() {

        controller.ShiftTilesHeightUp();

    }

    public void OnClickShiftHeightDownButton() {

        controller.ShiftTilesHeightDown();

    }


}

