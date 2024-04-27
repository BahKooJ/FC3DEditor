using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileEditPanel : MonoBehaviour {

    public GameObject debugTilePanel;
    public TileEditMode controller;

    public GameObject debugTilePanelView = null;



    void Start() {
        controller.view = this;

        if (Main.debug) {

            debugTilePanelView = Instantiate(debugTilePanel);

            debugTilePanelView.GetComponent<ShaderDebug>().controller = controller;

            debugTilePanelView.transform.SetParent(transform.parent, false);

        }

    }

    // Editing
    public void OnClickShiftHeightUpButton() {

        controller.ShiftTilesHeightUp();

    }

    public void OnClickShiftHeightDownButton() {

        controller.ShiftTilesHeightDown();

    }


}