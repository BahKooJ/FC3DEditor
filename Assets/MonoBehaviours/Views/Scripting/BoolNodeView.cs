
using UnityEngine;
using FCopParser;
using UnityEngine.UI;

public class BoolNodeView : ExpressionNodeView {

    public override void Init() {
        
        var litNode = parameterNode.scriptNode as LiteralNode;

        expressionText.text = (litNode.value == 1).ToString();

    }

    public void OnClick() {

        var litNode = parameterNode.scriptNode as LiteralNode;

        litNode.value = (litNode.value == 1) ? 0 : 1;

        expressionText.text = (litNode.value == 1).ToString();

    }

}