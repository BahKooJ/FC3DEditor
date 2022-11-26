
using FCopParser;
using UnityEngine;

public class AddTileButton : MonoBehaviour {

    public AddGeometryPanel view;

    public int defaultPreset = -1;

    public TilePreset preset;

    void Start() {

        if (defaultPreset != -1) {
            preset = TilePreset.defaultPresets[(int)defaultPreset];
        }

    }

    public void OnClick() {

        view.Select(preset);

    }

}