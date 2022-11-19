
using FCopParser;
using UnityEngine;

public class AddTileButton : MonoBehaviour {

    public Main controller;

    public int defaultPreset = -1;

    public TilePreset preset;

    void Start() {

        if (defaultPreset != -1) {
            preset = TilePreset.defaultPresets[(int)defaultPreset];
        }

    }

    public void OnClick() {

        if (controller.selectedColumn != null) {

            foreach (var foo in controller.selectedColumn.tiles) {
                foo.isStartInColumnArray = false;
            }

            //controller.selectedColumn.tiles.Clear();

            var tile = preset.Create(true);

            controller.selectedColumn.tiles.Add(tile);

            controller.selectedTile = tile;

        }

        controller.selectedSection.RefreshMesh();

    }

}