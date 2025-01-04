using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectMesh : MonoBehaviour {

    const float scale = 512f;

    // - Prefabs -
    public GameObject boundingBoxFab;

    // - Parameters -
    public FCopObject fCopObject;
    [HideInInspector]
    public int textureOffset;
    [HideInInspector]
    public Texture levelTexturePallet;

    [HideInInspector]
    public ObjectBoundingBox boundingBox;

    Mesh mesh;
    Material material;
    [HideInInspector]
    public MeshCollider meshCollider;

    List<Vector3> vertices = new();
    List<int> triangles = new();
    List<Vector2> textureCords = new();

    [HideInInspector]
    public bool failed = false;

    [HideInInspector]
    public float maxY = 0f;
    [HideInInspector]
    public float minY = 0f;

    public void Create() {

        try {
            Generate();
        } catch {
            failed = true;
        }

        if (!failed) {

            if (vertices.Count > 0) {

                maxY = vertices.Max(v => { return v.y; });
                minY = vertices.Min(v => { return v.y; });

            }
            else {

                Debug.LogWarning("Object has no vertices: " + fCopObject.rawFile.dataID);

                failed = true;

            }

        }
        else {

            Debug.LogWarning("Failed to create mesh for object " + fCopObject.rawFile.dataID);

        }

    }

    bool alreadyCreated = false;
    void Start() {

        if (alreadyCreated) return;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();
        material = GetComponent<MeshRenderer>().material;

        material.mainTexture = levelTexturePallet;

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

        //var gobj = Instantiate(boundingBoxFab);
        //gobj.transform.SetParent(transform, false);

        //boundingBox = gobj.GetComponent<ObjectBoundingBox>();
        //boundingBox.fCopObject = fCopObject;

    }

    public void ForceMake() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();
        material = GetComponent<MeshRenderer>().material;

        material.mainTexture = levelTexturePallet;

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

        alreadyCreated = true;

    }

    void Generate() {

        var vertexIndex = 0;

        foreach (var triangle in fCopObject.triangles) {

            foreach (var vert in triangle.vertices) {
                vertices.Add(new Vector3(vert.x / scale, vert.y / scale, vert.z / scale));
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
