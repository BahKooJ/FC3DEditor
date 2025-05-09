﻿

using FCopParser;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class MeshIDDebug: MonoBehaviour {

    public GameObject tilePreviewPrefab;
    public GameObject staticHeightPointPrefab;

    public TextMeshProUGUI meshIDtext;
    public SelectedTileOverlay tilePreview;

    IFFParser iffFile = new IFFParser(File.ReadAllBytes("MissionFiles/Mp"));
    FCopLevel level;
    TileColumn exampleColumn;
    Tile exampleTile;

    int meshID = 0;

    void Start () {

        level = new FCopLevel(iffFile.parsedData);

        exampleColumn = new TileColumn(0, 0, new(), new() {
            new HeightPoints(0f,1f,2f), new HeightPoints(0f,1f,2f), new HeightPoints(0f, 1f, 2f), new HeightPoints(0f,1f,2f)
        });

        exampleTile = new Tile(exampleColumn, 0, 0);

        var overlay = Instantiate(tilePreviewPrefab);
        var script = overlay.GetComponent<SelectedTileOverlay>();
        script.tile = exampleTile;
        tilePreview = script;

        AddHeightObjects(0);
        AddHeightObjects(1);
        AddHeightObjects(2);
        AddHeightObjects(3);

    }

    void Update () {

        if (Input.GetKeyDown(KeyCode.UpArrow)) {

            meshID++;

            exampleTile = new Tile(exampleColumn, meshID, 0);

            meshIDtext.text = meshID.ToString();

            tilePreview.tile = exampleTile;
            tilePreview.Refresh();

            var counterClockVertices = new List<TileVertex>();

            foreach (var vertex in exampleTile.verticies) {

                switch (vertex.vertexPosition) {

                    case VertexPosition.TopLeft:
                        counterClockVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomLeft));
                        break;
                    case VertexPosition.TopRight:
                        counterClockVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomRight));
                        break;
                    case VertexPosition.BottomLeft:
                        counterClockVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopLeft));
                        break;
                    case VertexPosition.BottomRight:
                        counterClockVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopRight));
                        break;

                }

            }

            var counterClockID = MeshType.IDFromVerticies(counterClockVertices);

            if (counterClockID == null) {
                meshIDtext.text = meshID.ToString();
            } else {
                meshIDtext.text = meshID.ToString();
            }

        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {

            meshID--;

            exampleTile = new Tile(exampleColumn, meshID, 0);

            meshIDtext.text = meshID.ToString();

            tilePreview.tile = exampleTile;
            tilePreview.Refresh();

            var counterClockVertices = new List<TileVertex>();

            foreach (var vertex in exampleTile.verticies) {

                switch (vertex.vertexPosition) {

                    case VertexPosition.TopLeft:
                        counterClockVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomLeft));
                        break;
                    case VertexPosition.TopRight:
                        counterClockVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopLeft));
                        break;
                    case VertexPosition.BottomLeft:
                        counterClockVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomRight));
                        break;
                    case VertexPosition.BottomRight:
                        counterClockVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopRight));
                        break;

                }

            }

            var counterClockID = MeshType.IDFromVerticies(counterClockVertices);

            if (counterClockID == null) {
                meshIDtext.text = meshID.ToString();
            }
            else {
                meshIDtext.text = meshID.ToString();
            }

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