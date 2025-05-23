﻿

using FCopParser;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpressionNodeView : MonoBehaviour {

    // - Unity Prefabs -
    public GameObject expressionNodePrefab;
    public GameObject literalNodePrefab;
    public GameObject varNodePrefab;
    public GameObject boolNodePrefab;
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

            GameObject nodeObj;

            if (parameter.scriptNode is LiteralNode) {

                nodeObj = parameter.dataType switch {
                    ScriptDataType.GlobalVar => Instantiate(varNodePrefab, transform, false),
                    ScriptDataType.SystemVar => Instantiate(varNodePrefab, transform, false),
                    ScriptDataType.TimerVar => Instantiate(varNodePrefab, transform, false),
                    ScriptDataType.UserVar => Instantiate(varNodePrefab, transform, false),
                    ScriptDataType.Int => Instantiate(literalNodePrefab, transform, false),
                    ScriptDataType.Bool => Instantiate(boolNodePrefab, transform, false),
                    ScriptDataType.Any => Instantiate(literalNodePrefab, transform, false),
                    _ => Instantiate(expressionNodePrefab, transform, false),
                };

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


}