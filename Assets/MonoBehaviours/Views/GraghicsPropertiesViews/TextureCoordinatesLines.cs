
using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TextureCoordinatesLines : MonoBehaviour {
    
    public GraphicsPropertiesView view;

    LineRenderer lineRenderer;

    public List<GameObject> points = new();

    void Start() {

        view.textureLines = this;

        lineRenderer = GetComponent<LineRenderer>();

        ReInit();

    }

    public void ReInit() {

        foreach (var point in points) {
            Destroy(point);
        }

        points.Clear();

        if (view.controller.selectedTiles.Count == 0) { return; }

        if (view.controller.selectedTiles.Count > 1) {

            var textureIndex = view.controller.selectedTiles[0].textureIndex;
            foreach (var tile in view.controller.selectedTiles) {

                if (textureIndex != tile.textureIndex) {
                    return;
                }

            }

        }

        var cords = view.controller.selectedSection.section.textureCoordinates;

        var index = view.controller.selectedTiles[0].textureIndex;

        if (cords.Count - index < 4) {
            return;
        }

        lineRenderer.positionCount = view.controller.selectedTiles[0].verticies.Count + 1;

        foreach (var i in Enumerable.Range(0, view.controller.selectedTiles[0].verticies.Count)) {

            var cord = cords[index + i];

            var point = Instantiate(view.textureCoordinatePoint);

            point.transform.SetParent(view.texturePalleteImage.transform, false);

            point.transform.localPosition = new Vector2(TextureCoordinate.GetXPixel(cord), TextureCoordinate.GetYPixel(cord));

            var script = point.GetComponent<TextureCoordinatePoint>();
            script.index = index + i;
            script.textureOffset = cord;
            script.controller = view.controller;
            script.imageTransform = (RectTransform)view.texturePalleteImage.transform;
            script.lines = this;

            var image = point.GetComponent<Image>();

            switch (i) {
                case 0:
                    image.color = Color.blue;
                    break;
                case 1:
                    image.color = Color.green;
                    break;
                case 2:
                    image.color = Color.red;
                    break;
                case 3:
                    image.color = Color.magenta;
                    break;
            }

            points.Add(point);

            lineRenderer.SetPosition(i, new Vector3(
                TextureCoordinate.GetXPixel(cord),
                TextureCoordinate.GetYPixel(cord), -1));

        }

        lineRenderer.SetPosition(view.controller.selectedTiles[0].verticies.Count, new Vector3(
            TextureCoordinate.GetXPixel(cords[index]),
            TextureCoordinate.GetYPixel(cords[index]), -1));

    }

    public void Refresh() {

        var cords = view.controller.selectedSection.section.textureCoordinates;

        var index = view.controller.selectedTiles[0].textureIndex;

        foreach (var i in Enumerable.Range(0, view.controller.selectedTiles[0].verticies.Count)) {

            var cord = cords[index + i];

            lineRenderer.SetPosition(i, new Vector3(
                TextureCoordinate.GetXPixel(cord),
                TextureCoordinate.GetYPixel(cord), -1));

        }

        lineRenderer.SetPosition(view.controller.selectedTiles[0].verticies.Count, new Vector3(
            TextureCoordinate.GetXPixel(cords[index]),
            TextureCoordinate.GetYPixel(cords[index]), -1));


    }

}