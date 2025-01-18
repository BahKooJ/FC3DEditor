using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectMesh : MonoBehaviour {

    public const float scale = 512f;

    // - Prefabs -
    public GameObject boundingBoxFab;
    public GameObject billboardFab;
    public GameObject lineFab;
    public GameObject starFab;
    public GameObject transparentSubMeshFab;
    public GameObject additionSubMeshFab;

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
    List<Color> colors = new();
    public List<FCopObject.Primitive> sortedPrimitives = new();
    public Dictionary<FCopObject.Primitive, List<FCopObject.Triangle>> trianglesByPrimitive = new();

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
            mesh.colors = colors.ToArray();

            mesh.RecalculateNormals();

            meshCollider.sharedMesh = mesh;

        }

        alreadyCreated = true;

    }

    void Generate() {

        var vertexIndex = 0;

        var transparentTriangles = new List<FCopObject.Triangle>();
        var additionTriangles = new List<FCopObject.Triangle>();


        foreach (var triangle in fCopObject.triangles) {

            foreach (var vert in triangle.vertices) {
                vertices.Add(new Vector3(vert.x / scale, vert.y / scale, vert.z / scale));
            }

            if (triangle.primitive.Material.visabilityMode == FCopObjectMaterial.VisabilityMode.Opaque) {

                if (triangle.uvs.SequenceEqual(new List<FCopObject.UV>() { new(0, 0), new(0, 0), new(0, 0) })) {

                    textureCords.Add(new Vector2(130f / 256f, 2565f / 2580f));
                    textureCords.Add(new Vector2(140f / 256f, 2565f / 2580f));
                    textureCords.Add(new Vector2(130f / 256f, 2570f / 2580f));

                }
                else {

                    foreach (var uv in triangle.uvs) {

                        var x = (uv.x + TextureCoordinate.GetXPixel(textureOffset)) / 256f;
                        var y = (uv.y + TextureCoordinate.GetYPixel(textureOffset) + (256 * triangle.texturePaletteIndex)) / 2580f;
                        textureCords.Add(new Vector2(x, y));

                    }

                }

                colors.Add(new Color(triangle.colors[0][0], triangle.colors[0][1], triangle.colors[0][2]));
                colors.Add(new Color(triangle.colors[1][0], triangle.colors[1][1], triangle.colors[1][2]));
                colors.Add(new Color(triangle.colors[2][0], triangle.colors[2][1], triangle.colors[2][2]));

            }
            else {

                textureCords.Add(new Vector2(5f / 256f, 2565f / 2580f));
                textureCords.Add(new Vector2(10f / 256f, 2565f / 2580f));
                textureCords.Add(new Vector2(5f / 256f, 2570f / 2580f));

                colors.Add(Color.white);
                colors.Add(Color.white);
                colors.Add(Color.white);

            }

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);

            vertexIndex += 3;

            sortedPrimitives.Add(triangle.primitive);

            if (triangle.primitive.Material.visabilityMode == FCopObjectMaterial.VisabilityMode.Transparent) {
                transparentTriangles.Add(triangle);
            }
            else if (triangle.primitive.Material.visabilityMode == FCopObjectMaterial.VisabilityMode.Addition) {
                additionTriangles.Add(triangle);
            }

            if (trianglesByPrimitive.ContainsKey(triangle.primitive)) {
                trianglesByPrimitive[triangle.primitive].Add(triangle);
            }
            else {
                trianglesByPrimitive[triangle.primitive] = new() { triangle };
            }

        }

        if (transparentTriangles.Count > 1) {

            var subMeshGobj = Instantiate(transparentSubMeshFab);
            subMeshGobj.transform.SetParent(transform, false);
            var subMesh = subMeshGobj.GetComponent<ObjectSubMesh>();
            subMesh.fCopTriangles = transparentTriangles;
            subMesh.textureOffset = textureOffset;
            subMesh.levelTexturePallet = levelTexturePallet;
            subMesh.Create();

        }

        if (additionTriangles.Count > 1) {

            var subFMeshGobj = Instantiate(additionSubMeshFab);
            subFMeshGobj.transform.SetParent(transform, false);
            var subFMesh = subFMeshGobj.GetComponent<ObjectSubMesh>();
            subFMesh.fCopTriangles = additionTriangles;
            subFMesh.textureOffset = textureOffset;
            subFMesh.levelTexturePallet = levelTexturePallet;
            subFMesh.Create();

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
