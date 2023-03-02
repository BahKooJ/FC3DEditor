

using FCopParser;
using TMPro;
using UnityEngine;

public class MeshIDDebug: MonoBehaviour {

    public TextMeshProUGUI meshIDtext;
    public GameObject tilePreviewPrefab;
    public SelectedTileOverlay tilePreview;

    TileColumn exampleColumn;
    Tile exampleTile;

    int meshID = 0;

    void Start () {

        exampleColumn = new TileColumn(0, 0, new(), new() {
            new HeightPoint(0,1,2), new HeightPoint(0,1,2), new HeightPoint(0,1,2), new HeightPoint(0,1,2)
        });

        exampleTile = new Tile(new TileBitfield(0, 0, 0, 0, meshID, 0));

        var overlay = Instantiate(tilePreviewPrefab);
        var script = overlay.GetComponent<SelectedTileOverlay>();
        script.tile = exampleTile;
        script.column = exampleColumn;
        tilePreview = script;

    }

    void Update () {

        if (Input.GetKeyDown(KeyCode.UpArrow)) {

            meshID++;

            exampleTile = new Tile(new TileBitfield(0, 0, 0, 0, meshID, 0));

            meshIDtext.text = meshID.ToString();

            tilePreview.tile = exampleTile;
            tilePreview.Refresh();

        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {

            meshID--;

            exampleTile = new Tile(new TileBitfield(0, 0, 0, 0, meshID, 0));

            meshIDtext.text = meshID.ToString();

            tilePreview.tile = exampleTile;
            tilePreview.Refresh();

        }

    }

}