
using UnityEngine;

public class GeometryEditorUI: MonoBehaviour {

    public GeometryEditMode controller;

    void Start() {

        controller.view = this;


    }

    public void OnClickShiftHeightUpButton() {

        controller.ShiftTilesHeightUp();

    }

    public void OnClickShiftHeightDownButton() {

        controller.ShiftTilesHeightDown();

    }


}

