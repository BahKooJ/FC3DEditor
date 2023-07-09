
using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class TexturePresetsView : MonoBehaviour {

    // Prefabs
    public GameObject uvPresetItem;

    // View refs
    public RectTransform presetListContent;

    public TextureEditMode controller;

    void Start() {

        var item = Instantiate(uvPresetItem);

        var script = item.GetComponent<UVPresentViewItem>();

        script.controller = controller;
        script.preset = new UVPreset(new List<int>() { TextureCoordinate.SetPixel(40,40), TextureCoordinate.SetPixel(60, 40), TextureCoordinate.SetPixel(40, 60), TextureCoordinate.SetPixel(60, 60) }, 3, "bonk");

        item.transform.SetParent(presetListContent, false);

    }


}