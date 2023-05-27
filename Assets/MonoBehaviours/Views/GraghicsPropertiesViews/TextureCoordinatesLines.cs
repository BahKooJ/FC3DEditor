
using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TextureCoordinatesLines : MonoBehaviour {
    
    public GraphicsPropertiesView view;

    LineRenderer lineRenderer;

    public List<GameObject> points = new();

    int index = 0;
    List<int> textureCoords = new();

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

        GrabTextureCoords();

        if (textureCoords.Count == 0) { return; }

        lineRenderer.positionCount = view.controller.selectedTiles[0].verticies.Count + 1;

        int it = 0;
        foreach (var coord in textureCoords) {

            var point = Instantiate(view.textureCoordinatePoint);

            point.transform.SetParent(view.texturePalleteImage.transform, false);

            point.transform.localPosition = new Vector2(TextureCoordinate.GetXPixel(coord), TextureCoordinate.GetYPixel(coord));

            var script = point.GetComponent<TextureCoordinatePoint>();
            script.index = index + it;
            script.textureOffset = coord;
            script.controller = view.controller;
            script.view = view;
            script.isGlobalPoint = view.globalTextureCoordIndex == null ? false : true;
            script.imageTransform = (RectTransform)view.texturePalleteImage.transform;
            script.lines = this;

            var image = point.GetComponent<Image>();

            switch (it) {
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

            lineRenderer.SetPosition(it, new Vector3(
                TextureCoordinate.GetXPixel(coord),
                TextureCoordinate.GetYPixel(coord), -1));

            it++;

        }

        lineRenderer.SetPosition(view.controller.selectedTiles[0].verticies.Count, new Vector3(
            TextureCoordinate.GetXPixel(textureCoords[0]),
            TextureCoordinate.GetYPixel(textureCoords[0]), -1));

    }

    public void Refresh() {

        GrabTextureCoords();

        if (textureCoords.Count == 0) { return; }

        int it = 0;
        foreach (var coord in textureCoords) {

            lineRenderer.SetPosition(it, new Vector3(
                TextureCoordinate.GetXPixel(coord),
                TextureCoordinate.GetYPixel(coord), -1));

            it++;
        }

        lineRenderer.SetPosition(view.controller.selectedTiles[0].verticies.Count, new Vector3(
            TextureCoordinate.GetXPixel(textureCoords[0]),
            TextureCoordinate.GetYPixel(textureCoords[0]), -1));


    }

    void GrabTextureCoords() {

        textureCoords.Clear();

        List<int> coords;

        if (view.globalTextureCoordIndex != null) {

            coords = GraphicsPropertiesView.textureCoordsClipboard;

            index = (int)view.globalTextureCoordIndex;

            var vertexCount = view.controller.selectedTiles[0].verticies.Count;

            if (coords.Count - index < vertexCount) {
                return;
            }

            foreach (var i in Enumerable.Range(0, vertexCount)) {

                var coord = coords[index + i];

                textureCoords.Add(coord);
            }

        } else {

            if (view.controller.selectedTiles.Count > 1) {

                var textureIndex = view.controller.selectedTiles[0].textureIndex;
                foreach (var tile in view.controller.selectedTiles) {

                    if (textureIndex != tile.textureIndex) {
                        return;
                    }

                }

            }

            coords = view.controller.selectedSection.section.textureCoordinates;

            index = view.controller.selectedTiles[0].textureIndex;

            var vertexCount = view.controller.selectedTiles[0].verticies.Count;

            if (coords.Count - index < vertexCount) {
                return;
            }

            foreach (var i in Enumerable.Range(0, vertexCount)) {

                var coord = coords[index + i];

                textureCoords.Add(coord);
            }

        }

    }

}