

using FCopParser;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


public abstract class MiniAssetManagerUtil {

    const float width = 200f;
    const float height = 250f;

    static public GameObject prefab;
    static public GameObject canvas;

    public static void RequestAsset(AssetType assetType, Main main, Action<FCopAsset> onAssetSelected) {

        var existingAssetManager = Object.FindAnyObjectByType<MiniAssetManagerView>();

        if (existingAssetManager != null) {
            Object.Destroy(existingAssetManager.gameObject);
        }

        var obj = Object.Instantiate(main.miniAssetManagerPrefab);
        obj.transform.SetParent(main.canvas.transform, false);

        var rectTrans = (RectTransform)obj.transform;

        // Off screen test.
        // Not really sure why but don't scale the width or height.
        var pos = Input.mousePosition / Main.uiScaleFactor;
        var scaledScreenWidth = Screen.width / Main.uiScaleFactor;

        if (pos.x + width > scaledScreenWidth) {
            var dif = pos.x + width - scaledScreenWidth;
            pos.x -= dif;
        }

        if (pos.y - height < 0) {
            pos.y -= pos.y - height;

        }

        rectTrans.anchoredPosition = pos;

        var assetManager = obj.GetComponent<MiniAssetManagerView>();

        assetManager.level = main.level;
        assetManager.main = main;
        assetManager.requestAssetType = assetType;
        assetManager.onAssetSelected = onAssetSelected;

    }

    public static void RequestUniversalData(Dictionary<int, string> requestingData, Main main, Action<int> onDataSelected) {

        var existingDataManager = Object.FindAnyObjectByType<UniversialMiniDataManagerView>();

        if (existingDataManager != null) {
            Object.Destroy(existingDataManager.gameObject);
        }

        var obj = Object.Instantiate(main.universialDataManagerPrefab);
        obj.transform.SetParent(main.canvas.transform, false);

        var rectTrans = (RectTransform)obj.transform;

        // Off screen test.
        // Not really sure why but don't scale the width or height.
        var pos = Input.mousePosition / Main.uiScaleFactor;
        var scaledScreenWidth = Screen.width / Main.uiScaleFactor;

        if (pos.x + width > scaledScreenWidth) {
            var dif = pos.x + width - scaledScreenWidth;
            pos.x -= dif;
        }

        if (pos.y - height < 0) {
            pos.y -= pos.y - height;

        }

        rectTrans.anchoredPosition = pos;

        var dataManager = obj.GetComponent<UniversialMiniDataManagerView>();

        dataManager.level = main.level;
        dataManager.main = main;
        dataManager.requestingData= requestingData;
        dataManager.onDataSelected = onDataSelected;

    }

}