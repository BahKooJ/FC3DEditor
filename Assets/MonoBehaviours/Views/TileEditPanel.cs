using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileEditPanel : MonoBehaviour {

    public GameObject debugTilePanel;
    public GameObject tilePresetPrefab;
    public TileEditMode controller;

    public RectTransform presetList;
    public TMP_Dropdown presetCatagory;

    public GameObject editTools;
    public GameObject buildTools;

    public GameObject debugTilePanelView = null;

    public Dictionary<int, List<TilePreset>> defaultPresets = new() {
        { 0, new() {
                new TilePreset(68, 0, 0, 0, "PresetsIcon/presetFloorH1"),
                new TilePreset(69, 0, 0, 0, "PresetsIcon/presetFloorH2"),
                new TilePreset(70, 0, 0, 0, "PresetsIcon/presetFloorH3")
            }
        },
        { 1, new() {
                new TilePreset(6, 0, 0, 0, "PresetsIcon/presetTriangle1H1"),
                new TilePreset(8, 0, 0, 0, "PresetsIcon/presetTriangle1H2"),
                new TilePreset(10, 0, 0, 0, "PresetsIcon/presetTriangle1H3"),
                new TilePreset(1, 0, 0, 0, "PresetsIcon/presetTriangle2H1"),
                new TilePreset(3, 0, 0, 0, "PresetsIcon/presetTriangle2H2"),
                new TilePreset(5, 0, 0, 0, "PresetsIcon/presetTriangle2H3"),
                new TilePreset(7, 0, 0, 0, "PresetsIcon/presetTriangle3H1"),
                new TilePreset(9, 0, 0, 0, "PresetsIcon/presetTriangle3H2"),
                new TilePreset(11, 0, 0, 0, "PresetsIcon/presetTriangle3H3"),
                new TilePreset(0, 0, 0, 0, "PresetsIcon/presetTriangle4H1"),
                new TilePreset(2, 0, 0, 0, "PresetsIcon/presetTriangle4H2"),
                new TilePreset(4, 0, 0, 0, "PresetsIcon/presetTriangle4H3"),

            }
        },
        { 2, new() {
                new TilePreset(107, 3, 0, 0, "PresetsIcon/presetWallLeftH1H2"),
                new TilePreset(109, 3, 0, 0, "PresetsIcon/presetWallLeftH2H3"),
                new TilePreset(105, 3, 0, 0, "PresetsIcon/presetWallLeftRightH1H2"),
                new TilePreset(106, 3, 0, 0, "PresetsIcon/presetWallLeftRightH2H3"),
                new TilePreset(103, 3, 0, 0, "PresetsIcon/presetWallRightLeftH1H2"),
                new TilePreset(104, 3, 0, 0, "PresetsIcon/presetWallRightLeftH2H3"),
                new TilePreset(108, 3, 0, 0, "PresetsIcon/presetWallTopH1H2"),
                new TilePreset(110, 3, 0, 0, "PresetsIcon/presetWallTopH2H3"),
            }
        },
        { 3, new() {
                new TilePreset(75, 3, 0, 0, "PresetsIcon/presetTriangleWallLeftBottomH1H2"),
                new TilePreset(77, 3, 0, 0, "PresetsIcon/presetTriangleWallLeftBottomH2H3"),
                new TilePreset(99, 3, 0, 0, "PresetsIcon/presetTriangleWallLeftRightLeftH1H2"),
                new TilePreset(101, 3, 0, 0, "PresetsIcon/presetTriangleWallLeftRightLeftH2H3"), 
                new TilePreset(95, 3, 0, 0, "PresetsIcon/presetTriangleWallLeftRightRightH1H2"), 
                new TilePreset(97, 3, 0, 0, "PresetsIcon/presetTriangleWallLeftRightRightH2H3"),
                new TilePreset(71, 3, 0, 0, "PresetsIcon/presetTriangleWallLeftTopH1H2"),
                new TilePreset(73, 3, 0, 0, "PresetsIcon/presetTriangleWallLeftTopH2H3"),
                new TilePreset(91, 3, 0, 0, "PresetsIcon/presetTriangleWallRightLeftBottomH1H2"),
                new TilePreset(93, 3, 0, 0, "PresetsIcon/presetTriangleWallRightLeftBottomH2H3"),
                new TilePreset(87, 3, 0, 0, "PresetsIcon/presetTriangleWallRightLeftTopH1H2"),
                new TilePreset(89, 3, 0, 0, "PresetsIcon/presetTriangleWallRightLeftTopH2H3"),
                new TilePreset(83, 3, 0, 0, "PresetsIcon/presetTriangleWallTopLeftH1H2"),
                new TilePreset(85, 3, 0, 0, "PresetsIcon/presetTriangleWallTopLeftH2H3"),
                new TilePreset(79, 3, 0, 0, "PresetsIcon/presetTriangleWallTopRightH1H2"),
                new TilePreset(81, 3, 0, 0, "PresetsIcon/presetTriangleWallTopRightH2H3"),
            }
        }
    };

    void Start() {
        controller.view = this;

        if (Main.debug) {

            debugTilePanelView = Instantiate(debugTilePanel);

            debugTilePanelView.GetComponent<ShaderDebug>().controller = controller;

            debugTilePanelView.transform.SetParent(transform.parent, false);

        }

    }

    public void ChangeTools() {
        
        editTools.SetActive(!controller.isBuildMode);
        buildTools.SetActive(controller.isBuildMode);
        
        if (controller.isBuildMode) {
            OnCatagoryChange();
        }

    }

    // Editing
    public void OnClickShiftHeightUpButton() {

        controller.ShiftTilesHeightUp();

    }

    public void OnClickShiftHeightDownButton() {

        controller.ShiftTilesHeightDown();

    }

    // Building
    public void OnCatagoryChange() {

        foreach (var obj in FindObjectsOfType<AddTileButton>()) {
            Destroy(obj.gameObject);
        }

        var presets = defaultPresets[presetCatagory.value];

        foreach (var preset in presets) {

            var presetButton = Instantiate(tilePresetPrefab);

            var script = presetButton.GetComponent<AddTileButton>();

            script.preset = preset;
            script.view = this;

            var image = presetButton.GetComponent<Image>();

            image.sprite = Resources.Load<Sprite>(preset.previewImagePath);

            presetButton.transform.SetParent(presetList.transform, false);

        }

    }

    public void Select(TilePreset preset) {
       
        var geometryAddMode = controller;

        geometryAddMode.selectedTilePreset = preset;

        geometryAddMode.RefreshTilePlacementOverlay();

    }



}