

using FCopParser;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGeometryMesh : MonoBehaviour {

    [Header("=== Required References ===")]
    [Space]
    [SerializeField] Material semiTransparentMaterial;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshFilter meshFilter;

    // === Parameters ===
    SectionObject sectionObject;

    Mesh generatedMesh;

    public LevelGeometryMesh Construct(SectionObject sectionObject, bool isTransparent) {
        this.sectionObject = sectionObject;

        generatedMesh = new();
        meshFilter.mesh = generatedMesh;

        if (isTransparent) {
            meshRenderer.material = Instantiate(semiTransparentMaterial);
        }

        meshRenderer.material.mainTexture = sectionObject.texture;

        return this;
    }

    public void Generate(List<Tile> tiles) {

        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector4> uvs = new();
        List<Color> colors = new();
        var vertexIndex = 0;

        foreach (var tile in tiles) {

            vertices.AddRange(LevelGeometryUtils.VerticesFromTile(tile));
            triangles.AddRange(LevelGeometryUtils.TrisFromTile(tile, vertexIndex));
            foreach (var uv in LevelGeometryUtils.UvsFromTile(tile)) {
                if (tile.isVectorAnimated) { uvs.Add(new(uv.x, uv.y, sectionObject.section.animationVector.x, sectionObject.section.animationVector.y)); }
                else { uvs.Add(uv); }
            }
            colors.AddRange(LevelGeometryUtils.ColorsFromTile(tile));

            vertexIndex += tile.vertices.Count;

        }

        generatedMesh.vertices = vertices.ToArray();
        generatedMesh.triangles = triangles.ToArray();
        generatedMesh.SetUVs(0, uvs);
        generatedMesh.colors = colors.ToArray();

        generatedMesh.RecalculateNormals();

    }

}