

using FCopParser;
using System.Drawing;
using UnityEngine;

public class TileSplitLine : MonoBehaviour {

    // - Unity Refs -
    public LineRenderer line;

    // - Parameters -
    public Tile tile;
    public bool topBottom = false;

    private void Start() {

        if (topBottom) {
            TopBottom();
        }
        else {
            BottomTop();
        }



    }

    void TopBottom() {

        int originalMeshID = (int)MeshType.IDFromVerticies(tile.verticies);

        float x1 = 0f;
        float y1 = 0f;
        float z1 = 0f;
        float x2 = 0f;
        float y2 = 0f;
        float z2 = 0f;

        if (MeshType.topWallMeshes.Contains(originalMeshID)) {

            x1 = tile.column.x;
            y1 = tile.column.heights[0].GetPoint(tile.verticies[1].heightChannel);
            z1 = tile.column.y;

            x2 = tile.column.x + 1;
            y2 = tile.column.heights[1].GetPoint(tile.verticies[2].heightChannel);
            z2 = tile.column.y;

        }
        else if (MeshType.leftWallMeshes.Contains(originalMeshID)) {

            x1 = tile.column.x;
            y1 = tile.column.heights[0].GetPoint(tile.verticies[0].heightChannel);
            z1 = tile.column.y;

            x2 = tile.column.x;
            y2 = tile.column.heights[2].GetPoint(tile.verticies[3].heightChannel);
            z2 = tile.column.y + 1;

        }
        else if (MeshType.diagonalTLeftBRightQuadWallMeshes.Contains(originalMeshID)) {

            x1 = tile.column.x;
            y1 = tile.column.heights[0].GetPoint(tile.verticies[1].heightChannel);
            z1 = tile.column.y;

            x2 = tile.column.x + 1;
            y2 = tile.column.heights[3].GetPoint(tile.verticies[2].heightChannel);
            z2 = tile.column.y + 1;

        }
        else if (MeshType.diagonalBLeftTRightQuadWallMeshes.Contains(originalMeshID)) {

            x1 = tile.column.x;
            y1 = tile.column.heights[2].GetPoint(tile.verticies[1].heightChannel);
            z1 = tile.column.y + 1;

            x2 = tile.column.x + 1;
            y2 = tile.column.heights[1].GetPoint(tile.verticies[2].heightChannel);
            z2 = tile.column.y;

        }
        else {

            x1 = tile.column.x;
            y1 = tile.column.heights[0].GetPoint(tile.verticies[0].heightChannel);
            z1 = tile.column.y;

            x2 = tile.column.x + 1;
            y2 = tile.column.heights[3].GetPoint(tile.verticies[3].heightChannel);
            z2 = tile.column.y + 1;

        }
        
        line.SetPosition(0, new Vector3(x1, y1, z1));
        line.SetPosition(1, new Vector3(x2, y2, z2));

    }

    void BottomTop() {

        int originalMeshID = (int)MeshType.IDFromVerticies(tile.verticies);

        float x1 = 0f;
        float y1 = 0f;
        float z1 = 0f;
        float x2 = 0f;
        float y2 = 0f;
        float z2 = 0f;

        if (MeshType.topWallMeshes.Contains(originalMeshID)) {

            x1 = tile.column.x;
            y1 = tile.column.heights[0].GetPoint(tile.verticies[0].heightChannel);
            z1 = tile.column.y;

            x2 = tile.column.x + 1;
            y2 = tile.column.heights[1].GetPoint(tile.verticies[3].heightChannel);
            z2 = tile.column.y;

        }
        else if (MeshType.leftWallMeshes.Contains(originalMeshID)) {

            x1 = tile.column.x;
            y1 = tile.column.heights[0].GetPoint(tile.verticies[1].heightChannel);
            z1 = tile.column.y;

            x2 = tile.column.x;
            y2 = tile.column.heights[2].GetPoint(tile.verticies[2].heightChannel);
            z2 = tile.column.y + 1;

        }
        else if (MeshType.diagonalTLeftBRightQuadWallMeshes.Contains(originalMeshID)) {

            x1 = tile.column.x;
            y1 = tile.column.heights[0].GetPoint(tile.verticies[0].heightChannel);
            z1 = tile.column.y;

            x2 = tile.column.x + 1;
            y2 = tile.column.heights[3].GetPoint(tile.verticies[3].heightChannel);
            z2 = tile.column.y + 1;

        }
        else if (MeshType.diagonalBLeftTRightQuadWallMeshes.Contains(originalMeshID)) {

            x1 = tile.column.x;
            y1 = tile.column.heights[2].GetPoint(tile.verticies[0].heightChannel);
            z1 = tile.column.y + 1;

            x2 = tile.column.x + 1;
            y2 = tile.column.heights[1].GetPoint(tile.verticies[3].heightChannel);
            z2 = tile.column.y;

        }
        else {

            x1 = tile.column.x;
            y1 = tile.column.heights[2].GetPoint(tile.verticies[2].heightChannel);
            z1 = tile.column.y + 1;

            x2 = tile.column.x + 1;
            y2 = tile.column.heights[1].GetPoint(tile.verticies[1].heightChannel);
            z2 = tile.column.y;

        }

        line.SetPosition(0, new Vector3(x1, y1, z1));
        line.SetPosition(1, new Vector3(x2, y2, z2));

    }


}