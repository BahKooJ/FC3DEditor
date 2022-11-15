
using FCopParser;
using UnityEngine;

class AddTileButton : MonoBehaviour {

    public Main controller;

    public void OnClick() {

        if (controller.selectedColumn != null) {

            var newTile = new Tile(new TileBitfield(1, 0, 0, 0, 68, 0));

            controller.selectedColumn.tiles.Add(newTile);

            controller.selectedTile = newTile;

            controller.selectedSection.RefreshMesh();

        }

    }

}