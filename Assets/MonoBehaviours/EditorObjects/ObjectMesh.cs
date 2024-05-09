using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectMesh : MonoBehaviour {

    const float scale = 512f;

    public FCopObject fCopObject;

    public int textureOffset;

    public Main controller;

    Mesh mesh;
    Material material;
    public MeshCollider meshCollider;

    List<Vector3> vertices = new();
    List<int> triangles = new();
    List<Vector2> textureCords = new();

    public bool failed = false;

    public float maxY = 0f;
    public float minY = 0f;

    public void Create() {

        try {
            Generate();
        } catch {
            failed = true;
            return;
        }

        var verticesSet = new HashSet<Vector3>(vertices);

        if (verticesSet.Count < 3) {
            failed = true;
        }

        if (!failed) {

            maxY = vertices.Max(v => { return v.y; });
            minY = vertices.Min(v => { return v.y; });

        } else {

            Debug.LogWarning("Failed to create mesh for object " + fCopObject.rawFile.dataID);

        }

    }

    void Start() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();

        material = GetComponent<MeshRenderer>().material;

        material.mainTexture = controller.levelTexturePallet;

        if (vertices.Count == 0 && failed == false) {
            Create();
        }

        if (!failed) {

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.uv = textureCords.ToArray();

            mesh.RecalculateNormals();

            meshCollider.sharedMesh = mesh;

        }

    }

    void Generate() {

        var vertexIndex = 0;

        void AddVertex(FCopObject.FCopVertex vertex) {

            vertices.Add(new Vector3(vertex.x / scale, vertex.y / scale, vertex.z / scale));

        }

        void AddSingleTextureCoord(FCopObject.FCopUVMap uvMap, int index) {

            var x = (uvMap.x[index] + TextureCoordinate.GetXPixel(textureOffset)) / 256f;
            var y = (uvMap.y[index] + TextureCoordinate.GetYPixel(textureOffset) + (256 * uvMap.textureResourceIndex)) / 2580f;

            textureCords.Add(new Vector2(x, y));

        }

        void GenerateTriangle(FCopObject.FCopPolygon polygon) {

            if (polygon.textureIndex / 16 >= fCopObject.uvMaps.Count) {
                failed = true;
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
                failed = true;
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
