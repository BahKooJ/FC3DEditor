

using FCopParser;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpressionNodeView : MonoBehaviour {

    static Color expressionColor = new Color(0x00 / 255f, 0x30 / 255f, 0x24 / 255f);
    static Color literalColor = new Color(0x01 / 255f, 0x71 / 255f, 0x71 / 255f);
    static Color varColor = new Color(0x2C / 255f, 0x6C / 255f, 0x00 / 255f);

    // - Unity Prefabs -
    public GameObject expressionNodePrefab;
    public GameObject literalNodePrefab;
    public GameObject paddingPrefab;

    // - Unity Refs -
    public TMP_Text expressionText;
    public Transform endBracket;
    public Image backgroundImage;
    
    // - Parameters -
    public ParameterNode parameterNode;

    public List<ExpressionNodeView> expressionNodes;

    public virtual void Init() {

        void Pad() {
            var padObj = Instantiate(paddingPrefab);
            padObj.transform.SetParent(transform, false);
            padObj.SetActive(true);
        }

        void CreateNode(ParameterNode parameter) {

            GameObject nodeObj = parameter.scriptNode switch {
                LiteralNode => Instantiate(literalNodePrefab, transform, false),
                _ => Instantiate(expressionNodePrefab, transform, false),
            };

            var node = nodeObj.GetComponent<ExpressionNodeView>();
            node.parameterNode = parameter;

            expressionNodes.Add(node);

            node.expressionNodePrefab = expressionNodePrefab;

            node.Init();

        }
        
        if (parameterNode.scriptNode is DoubleOperator doubleOperator) {

            expressionText.text = parameterNode.scriptNode.name;

            var parameters = parameterNode.scriptNode.GetParameters();

            CreateNode(parameters[0]);

            Pad();

            expressionText.transform.SetSiblingIndex(transform.childCount - 1);

            Pad();

            CreateNode(parameters[1]);

        }
        else {

            expressionText.text = parameterNode.scriptNode.name;

            foreach (var parameters in parameterNode.scriptNode.GetParameters()) {

                Pad();

                CreateNode(parameters);

            }

            backgroundImage.color = expressionColor;

        }

        endBracket.transform.SetSiblingIndex(transform.childCount - 1);

    }


}