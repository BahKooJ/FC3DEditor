
using FCopParser;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UniversalUVView : MonoBehaviour {

    // - Parameters -
    [HideInInspector]
    public UniversalUVMapperView view;
    [HideInInspector]
    public int index;

    bool drag = false;

    public void ChangePosition(int x, int y, bool modify) {

        transform.localPosition = new Vector2(x, y);

        var pos = transform.localPosition;

        if (pos.x > 255) {
            pos.x = 255;
        }
        if (pos.x < 0) {
            pos.x = 0;
        }
        if (pos.y > 255) {
            pos.y = 255;
        }
        if (pos.y < 0) {
            pos.y = 0;
        }

        transform.localPosition = pos;

        if (!modify) { return; }

        view.uvs[index] = TextureCoordinate.SetPixel((int)transform.localPosition.x, (int)transform.localPosition.y);

        if (!view.forceRect) return;

        switch (index) {
            case 0:
                view.uvs[3] = TextureCoordinate.SetPixel((int)transform.localPosition.x, TextureCoordinate.GetYPixel(view.uvs[3]));
                view.uvs[1] = TextureCoordinate.SetPixel(TextureCoordinate.GetXPixel(view.uvs[1]), (int)transform.localPosition.y);
                break;
            case 1:
                view.uvs[2] = TextureCoordinate.SetPixel((int)transform.localPosition.x, TextureCoordinate.GetYPixel(view.uvs[2]));
                view.uvs[0] = TextureCoordinate.SetPixel(TextureCoordinate.GetXPixel(view.uvs[0]), (int)transform.localPosition.y);
                break;
            case 2:
                view.uvs[1] = TextureCoordinate.SetPixel((int)transform.localPosition.x, TextureCoordinate.GetYPixel(view.uvs[1]));
                view.uvs[3] = TextureCoordinate.SetPixel(TextureCoordinate.GetXPixel(view.uvs[3]), (int)transform.localPosition.y);
                break;
            case 3:
                view.uvs[0] = TextureCoordinate.SetPixel((int)transform.localPosition.x, TextureCoordinate.GetYPixel(view.uvs[0]));
                view.uvs[2] = TextureCoordinate.SetPixel(TextureCoordinate.GetXPixel(view.uvs[2]), (int)transform.localPosition.y);
                break;
        }

        view.RefreshUVPositions();

    }

    public void ChangePosByDifference(int x, int y) {

        var pos = transform.localPosition;

        pos.x += x;
        pos.y += y;

        if (pos.x > 255) {
            pos.x = 255;
        }
        if (pos.x < 0) {
            pos.x = 0;
        }
        if (pos.y > 255) {
            pos.y = 255;
        }
        if (pos.y < 0) {
            pos.y = 0;
        }

        transform.localPosition = pos;

        view.uvs[index] = TextureCoordinate.SetPixel((int)transform.localPosition.x, (int)transform.localPosition.y);

    }
    void Start() {

        var image = GetComponent<Image>();

        switch (index) {
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

    }

    void Update() {
        
        if (!drag) {
            return;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)view.texturePaletteImage.transform, Input.mousePosition, Camera.main, out Vector2 pointOnPallete);

        var originalX = transform.localPosition.x;
        var originalY = transform.localPosition.y;

        var x = Mathf.Floor(pointOnPallete.x);
        var y = Mathf.Floor(pointOnPallete.y);

        if (Controls.IsDown("LockUVU")) {

            y = originalY;

        }
        else if (Controls.IsDown("LockUVV")) {

            x = originalX;

        }

        // Moves all points
        if (Controls.IsDown("MultiSelect")) {

            int difX = (int)(x - transform.localPosition.x);
            int difY = (int)(y - transform.localPosition.y);

            foreach (var uv in view.uvObjects) {

                uv.ChangePosByDifference(difX, difY);

            }

        }
        else {
            ChangePosition((int)x, (int)y, true);
        }

        view.uvLines.Refresh();

    }

    public void MouseDown() {
        drag = true;
    }

    public void MouseUp() {
        drag = false;
    }

}