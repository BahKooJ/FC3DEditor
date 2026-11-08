

using FCopParser;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SectionObject : MonoBehaviour {

    [Header("=== Prefabs ===")]
    [Space]
    public GameObject levelGeometryMeshPrefab;

    [Header("=== Required References ===")]
    [Space]
    public MeshCollider tileSelectionCollider;

    // === Parameters ===
    [NonSerialized] public FCopLevelSection section;
    [NonSerialized] public Texture2D texture;
    [NonSerialized] public Main main;
    [NonSerialized] public Vector2 position;
    [NonSerialized] public Vector2Int arrayPosition;

    // === Public Fields ===
    [NonSerialized] public List<Tile> sortedTilesByTriangle = new();

    // === Private Fields ===
    List<LevelGeometryMesh> levelMeshes = new();
    Mesh collisionMesh;

    public SectionObject Construct(FCopLevelSection section, Texture2D texture, Main main, Vector2 position, Vector2Int arrayPosition) {
        this.section = section;
        this.texture = texture;
        this.main = main;
        this.position = position;
        this.arrayPosition = arrayPosition;

        collisionMesh = new();
        tileSelectionCollider.sharedMesh = collisionMesh;

        BuildMeshes();

        return this;
    }

    public void RefreshMesh() {

        foreach (var mesh in levelMeshes) {
            Destroy(mesh.gameObject);
        }

        levelMeshes.Clear();
        sortedTilesByTriangle.Clear();

        BuildMeshes();

    }

    void BuildMeshes() {

        List<Tile> allTiles = new();
        List<Tile> opaqueTiles = new();
        List<Tile> transparentTiles = new();
        List<Tile> vectorAnimatedTiles = new();
        List<Tile> frameAnimationTiles = new();

        foreach (var column in section.tileColumns) {

            foreach (var tile in column.tiles) {

                allTiles.Add(tile);
                if (tile.isSemiTransparent) {
                    transparentTiles.Add(tile);
                }
                else {
                    opaqueTiles.Add(tile);
                }


            }

        }

        CreateMesh(opaqueTiles, false);
        CreateMesh(transparentTiles, true);

        GenerateCollisionMesh(allTiles);

    }

    void CreateMesh(List<Tile> tiles, bool isTransparent) {
        var meshObj = Instantiate(levelGeometryMeshPrefab, transform, false).GetComponent<LevelGeometryMesh>();
        meshObj.Construct(this, isTransparent);
        meshObj.Generate(tiles);

        levelMeshes.Add(meshObj);
    }

    void GenerateCollisionMesh(List<Tile> tiles) {

        List<Vector3> vertices = new();
        List<int> triangles = new();
        int i = 0;

        foreach (var tile in tiles) {
            vertices.AddRange(LevelGeometryUtils.VerticesFromTile(tile));
            triangles.AddRange(LevelGeometryUtils.TrisFromTile(tile, i));

            if (tile.IsQuad) {
                sortedTilesByTriangle.Add(tile);
                sortedTilesByTriangle.Add(tile);
            }
            else {
                sortedTilesByTriangle.Add(tile);
            }

            i += tile.vertices.Count;
        }

        collisionMesh.vertices = vertices.ToArray();
        collisionMesh.triangles = triangles.ToArray();

    }

}