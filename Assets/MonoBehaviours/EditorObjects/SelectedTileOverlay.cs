
using FCopParser;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedTileOverlay : MonoBehaviour {

    public Main controller;

    public Tile tile;
    public FCopLevelSection section;

    public TileEffectType? effectType = null;

    Mesh mesh;
    Material material;

    List<Vector3> vertices = new();
    List<int> triangles = new();
    List<Vector2> textureCords = new();

    void Start() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        material = GetComponent<MeshRenderer>().material;

        Refresh();

    }

    public void Refresh() {

        if (mesh == null) {
            return;
        }

        mesh.Clear();
        vertices.Clear();
        triangles.Clear();

        if (section != null) {

            var doesCaseExist = Enum.IsDefined(typeof(TileEffectType), (int)section.tileEffects[tile.effectIndex]);

            if (doesCaseExist) {

                var effectType = (TileEffectType)section.tileEffects[tile.effectIndex];

                if (effectType != TileEffectType.Normal) {
                    this.effectType = effectType;
                }

            } else {

                effectType = TileEffectType.Other;

            }

        }

        if (MeshType.IDFromVerticies(tile.verticies) == null) {
            material.color = Color.red;
        } else {

            if (effectType != null) {
                material.mainTexture = controller.tileEffectTexture;
                material.color = Color.white;
            } else {
                material.color = new Color(0.0f, 0.4f, 0.0f);
            }

        }

        Generate();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        if (effectType != null) {
            mesh.uv = textureCords.ToArray();
        }

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

            textureCords.Add(new Vector2(0f, 0f));
            textureCords.Add(new Vector2(1f, 0f));
            textureCords.Add(new Vector2(0f, 1f));
            textureCords.Add(new Vector2(1f, 1f));


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