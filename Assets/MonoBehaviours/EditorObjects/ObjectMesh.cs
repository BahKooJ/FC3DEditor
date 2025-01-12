using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FCopParser.FCopObject;

public class ObjectMesh : MonoBehaviour {

    public const float scale = 512f;

    // - Prefabs -
    public GameObject boundingBoxFab;
    public GameObject billboardFab;
    public GameObject lineFab;
    public GameObject starFab;

    // - Parameters -
    public FCopObject fCopObject;
    [HideInInspector]
    public int textureOffset;
    [HideInInspector]
    public Texture levelTexturePallet;

    [HideInInspector]
    public ObjectBoundingBox boundingBox;
    [HideInInspector]
    public List<GameObject> specialPrimitives = new();


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

    bool alreadyCreated = false;
    void Start() {

        if (alreadyCreated) return;

        ForceMake();

        //var gobj = Instantiate(boundingBoxFab);
        //gobj.transform.SetParent(transform, false);

        //boundingBox = gobj.GetComponent<ObjectBoundingBox>();
        //boundingBox.fCopObject = fCopObject;

    }

    void Create() {

        //try {
            Generate();
        //}
        //catch {
            //failed = true;
        //}

        if (!failed) {

            if (vertices.Count > 0) {

                maxY = vertices.Max(v => { return v.y; });
                minY = vertices.Min(v => { return v.y; });

            }
            else if (specialPrimitives.Count == 0) {

                Debug.LogWarning("Object has no primitives: " + fCopObject.rawFile.dataID);

                failed = true;

            }

        }
        else {

            Debug.LogWarning("Failed to create mesh for object " + fCopObject.rawFile.dataID);

        }

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

        foreach (var billboard in fCopObject.billboards) {

            var gobj = Instantiate(billboardFab);
            gobj.transform.SetParent(transform, false);
            var billboardMesh = gobj.GetComponent<BillboardMesh>();
            billboardMesh.billboard = billboard;
            billboardMesh.levelTexturePallet = levelTexturePallet;
            specialPrimitives.Add(gobj);
            billboardMesh.Create();

        }

        foreach (var line in fCopObject.lines) {

            var gobj = Instantiate(lineFab);
            gobj.transform.SetParent(transform, false);
            var lineMesh = gobj.GetComponent<LineMesh>();
            lineMesh.line = line;
            lineMesh.levelTexturePallet = levelTexturePallet;
            specialPrimitives.Add(gobj);
            lineMesh.Create();

        }

        foreach (var star in fCopObject.stars) {

            var gobj = Instantiate(starFab);
            gobj.transform.SetParent(transform, false);
            var starMesh = gobj.GetComponent<StarMesh>();
            starMesh.star = star;
            specialPrimitives.Add(gobj);
            starMesh.Create();

        }

    }


}
