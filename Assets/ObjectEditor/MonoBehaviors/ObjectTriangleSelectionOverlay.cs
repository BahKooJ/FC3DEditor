

using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTriangleSelectionOverlay : MonoBehaviour {

    // - Parameters -
    public List<FCopObject.Triangle> fCopTriangles;
    [HideInInspector]
    public int textureOffset;
    [HideInInspector]
    public Texture levelTexturePallet;

    Mesh mesh;
    Material material;
    [HideInInspector]
    public MeshCollider meshCollider;

    List<Vector3> vertices = new();
    List<int> triangles = new();
    List<Vector2> textureCords = new();


    public void Create() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();
        material = GetComponent<MeshRenderer>().material;

        material.mainTexture = levelTexturePallet;

        Generate();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.uv = textureCords.ToArray();

        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;

    }

    void Generate() {

        var vertexIndex = 0;

        foreach (var triangle in fCopTriangles) {
            foreach (var vert in triangle.vertices) {
                vertices.Add(new Vector3(vert.x / ObjectMesh.scale, vert.y / ObjectMesh.scale, vert.z / ObjectMesh.scale));
            }
            foreach (var uv in triangle.uvs) {

                var x = (uv.x + TextureCoordinate.GetXPixel(textureOffset)) / 256f;
                var y = (uv.y + TextureCoordinate.GetYPixel(textureOffset) + (256 * triangle.texturePaletteIndex)) / 2580f;
                textureCords.Add(new Vector2(x, y));
            }

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);

            vertexIndex += 3;

        }

    }

}