
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

    public Image ghostPoint;

    bool drag = false;

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

        if (view.frameSelected != -1) {

            var tile = view.controller.FirstTile;

            tile.animatedUVs[(view.frameSelected * 4) + uvOffset] = TextureCoordinate.SetPixel((int)transform.localPosition.x, (int)transform.localPosition.y);

            if (view.frameSelected == 0) {
                tile.uvs[uvOffset] = TextureCoordinate.SetPixel((int)transform.localPosition.x, (int)transform.localPosition.y);
            }

        }
        else {

            view.controller.FirstTile.uvs[uvOffset] = TextureCoordinate.SetPixel((int)transform.localPosition.x, (int)transform.localPosition.y);

        }

        controller.DuplicateTileUVs(false);
        controller.RefreshTileOverlayTexture();

    }

    public void ChangePosition(int x, int y) {

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

        if (view.frameSelected != -1) {

            var tile = view.controller.FirstTile;

            tile.animatedUVs[(view.frameSelected * 4) + uvOffset] = TextureCoordinate.SetPixel((int)transform.localPosition.x, (int)transform.localPosition.y);

            if (view.frameSelected == 0) {

                if (!(tile.uvs.Count <= uvOffset)) {
                    tile.uvs[uvOffset] = TextureCoordinate.SetPixel((int)transform.localPosition.x, (int)transform.localPosition.y);
                }

            }

        } else {

            view.controller.FirstTile.uvs[uvOffset] = TextureCoordinate.SetPixel((int)transform.localPosition.x, (int)transform.localPosition.y);

        }

        controller.DuplicateTileUVs(false);
        controller.RefreshTileOverlayTexture();

    }

    public void ChangeGhostPos() {

        var ghostPos = ghostPoint.transform.localPosition;
        if (view.controller.FirstItem.section.section.animationVector.x != 0) {
            

            if (view.controller.FirstItem.section.section.animationVector.x > 0) {
                ghostPos.x = 27;
            }
            else {
                ghostPos.x = -27;
            }

        } else {
            ghostPos.x = 0;
        }

        if (view.controller.FirstItem.section.section.animationVector.y != 0) {


            if (view.controller.FirstItem.section.section.animationVector.y > 0) {
                ghostPos.y = 27;
            }
            else {
                ghostPos.y = -27;
            }

        } else {
            ghostPos.y = 0;
        }

        ghostPoint.transform.localPosition = ghostPos;

    }

    public void ShowGhostPos() {
        ghostPoint.gameObject.SetActive(true);
        var originalColor = GetComponent<Image>().color;
        originalColor.r -= 0.5f;
        originalColor.g -= 0.5f;
        originalColor.b -= 0.5f;
        ghostPoint.color = originalColor;
        ChangeGhostPos();
    }

    public void HideGhostPos() {
        ghostPoint.gameObject.SetActive(false);
    }

    void Start() {
        
        if (view.controller.FirstTile.isVectorAnimated) {
            ShowGhostPos();
        }

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
        if (Controls.IsDown("MultiSelect")) {

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
        
        if (drag && view.frameSelected != -1) {
            view.RefreshUVFrameItems();
        }

        drag = false;

    }


}