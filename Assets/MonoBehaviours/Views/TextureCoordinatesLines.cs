
using FCopParser;
using System.Linq;
using UnityEngine;

class TextureCoordinatesLines : MonoBehaviour {
    
    public GraphicsPropertiesView view;

    LineRenderer lineRenderer;

    void Start() {

        view.textureLines = this;

        lineRenderer = GetComponent<LineRenderer>();

        Refresh();

    }

    public void Refresh() {

        var multiplier = 485f / 256f;

        var cords = view.controller.selectedSection.section.textureCoordinates;

        var index = view.controller.selectedTile.textureIndex;

        if (cords.Count - index < 4) {
            return;
        }

        foreach (var i in Enumerable.Range(0,4)) {

            var cord = cords[index + i];

            lineRenderer.SetPosition(i, new Vector3(
                (TextureCoordinate.GetXPixel(cord) * multiplier),
                (TextureCoordinate.GetYPixel(cord) * multiplier), -1));

        }

        lineRenderer.SetPosition(4, new Vector3(
            (TextureCoordinate.GetXPixel(cords[index]) * multiplier),
            (TextureCoordinate.GetYPixel(cords[index]) * multiplier), -1));


    }

}