using FCopParser;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMesh : MonoBehaviour {

    const float scale = 0.01f;

    public FCopObject fCopObject;

    Mesh mesh;

    List<Vector3> vertices = new();
    List<int> triangles = new();

    void Start() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Generate();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();

    }

    void Generate() {

        var vertexIndex = 0;

        void GenerateTriangle(FCopObject.FCopPolygon polygon) {

            triangles.Add(polygon.vertices[0]);
            triangles.Add(polygon.vertices[1]);
            triangles.Add(polygon.vertices[2]);

            vertexIndex += 3;

        }

        void GenerateSquare(FCopObject.FCopPolygon polygon) {

            triangles.Add(polygon.vertices[0]);
            triangles.Add(polygon.vertices[1]);
            triangles.Add(polygon.vertices[2]);

            triangles.Add(polygon.vertices[2]);
            triangles.Add(polygon.vertices[3]);
            triangles.Add(polygon.vertices[0]);

            vertexIndex += 4;

        }

        foreach (var vertex in fCopObject.vertices) {

            vertices.Add(new Vector3(vertex.x * scale, vertex.y * scale, vertex.z * scale));

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
