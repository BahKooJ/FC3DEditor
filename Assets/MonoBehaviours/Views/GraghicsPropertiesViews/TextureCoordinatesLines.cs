
using FCopParser;
using System.Linq;
using UnityEngine;

public class TextureCoordinatesLines : MonoBehaviour {
    
    public GraphicsPropertiesView view;

    LineRenderer lineRenderer;

    void Start() {

        view.textureLines = this;

        lineRenderer = GetComponent<LineRenderer>();

        Refresh();

    }

    public void Refresh() {

        if (view.controller.selectedTiles.Count == 0) { return; }

        if (view.controller.selectedTiles.Count > 1) {

            var textureIndex = view.controller.selectedTiles[0].textureIndex;
            foreach (var tile in view.controller.selectedTiles) {

                if (textureIndex != tile.textureIndex) {
                    return;
                }

            }

        }

        var multiplier = 485f / 256f;

        var cords = view.controller.selectedSection.section.textureCoordinates;

        var index = view.controller.selectedTiles[0].textureIndex;

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