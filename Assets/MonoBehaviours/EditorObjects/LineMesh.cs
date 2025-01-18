using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class LineMesh : MonoBehaviour {

    // - Unity Assets -
    public Material transparentMaterial;
    public Material AdditionMaterial;

    [HideInInspector]
    public MeshCollider meshCollider;

    // - Parameters -
    [HideInInspector]
    public FCopObject.Line line;
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

        var meshRenderer = GetComponent<MeshRenderer>();

        if (line.primitive.Material.visabilityMode == FCopObjectMaterial.VisabilityMode.Transparent) {
            meshRenderer.material = transparentMaterial;
        }
        else if (line.primitive.Material.visabilityMode == FCopObjectMaterial.VisabilityMode.Addition) {
            meshRenderer.material = AdditionMaterial;
        }

        meshRenderer.material.mainTexture = levelTexturePallet;

        Generate();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.uv = textureCords.ToArray();

        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;

    }

    private void LateUpdate() {

        //transform.LookAt(Camera.main.transform);
        //transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

    }

    void Generate() {

        var startVector = new Vector3(line.startPosition.x / ObjectMesh.scale, line.startPosition.y / ObjectMesh.scale, line.startPosition.z / ObjectMesh.scale);
        var endVector = new Vector3(line.endPosition.x / ObjectMesh.scale, line.endPosition.y / ObjectMesh.scale, line.endPosition.z / ObjectMesh.scale);

        var direction = (endVector - startVector).normalized;
        var directionToStart = (startVector - endVector);

        var left = Vector3.Cross(direction, Vector3.up);

        if (left == Vector3.zero) {
            left = Vector3.left;
        }


        vertices.Add(startVector + left * (line.startWidth / ObjectMesh.scale));
        vertices.Add(startVector + -left * (line.startWidth / ObjectMesh.scale));
        vertices.Add(endVector + -left * (line.endWidth / ObjectMesh.scale));
        vertices.Add(endVector + left * (line.endWidth / ObjectMesh.scale));

        if (line.surface.uvMap != null) {

            foreach (var uv in line.surface.uvMap.Value.uvs) {

                var x = uv.x / 256f;
                var y = (uv.y + (256 * line.surface.uvMap.Value.texturePaletteIndex)) / 2580f;
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