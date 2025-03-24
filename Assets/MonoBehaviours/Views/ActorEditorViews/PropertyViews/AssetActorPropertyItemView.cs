
using UnityEngine;
using TMPro;
using FCopParser;

public class AssetActorPropertyItemView : ActorPropertyItemView {

    // - Unity Refs -
    public TMP_Text nameText;
    public TMP_Text assetNameText;

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
                break;
            case AssetType.Team:
                assetNameText.text = controller.main.level.sceneActors.teams[assetProperty.assetID];
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
        else {

            MiniAssetManagerUtil.RequestAsset(assetProperty.assetType, controller.main, asset => {
                assetProperty.assetID = asset.DataID;
                Refresh();
            });

        }

    }

}