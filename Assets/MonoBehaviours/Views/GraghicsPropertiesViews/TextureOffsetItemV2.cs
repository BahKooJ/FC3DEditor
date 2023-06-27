
using FCopParser;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TextureOffsetItemV2 : MonoBehaviour {

    public GeometryEditMode controller;
    public GraphicsPropertiesView view;

    public TMP_Text nr_Display;
    public TMP_InputField x_Display;
    public TMP_InputField y_Display;

    public int index;

    public bool isGlobal = false;

    Image image;


    void Start() {

        int textureOffset;

        if (isGlobal) {
            textureOffset = GraphicsPropertiesView.textureCoordsClipboard[index];
        } else {
            textureOffset = controller.selectedSection.section.textureCoordinates[index];
        }

        image = GetComponent<Image>();

        if (controller.selectedTiles.Count > 0) {

            if (isGlobal) {

                if (view.globalTextureCoordIndex == index) {
                    image.color = new Color(0.2f, 0.1f, 0.2f);
                } else {
                    image.color = new Color(0.2f, 0.2f, 0.1f);
                }

            } 
            else if (controller.selectedTiles[0].textureIndex == index) {
                image.color = new Color(0.1f, 0.2f, 0.1f);
            }

        }

        var textureX = TextureCoordinate.GetXPixel(textureOffset);
        var textureY = TextureCoordinate.GetYPixel(textureOffset);

        nr_Display.text = index.ToString();
        x_Display.SetTextWithoutNotify(textureX.ToString());
        y_Display.SetTextWithoutNotify(textureY.ToString());

        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;

    }

    public void OnClick() {

        view.ChangeTextureIndex(index, isGlobal);

    }

    public void UpdateNodes()
    {

        controller.selectedSection.section.textureCoordinates[index] = TextureCoordinate.SetPixel(int.Parse(x_Display.text), int.Parse(y_Display.text));
        view.UpdateDynamicTextureOffsets();
        view.textureLines.ReInit();
    }
}
