using FCopParser;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectMesh : MonoBehaviour {

    const float scale = 512f;

    public FCopObject fCopObject;

    public Main controller;

    Mesh mesh;
    Material material;
    public MeshCollider meshCollider;

    List<Vector3> vertices = new();
    List<int> triangles = new();
    List<Vector2> textureCords = new();

    public bool failed = false;


    void Start() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();

        material = GetComponent<MeshRenderer>().material;

        material.mainTexture = controller.levelTexturePallet;

        try {
            Generate();
        } catch {
            failed = true;
            Debug.LogWarning("Failed to create mesh for object " + fCopObject.rawFile.dataID);
            return;
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.uv = textureCords.ToArray();

        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;

    }

    void Generate() {

        var vertexIndex = 0;

        void AddVertex(FCopObject.FCopVertex vertex) {

            vertices.Add(new Vector3(vertex.x / scale, vertex.y / scale, vertex.z / scale));

        }

        void AddSingleTextureCoord(FCopObject.FCopUVMap uvMap, int index) {

            var x = uvMap.x[index] / 256f;
            var y = (uvMap.y[index] + (256 * uvMap.textureResourceIndex)) / 2560f;

            textureCords.Add(new Vector2(x, y));

        }

        void GenerateTriangle(FCopObject.FCopPolygon polygon) {

            if (polygon.textureIndex / 16 >= fCopObject.uvMaps.Count) {
                Debug.Log(polygon.textureIndex / 16);
                Debug.Log(fCopObject.uvMaps.Count);
                return;

            }

            var uvMap = fCopObject.uvMaps[polygon.textureIndex / 16];

            AddVertex(fCopObject.vertices[polygon.vertices[0]]);
            AddVertex(fCopObject.vertices[polygon.vertices[1]]);
            AddVertex(fCopObject.vertices[polygon.vertices[2]]);
            AddVertex(fCopObject.vertices[polygon.vertices[3]]);

            AddSingleTextureCoord(uvMap, 0);
            AddSingleTextureCoord(uvMap, 1);
            AddSingleTextureCoord(uvMap, 2);
            AddSingleTextureCoord(uvMap, 3);

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);

            vertexIndex += 4;

        }

        void GenerateSquare(FCopObject.FCopPolygon polygon) {

            if (polygon.textureIndex / 16 >= fCopObject.uvMaps.Count) {
                Debug.Log(polygon.textureIndex / 16);
                Debug.Log(fCopObject.uvMaps.Count);
                return;

            }

            var uvMap = fCopObject.uvMaps[polygon.textureIndex / 16];

            AddVertex(fCopObject.vertices[polygon.vertices[0]]);
            AddVertex(fCopObject.vertices[polygon.vertices[1]]);
            AddVertex(fCopObject.vertices[polygon.vertices[2]]);
            AddVertex(fCopObject.vertices[polygon.vertices[3]]);

            AddSingleTextureCoord(uvMap, 0);
            AddSingleTextureCoord(uvMap, 1);
            AddSingleTextureCoord(uvMap, 2);
            AddSingleTextureCoord(uvMap, 3);

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);

            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);
            triangles.Add(vertexIndex);

            vertexIndex += 4;

        }

        foreach (var polygon in fCopObject.polygons) {

            if ((polygon.num1 & 0x07) == 3) {
                GenerateTriangle(polygon);
            } else {
                GenerateSquare(polygon);
            }

        }

    }


}
