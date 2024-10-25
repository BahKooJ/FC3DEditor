

using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class BillboardMesh : MonoBehaviour {


    public Material material;
    public MeshCollider meshCollider;

    // - Parameters -
    public FCopObject fCopObject;
    public Texture levelTexturePallet;

    Mesh mesh;
    List<Vector3> vertices = new();
    List<int> triangles = new();
    List<Vector2> textureCords = new();

    private void Start() {

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

    }

    void Generate() {



    }

}