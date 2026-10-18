

using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public static class LevelGeometryUtils {

    public static List<Vector3> VerticesFromTile(Tile tile) {

        var vertices = new List<Vector3>();

        var column = tile.column;

        foreach (var point in tile.vertices) {

            switch (point.vertexPosition) {
                case VertexPosition.TopLeft:
                    vertices.Add(new Vector3(x: column.x, y: column.heights[0].GetPoint(point.heightChannel), z: column.y));
                    break;
                case VertexPosition.TopRight:
                    vertices.Add(new Vector3(x: column.x + 1, y: column.heights[1].GetPoint(point.heightChannel), z: column.y));
                    break;
                case VertexPosition.BottomLeft:
                    vertices.Add(new Vector3(x: column.x, y: column.heights[2].GetPoint(point.heightChannel), z: column.y + 1));
                    break;
                case VertexPosition.BottomRight:
                    vertices.Add(new Vector3(x: column.x + 1, y: column.heights[3].GetPoint(point.heightChannel), z: column.y + 1));
                    break;
            }

        }

        return vertices;

    }

    public static List<int> TrisFromTile(Tile tile, int offset) {

        if (tile.IsQuad) {
            return new() {
                offset,
                offset + 2,
                offset + 1,

                offset + 1,
                offset + 2,
                offset + 3,
            };
        }
        else {
            return new() {
                offset,
                offset + 1,
                offset + 2
            };
        }

    }

    public static List<Vector2> UvsFromTile(Tile tile) {

        Vector2 UvFromTile(int i) {
            return new Vector2(TextureCoordinate.GetX(tile.uvs[i] + tile.texturePalette * 65536), TextureCoordinate.GetY(tile.uvs[i] + tile.texturePalette * 65536));
        }

        if (tile.IsQuad) {
            return new() {
                UvFromTile(0),
                UvFromTile(1),
                UvFromTile(3),
                UvFromTile(2),
            };
        }
        else {
            return new() {
                UvFromTile(0),
                UvFromTile(2),
                UvFromTile(1),
            };
        }

    }

    public static List<Color> ColorsFromTile(Tile tile) {

        var colors = tile.shaders.colors;

        if (tile.IsQuad) {
            return new() {
                // Was it really that hard to just make a color struct?
                new Color(colors[0][0], colors[0][1], colors[0][2]),
                new Color(colors[1][0], colors[1][1], colors[1][2]),
                new Color(colors[2][0], colors[2][1], colors[2][2]),
                new Color(colors[3][0], colors[3][1], colors[3][2])
            };
        }
        else {
            return new() {
                new Color(colors[0][0], colors[0][1], colors[0][2]),
                new Color(colors[1][0], colors[1][1], colors[1][2]),
                new Color(colors[2][0], colors[2][1], colors[2][2])
            };

        }
    }

}