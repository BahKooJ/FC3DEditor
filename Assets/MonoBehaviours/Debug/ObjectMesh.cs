using FCopParser;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectMesh : MonoBehaviour {

    const float scale = 0.01f;

    public FCopObject fCopObject;

    public ObjectDebug controller;

    Mesh mesh;
    Material material;

    List<Vector3> vertices = new();
    List<int> triangles = new();
    List<Vector2> textureCords = new();


    void Start() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        material = GetComponent<MeshRenderer>().material;

        material.mainTexture = controller.levelTexturePallet;

        Generate();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.uv = textureCords.ToArray();

        mesh.RecalculateNormals();

    }

    void Generate() {

        var vertexIndex = 0;

        void AddVertex(FCopObject.FCopVertex vertex) {

            vertices.Add(new Vector3(vertex.x * scale, vertex.y * scale, vertex.z * scale));

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

            textureCords.Add(new Vector2(uvMap.x[0] / 256, (uvMap.y[0] + (256 * uvMap.textureResourceIndex)) / 2048));
            textureCords.Add(new Vector2(uvMap.x[1] / 256, (uvMap.y[1] + (256 * uvMap.textureResourceIndex)) / 2048));
            textureCords.Add(new Vector2(uvMap.x[2] / 256, (uvMap.y[2] + (256 * uvMap.textureResourceIndex)) / 2048));

            triangles.Add(polygon.vertices[0]);
            triangles.Add(polygon.vertices[1]);
            triangles.Add(polygon.vertices[2]);

            vertexIndex += 3;

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

            textureCords.Add(new Vector2(uvMap.x[0] / 256, (uvMap.y[0] + (256 * uvMap.textureResourceIndex)) / 2048));
            textureCords.Add(new Vector2(uvMap.x[1] / 256, (uvMap.y[1] + (256 * uvMap.textureResourceIndex)) / 2048));
            textureCords.Add(new Vector2(uvMap.x[2] / 256, (uvMap.y[2] + (256 * uvMap.textureResourceIndex)) / 2048));
            textureCords.Add(new Vector2(uvMap.x[3] / 256, (uvMap.y[3] + (256 * uvMap.textureResourceIndex)) / 2048));

            triangles.Add(polygon.vertices[0]);
            triangles.Add(polygon.vertices[1]);
            triangles.Add(polygon.vertices[2]);

            triangles.Add(polygon.vertices[2]);
            triangles.Add(polygon.vertices[3]);
            triangles.Add(polygon.vertices[0]);

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
