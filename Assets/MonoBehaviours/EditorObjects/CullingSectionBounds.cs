

using System;
using UnityEngine;

public class CullingSectionBounds : MonoBehaviour {

    // --View Refs--
    public LineRenderer lineRenderer;

    public SectionEditMode controller;

    void Start() {

        var data = controller.selectedSection.section.parser.rawFile.data.GetRange(880, 24);

        //8192f

        var div = 1000000f;

        CreateBounds(
            BitConverter.ToUInt32(data.GetRange(0,4).ToArray()) / div,
            BitConverter.ToUInt32(data.GetRange(4, 4).ToArray()) / div,
            BitConverter.ToUInt32(data.GetRange(8, 4).ToArray()) / div,
            BitConverter.ToUInt32(data.GetRange(12, 4).ToArray()) / div,
            BitConverter.ToUInt32(data.GetRange(16, 4).ToArray()) / div,
            BitConverter.ToUInt32(data.GetRange(20, 4).ToArray()) / div
            );

    }


    void CreateBounds(float x, float y, float z, float length, float width, float height) {

        lineRenderer.SetPosition(0, new Vector3(x, y, z));
        lineRenderer.SetPosition(1, new Vector3(x + length, y, z));
        lineRenderer.SetPosition(2, new Vector3(x + length, y, z + width));
        lineRenderer.SetPosition(3, new Vector3(x, y, z + width));
        lineRenderer.SetPosition(4, new Vector3(x, y, z));
        lineRenderer.SetPosition(5, new Vector3(x, y + height, z));
        lineRenderer.SetPosition(6, new Vector3(x + length, y + height, z));
        lineRenderer.SetPosition(7, new Vector3(x + length, y, z));
        lineRenderer.SetPosition(8, new Vector3(x + length, y + height, z));
        lineRenderer.SetPosition(9, new Vector3(x + length, y + height, z + width));
        lineRenderer.SetPosition(10, new Vector3(x + length, y, z + width));
        lineRenderer.SetPosition(11, new Vector3(x + length, y + height, z + width));
        lineRenderer.SetPosition(12, new Vector3(x, y + height, z + width));
        lineRenderer.SetPosition(13, new Vector3(x, y, z + width));
        lineRenderer.SetPosition(14, new Vector3(x, y + height, z + width));
        lineRenderer.SetPosition(15, new Vector3(x, y + height, z));

    }


}