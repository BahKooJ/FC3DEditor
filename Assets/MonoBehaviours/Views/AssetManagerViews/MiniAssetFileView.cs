﻿

using FCopParser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniAssetFileView : MonoBehaviour {

    // - Unity Refs -
    public TMP_Text title;
    public Image icon;

    // - Parameters -
    public FCopAsset asset;
    [HideInInspector]
    public MiniAssetManagerView view;

    void Start() {

        title.text = asset.name;

    }

    public void OnClick() {

        view.OnSelectAsset(asset);

    }

}