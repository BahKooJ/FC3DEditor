

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
            line.SetPosition(0, new Vector3(
                x: tile.column.x,
                y: tile.column.heights[0].GetPoint(tile.verticies[0].heightChannel),
                z: tile.column.y));

            line.SetPosition(1, new Vector3(
                x: tile.column.x + 1,
                y: tile.column.heights[3].GetPoint(tile.verticies[3].heightChannel),
                z: tile.column.y + 1));
        }
        else {
            line.SetPosition(0, new Vector3(
                x: tile.column.x,
                y: tile.column.heights[2].GetPoint(tile.verticies[2].heightChannel),
                z: tile.column.y + 1));

            line.SetPosition(1, new Vector3(
                x: tile.column.x + 1,
                y: tile.column.heights[1].GetPoint(tile.verticies[1].heightChannel),
                z: tile.column.y));
        }



    }


}