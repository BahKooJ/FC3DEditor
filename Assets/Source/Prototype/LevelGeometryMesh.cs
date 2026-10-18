

using FCopParser;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGeometryMesh : MonoBehaviour {

    [Header("=== Required References ===")]
    [Space]
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshFilter meshFilter;

    // === Parameters ===
    [NonSerialized] Texture2D texture;

    Mesh generatedMesh;

    public LevelGeometryMesh Construct(Texture2D texture) {
        generatedMesh = new();
        meshFilter.mesh = generatedMesh;
        meshRenderer.material.mainTexture = texture;

        return this;
    }

    public void Generate(List<Tile> tiles) {

        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();
        List<Color> colors = new();
        var vertexIndex = 0;

        foreach (var tile in tiles) {

            vertices.AddRange(LevelGeometryUtils.VerticesFromTile(tile));
            triangles.AddRange(LevelGeometryUtils.TrisFromTile(tile, vertexIndex));
            uvs.AddRange(LevelGeometryUtils.UvsFromTile(tile));
            colors.AddRange(LevelGeometryUtils.ColorsFromTile(tile));

            vertexIndex += tile.vertices.Count;

        }

        generatedMesh.vertices = vertices.ToArray();
        generatedMesh.triangles = triangles.ToArray();
        generatedMesh.uv = uvs.ToArray();
        generatedMesh.colors = colors.ToArray();

        generatedMesh.RecalculateNormals();

    }



}