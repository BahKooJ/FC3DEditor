
using FCopParser;
using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;

public class SelectedTileOverlay : MonoBehaviour {

    public Main controller;

    public Tile tile;
    public TileColumn column;

    Mesh mesh;
    Material material;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    void Start() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        material = GetComponent<MeshRenderer>().material;
        material.color = Color.green;

        Generate();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();

    }


    void Generate() {

        var vertexIndex = 0;

        void AddVerticies(Tile tile) {

            foreach (var point in tile.verticies) {

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

        }

        void GenerateTriangle(Tile tile) {

            AddVerticies(tile);

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);

            vertexIndex += 3;

        }

        void GenerateSquare(Tile tile) {

            AddVerticies(tile);

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