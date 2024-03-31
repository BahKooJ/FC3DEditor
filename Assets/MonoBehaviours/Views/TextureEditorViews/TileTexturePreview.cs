using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class TileTexturePreview : MonoBehaviour {

    public Main controller;

    public Tile tile;
    public TileColumn column;
    public FCopLevelSection section;
    public bool showShaders = false;

    Mesh mesh;
    Material material;

    List<Vector3> vertices = new();
    List<int> triangles = new();
    List<Vector2> textureCords = new();
    List<Color> vertexColors = new();

    AnimatedTile animatedTile = null;

    void Start() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        material = GetComponent<MeshRenderer>().material;

        material.mainTexture = controller.levelTexturePallet;

        Refresh();

    }

    void Update() {

        if (animatedTile == null) {
            return;
        }

        var didChange = false;

        var value = animatedTile.Update(textureCords);

        if (value) {
            didChange = true;
        }


        if (didChange) {
            RefreshCurrentUVs();
        }

    }

    public void Refresh(bool newTexture = false) {

        if (mesh == null) {
            return;
        }

        if (newTexture) {
            material.mainTexture = controller.levelTexturePallet;
        }

        vertexColors.Clear();
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        textureCords.Clear();

        Generate();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.uv = textureCords.ToArray();

        if (showShaders) {
            mesh.colors = vertexColors.ToArray();
        }
        else {
            mesh.colors = new Color[0];
        }

        if (tile.isVectorAnimated) {
            animatedTile = new VectorAnimatedTile(tile, 0, section);
        }
        else if (tile.GetFrameCount() != 0) {
            animatedTile = new FrameAnimatedTile(tile, 0);
        }
        else {
            animatedTile = null;
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

            textureCords.Add(new Vector2(
                TextureCoordinate.GetX(tile.uvs[0] + tile.texturePalette * 65536),
                TextureCoordinate.GetY(tile.uvs[0] + tile.texturePalette * 65536)
            ));

            textureCords.Add(new Vector2(
                TextureCoordinate.GetX(tile.uvs[2] + tile.texturePalette * 65536),
                TextureCoordinate.GetY(tile.uvs[2] + tile.texturePalette * 65536)
            ));

            textureCords.Add(new Vector2(
                TextureCoordinate.GetX(tile.uvs[1] + tile.texturePalette * 65536),
                TextureCoordinate.GetY(tile.uvs[1] + tile.texturePalette * 65536)
            ));

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);

            vertexColors.Add(new Color(tile.shaders.colors[0][0], tile.shaders.colors[0][1], tile.shaders.colors[0][2]));
            vertexColors.Add(new Color(tile.shaders.colors[1][0], tile.shaders.colors[1][1], tile.shaders.colors[1][2]));
            vertexColors.Add(new Color(tile.shaders.colors[2][0], tile.shaders.colors[2][1], tile.shaders.colors[2][2]));

            vertexIndex += 3;

        }

        void GenerateSquare(Tile tile) {

            AddVerticies(tile);

            textureCords.Add(new Vector2(
                TextureCoordinate.GetX(tile.uvs[0] + tile.texturePalette * 65536),
                TextureCoordinate.GetY(tile.uvs[0] + tile.texturePalette * 65536)
            ));

            textureCords.Add(new Vector2(
                TextureCoordinate.GetX(tile.uvs[1] + tile.texturePalette * 65536),
                TextureCoordinate.GetY(tile.uvs[1] + tile.texturePalette * 65536)
            ));

            textureCords.Add(new Vector2(
                TextureCoordinate.GetX(tile.uvs[3] + tile.texturePalette * 65536),
                TextureCoordinate.GetY(tile.uvs[3] + tile.texturePalette * 65536)
            ));

            textureCords.Add(new Vector2(
                TextureCoordinate.GetX(tile.uvs[2] + tile.texturePalette * 65536),
                TextureCoordinate.GetY(tile.uvs[2] + tile.texturePalette * 65536)
            ));



            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);

            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);

            vertexColors.Add(new Color(tile.shaders.colors[0][0], tile.shaders.colors[0][1], tile.shaders.colors[0][2]));
            vertexColors.Add(new Color(tile.shaders.colors[1][0], tile.shaders.colors[1][1], tile.shaders.colors[1][2]));
            vertexColors.Add(new Color(tile.shaders.colors[2][0], tile.shaders.colors[2][1], tile.shaders.colors[2][2]));
            vertexColors.Add(new Color(tile.shaders.colors[3][0], tile.shaders.colors[3][1], tile.shaders.colors[3][2]));

            vertexIndex += 4;

        }

        if (tile.verticies.Count == 3) {
            GenerateTriangle(tile);
        } else if (tile.verticies.Count == 4) {
            GenerateSquare(tile);
        }

    }

    public void RefreshCurrentUVs() {
        mesh.uv = textureCords.ToArray();
    }

}