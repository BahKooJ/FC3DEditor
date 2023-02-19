
using FCopParser;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsPresetItem : MonoBehaviour {

    public GeometryEditMode controller;
    public GraphicsPropertiesView view;

    public int index;

    Image image;

    void Start() {

        var graphicsOffset = controller.selectedSection.section.tileGraphics[index];

        image = GetComponent<Image>();

        if (controller.selectedTiles.Count > 0) {

            if (controller.selectedTiles[0].graphicsIndex == index) {
                image.color = new Color(0.1f, 0.2f, 0.1f);
            }

        }

        foreach (Object obj in transform) {

            obj.GameObject().GetComponent<TMP_Text>().text = index.ToString();

        }

        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;

    }

    public void OnClick() {

        foreach (var tile in controller.selectedTiles) {
            tile.graphicsIndex = index;

        }

        var graphicsOffset = controller.selectedSection.section.tileGraphics[index];

        Debug.Log(
            graphicsOffset.number1.ToString() + " " +
            graphicsOffset.number2.ToString() + " " +
            graphicsOffset.number3.ToString() + " " +
            graphicsOffset.number4.ToString() + " " +
            graphicsOffset.number5.ToString());

        view.RefreshView();

    }

}