

using FCopParser;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ScriptingPanelView;

public class ExpressionNodeView : MonoBehaviour {

    // - Unity Prefabs -
    public GameObject expressionNodePrefab;
    public GameObject literalNodePrefab;
    public GameObject varNodePrefab;
    public GameObject boolNodePrefab;
    public GameObject enumNodePrefab;
    public GameObject assetNodePrefab;
    public GameObject specialActorNodePrefab;
    public GameObject paddingPrefab;

    // - Unity Refs -
    public TMP_Text expressionText;
    public Transform endBracket;
    public Image backgroundImage;

    // - Parameters -
    [HideInInspector]
    public VisualScriptingLineView currentLine;
    public ParameterNode parameterNode;

    public List<ExpressionNodeView> expressionNodes;

    public virtual void Init() {

        void Pad() {
            var padObj = Instantiate(paddingPrefab);
            padObj.transform.SetParent(transform, false);
            padObj.SetActive(true);
        }

        void CreateNode(ParameterNode parameter) {

            GameObject nodeObj = null;

            if (parameter.scriptNode is LiteralNode) {

                var set = false;
                if (parameter.scriptNode is VariableNode varNode) {

                    if (varNode.isGet) {
                        nodeObj = Instantiate(varNodePrefab, transform, false);
                        set = true;
                    }

                }

                if (!set) {
                    nodeObj = parameter.dataType switch {
                        ScriptDataType.GlobalVar => Instantiate(varNodePrefab, transform, false),
                        ScriptDataType.SystemVar => Instantiate(varNodePrefab, transform, false),
                        ScriptDataType.TimerVar => Instantiate(varNodePrefab, transform, false),
                        ScriptDataType.UserVar => Instantiate(varNodePrefab, transform, false),
                        ScriptDataType.Int => Instantiate(literalNodePrefab, transform, false),
                        ScriptDataType.Bool => Instantiate(boolNodePrefab, transform, false),
                        ScriptDataType.Enum => Instantiate(enumNodePrefab, transform, false),
                        ScriptDataType.Cwav => Instantiate(assetNodePrefab, transform, false),
                        ScriptDataType.Cnet => Instantiate(assetNodePrefab, transform, false),
                        ScriptDataType.Stream => Instantiate(assetNodePrefab, transform, false),
                        ScriptDataType.Actor => Instantiate(specialActorNodePrefab, transform, false),
                        ScriptDataType.Group => Instantiate(specialActorNodePrefab, transform, false),
                        ScriptDataType.Team => Instantiate(specialActorNodePrefab, transform, false),
                        ScriptDataType.Any => Instantiate(literalNodePrefab, transform, false),
                        _ => Instantiate(expressionNodePrefab, transform, false),
                    };
                }

            }
            else {
                nodeObj = Instantiate(expressionNodePrefab, transform, false);
            }

            var node = nodeObj.GetComponent<ExpressionNodeView>();
            node.parameterNode = parameter;
            node.currentLine = currentLine;

            expressionNodes.Add(node);

            node.expressionNodePrefab = expressionNodePrefab;

            node.Init();

        }
        
        if (parameterNode.scriptNode is OperatorNode doubleOperator) {

            expressionText.text = parameterNode.scriptNode.name;

            var parameters = parameterNode.scriptNode.GetParameters();

            CreateNode(parameters[0]);

            Pad();

            expressionText.transform.SetSiblingIndex(transform.childCount - 1);

            if (parameters.Count > 1) {

                Pad();

                CreateNode(parameters[1]);

            }

        }
        else {

            expressionText.text = parameterNode.scriptNode.name;

            foreach (var parameters in parameterNode.scriptNode.GetParameters()) {

                Pad();

                CreateNode(parameters);

            }

        }

        endBracket.transform.SetSiblingIndex(transform.childCount - 1);

    }

    public void OnReceiverDrag() {

        ScriptNode scriptNode = null;

        if (Main.draggingElement.TryGetComponent<StatementNodeView>(out var viewItem)) {

            scriptNode = viewItem.scriptNode;

        }
        else if (Main.draggingElement.TryGetComponent<ScriptNodeCreatorItemView>(out var creatorItem)) {

            scriptNode = creatorItem.Create(parameterNode.scriptNode);

        }

        if (scriptNode.ReturnType() == ScriptDataType.Void) return;
        if (!parameterNode.acceptsExpression) return;

        Main.AddCounterAction(new ScriptSaveStateCounterAction(currentLine.view.script, currentLine.view));

        currentLine.view.script.RemoveNode(scriptNode);

        parameterNode.parent.parameters[parameterNode.parameterIndex] = scriptNode;

        currentLine.view.Init();

    }


}