

using FCopParser;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MiniAssetManagerView : MonoBehaviour {

    // - Unity Prefabs -
    public GameObject miniAssetFilePrefab;

    // - Unity Refs -
    public Transform fileContent;
    public TMP_InputField searchBar;

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

        InitFile(null);

        switch (requestAssetType) {

            case AssetType.WavSound:
                foreach (var sound in level.audio.globalSoundEffects) {

                    InitFile(sound);

                }
                foreach (var sound in level.audio.soundEffects) {

                    InitFile(sound);

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
            case AssetType.NavMesh:

                foreach (var navMesh in level.navMeshes) {
                    
                    InitFile(navMesh);

                }

                break;
            case AssetType.Actor:

                foreach (var act in level.sceneActors.actors) {
                    
                    InitFile(act);

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

    public void StartType() {
        Main.ignoreAllInputs = true;
    }

    public void StopType() {
        Main.ignoreAllInputs = false;
    }

    public void OnTypeInSearch() {

        foreach (var file in files) {

            if (file.asset != null) {

                if (file.asset.name.Contains(searchBar.text) || searchBar.text == "") {
                    file.gameObject.SetActive(true);
                }
                else {
                    file.gameObject.SetActive(false);
                }

            }

        }

    }

}