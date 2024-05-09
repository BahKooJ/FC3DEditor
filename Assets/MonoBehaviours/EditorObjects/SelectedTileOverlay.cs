
using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class SelectedTileOverlay : MonoBehaviour {

    public Main controller;

    public Tile tile;

    Mesh mesh;
    Material material;

    List<Vector3> vertices = new();
    List<int> triangles = new();

    void Start() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        material = GetComponent<MeshRenderer>().material;

        if (MeshType.IDFromVerticies(tile.verticies) == null) {
            material.color = Color.red;
        } else {
            material.color = new Color(0.0f, 0.4f, 0.0f);
        }

        Generate();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();

    }

    public void Refresh() {

        if (mesh == null) {
            return;
        }

        mesh.Clear();
        vertices.Clear();
        triangles.Clear();

        if (MeshType.IDFromVerticies(tile.verticies) == null) {
            material.color = Color.red;
        } else {
            material.color = new Color(0.0f, 0.4f, 0.0f);
        }

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
                        vertices.Add(new Vector3(x: tile.column.x, y: tile.column.heights[0].GetPoint(point.heightChannel), z: tile.column.y));

                        break;
                    case VertexPosition.TopRight:
                        vertices.Add(new Vector3(x: tile.column.x + 1, y: tile.column.heights[1].GetPoint(point.heightChannel), z: tile.column.y));

                        break;
                    case VertexPosition.BottomLeft:
                        vertices.Add(new Vector3(x: tile.column.x, y: tile.column.heights[2].GetPoint(point.heightChannel), z: tile.column.y + 1));

                        break;
                    case VertexPosition.BottomRight:
                        vertices.Add(new Vector3(x: tile.column.x + 1, y: tile.column.heights[3].GetPoint(point.heightChannel), z: tile.column.y + 1));

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