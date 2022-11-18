
using FCopParser;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

class TextureOffsetItem : MonoBehaviour {

    public Main controller;

    public int index;

    void Start() {

        var textureOffset = controller.selectedSection.section.textureCoordinates[index];

        var textureX = TextureCoordinate.GetXPixel(textureOffset);
        var textureY = TextureCoordinate.GetYPixel(textureOffset);

        foreach (Object obj in transform) {

            obj.GameObject().GetComponent<TMP_Text>().text = index.ToString() + " : (" + textureX.ToString() + ", " + textureY.ToString() + ")";

        }

    }

}