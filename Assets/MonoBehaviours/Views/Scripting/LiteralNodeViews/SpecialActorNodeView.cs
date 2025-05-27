

using FCopParser;
using UnityEngine;

public class SpecialActorNodeView : ExpressionNodeView {

    // - Prefabs -
    public GameObject actorSelectorView;

    public override void Init() {

        var main = FindAnyObjectByType<Main>();

        var litNode = parameterNode.scriptNode as LiteralNode;

        void NameCheck() {

            if (litNode.value == 0) {
                expressionText.text = "None";
            }
            else {
                expressionText.text = "Missing";
            }

        }

        if (parameterNode.dataType == ScriptDataType.Actor) {
            try {
                expressionText.text = main.level.sceneActors.actorsByID[litNode.value].name;
            }
            catch {
                NameCheck();
            }
        }
        else if (parameterNode.dataType == ScriptDataType.Group) {
            try {
                expressionText.text = main.level.sceneActors.scriptingGroupedActors[litNode.value].name;
            }
            catch {
                NameCheck();
            }
        }
        else if (parameterNode.dataType == ScriptDataType.Team) {
            try {
                expressionText.text = main.level.sceneActors.teams[litNode.value];
            }
            catch {
                NameCheck();
            }
        }

    }

    public void OnClick() {

        var existingActorSelector = Object.FindAnyObjectByType<SpecialActorSelectorView>();

        if (existingActorSelector != null) {
            Object.Destroy(existingActorSelector.gameObject);
        }

        // spaghetti code? never heard of it
        var obj = Instantiate(actorSelectorView, MiniAssetManagerUtil.canvas.transform, false);
        ((RectTransform)obj.transform).anchoredPosition = Input.mousePosition / Main.uiScaleFactor;

        var view = obj.GetComponent<SpecialActorSelectorView>();

        //if (parameterNode.parent is ActorMethodNode) {
            view.allowedActorRefs = ActorMethodNode.allowedActorRefs;
        //}

        view.onDataSelected = (id, type) => {
            (parameterNode.parent as ActorMethodNode).SetActorRef(type, id);
            Init();

            currentLine.RebuildScriptNode();

        };
    }

}