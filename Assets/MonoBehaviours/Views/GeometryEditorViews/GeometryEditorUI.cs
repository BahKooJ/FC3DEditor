
using FCopParser;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GeometryEditorUI: MonoBehaviour {

    public GeometryEditMode controller;

    public GameObject graphicsPropertiesView;

    public GameObject activeGraphicsPropertiesView = null;

    void Start() {

        controller.view = this;


    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {

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

    public void OnClickCopySectionDataButton() {
        controller.CopySectionData();
    }

    public void OnClickPasteSectionDataButton() {
        controller.PasteSectionData();
    }

    public void OnClickMirrorSectionVerticallyButton() {
        controller.MirrorSectionVertically();
    }

    public void OnClickMirrorSectionHorizontallyButton() {
        controller.MirrorSectionHorizontally();
    }

    public void OnClickMirrorSectionDiagonallyButton() {
        controller.MirrorSectionDiagonally();
    }



}

