using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileTexturePreview : MonoBehaviour {

    public Main controller;

    public Tile tile;
    public TileColumn column;
    public FCopLevelSection section;

    Mesh mesh;
    Material material;

    List<Vector3> vertices = new();
    List<int> triangles = new();
    List<Vector2> textureCords = new();


    void Start() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        material = GetComponent<MeshRenderer>().material;

        material.mainTexture = controller.levelTexturePallet;

        Refresh();

        // Grabs the cameras rotation to rotate the tile the same as it was seen
        var startAngle = Camera.main.transform.localEulerAngles.y;

        // Not sure why this needs to be done, but the camera angle needs 180 removed to get the angle to line up
        if (startAngle > 0) {
            startAngle = 180 - startAngle;
        } else {
            startAngle = -180 + startAngle;
        }

        transform.localEulerAngles = new Vector3(0, startAngle, 0);


    }

    public void Refresh() {

        if (mesh == null) {
            return;
        }

        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        textureCords.Clear();

        Generate();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.uv = textureCords.ToArray();

        mesh.RecalculateNormals();

    }


    void Generate() {

        var vertexIndex = 0;

        var heights = new List<float>();

        // The lowest high needs to be found to bring the base to 0 while still keeping the right geometry.
        // This is so the tile stays inside the UI container
        foreach (var point in tile.verticies) {

            switch (point.vertexPosition) {

                case VertexPosition.TopLeft:
                    heights.Add(column.heights[0].GetPoint(point.heightChannel));

                    break;
                case VertexPosition.TopRight:
                    heights.Add(column.heights[1].GetPoint(point.heightChannel));

                    break;
                case VertexPosition.BottomLeft:
                    heights.Add(column.heights[2].GetPoint(point.heightChannel));

                    break;
                case VertexPosition.BottomRight:
                    heights.Add(column.heights[3].GetPoint(point.heightChannel));

                    break;
            }

        }

        var lowestHeight = heights.Min();

        void AddVerticies(Tile tile) {

            foreach (var point in tile.verticies) {

                switch (point.vertexPosition) {

                    case VertexPosition.TopLeft:
                        vertices.Add(new Vector3(x: -0.5f, y: column.heights[0].GetPoint(point.heightChannel) - lowestHeight, z: -0.5f));

                        break;
                    case VertexPosition.TopRight:
                        vertices.Add(new Vector3(x: 0.5f, y: column.heights[1].GetPoint(point.heightChannel) - lowestHeight, z: -0.5f));

                        break;
                    case VertexPosition.BottomLeft:
                        vertices.Add(new Vector3(x: -0.5f, y: column.heights[2].GetPoint(point.heightChannel) - lowestHeight, z: 0.5f));

                        break;
                    case VertexPosition.BottomRight:
                        vertices.Add(new Vector3(x: 0.5f, y: column.heights[3].GetPoint(point.heightChannel) - lowestHeight, z: 0.5f));

                        break;
                }

            }

        }
        
        //TODO: If texture index that is more than avaliable will cause crash
        void GenerateTriangle(Tile tile) {

            var textureOffset = tile.textureIndex;
            var grpahics = section.tileGraphics[tile.graphicsIndex];
            textureCords.Add(new Vector2(
                TextureCoordinate.GetX(section.textureCoordinates[textureOffset] + grpahics.number2 * 65536),
                TextureCoordinate.GetY(section.textureCoordinates[textureOffset] + grpahics.number2 * 65536)
            ));
            textureCords.Add(new Vector2(
                TextureCoordinate.GetX(section.textureCoordinates[textureOffset + 2] + grpahics.number2 * 65536),
                TextureCoordinate.GetY(section.textureCoordinates[textureOffset + 2] + grpahics.number2 * 65536)
            ));
            textureCords.Add(new Vector2(
                TextureCoordinate.GetX(section.textureCoordinates[textureOffset + 1] + grpahics.number2 * 65536),
                TextureCoordinate.GetY(section.textureCoordinates[textureOffset + 1] + grpahics.number2 * 65536)
            ));

            AddVerticies(tile);

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);

            vertexIndex += 3;

        }

        void GenerateSquare(Tile tile) {

            AddVerticies(tile);

            var textureOffset = tile.textureIndex;
            var grpahics = section.tileGraphics[tile.graphicsIndex];
            textureCords.Add(new Vector2(
                TextureCoordinate.GetX(section.textureCoordinates[textureOffset] + grpahics.number2 * 65536),
                TextureCoordinate.GetY(section.textureCoordinates[textureOffset] + grpahics.number2 * 65536)
            ));
            textureCords.Add(new Vector2(
                TextureCoordinate.GetX(section.textureCoordinates[textureOffset + 1] + grpahics.number2 * 65536),
                TextureCoordinate.GetY(section.textureCoordinates[textureOffset + 1] + grpahics.number2 * 65536)
            ));
            textureCords.Add(new Vector2(
                TextureCoordinate.GetX(section.textureCoordinates[textureOffset + 3] + grpahics.number2 * 65536),
                TextureCoordinate.GetY(section.textureCoordinates[textureOffset + 3] + grpahics.number2 * 65536)
            ));
            textureCords.Add(new Vector2(
                TextureCoordinate.GetX(section.textureCoordinates[textureOffset + 2] + grpahics.number2 * 65536),
                TextureCoordinate.GetY(section.textureCoordinates[textureOffset + 2] + grpahics.number2 * 65536)
            ));

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);

            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);

            vertexIndex += 4;

        }

        if (tile.verticies.Count == 3) {
            GenerateTriangle(tile);
        } else if (tile.verticies.Count == 4) {
            GenerateSquare(tile);
        }

    }

}