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

    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            OnClickSaveSchematic();
        }

    }

    // Editing
    public void OnClickShiftHeightUpButton() {

        controller.ShiftTilesHeightUp();

    }

    public void OnClickShiftHeightDownButton() {

        controller.ShiftTilesHeightDown();

    }

    public void OnClickSaveSchematic() {

        var foo = new Schematic(controller.selectedItems);

        foo.width = 2;

    }


}