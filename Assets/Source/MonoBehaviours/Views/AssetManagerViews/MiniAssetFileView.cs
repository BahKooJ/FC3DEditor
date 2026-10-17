

using FCopParser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniAssetFileView : MonoBehaviour {

    // - Asset Refs -
    public Sprite wavIconSprite;
    public Sprite actorIconSprite;
    public Sprite textureIconSprite;
    public Sprite objectIconSprite;
    public Sprite navMeshIconSprite;

    // - Unity Refs -
    public TMP_Text title;
    public Image icon;

    // - Parameters -
    public FCopAsset asset;
    [HideInInspector]
    public MiniAssetManagerView view;

    void Start() {

        if (asset == null) {
            title.text = "None";
        }
        else {
            title.text = asset.name;
        }

        switch (view.requestAssetType) {
            case AssetType.WavSound:
                icon.sprite = wavIconSprite;
                break;
            case AssetType.Texture:
                icon.sprite = textureIconSprite;
                break;
            case AssetType.Object:
                icon.sprite = objectIconSprite;
                break;
            case AssetType.NavMesh:
                icon.sprite = navMeshIconSprite;
                break;
            case AssetType.SndsSound:
                icon.sprite = wavIconSprite;
                break;
            case AssetType.Music:
                icon.sprite = wavIconSprite;
                break;
            case AssetType.Actor:
                icon.sprite = actorIconSprite;
                break;
        }

    }

    public void OnClick() {

        view.OnSelectAsset(asset);

    }

}