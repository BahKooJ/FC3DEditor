

using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FCopParser.FCopObject;

public class StarMesh : MonoBehaviour {

    [HideInInspector]
    public Material material;
    [HideInInspector]
    public MeshCollider meshCollider;

    // - Parameters -
    [HideInInspector]
    public FCopObject.Star star;

    Mesh mesh;
    List<Vector3> vertices = new();
    List<int> triangles = new();
    List<Color> colors = new();

    public void Create() {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();
        material = GetComponent<MeshRenderer>().material;

        Generate();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();

        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;

    }

    private void LateUpdate() {

        transform.LookAt(Camera.main.transform);
        transform.Rotate(Vector3.up, 90f, Space.Self);

    }

    void Generate() {

        transform.localPosition = new Vector3(star.position.x / ObjectMesh.scale, star.position.y / ObjectMesh.scale, star.position.z / ObjectMesh.scale);

        var color = new Color(star.red / 255f, star.green / 255f, star.blue / 255f);

        var angleI = 360f / star.triCount;

        var vertexIndex = 0;
        var angle = 0f;
        foreach (var i in Enumerable.Range(0, star.triCount)) {

            vertices.Add(new Vector3(0, 0, 0));

            var rot = Quaternion.AngleAxis(angle, Vector3.right);
            var lDirection = rot * Vector3.forward;
            vertices.Add(lDirection * (star.length / ObjectMesh.scale));

            var rot2 = Quaternion.AngleAxis(angle + angleI, Vector3.right);
            var lDirection2 = rot2 * Vector3.forward;
            vertices.Add(lDirection2 * (star.length / ObjectMesh.scale));

            angle += angleI;

            colors.Add(color);
            colors.Add(new Color(color.r, color.g, color.b, 0f));
            colors.Add(new Color(color.r, color.g, color.b, 0f));

            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);

            vertexIndex += 3;

        }

    }

}