
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;

public class TextureUVMapper : MonoBehaviour {

    public TextureEditMode controller;

    public GameObject textureCoordinatePoint;

    // --View References--
    public GameObject texturePaletteDropdown;
    public GameObject texturePalette;
    public GameObject texturePaletteImage;
    public TextureCoordinatesLines textureLines;

    public int bmpID;

    void ScaleToScreen() {

        var screenWidth = Screen.width * 0.40f;
        var screenHeight = Screen.height * 0.80f;

        var multiplierWidth = screenWidth / ((RectTransform)transform).rect.width;

        var multiplierHeight = screenHeight / ((RectTransform)transform).rect.height;

        var multiplier = new float[] { multiplierWidth, multiplierHeight }.Min();

        transform.localScale = new Vector3(multiplier, multiplier, multiplier);

    }

    void Start() {

        InitView();

        ScaleToScreen();

    }

    void Update() {

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0) {

            if (IsCursorInTexturePallete()) {

                var scale = texturePaletteImage.transform.localScale;

                scale.x += axis * 4;
                scale.y += axis * 4;

                texturePaletteImage.transform.localScale = scale;

                var position = texturePaletteImage.transform.localPosition;

                position.x -= 265 * (axis * 2);
                position.y -= 265 * (axis * 2);

                texturePaletteImage.transform.localPosition = position;

            }

        }

        if (Controls.IsDown("DragPalette")) {

            if (IsCursorInTexturePallete()) {

                var position = texturePaletteImage.transform.localPosition;

                position.x += Input.GetAxis("Mouse X") * 18;
                position.y += Input.GetAxis("Mouse Y") * 18;

                texturePaletteImage.transform.localPosition = position;

            }

        }

    }

    bool IsCursorInTexturePallete() {

        Vector2 pointOnPallete = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)texturePalette.transform, Input.mousePosition, Camera.main, out pointOnPallete);

        var palleteRect = (RectTransform)texturePalette.transform;

        return pointOnPallete.x < palleteRect.rect.width && pointOnPallete.y < palleteRect.rect.height;

    }

    void InitTexturePallette() {

        var sameTexture = true;

        // If multiple tiles are select this checks to see if they're all the same texture
        bmpID = controller.selectedTiles[0].texturePalette;
        foreach (var tile in controller.selectedTiles) {

            if (bmpID != tile.texturePalette) {
                sameTexture = false;
                bmpID = -1;
                break;
            }

        }

        if (sameTexture) {

            var bmpID = controller.selectedTiles[0].texturePalette;

            texturePaletteDropdown.GetComponent<TMP_Dropdown>().value = bmpID;

            texturePaletteImage.GetComponent<Image>().sprite = controller.main.bmpTextures[bmpID];

        }

    }

    void InitView() {

        InitTexturePallette();

    }

    public void RefreshView() {

        InitTexturePallette();

        textureLines.ReInit();

    }

    // --Event Handlers--

    public void CloseWindow() {
        controller.selectedSection.RefreshMesh();
        Destroy(gameObject);
    }

    public void OnChangeTexturePalleteValue() {

        controller.ChangeTexturePallette(texturePaletteDropdown.GetComponent<TMP_Dropdown>().value);

        texturePaletteDropdown.GetComponent<TMP_Dropdown>().value = controller.selectedTiles[0].texturePalette;

        InitTexturePallette();

    }

    public void OnClickRotateClockwise() {

        List<Vector2> oldPoints = new();

        foreach (var point in textureLines.points) {
            oldPoints.Add(point.transform.localPosition);
        }

        int index = 1;
        foreach (var point in textureLines.points) {

            var script = point.GetComponent<TextureCoordinatePoint>();

            if (index == oldPoints.Count) {

                var vector = oldPoints[0];

                script.ChangePosition((int)vector.x, (int)vector.y);

            } else {

                var vector = oldPoints[index];

                script.ChangePosition((int)vector.x, (int)vector.y);

            }

            index++;

        }

    }

    public void OnFlipTextureCoordsVertically() {

        var minX = textureLines.points.Min(obj => {
            return obj.transform.localPosition.x;
        });

        var maxX = textureLines.points.Max(obj => {
            return obj.transform.localPosition.x;
        });

        var width = maxX - minX;
        var center = width / 2;

        foreach (var point in textureLines.points) {

            var localX = point.transform.localPosition.x - minX;

            var distanceFromCenter = localX - center;

            var vFlippedX = center - distanceFromCenter;

            var script = point.GetComponent<TextureCoordinatePoint>();

            script.ChangePosition((int)(minX + vFlippedX), (int)point.transform.localPosition.y);

        }

    }

    public void OnFlipTextureCoordsHorizontally() {

        var minY = textureLines.points.Min(obj => {
            return obj.transform.localPosition.y;
        });

        var maxY = textureLines.points.Max(obj => {
            return obj.transform.localPosition.y;
        });

        var height = maxY - minY;
        var center = height / 2;

        foreach (var point in textureLines.points) {

            var localY = point.transform.localPosition.y - minY;

            var distanceFromCenter = localY - center;

            var vFlippedY = center - distanceFromCenter;

            var script = point.GetComponent<TextureCoordinatePoint>();

            script.ChangePosition((int)point.transform.localPosition.x, (int)(minY + vFlippedY));

        }

    }

    public void OnClickRotateCounterClockwise() {

        List<Vector2> oldPoints = new();

        foreach (var point in textureLines.points) {
            oldPoints.Add(point.transform.localPosition);
        }

        int index = oldPoints.Count - 1;
        foreach (var point in textureLines.points) {

            var script = point.GetComponent<TextureCoordinatePoint>();

            if (index == oldPoints.Count - 1) {

                var vector = oldPoints.Last();

                script.ChangePosition((int)vector.x, (int)vector.y);

                index = -1;

            } else {

                var vector = oldPoints[index];

                script.ChangePosition((int)vector.x, (int)vector.y);

            }

            index++;

        }

    }

}