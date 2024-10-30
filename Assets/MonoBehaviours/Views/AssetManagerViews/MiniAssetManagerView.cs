

using FCopParser;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MiniAssetManagerView : MonoBehaviour {

    // - Unity Prefabs -
    public GameObject miniAssetFilePrefab;

    // - Unity Refs -
    public Transform fileContent;

    // - Parameters -
    public FCopLevel level;
    [HideInInspector]
    public Main main;
    [HideInInspector]
    public AssetType requestAssetType;
    public Action<FCopAsset> onAssetSelected;

    List<MiniAssetFileView> files = new();

    private void Start() {

        void InitFile(FCopAsset asset) {

            var obj = Instantiate(miniAssetFilePrefab);
            obj.transform.SetParent(fileContent.transform, false);

            var fileView = obj.GetComponent<MiniAssetFileView>();
            fileView.asset = asset;
            fileView.view = this;

            files.Add(fileView);

        }

        switch (requestAssetType) {

            case AssetType.WavSound:
                foreach (var item in level.audio.soundEffects) {

                    foreach (var sound in item.Value) {
                        InitFile(sound);
                    }

                }
                break;
            case AssetType.Texture:
                foreach (var texture in level.textures) {

                    InitFile(texture);

                }
                break;
            case AssetType.Object:
                foreach (var obj in level.objects) {

                    InitFile(obj);

                }
                break;
            case AssetType.SndsSound:
                break;
            case AssetType.Music:
                break;
            case AssetType.MiniAnimation:
                break;
            case AssetType.Mixed:
                break;

        }

    }

    public void OnSelectAsset(FCopAsset asset) {

        onAssetSelected(asset);
        Destroy(gameObject);

    }

}