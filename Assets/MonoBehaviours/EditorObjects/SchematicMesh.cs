

using FCopParser;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SchematicMesh : MonoBehaviour {

    // -View Refs-
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public TileAddMode controller;

    public Schematic schematic;

    public List<TileColumn> previewColumns = new List<TileColumn>();

    SubMesh subMesh;

    void Start() {

        subMesh = new() {
            meshRenderer = meshRenderer
        };

        meshRenderer.material.mainTexture = controller.main.levelTexturePallet;

        meshFilter.mesh = subMesh.mesh;

        RefreshPreviewColumns();

        RefreshMesh();

    }

    public void ForceMake() {

        subMesh = new() {
            meshRenderer = meshRenderer
        };

        meshRenderer.material.mainTexture = controller.main.levelTexturePallet;

        meshFilter.mesh = subMesh.mesh;

        RefreshPreviewColumns();

        RefreshMesh();

    }

    public TileColumn GetTileColumn(int x, int y) {
        return previewColumns[(y * schematic.width) + x];
    }

    public void RefreshMesh() {

        subMesh.ClearMesh();

        Generate();

        subMesh.SetMesh();

        if (SettingsManager.clipBlack) {
            subMesh.meshRenderer.material.SetFloat("_ClipBlack", 0.1f);
        } else {
            subMesh.meshRenderer.material.SetFloat("_ClipBlack", 0f);
        }

    }

    public void RefreshPreviewColumns() {

        previewColumns.Clear();

        if (schematic.transformedSchematic != null) {

            foreach (var column in schematic.transformedSchematic.tileColumns) {

                previewColumns.Add(column.CloneWithHeights());

            }

        }
        else {

            foreach (var column in schematic.tileColumns) {

                previewColumns.Add(column.CloneWithHeights());

            }

        }



    }

    void Generate() {

        var vertexIndex = 0;
        var index = 0;
        foreach (var column in previewColumns) {

            List<Vector3> AddVerticies(Tile tile) {

                var vertices = new List<Vector3>();

                foreach (var point in tile.verticies) {

                    switch (point.vertexPosition) {

                        case VertexPosition.TopLeft:
                            subMesh.vertices.Add(new Vector3(x: column.x, y: column.heights[0].GetPoint(point.heightChannel), z: column.y));

                            break;
                        case VertexPosition.TopRight:
                            subMesh.vertices.Add(new Vector3(x: column.x + 1, y: column.heights[1].GetPoint(point.heightChannel), z: column.y));

                            break;
                        case VertexPosition.BottomLeft:
                            subMesh.vertices.Add(new Vector3(x: column.x, y: column.heights[2].GetPoint(point.heightChannel), z: column.y + 1));

                            break;
                        case VertexPosition.BottomRight:
                            subMesh.vertices.Add(new Vector3(x: column.x + 1, y: column.heights[3].GetPoint(point.heightChannel), z: column.y + 1));

                            break;
                    }

                }

                return vertices;

            }

            Vector2 GetTextureCoord(Tile tile, int i) {
                return new Vector2(
                        TextureCoordinate.GetX(tile.uvs[i] + tile.texturePalette * 65536),
                        TextureCoordinate.GetY(tile.uvs[i] + tile.texturePalette * 65536)
                    );
            }

            void GenerateTriangle(Tile tile) {

                subMesh.vertices.AddRange(AddVerticies(tile));

                if (tile.GetFrameCount() > 0 && SettingsManager.showAnimations) {
                    subMesh.animatedTiles.Add(new FrameAnimatedTile(tile, vertexIndex));
                }

                subMesh.textureCords.Add(GetTextureCoord(tile, 0));
                subMesh.textureCords.Add(GetTextureCoord(tile, 2));
                subMesh.textureCords.Add(GetTextureCoord(tile, 1));

                subMesh.vertexColors.Add(new Color(tile.shaders.colors[0][0], tile.shaders.colors[0][1], tile.shaders.colors[0][2]));
                subMesh.vertexColors.Add(new Color(tile.shaders.colors[1][0], tile.shaders.colors[1][1], tile.shaders.colors[1][2]));
                subMesh.vertexColors.Add(new Color(tile.shaders.colors[2][0], tile.shaders.colors[2][1], tile.shaders.colors[2][2]));



                subMesh.triangles.Add(vertexIndex);
                subMesh.triangles.Add(vertexIndex + 1);
                subMesh.triangles.Add(vertexIndex + 2);

                vertexIndex += 3;

            }

            void GenerateSquare(Tile tile) {

                subMesh.vertices.AddRange(AddVerticies(tile));

                if (tile.GetFrameCount() > 0 && SettingsManager.showAnimations) {
                    subMesh.animatedTiles.Add(new FrameAnimatedTile(tile, vertexIndex));
                }

                if (tile.shaders.type == VertexColorType.ColorAnimated) {
                    subMesh.shaderAnimatedTiles.Add(new ShaderAnimatedTile(tile, vertexIndex));
                }

                subMesh.textureCords.Add(GetTextureCoord(tile, 0));
                subMesh.textureCords.Add(GetTextureCoord(tile, 1));
                subMesh.textureCords.Add(GetTextureCoord(tile, 3));
                subMesh.textureCords.Add(GetTextureCoord(tile, 2));

                subMesh.vertexColors.Add(new Color(tile.shaders.colors[0][0], tile.shaders.colors[0][1], tile.shaders.colors[0][2]));
                subMesh.vertexColors.Add(new Color(tile.shaders.colors[1][0], tile.shaders.colors[1][1], tile.shaders.colors[1][2]));
                subMesh.vertexColors.Add(new Color(tile.shaders.colors[2][0], tile.shaders.colors[2][1], tile.shaders.colors[2][2]));
                subMesh.vertexColors.Add(new Color(tile.shaders.colors[3][0], tile.shaders.colors[3][1], tile.shaders.colors[3][2]));



                subMesh.triangles.Add(vertexIndex);
                subMesh.triangles.Add(vertexIndex + 2);
                subMesh.triangles.Add(vertexIndex + 1);

                subMesh.triangles.Add(vertexIndex + 1);
                subMesh.triangles.Add(vertexIndex + 2);
                subMesh.triangles.Add(vertexIndex + 3);

                vertexIndex += 4;

            }

            foreach (var tile in column.tiles) {

                if (tile.verticies.Count == 3) {
                    GenerateTriangle(tile);
                } else if (tile.verticies.Count == 4) {
                    GenerateSquare(tile);
                }

            }

            index++;

        }

    }

}