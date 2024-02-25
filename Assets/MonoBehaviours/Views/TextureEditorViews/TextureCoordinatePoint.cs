
using FCopParser;
using UnityEngine;
using UnityEngine.UI;

class TextureCoordinatePoint : MonoBehaviour {

    public int uvOffset;
    public int textureOffset;
    public TextureEditMode controller;
    public TextureUVMapper view;
    public RectTransform imageTransform;
    public TextureCoordinatesLines lines;

    public Button button;

    bool drag = false;

    public void ChangePosByDifference(int x, int y) {

        var pos = transform.localPosition;

        pos.x += x;
        pos.y += y;

        transform.localPosition = pos;

        foreach (var tile in view.controller.selectedTiles) {
            tile.uvs[uvOffset] = TextureCoordinate.SetPixel((int)transform.localPosition.x, (int)transform.localPosition.y);
        }

        controller.RefreshTileOverlayTexture();

    }

    public void ChangePosition(int x, int y) {

        transform.localPosition = new Vector2(x, y);

        foreach (var tile in view.controller.selectedTiles) {
            tile.uvs[uvOffset] = TextureCoordinate.SetPixel((int)transform.localPosition.x, (int)transform.localPosition.y);
        }

        controller.RefreshTileOverlayTexture();

    }

    void Update() {

        if (!drag) {
            return;
        }

        Vector2 pointOnPallete = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(imageTransform, Input.mousePosition, Camera.main, out pointOnPallete);

        var x = Mathf.Floor(pointOnPallete.x);
        var y = Mathf.Floor(pointOnPallete.y);

        // Moves all points
        if (Controls.IsDown("ModifierMultiSelect")) {

            int difX = (int)(x - transform.localPosition.x);
            int difY = (int)(y - transform.localPosition.y);

            foreach (var point in lines.points) {
                
                var script = point.GetComponent<TextureCoordinatePoint>();

                script.ChangePosByDifference(difX, difY);

            }

        } else {
            ChangePosition((int)x, (int)y);
        }

        lines.Refresh();

    }

    // --Event methods called by Unity--
    public void MouseDown() {

        if (Input.GetMouseButton(0)) {
            drag = true;
        }

    }

    public void MouseUp() {
        
        drag = false;

    }


}