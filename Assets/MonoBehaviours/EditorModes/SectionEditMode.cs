

using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class SectionEditMode : EditMode {
    public Main main { get; set; }

    public bool HasSelection {
        get => selectedSections.Count > 0;
    }

    public LevelMesh FirstItem {
        get => selectedSections[0];
    }

    FCopLevelSection copySection = null;

    public List<LevelMesh> selectedSections = new();
    public List<GameObject> selectedSectionOverlays = new();

    public SectionEditMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {
        
    }

    public void OnDestroy() {

        ClearOverlays();

    }

    public void Update() {

        if (Controls.OnDown("Select")) {

            var selection = main.GetTileOnLevelMesh(!FreeMove.looking);

            if (selection != null) {
                SelectSection(selection.section);
            }

        }

        if (Controls.OnDown("Unselect")) {

            ClearSelection();

        }

    }

    public void SelectSection(LevelMesh section) {

        if (!Controls.IsDown("MultiSelect")) {
            ClearSelection();
        }

        if (!selectedSections.Contains(section)) {

            selectedSections.Add(section);

            selectedSectionOverlays.Add(Object.Instantiate(main.SectionBoarders, new Vector3(section.x, 0, -section.y), Quaternion.identity));

        }

    }

    void ClearSelection() {

        selectedSections.Clear();

        ClearOverlays();

    }

    void ClearOverlays() {

        foreach (var obj in selectedSectionOverlays) {
            Object.Destroy(obj);
        }

        selectedSections.Clear();

    }

    //void LogCulling() {

    //    var total = "";

    //    total += selectedSection.section.culling.sectionCulling.radius.ToString() + " ";
    //    total += selectedSection.section.culling.sectionCulling.height.ToString() + "\n\n";

    //    foreach (var culling in selectedSection.section.culling.chunkCulling8) {
    //        total += culling.radius.ToString() + " ";
    //        total += culling.height.ToString() + "\n";
    //    }

    //    total += "\n";

    //    foreach (var culling in selectedSection.section.culling.chunkCulling4) {
    //        total += culling.radius.ToString() + " ";
    //        total += culling.height.ToString() + "\n";
    //    }

    //    total += "\n";

    //    selectedSection.section.culling.CalculateCulling(selectedSection.section);

    //    total += selectedSection.section.culling.sectionCulling.radius.ToString() + " ";
    //    total += selectedSection.section.culling.sectionCulling.height.ToString() + "\n\n";

    //    foreach (var culling in selectedSection.section.culling.chunkCulling8) {
    //        total += culling.radius.ToString() + " ";
    //        total += culling.height.ToString() + "\n";
    //    }

    //    total += "\n";

    //    foreach (var culling in selectedSection.section.culling.chunkCulling4) {
    //        total += culling.radius.ToString() + " ";
    //        total += culling.height.ToString() + "\n";
    //    }

    //    Debug.Log(total);


    //}

    public void CopySectionData() {

        if (HasSelection) {
            copySection = FirstItem.section;
            QuickLogHandler.Log("Section copied to clipboard", LogSeverity.Success);
        } else {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);

        }

    }

    public void PasteSectionData() {

        if (!HasSelection) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }
        if (copySection == null) {
            QuickLogHandler.Log("No section copied to clipboard", LogSeverity.Info);
            return;
        }

        DialogWindowUtil.Dialog("Warning", "This will overwrite all current map data, are you sure you want to continue?", () => {

            AddSectionSaveStateCounterAction();

            foreach (var levelMesh in selectedSections) {

                if (copySection != null) {
                    levelMesh.section.Overwrite(copySection);
                    levelMesh.RefreshMesh();
                }

            }

            return true;

        });

    }

    public void MirrorSectionVertically() {

        if (!HasSelection) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }

        DialogWindowUtil.Dialog("Warning",
            "Some tiles cannot be mirrored correct and may be deleted. This will overwrite all current map data, are you sure you want to continue?",
            () => {

                AddSectionSaveStateCounterAction();

                foreach (var levelMesh in selectedSections) {
                    levelMesh.section.MirrorVertically();
                    levelMesh.RefreshMesh();

                }

                return true;

            });

    }

    public void MirrorSectionHorizontally() {

        if (!HasSelection) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }

        DialogWindowUtil.Dialog("Warning",
            "Some tiles cannot be mirrored correct and may be deleted. This will overwrite all current map data, are you sure you want to continue?",
            () => {

                AddSectionSaveStateCounterAction();

                foreach (var levelMesh in selectedSections) {
                    levelMesh.section.MirrorHorizontally();
                    levelMesh.RefreshMesh();
                }

                return true;

            });

    }

    public void MirrorSectionDiagonally() {

        if (!HasSelection) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }

        DialogWindowUtil.Dialog("Warning",
            "Some tiles cannot be mirrored correct and may be deleted. This will overwrite all current map data, are you sure you want to continue?",
            () => {

                AddSectionSaveStateCounterAction();

                foreach (var levelMesh in selectedSections) {
                    levelMesh.section.MirrorDiagonally();
                    levelMesh.RefreshMesh();
                }

                return true;

            });

    }

    public void RotateSectionClockwise() {

        if (!HasSelection) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }

        DialogWindowUtil.Dialog("Warning",
            "Some tiles cannot be rotated correct and may be deleted. This will overwrite all current map data, are you sure you want to continue?",
            () => {

                AddSectionSaveStateCounterAction();

                foreach (var levelMesh in selectedSections) {
                    levelMesh.section.RotateClockwise();
                    levelMesh.RefreshMesh();
                }

                return true;

            });

    }

    public void RotateSectionCounterClockwise() {

        if (!HasSelection) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }

        DialogWindowUtil.Dialog("Warning",
            "Some tiles cannot be rotated correct and may be deleted. This will overwrite all current map data, are you sure you want to continue?",
            () => {

                AddSectionSaveStateCounterAction();

                foreach (var levelMesh in selectedSections) {
                    levelMesh.section.RotateCounterClockwise();
                    levelMesh.RefreshMesh();
                }

                return true;

            });

    }

    public void RemoveShadersFromSection() {

        if (!HasSelection) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }

        DialogWindowUtil.Dialog("Warning",
        "This will remove all tile shader data, are you sure you want to continue?",
        () => {

            AddSectionSaveStateCounterAction();

            foreach (var levelMesh in selectedSections) {

                foreach (var column in levelMesh.section.tileColumns) {

                    foreach (var tile in column.tiles) {
                        tile.ChangeShader(VertexColorType.MonoChrome);
                    }

                }

                levelMesh.RefreshMesh();
            }

            return true;

        });


    }

    public void PasteHeightMapData() {

        if (!HasSelection) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }
        if (copySection == null) {
            QuickLogHandler.Log("No section copied to clipboard", LogSeverity.Info);
            return;
        }

        DialogWindowUtil.Dialog("Warning", "This will overwrite all current map data, are you sure you want to continue?", () => {

            if (copySection != null) {

                AddSectionSaveStateCounterAction();

                foreach (var levelMesh in selectedSections) {
                    levelMesh.section.OverwriteHeights(copySection);
                    levelMesh.RefreshMesh();
                }

            }

            return true;
        });

    }

    public void PasteTileData() {

        if (!HasSelection) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }
        if (copySection == null) {
            QuickLogHandler.Log("No section copied to clipboard", LogSeverity.Info);
            return;
        }

        DialogWindowUtil.Dialog("Warning", "This will overwrite all current map data, are you sure you want to continue?", () => {

            if (copySection != null) {

                AddSectionSaveStateCounterAction();

                foreach (var levelMesh in selectedSections) {
                    levelMesh.section.OverwriteTiles(copySection);
                    levelMesh.RefreshMesh();
                }

            }

            return true;
        });

    }

    public void ClearSectionData() {

        if (!HasSelection) {
            QuickLogHandler.Log("No section is selected", LogSeverity.Info);
            return;
        }

        DialogWindowUtil.Dialog("Warning", "This will overwrite all current map data, are you sure you want to continue?", () => {

            AddSectionSaveStateCounterAction();

            foreach (var levelMesh in selectedSections) {
                levelMesh.section.Overwrite(FCopLevelSection.CreateEmpty(-120, -100, -80));
                levelMesh.RefreshMesh();
            }

            return true;
        });

    }

    #region Counter-Actions

    public class MultiSectionAddCounterAction : CounterAction {

        public string name { get; set; }

        List<SectionSaveStateCounterAction> savedSections = new();

        public MultiSectionAddCounterAction(List<LevelMesh> sectionMeshes) {

            foreach (var section in sectionMeshes) {
                savedSections.Add(new SectionSaveStateCounterAction(section));
            }

            name = "Section Changes";
        }

        public void Action() {

            foreach (var state in savedSections) {
                state.Action();
            }

        }

    }

    void AddSectionSaveStateCounterAction() {
        Main.AddCounterAction(new MultiSectionAddCounterAction(selectedSections));
    }

    #endregion

}