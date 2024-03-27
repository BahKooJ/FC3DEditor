
using FCopParser;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextureCoordinatesLines : MonoBehaviour {
    
    public TextureUVMapper view;

    LineRenderer lineRenderer;

    public List<GameObject> points = new();

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

        if (view.controller.selectedTiles.Count == 0 || view.editTransparency) { return; }

        GrabTextureCoords();

        if (textureCoords.Count == 0) { return; }

        lineRenderer.positionCount = view.controller.selectedTiles[0].verticies.Count + 1;

        int it = 0;
        foreach (var coord in textureCoords) {

            var point = Instantiate(view.textureCoordinatePoint);

            point.transform.SetParent(view.texturePaletteImage.transform, false);

            point.transform.localPosition = new Vector2(TextureCoordinate.GetXPixel(coord), TextureCoordinate.GetYPixel(coord));

            var script = point.GetComponent<TextureCoordinatePoint>();
            script.uvOffset = it;
            script.textureOffset = coord;
            script.controller = view.controller;
            script.view = view;
            script.imageTransform = (RectTransform)view.texturePaletteImage.transform;
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

    public void RefreshGhostPoints() {
        foreach (var point in points) {
            point.GetComponent<TextureCoordinatePoint>().ChangeGhostPos();
        }
    }

    void GrabTextureCoords() {

        textureCoords.Clear();

        var uvs = view.controller.selectedTiles[0].uvs;
        if (view.controller.selectedTiles.Count > 1) {

            foreach (var tile in view.controller.selectedTiles) {

                if (!uvs.Equals(tile.uvs)) {

                    //TODO: This isn't the best way of doing things because it overwrites immediately. 
                    tile.uvs = new List<int>(uvs);

                }

            }

        }

        textureCoords = new List<int>(uvs);

    }

}