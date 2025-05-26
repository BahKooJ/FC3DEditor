

using FCopParser;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AssetNodeView : ExpressionNodeView {

    // - Asset Refs -
    public Sprite wavIconSprite;

    // - Unity Refs -
    public Image assetIcon;

    LiteralNode literalNode;
    AssetType assetType;
    Main main;

    public override void Init() {

        void NameCheck() {

            if (literalNode.value == 0) {
                expressionText.text = "None";
            }
            else {
                expressionText.text = "Missing";
            }

        }

        literalNode = (LiteralNode)parameterNode.scriptNode;
        main = FindAnyObjectByType<Main>();

        switch (parameterNode.dataType) {
            case ScriptDataType.Cwav:
                assetType = AssetType.WavSound;
                break;
            default:
                assetType = AssetType.None;
                break;
        }

        switch (assetType) {

            case AssetType.WavSound:
                try {
                    expressionText.text = main.level.audio.soundEffects.First(t => t.scriptingID == literalNode.value).name;
                }
                catch {
                    NameCheck();
                }

                assetIcon.sprite = wavIconSprite;
                break;

        }

    }
    public void OnClick() {

        MiniAssetManagerUtil.RequestAsset(assetType, main, asset => {

            if (asset != null) {

                if (asset is FCopAudio audio) {

                    if (audio.rawDataHasHeader) {

                        literalNode.value = audio.scriptingID;

                    }
                    else {

                        literalNode.value = asset.DataID;

                    }


                }
                else {

                    literalNode.value = asset.DataID;

                }

            }
            else {
                literalNode.value = 0;
            }

            Init();
            currentLine.RefreshLayout();

        });

    }


}