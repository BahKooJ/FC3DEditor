using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelMesh : MonoBehaviour {

    class SubMesh {

        public Mesh mesh = new Mesh();
        public MeshRenderer meshRenderer;

        public List<Vector3> vertices = new List<Vector3>();
        public List<int> triangles = new List<int>();
        public List<Vector2> textureCords = new List<Vector2>();
        public List<Color> vertexColors = new();

        public int vertexIndex = 0;

        public void ClearMesh() {
            vertexIndex = 0;
            mesh.Clear();
            vertices.Clear();
            triangles.Clear();
            textureCords.Clear();
            vertexColors.Clear();
        }

        public void SetMesh() {

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.uv = textureCords.ToArray();
            if (Main.showShaders) {
                mesh.colors = vertexColors.ToArray();
            }
            else {
                mesh.colors = new Color[0];
            }

            mesh.RecalculateNormals();

        }

    }

    // Prefabs
    public GameObject subMeshTransparent;

    Mesh mesh;
    MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    public FCopLevelSection section;
    public Texture2D levelTexturePallet;
    public Main controller;

    SubMesh transparentSubMesh;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> textureCords = new List<Vector2>();

    List<Color> vertexColors = new();

    public List<Tile> sortedTilesByTriangle = new List<Tile>();

    public float x = 0;
    public float y = 0;

    // Start is called before the first frame update
    void Start() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();

        transparentSubMesh = new();
        var subMesh = Instantiate(subMeshTransparent);
        subMesh.transform.SetParent(transform, false);
        subMesh.GetComponent<MeshFilter>().mesh = transparentSubMesh.mesh;
        transparentSubMesh.meshRenderer = subMesh.GetComponent<MeshRenderer>();
        transparentSubMesh.meshRenderer.material.mainTexture = levelTexturePallet;

        meshRenderer.material.mainTexture = levelTexturePallet;
        RefreshMesh();

    }

    // Update is called once per frame
    void Update() {
       

    }

    void Generate(FCopLevelSection section) {

        var vertexIndex = 0;
        var index = 0;
        foreach (var column in section.tileColumns) {

            List<Vector3> AddVerticies(Tile tile) {

                var vertices = new List<Vector3>();

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

                return vertices;

            }

            Vector2 GetTextureCoord(Tile tile, int i) {
                return new Vector2(
                        TextureCoordinate.GetX(tile.uvs[i] + tile.texturePalette * 65536),
                        TextureCoordinate.GetY(tile.uvs[i] + tile.texturePalette * 65536)
                    );
            }

            void MakeEmptyTile(bool isQuad) {

                if (isQuad) {

                    textureCords.Add(new Vector2(5f / 256f, 2565f / 2580f));
                    textureCords.Add(new Vector2(10f / 256f, 2565f / 2580f));
                    textureCords.Add(new Vector2(5f / 256f, 2570f / 2580f));
                    textureCords.Add(new Vector2(10f / 256f, 2570f / 2580f));

                    vertexColors.Add(Color.white);
                    vertexColors.Add(Color.white);
                    vertexColors.Add(Color.white);
                    vertexColors.Add(Color.white);



                }
                else {

                    textureCords.Add(new Vector2(5f / 256f, 2565f / 2580f));
                    textureCords.Add(new Vector2(10f / 256f, 2565f / 2580f));
                    textureCords.Add(new Vector2(5f / 256f, 2570f / 2580f));

                    vertexColors.Add(Color.white);
                    vertexColors.Add(Color.white);
                    vertexColors.Add(Color.white);

                }

            }

            void GenerateTriangle(Tile tile) {

                vertices.AddRange(AddVerticies(tile));

                if (tile.graphics.isSemiTransparent == 1 && SettingsManager.showTransparency) {

                    MakeEmptyTile(false);

                    transparentSubMesh.vertices.AddRange(AddVerticies(tile));

                    transparentSubMesh.textureCords.Add(GetTextureCoord(tile, 0));
                    transparentSubMesh.textureCords.Add(GetTextureCoord(tile, 2));
                    transparentSubMesh.textureCords.Add(GetTextureCoord(tile, 1));

                    transparentSubMesh.vertexColors.Add(new Color(tile.shaders.colors[0][0], tile.shaders.colors[0][1], tile.shaders.colors[0][2]));
                    transparentSubMesh.vertexColors.Add(new Color(tile.shaders.colors[1][0], tile.shaders.colors[1][1], tile.shaders.colors[1][2]));
                    transparentSubMesh.vertexColors.Add(new Color(tile.shaders.colors[2][0], tile.shaders.colors[2][1], tile.shaders.colors[2][2]));

                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex);
                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex + 1);
                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex + 2);

                    transparentSubMesh.vertexIndex += 3;

                }
                else {

                    textureCords.Add(GetTextureCoord(tile, 0));
                    textureCords.Add(GetTextureCoord(tile, 2));
                    textureCords.Add(GetTextureCoord(tile, 1));

                    vertexColors.Add(new Color(tile.shaders.colors[0][0], tile.shaders.colors[0][1], tile.shaders.colors[0][2]));
                    vertexColors.Add(new Color(tile.shaders.colors[1][0], tile.shaders.colors[1][1], tile.shaders.colors[1][2]));
                    vertexColors.Add(new Color(tile.shaders.colors[2][0], tile.shaders.colors[2][1], tile.shaders.colors[2][2]));

                }

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);

                vertexIndex += 3;

            }

            void GenerateSquare(Tile tile) {

                vertices.AddRange(AddVerticies(tile));

                if (tile.graphics.isSemiTransparent == 1 && SettingsManager.showTransparency) {

                    MakeEmptyTile(true);

                    transparentSubMesh.vertices.AddRange(AddVerticies(tile));

                    transparentSubMesh.textureCords.Add(GetTextureCoord(tile, 0));
                    transparentSubMesh.textureCords.Add(GetTextureCoord(tile, 1));
                    transparentSubMesh.textureCords.Add(GetTextureCoord(tile, 3));
                    transparentSubMesh.textureCords.Add(GetTextureCoord(tile, 2));

                    transparentSubMesh.vertexColors.Add(new Color(tile.shaders.colors[0][0], tile.shaders.colors[0][1], tile.shaders.colors[0][2]));
                    transparentSubMesh.vertexColors.Add(new Color(tile.shaders.colors[1][0], tile.shaders.colors[1][1], tile.shaders.colors[1][2]));
                    transparentSubMesh.vertexColors.Add(new Color(tile.shaders.colors[2][0], tile.shaders.colors[2][1], tile.shaders.colors[2][2]));
                    transparentSubMesh.vertexColors.Add(new Color(tile.shaders.colors[3][0], tile.shaders.colors[3][1], tile.shaders.colors[3][2]));

                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex);
                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex + 2);
                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex + 1);

                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex + 1);
                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex + 2);
                    transparentSubMesh.triangles.Add(transparentSubMesh.vertexIndex + 3);

                    transparentSubMesh.vertexIndex += 4;

                }
                else {

                    textureCords.Add(GetTextureCoord(tile, 0));
                    textureCords.Add(GetTextureCoord(tile, 1));
                    textureCords.Add(GetTextureCoord(tile, 3));
                    textureCords.Add(GetTextureCoord(tile, 2));

                    vertexColors.Add(new Color(tile.shaders.colors[0][0], tile.shaders.colors[0][1], tile.shaders.colors[0][2]));
                    vertexColors.Add(new Color(tile.shaders.colors[1][0], tile.shaders.colors[1][1], tile.shaders.colors[1][2]));
                    vertexColors.Add(new Color(tile.shaders.colors[2][0], tile.shaders.colors[2][1], tile.shaders.colors[2][2]));
                    vertexColors.Add(new Color(tile.shaders.colors[3][0], tile.shaders.colors[3][1], tile.shaders.colors[3][2]));

                }

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);

                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 3);

                vertexIndex += 4;

            }


            foreach (var tile in column.tiles) {

                if (tile.verticies.Count == 3) {
                    GenerateTriangle(tile);
                    sortedTilesByTriangle.Add(tile);
                } else if (tile.verticies.Count == 4) {
                    GenerateSquare(tile);
                    sortedTilesByTriangle.Add(tile);
                    sortedTilesByTriangle.Add(tile);
                }

            }

            index++;

        }

    }

    public void RefreshMesh() {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        textureCords.Clear();
        vertexColors.Clear();
        sortedTilesByTriangle.Clear();

        transparentSubMesh.ClearMesh();

        Generate(section);

        if (transparentSubMesh.vertices.Count != 0) {
            transparentSubMesh.SetMesh();
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.uv = textureCords.ToArray();
        if (Main.showShaders) {
            mesh.colors = vertexColors.ToArray();
        } else {
            mesh.colors = new Color[0];
        }

        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;

        if (SettingsManager.clipBlack) {
            meshRenderer.material.SetFloat("_ClipBlack", 0.1f);
        } else {
            meshRenderer.material.SetFloat("_ClipBlack", 0f);
        }


    }

    public void RefreshTexture() {
        levelTexturePallet = controller.levelTexturePallet;
        meshRenderer.material.mainTexture = levelTexturePallet;

    }

}