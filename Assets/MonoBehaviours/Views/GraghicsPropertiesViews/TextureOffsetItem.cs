
using FCopParser;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TextureOffsetItem : MonoBehaviour {

    public Main controller;
    public GraphicsPropertiesView view;

    public int index;

    Image image;


    void Start() {

        var textureOffset = controller.selectedSection.section.textureCoordinates[index];

        image = GetComponent<Image>();

        if (controller.selectedTiles.Count == 1) {

            if (controller.selectedTiles[0].textureIndex == index) {
                image.color = new Color(0.1f, 0.2f, 0.1f);
            }

        }

        var textureX = TextureCoordinate.GetXPixel(textureOffset);
        var textureY = TextureCoordinate.GetYPixel(textureOffset);

        foreach (Object obj in transform) {

            obj.GameObject().GetComponent<TMP_Text>().text = index.ToString() + " : (" + textureX.ToString() + ", " + textureY.ToString() + ")";
        }

        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;

    }

    public void OnClick() {

        controller.SetTextureIndex(index);

        view.textureLines.Refresh();

        view.RefreshTextureOffsetsView();

    }

}
