
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class AddGeometryPanel : MonoBehaviour {

    public GameObject tilePresetPrefab;
    public Main controller;

    public RectTransform presetList;
    public TMP_Dropdown presetCatagory;

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
                new TilePreset(107, 0, 0, 0, "PresetsIcon/presetWallLeftH1H2"),
                new TilePreset(109, 0, 0, 0, "PresetsIcon/presetWallLeftH2H3"),
                new TilePreset(105, 0, 0, 0, "PresetsIcon/presetWallLeftRightH1H2"),
                new TilePreset(106, 0, 0, 0, "PresetsIcon/presetWallLeftRightH2H3"),
                new TilePreset(103, 0, 0, 0, "PresetsIcon/presetWallRightLeftH1H2"),
                new TilePreset(104, 0, 0, 0, "PresetsIcon/presetWallRightLeftH2H3"),
                new TilePreset(108, 0, 0, 0, "PresetsIcon/presetWallTopH1H2"),
                new TilePreset(110, 0, 0, 0, "PresetsIcon/presetWallTopH2H3"),
            }
        },
        { 3, new() {
                new TilePreset(75, 0, 0, 0, "PresetsIcon/presetTriangleWallLeftBottomH1H2"),
                new TilePreset(77, 0, 0, 0, "PresetsIcon/presetTriangleWallLeftBottomH2H3"),
                new TilePreset(99, 0, 0, 0, "PresetsIcon/presetTriangleWallLeftRightLeftH1H2"),
                // new TilePreset(0, 0, 0, 0, "PresetsIcon/presetTriangleWallLeftRightLeftH2H3.png"), missing
                // new TilePreset(0, 0, 0, 0, "PresetsIcon/presetTriangleWallLeftRightRightH1H2.png"), missing
                new TilePreset(97, 0, 0, 0, "PresetsIcon/presetTriangleWallLeftRightRightH2H3"),
                new TilePreset(71, 0, 0, 0, "PresetsIcon/presetTriangleWallLeftTopH1H2"),
                new TilePreset(73, 0, 0, 0, "PresetsIcon/presetTriangleWallLeftTopH2H3"),
                new TilePreset(91, 0, 0, 0, "PresetsIcon/presetTriangleWallRightLeftBottomH1H2"),
                new TilePreset(93, 0, 0, 0, "PresetsIcon/presetTriangleWallRightLeftBottomH2H3"),
                new TilePreset(93, 0, 0, 0, "PresetsIcon/presetTriangleWallRightLeftBottomH2H3"),
                new TilePreset(87, 0, 0, 0, "PresetsIcon/presetTriangleWallRightLeftTopH1H2"),
                new TilePreset(89, 0, 0, 0, "PresetsIcon/presetTriangleWallRightLeftTopH2H3"),
                new TilePreset(83, 0, 0, 0, "PresetsIcon/presetTriangleWallTopLeftH1H2"),
                new TilePreset(85, 0, 0, 0, "PresetsIcon/presetTriangleWallTopLeftH2H3"),
                new TilePreset(80, 0, 0, 0, "PresetsIcon/presetTriangleWallTopRightH1H2"),
                new TilePreset(81, 0, 0, 0, "PresetsIcon/presetTriangleWallTopRightH2H3"),
            }
        }
    };

    void Start() {
        OnCatagoryChange();
    }

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
       
        // A try shoud be placed here. It should never not be GeometryAddMode so leaving it here to see if an exeption is thrown.
        var geometryAddMode = (GeometryAddMode)controller.editMode;

        geometryAddMode.selectedTilePreset = preset;

        geometryAddMode.RefreshTilePlacementOverlay();

    }

}