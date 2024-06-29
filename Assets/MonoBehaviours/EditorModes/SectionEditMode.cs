

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

    public void OnCreateMode() {
        
    }

    public void OnDestroy() {

        if (selectedSectionOverlay != null) {
            Object.Destroy(selectedSectionOverlay);
            selectedSectionOverlay = null;
        }

    }

    public void Update() {

        if (FreeMove.looking) {

            if (Controls.OnDown("Select")) {

                var selection = main.GetTileOnLevelMesh(!FreeMove.looking);

                if (selection != null) {
                    SelectSection(selection.section);
                }

            }

        }

        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            selectedSection.section.RotateClockwise();
            selectedSection.RefreshMesh();
        }

    }

    public void SelectSection(LevelMesh section) {

        selectedSection = section;

        if (selectedSectionOverlay != null) {
            Object.Destroy(selectedSectionOverlay);
        }

        selectedSectionOverlay = Object.Instantiate(main.SectionBoarders, new Vector3(section.x, 0, -section.y), Quaternion.identity);

        //var obj = Object.Instantiate(main.boundsPrefab);
        //obj.GetComponent<CullingSectionBounds>().controller = this;

        //LogCulling();

    }

    void LogCulling() {

        var total = "";

        total += selectedSection.section.culling.sectionCulling.radius.ToString() + " ";
        total += selectedSection.section.culling.sectionCulling.height.ToString() + "\n\n";

        foreach (var culling in selectedSection.section.culling.chunkCulling8) {
            total += culling.radius.ToString() + " ";
            total += culling.height.ToString() + "\n";
        }

        total += "\n";

        foreach (var culling in selectedSection.section.culling.chunkCulling4) {
            total += culling.radius.ToString() + " ";
            total += culling.height.ToString() + "\n";
        }

        total += "\n";

        selectedSection.section.culling.CalculateCulling(selectedSection.section);

        total += selectedSection.section.culling.sectionCulling.radius.ToString() + " ";
        total += selectedSection.section.culling.sectionCulling.height.ToString() + "\n\n";

        foreach (var culling in selectedSection.section.culling.chunkCulling8) {
            total += culling.radius.ToString() + " ";
            total += culling.height.ToString() + "\n";
        }

        total += "\n";

        foreach (var culling in selectedSection.section.culling.chunkCulling4) {
            total += culling.radius.ToString() + " ";
            total += culling.height.ToString() + "\n";
        }

        Debug.Log(total);


    }

    public void CopySectionData() {

        if (selectedSection != null) {
            copySection = selectedSection.section;
            QuickLogHandler.Log("Section copied to clipboard", LogSeverity.Success);
        } else {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);

        }

    }

    public void PasteSectionData() {

        if (selectedSection == null) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }
        if (copySection == null) {
            QuickLogHandler.Log("No section copied to clipboard!", LogSeverity.Error);
            return;
        }

        DialogWindowUtil.Dialog("Warning", "This will overwrite all current map data, are you sure you want to continue?", () => {

            if (selectedSection != null && copySection != null) {
                AddSectionSaveStateCounterAction();
                selectedSection.section.Overwrite(copySection);
                selectedSection.RefreshMesh();
            }

            return true;
        });

    }

    public void MirrorSectionVertically() {

        if (selectedSection == null) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }

        DialogWindowUtil.Dialog("Warning",
            "Some tiles cannot be mirrored correct and may be deleted. This will overwrite all current map data, are you sure you want to continue?",
            () => {

                if (selectedSection != null) {
                    AddSectionSaveStateCounterAction();
                    selectedSection.section.MirrorVertically();
                    selectedSection.RefreshMesh();
                }

                return true;

            });

    }

    public void MirrorSectionHorizontally() {

        if (selectedSection == null) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }

        DialogWindowUtil.Dialog("Warning",
            "Some tiles cannot be mirrored correct and may be deleted. This will overwrite all current map data, are you sure you want to continue?",
            () => {

                if (selectedSection != null) {
                    AddSectionSaveStateCounterAction();
                    selectedSection.section.MirrorHorizontally();
                    selectedSection.RefreshMesh();
                }

                return true;

            });

    }

    public void MirrorSectionDiagonally() {

        if (selectedSection == null) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }

        DialogWindowUtil.Dialog("Warning",
            "Some tiles cannot be mirrored correct and may be deleted. This will overwrite all current map data, are you sure you want to continue?",
            () => {

                if (selectedSection != null) {
                    AddSectionSaveStateCounterAction();
                    selectedSection.section.MirrorDiagonally();
                    selectedSection.RefreshMesh();
                }

                return true;

            });

    }

    public void RotateSectionClockwise() {

        if (selectedSection == null) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }

        DialogWindowUtil.Dialog("Warning",
            "Some tiles cannot be rotated correct and may be deleted. This will overwrite all current map data, are you sure you want to continue?",
            () => {

                if (selectedSection != null) {
                    AddSectionSaveStateCounterAction();
                    selectedSection.section.RotateClockwise();
                    selectedSection.RefreshMesh();
                }

                return true;

            });

    }

    public void RotateSectionCounterClockwise() {

        if (selectedSection == null) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }

        DialogWindowUtil.Dialog("Warning",
            "Some tiles cannot be rotated correct and may be deleted. This will overwrite all current map data, are you sure you want to continue?",
            () => {

                if (selectedSection != null) {
                    AddSectionSaveStateCounterAction();
                    selectedSection.section.RotateCounterClockwise();
                    selectedSection.RefreshMesh();
                }

                return true;

            });

    }

    public void RemoveShadersFromSection() {

        if (selectedSection == null) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }

        DialogWindowUtil.Dialog("Warning",
        "This will remove all tile shader data, are you sure you want to continue?",
        () => {

        if (selectedSection != null) {

                AddSectionSaveStateCounterAction();

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

    void AddSectionSaveStateCounterAction() {

        Main.AddCounterAction(new SectionSaveStateCounterAction(selectedSection));

    }

}