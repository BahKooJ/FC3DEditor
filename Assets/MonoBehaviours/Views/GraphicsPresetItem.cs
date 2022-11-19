
using FCopParser;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

class GraphicsPresetItem : MonoBehaviour {

    public Main controller;
    public GraphicsPropertiesView view;

    public int index;

    Image image;

    void Start() {

        var graphicsOffset = controller.selectedSection.section.tileGraphics[index];

        image = GetComponent<Image>();

        if (controller.selectedTile.graphicsIndex == index) {
            image.color = new Color(0.1f,0.2f,0.1f);
        }

        foreach (Object obj in transform) {

            obj.GameObject().GetComponent<TMP_Text>().text = index.ToString();

        }

    }

    public void OnClick() {

        controller.selectedTile.graphicsIndex = index;

        view.RefreshView();

    }

}