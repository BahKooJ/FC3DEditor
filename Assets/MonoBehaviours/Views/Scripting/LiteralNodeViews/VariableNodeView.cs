

using UnityEngine;
using FCopParser;
using UnityEngine.UI;
using static ScriptingPanelView;

public class VariableNodeView : ExpressionNodeView {

    // - Prefabs -
    public GameObject variableSelectorView;

    public override void Init() {

        var varNode = parameterNode.scriptNode as VariableNode;

        varNode.RefreshData();

        base.Init();

    }

    public void OnClick() {

        var existingVariableSelector = Object.FindAnyObjectByType<VariableSelectorView>();

        if (existingVariableSelector != null) {
            Object.Destroy(existingVariableSelector.gameObject);
        }

        // spaghetti code? never heard of it
        var obj = Instantiate(variableSelectorView, MiniAssetManagerUtil.canvas.transform, false);
        ((RectTransform)obj.transform).anchoredPosition = Input.mousePosition / Main.uiScaleFactor;

        var view = obj.GetComponent<VariableSelectorView>();

        var varNode = parameterNode.scriptNode as VariableNode;

        view.allowedVars = varNode.allowedVarTypeConverstion;
        view.onDataSelected = (id, type) => {

            Main.AddCounterAction(new ScriptSaveStateCounterAction(currentLine.view.script, currentLine.view));

            varNode.SetData(type, id);
            expressionText.text = varNode.name;

            currentLine.RefreshLayout();

        };
    }

}