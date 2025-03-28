
using UnityEngine;
using TMPro;
using FCopParser;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

public class AssetActorPropertyItemView : ActorPropertyItemView {

    // - Asset Refs =
    public Sprite actorIconSprite;
    public Sprite textureSnippetIconSprite;
    public Sprite teamIconSprite;

    // - Unity Refs -
    public TMP_Text nameText;
    public TMP_Text assetNameText;
    public Image assetIcon;

    public AssetActorProperty assetProperty;

    void Start() {

        assetProperty = (AssetActorProperty)property;

        Refresh();


    }

    public override void Refresh() {

        nameText.text = property.name;

        switch (assetProperty.assetType) {
            case AssetType.WavSound:
                assetNameText.text = "TODO";
                break;
            case AssetType.Texture:
                assetNameText.text = "TODO";
                break;
            case AssetType.Object:
                assetNameText.text = "TODO";
                break;
            case AssetType.NavMesh:
                assetNameText.text = "TODO";
                break;
            case AssetType.SndsSound:
                assetNameText.text = "TODO";
                break;
            case AssetType.Music:
                assetNameText.text = "TODO";
                break;
            case AssetType.MiniAnimation:
                assetNameText.text = "TODO";
                break;
            case AssetType.Mixed:
                assetNameText.text = "TODO";
                break;
            case AssetType.Actor:
                assetNameText.text = controller.main.level.sceneActors.actorsByID[assetProperty.assetID].name;
                assetIcon.sprite = actorIconSprite;
                break;
            case AssetType.Team:
                assetNameText.text = controller.main.level.sceneActors.teams[assetProperty.assetID];
                assetIcon.sprite = teamIconSprite;
                break;
            case AssetType.TextureSnippet:

                try {
                    assetNameText.text = controller.main.level.textureSnippets.First(t => t.id == assetProperty.assetID).name;
                }
                catch {
                    assetNameText.text = "Missing";
                }

                assetIcon.sprite = textureSnippetIconSprite;

                break;
            case AssetType.None:
                assetNameText.text = "TODO";
                break;
        }

    }

    public void OnClickAsset() {

        if (assetProperty.assetType == AssetType.Team) {

            MiniAssetManagerUtil.RequestUniversalData(controller.main.level.sceneActors.teams, controller.main, id => {
                assetProperty.assetID = id;
                Refresh();
            });

        }
        else if (assetProperty.assetType == AssetType.TextureSnippet) {

            var snippetDic = new Dictionary<int, string>();

            foreach (var ts in controller.main.level.textureSnippets) {
                snippetDic[ts.id] = ts.name;
            }

            MiniAssetManagerUtil.RequestUniversalData(snippetDic, controller.main, id => {
                assetProperty.assetID = id;
                Refresh();
            });

        }

        else {

            MiniAssetManagerUtil.RequestAsset(assetProperty.assetType, controller.main, asset => {
                assetProperty.assetID = asset.DataID;
                Refresh();
            });

        }

    }

}