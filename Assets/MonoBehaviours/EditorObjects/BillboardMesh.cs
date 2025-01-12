

using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class BillboardMesh : MonoBehaviour {

    [HideInInspector]
    public Material material;
    [HideInInspector]
    public MeshCollider meshCollider;

    // - Parameters -
    [HideInInspector]
    public FCopObject.Billboard billboard;
    [HideInInspector]
    public Texture levelTexturePallet;

    Mesh mesh;
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

    private void LateUpdate() {

        transform.LookAt(Camera.main.transform);

    }

    void Generate() {

        transform.localPosition = new Vector3(billboard.position.x / ObjectMesh.scale, billboard.position.y / ObjectMesh.scale, billboard.position.z / ObjectMesh.scale);

        vertices.Add(new Vector3(-(billboard.length / ObjectMesh.scale), billboard.length / ObjectMesh.scale, 0.0f));
        vertices.Add(new Vector3(billboard.length / ObjectMesh.scale, billboard.length / ObjectMesh.scale, 0.0f));
        vertices.Add(new Vector3(billboard.length / ObjectMesh.scale, -(billboard.length / ObjectMesh.scale), 0.0f));
        vertices.Add(new Vector3(-(billboard.length / ObjectMesh.scale), -(billboard.length / ObjectMesh.scale), 0.0f));

        if (billboard.surface.uvMap != null) {

            foreach (var uv in billboard.surface.uvMap.Value.uvs) {

                var x = uv.x / 256f;
                var y = (uv.y + (256 * billboard.surface.uvMap.Value.texturePaletteIndex)) / 2580f;
                textureCords.Add(new Vector2(x, y));

            }

        }

        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);

        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(3);

    }

}