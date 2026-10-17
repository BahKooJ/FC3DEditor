

using FCopParser;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static ScriptingPanelView;

public class AssetNodeView : ExpressionNodeView {

    // - Asset Refs -
    public Sprite wavIconSprite;
    public Sprite navMeshIcon;
    public Sprite teamIconSprite;

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

        assetType = parameterNode.dataType switch {
            ScriptDataType.Cwav => AssetType.WavSound,
            ScriptDataType.Cnet => AssetType.NavMesh,
            ScriptDataType.Stream => AssetType.Stream,
            ScriptDataType.TeamDirect => AssetType.Team,
            _ => AssetType.None,
        };

        switch (assetType) {

            case AssetType.WavSound:
                try {
                    expressionText.text = main.level.audio.soundEffects.First(t => t.scriptingID == literalNode.value).name;
                }
                catch {

                    try {
                        expressionText.text = main.level.audio.globalSoundEffects.First(t => t.scriptingID == literalNode.value).name;

                    }
                    catch {
                        NameCheck();
                    }

                }


                assetIcon.sprite = wavIconSprite;
                break;
            case AssetType.NavMesh:

                try {
                    expressionText.text = main.level.navMeshes[literalNode.value].name;
                }
                catch {
                    NameCheck();
                }

                assetIcon.sprite = navMeshIcon;
                break;
            case AssetType.Stream:

                try {
                    expressionText.text = main.level.audio.soundStreams[literalNode.value].name;
                }
                catch {
                    NameCheck();
                }

                assetIcon.sprite = wavIconSprite;
                break;
            case AssetType.Team:

                try {
                    expressionText.text = main.level.sceneActors.teams[literalNode.value];
                }
                catch {
                    NameCheck();
                }

                assetIcon.sprite = teamIconSprite;

                break;

        }

    }
    public void OnClick() {

        if (assetType == AssetType.Team) {

            MiniAssetManagerUtil.RequestUniversalData(main.level.sceneActors.teams, main, id => {
                Main.AddCounterAction(new ScriptSaveStateCounterAction(currentLine.view.script, currentLine.view));

                literalNode.value = id;
                Init();
                currentLine.RefreshLayout();

            });

            return;
        }

        MiniAssetManagerUtil.RequestAsset(assetType, main, asset => {

            Main.AddCounterAction(new ScriptSaveStateCounterAction(currentLine.view.script, currentLine.view));

            if (asset != null) {

                if (asset is FCopAudio audio) {

                    if (audio.rawDataHasHeader) {

                        literalNode.value = audio.scriptingID;

                    }
                    else {

                        literalNode.value = asset.DataID;

                    }

                }
                else if (asset is FCopNavMesh navMesh) {

                    literalNode.value = main.level.navMeshes.IndexOf(navMesh);

                }
                else if (asset is FCopStream stream) {

                    literalNode.value = main.level.audio.soundStreams.IndexOf(stream);

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