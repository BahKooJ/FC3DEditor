

using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class UniversalUVLines : MonoBehaviour {

    // - Unity Ref -
    public LineRenderer lineRenderer;

    // - Parameters -
    [HideInInspector]
    public UniversalUVMapperView view;

    void Start() {
        Refresh();
    }

    public void Refresh() {
        
        var positions = new List<Vector3>();

        foreach (var uv in view.uvs) {

            positions.Add(new Vector3(TextureCoordinate.GetXPixel(uv), TextureCoordinate.GetYPixel(uv), -1));

        }

        positions.Add(new Vector3(TextureCoordinate.GetXPixel(view.uvs[0]), TextureCoordinate.GetYPixel(view.uvs[0]), -1));

        lineRenderer.SetPositions(positions.ToArray());

    }

}