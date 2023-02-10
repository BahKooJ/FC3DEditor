
using FCopParser;
using UnityEngine;
using UnityEngine.UI;

class TextureCoordinatePoint : MonoBehaviour {

    public int index;
    public int textureOffset;
    public GeometryEditMode controller;
    public RectTransform imageTransform;
    public TextureCoordinatesLines lines;

    public Button button;


    public void OnDrag() {

        Vector2 pointOnPallete = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(imageTransform, Input.mousePosition, Camera.main, out pointOnPallete);

        var x = Mathf.Floor(pointOnPallete.x);
        var y = Mathf.Floor(pointOnPallete.y);

        this.transform.localPosition = new Vector2(x, y);

        controller.selectedSection.section.textureCoordinates[index] = TextureCoordinate.SetPixel((int)x, (int)y);

        lines.Refresh();

    }


}