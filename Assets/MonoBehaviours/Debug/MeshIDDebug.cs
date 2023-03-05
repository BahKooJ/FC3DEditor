

using FCopParser;
using TMPro;
using UnityEngine;

public class MeshIDDebug: MonoBehaviour {

    public GameObject tilePreviewPrefab;
    public GameObject staticHeightPointPrefab;

    public TextMeshProUGUI meshIDtext;
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

        AddHeightObjects(0);
        AddHeightObjects(1);
        AddHeightObjects(2);
        AddHeightObjects(3);

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

    void AddHeightObjects(int corner) {

        var worldX = 0;
        var worldY = 0;

        switch (corner) {
            case 1:
                worldX += 1;
                break;
            case 2:
                worldY -= 1;
                break;
            case 3:
                worldX += 1;
                worldY -= 1;
                break;
            default:
                break;
        }

        var point = Instantiate(staticHeightPointPrefab, new Vector3(worldX, exampleColumn.heights[corner].height1, worldY), Quaternion.identity);
        var material = point.GetComponent<MeshRenderer>().material;
        material.color = Color.blue;

        point = Instantiate(staticHeightPointPrefab, new Vector3(worldX, exampleColumn.heights[corner].height2, worldY), Quaternion.identity);
        material = point.GetComponent<MeshRenderer>().material;
        material.color = Color.green;


        point = Instantiate(staticHeightPointPrefab, new Vector3(worldX, exampleColumn.heights[corner].height3, worldY), Quaternion.identity);
        material = point.GetComponent<MeshRenderer>().material;
        material.color = Color.red;

    }


}