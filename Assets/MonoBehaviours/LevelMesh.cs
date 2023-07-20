using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelMesh : MonoBehaviour {

    Mesh mesh;
    MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    public FCopLevelSection section;
    public Texture2D levelTexturePallet;
    public Main controller;

    public GameObject debugText;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> textureCords = new List<Vector2>();

    public List<Tile> sortedTilesByTriangle = new List<Tile>();

    public float x = 0;
    public float y = 0;

    // Start is called before the first frame update
    void Start() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();

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

                vertexIndex += 4;

            }

            //var tall = -5f;
            foreach (var tile in column.tiles) {


                if (tile.verticies.Count == 0) {

                    //var text = Instantiate(debugText, new Vector3(column.x + 0.5f + this.x, tall, -column.y - 0.5f - this.y), Quaternion.identity);
                    //text.GetComponent<TextMeshPro>().text = tile.parsedTile.number5.ToString();

                    //tall += 0.5f;

                    Debug.Log(tile.parsedTile.meshID.ToString() + " " + x.ToString() + " " + y.ToString());

                } else if (tile.verticies.Count == 3) {
                    GenerateTriangle(tile);
                    sortedTilesByTriangle.Add(tile);
                } else if (tile.verticies.Count == 4) {
                    GenerateSquare(tile);
                    sortedTilesByTriangle.Add(tile);
                    sortedTilesByTriangle.Add(tile);
                }

                //var text = Instantiate(debugText, new Vector3(column.x + 0.5f + this.x, tall, -column.y - 0.5f - this.y), Quaternion.identity);
                //text.GetComponent<TextMeshPro>().text = tile.parsedTile.number5.ToString();

                //tall += 0.5f;

            }

            index++;

        }

    }

    public void RefreshMesh() {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        textureCords.Clear();
        sortedTilesByTriangle.Clear();

        Generate(section);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.uv = textureCords.ToArray();

        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;

    }

    public void RefreshTexture() {
        levelTexturePallet = controller.levelTexturePallet;
        meshRenderer.material.mainTexture = levelTexturePallet;

    }

}