

using FCopParser;
using UnityEngine;

public class SectionEditMode : EditMode {
    public Main main { get; set; }

    public LevelMesh selectedSection = null;
    FCopLevelSection copySection = null;
    public GameObject selectedSectionOverlay = null;

    public SectionEditMode(Main main) {
        this.main = main;
    }

    public void LookTile(Tile tile, TileColumn column, LevelMesh section) {
        
    }

    public void OnCreateMode() {
        
    }

    public void OnDestroy() {

        if (selectedSectionOverlay != null) {
            Object.Destroy(selectedSectionOverlay);
            selectedSectionOverlay = null;
        }

    }

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section) {

        selectedSection = section;

        if (selectedSectionOverlay != null) {
            Object.Destroy(selectedSectionOverlay);
        }

        selectedSectionOverlay = Object.Instantiate(main.SectionBoarders, new Vector3(section.x, 0, -section.y), Quaternion.identity);

    }

    public void Update() {

        if (FreeMove.looking) {
            main.TestRayOnLevelMesh();
        }

    }

    public void CopySectionData() {

        if (selectedSection != null) {
            copySection = selectedSection.section;
        }

    }

    public void PasteSectionData() {

        DialogWindowUtil.Dialog("Warning", "This will overwrite all current map data, are you sure you want to continue?", () => {

            if (selectedSection != null && copySection != null) {
                selectedSection.section.Overwrite(copySection);
                selectedSection.RefreshMesh();
            }

            return true;
        });

    }

    public void MirrorSectionVertically() {

        DialogWindowUtil.Dialog("Warning",
            "Some tiles cannot be mirrored correct and may be deleted. This will overwrite all current map data, are you sure you want to continue?",
            () => {

                if (selectedSection != null) {
                    selectedSection.section.MirrorVertically();
                    selectedSection.RefreshMesh();
                }

                return true;

            });

    }

    public void MirrorSectionHorizontally() {

        DialogWindowUtil.Dialog("Warning",
            "Some tiles cannot be mirrored correct and may be deleted. This will overwrite all current map data, are you sure you want to continue?",
            () => {

                if (selectedSection != null) {
                    selectedSection.section.MirrorHorizontally();
                    selectedSection.RefreshMesh();
                }

                return true;

            });

    }

    public void MirrorSectionDiagonally() {

        DialogWindowUtil.Dialog("Warning",
            "Some tiles cannot be mirrored correct and may be deleted. This will overwrite all current map data, are you sure you want to continue?",
            () => {

                if (selectedSection != null) {
                    selectedSection.section.MirrorDiagonally();
                    selectedSection.RefreshMesh();
                }

                return true;

            });

    }

    public void RemoveShadersFromSection() {

        DialogWindowUtil.Dialog("Warning",
        "This will remove all tile shader data, are you sure you want to continue?",
        () => {

        if (selectedSection != null) {

                foreach (var column in selectedSection.section.tileColumns) {

                    foreach (var tile in column.tiles) {
                        tile.ChangeShader(VertexColorType.MonoChrome);
                    }

                }

                selectedSection.RefreshMesh();
            }

            return true;

        });


    }

}