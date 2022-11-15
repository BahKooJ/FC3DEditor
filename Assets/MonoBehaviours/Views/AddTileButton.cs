
using FCopParser;
using UnityEngine;

class AddTileButton : MonoBehaviour {

    public Main controller;

    //Appearently the order of the tiles matters as well...
    public void OnClick() {

        if (controller.selectedColumn != null) {

            var newTile = new Tile(new TileBitfield(0, 0, 2, 0, 68, 0));

            controller.selectedColumn.tiles.Add(newTile);

            controller.selectedTile = newTile;

            controller.selectedSection.RefreshMesh();

        }

    }

}