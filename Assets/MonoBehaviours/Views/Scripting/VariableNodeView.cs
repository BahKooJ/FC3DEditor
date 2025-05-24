

using UnityEngine;
using FCopParser;
using UnityEngine.UI;

public class VariableNodeView : ExpressionNodeView {

    // - Prefabs -
    public GameObject variableSelectorView;

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
            varNode.SetData(type, id);
            expressionText.text = varNode.name;

            currentLine.RefreshLayout();

        };
    }

}