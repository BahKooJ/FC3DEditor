
using UnityEngine;
using TMPro;
using FCopParser;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

public class AssetActorPropertyItemView : ActorPropertyItemView {

    // - Asset Refs -
    public Sprite wavIconSprite;
    public Sprite actorIconSprite;
    public Sprite textureSnippetIconSprite;
    public Sprite teamIconSprite;

    // - Unity Refs -
    public TMP_Text nameText;
    public TMP_Text assetNameText;
    public Image assetIcon;

    public AssetActorProperty assetProperty;

    bool refuseCallback = false;

    void Start() {

        assetProperty = (AssetActorProperty)property;

        Refresh();

    }

    public override void Refresh() {

        refuseCallback = true;

        nameText.text = property.name;

        void NameCheck() {

            if (assetProperty.assetID == 0) {
                assetNameText.text = "None";
            }
            else {
                assetNameText.text = "Missing";
            }

        }

        switch (assetProperty.assetType) {
            case AssetType.WavSound:

                try {
                    assetNameText.text = controller.main.level.audio.soundEffects.First(t => t.scriptingID == assetProperty.assetID).name;
                }
                catch {

                    try {
                        assetNameText.text = controller.main.level.audio.globalSoundEffects.First(t => t.scriptingID == assetProperty.assetID).name;

                    }
                    catch {
                        NameCheck();
                    }

                }

                assetIcon.sprite = wavIconSprite;

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
            case AssetType.Actor:

                try {
                    assetNameText.text = controller.main.level.sceneActors.actorsByID[assetProperty.assetID].name;
                }
                catch {
                    NameCheck();
                }

                assetIcon.sprite = actorIconSprite;

                break;
            case AssetType.Team:

                try {
                    assetNameText.text = controller.main.level.sceneActors.teams[assetProperty.assetID];
                }
                catch {
                    NameCheck();
                }

                assetIcon.sprite = teamIconSprite;

                break;
            case AssetType.TextureSnippet:

                try {
                    assetNameText.text = controller.main.level.textureSnippets.First(t => t.id == assetProperty.assetID).name;
                }
                catch {
                    NameCheck();
                }

                assetIcon.sprite = textureSnippetIconSprite;

                break;
            case AssetType.None:
                assetNameText.text = "TODO";
                break;
        }

        refuseCallback = false;

    }

    public void OnClickAsset() {

        if (refuseCallback) return;

        if (assetProperty.assetType == AssetType.Team) {

            MiniAssetManagerUtil.RequestUniversalData(controller.main.level.sceneActors.teams, controller.main, id => {
                ActorEditMode.AddPropertyChangeCounterAction(property, actor);
                assetProperty.assetID = id;
                Refresh();

                if (ActorPropertyChangeEvent.changeEventsByPropertyName.ContainsKey(property.name)) {

                    ActorPropertyChangeEvent.changeEventsByPropertyName[property.name](controller, property);

                }
            });

        }
        else if (assetProperty.assetType == AssetType.TextureSnippet) {

            var snippetDic = new Dictionary<int, string>();

            foreach (var ts in controller.main.level.textureSnippets) {
                snippetDic[ts.id] = ts.name;
            }

            MiniAssetManagerUtil.RequestUniversalData(snippetDic, controller.main, id => {
                ActorEditMode.AddPropertyChangeCounterAction(property, actor);
                assetProperty.assetID = id;
                Refresh();

                if (ActorPropertyChangeEvent.changeEventsByPropertyName.ContainsKey(property.name)) {

                    ActorPropertyChangeEvent.changeEventsByPropertyName[property.name](controller, property);

                }
            });

        }

        else {

            MiniAssetManagerUtil.RequestAsset(assetProperty.assetType, controller.main, asset => {

                ActorEditMode.AddPropertyChangeCounterAction(property, actor);

                if (asset != null) {

                    if (asset is FCopAudio audio) {

                        if (audio.rawDataHasHeader) {

                            assetProperty.assetID = audio.scriptingID;

                        }
                        else {

                            assetProperty.assetID = asset.DataID;

                        }


                    }
                    else {

                        assetProperty.assetID = asset.DataID;

                    }

                }
                else {
                    assetProperty.assetID = 0;
                }

                Refresh();

                if (ActorPropertyChangeEvent.changeEventsByPropertyName.ContainsKey(property.name)) {

                    ActorPropertyChangeEvent.changeEventsByPropertyName[property.name](controller, property);

                }
            });

        }

    }

}