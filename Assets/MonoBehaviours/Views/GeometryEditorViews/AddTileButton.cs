
using UnityEngine;

public class AddTileButton : MonoBehaviour {

    public TileEditPanel view;

    public TilePreset preset;

    void Start() {

    }

    public void OnClick() {

        view.Select(preset);

    }

}