
using UnityEditorInternal;
using UnityEngine;

public class AddGeometryPanel : MonoBehaviour {

    public Main controller;

    public void Select(TilePreset preset) {
       
        // A try shoud be placed here. It should never not be GeometryAddMode so leaving it here to see if an exeption is thrown.
        var geometryAddMode = (GeometryAddMode)controller.editMode;

        geometryAddMode.selectedTilePreset = preset;

        geometryAddMode.RefreshTilePlacementOverlay();

    }

}