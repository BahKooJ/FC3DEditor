

using FCopParser;
using UnityEngine;

public class ObjectBoundingBox : MonoBehaviour {

    const float scale = 512;

    // - Unity Ref -
    public LineRenderer lineRenderer;

    // - Parameters -
    public FCopObject fCopObject;

    private void Start() {

        var box = fCopObject.boundingBoxes[0];

        var x = box.x / scale;
        var y = box.y / scale;
        var z = box.z / scale;
        var lengthX = box.lengthX / scale;
        var lengthY = box.lengthY / scale;
        var lengthZ = box.lengthZ / scale;

        var startX = x - lengthX;
        var startY = y - lengthY;
        var startZ = z - lengthZ;
        var endX = x + lengthX;
        var endY = y + lengthY;
        var endZ = z + lengthZ;

        lineRenderer.SetPosition(0, new Vector3(startX, startY, startZ));
        lineRenderer.SetPosition(1, new Vector3(endX, startY, startZ));
        lineRenderer.SetPosition(2, new Vector3(endX, startY, endZ));
        lineRenderer.SetPosition(3, new Vector3(startX, startY, endZ));
        lineRenderer.SetPosition(4, new Vector3(startX, startY, startZ));
        lineRenderer.SetPosition(5, new Vector3(startX, endY, startZ));
        lineRenderer.SetPosition(6, new Vector3(endX, endY, startZ));
        lineRenderer.SetPosition(7, new Vector3(endX, startY, startZ));
        lineRenderer.SetPosition(8, new Vector3(endX, endY, startZ));
        lineRenderer.SetPosition(9, new Vector3(endX, endY, endZ));
        lineRenderer.SetPosition(10, new Vector3(endX, startY, endZ));
        lineRenderer.SetPosition(11, new Vector3(endX, endY, endZ));
        lineRenderer.SetPosition(12, new Vector3(startX, endY, endZ));
        lineRenderer.SetPosition(13, new Vector3(startX, startY, endZ));
        lineRenderer.SetPosition(14, new Vector3(startX, endY, endZ));
        lineRenderer.SetPosition(15, new Vector3(startX, endY, startZ));

    }

}