
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

        controller.AddTile(preset);

    }

}