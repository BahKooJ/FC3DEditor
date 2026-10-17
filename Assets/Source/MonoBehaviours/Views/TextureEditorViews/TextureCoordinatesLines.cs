
using FCopParser;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextureCoordinatesLines : MonoBehaviour {
    
    public TextureUVMapper view;

    LineRenderer lineRenderer;

    public List<TextureCoordinatePoint> points = new();

    List<int> textureCoords = new();

    void Start() {

        view.textureLines = this;

        lineRenderer = GetComponent<LineRenderer>();

        ReInit();

    }

    public void ReInit() {

        if (lineRenderer == null) {
            return;
        }

        foreach (var point in points) {
            Destroy(point.gameObject);
        }

        points.Clear();

        if (view.editTransparency) { return; }

        GrabTextureCoords();

        if (textureCoords.Count == 0) { return; }

        if (view.controller.HasSelection) {
            lineRenderer.positionCount = view.controller.FirstTile.verticies.Count + 1;
        }
        else {
            lineRenderer.positionCount = 5;
        }


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

            points.Add(script);

            lineRenderer.SetPosition(it, new Vector3(
                TextureCoordinate.GetXPixel(coord),
                TextureCoordinate.GetYPixel(coord), -1));

            it++;

        }

        lineRenderer.SetPosition(textureCoords.Count, new Vector3(
            TextureCoordinate.GetXPixel(textureCoords[0]),
            TextureCoordinate.GetYPixel(textureCoords[0]), -1));

    }

    public void ReInit(List<int> uvs) {

        if (lineRenderer == null) {
            return;
        }

        foreach (var point in points) {
            Destroy(point.gameObject);
        }

        points.Clear();

        if (view.editTransparency) { return; }

        textureCoords = new List<int>(uvs);

        if (textureCoords.Count == 0) { return; }
        
        lineRenderer.positionCount = textureCoords.Count + 1;

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

            points.Add(script);

            lineRenderer.SetPosition(it, new Vector3(
                TextureCoordinate.GetXPixel(coord),
                TextureCoordinate.GetYPixel(coord), -1));

            it++;

        }

        lineRenderer.SetPosition(textureCoords.Count, new Vector3(
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

        lineRenderer.SetPosition(textureCoords.Count, new Vector3(
            TextureCoordinate.GetXPixel(textureCoords[0]),
            TextureCoordinate.GetYPixel(textureCoords[0]), -1));


    }

    public void RefreshGhostPoints() {
        foreach (var point in points) {
            point.ChangeGhostPos();
        }
    }

    // This is used for UV drag
    public void SetUVs(int x, int y, int startX, int startY) {

        points[0].ChangePosition(startX, startY);
        points[1].ChangePosition(x, startY);
        points[2].ChangePosition(x, y);

        if (points.Count > 3) {
            points[3].ChangePosition(startX, y);
        }

    }

    void GrabTextureCoords() {

        textureCoords.Clear();

        if (view.controller.HasSelection) {

            if (view.frameSelected != -1) {
                var animatedUVs = view.controller.FirstTile.animatedUVs.GetRange(view.frameSelected * 4, 4);
                textureCoords = new List<int>(animatedUVs);
                return;
            }

            var uvs = view.controller.FirstTile.uvs;

            textureCoords = new List<int>(uvs);

        }
        else if (points.Count == 0) {
            textureCoords = new List<int>() { 57200, 57228, 50060, 50032 };
        }
        else {

            var uvs = new List<int>();
            foreach (var point in points) {

                uvs.Add(TextureCoordinate.SetPixel((int)point.transform.localPosition.x, (int)point.transform.localPosition.y));

            }

            textureCoords = new List<int>(uvs);

        }

    }

}