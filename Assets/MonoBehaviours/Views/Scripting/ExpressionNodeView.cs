

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
    public GameObject paddingPrefab;

    // - Unity Refs -
    public TMP_Text expressionText;
    public Transform endBracket;
    public Image backgroundImage;

    // - Parameters -
    public ScriptNode scriptNode;

    public List<ExpressionNodeView> expressionNodes;

    public void Init() {

        void Pad() {
            var padObj = Instantiate(paddingPrefab);
            padObj.transform.SetParent(transform, false);
            padObj.SetActive(true);
        }

        void CreateNode(ScriptNode nestedExpressions) {

            var nodeObj = Instantiate(expressionNodePrefab);
            nodeObj.transform.SetParent(transform, false);

            var node = nodeObj.GetComponent<ExpressionNodeView>();
            node.scriptNode = nestedExpressions;

            expressionNodes.Add(node);

            node.expressionNodePrefab = expressionNodePrefab;

            node.Init();

        }

        if (scriptNode is LiteralNode lNode) {

            expressionText.text = lNode.value.ToString();

            backgroundImage.color = literalColor;

        }
        else {

            expressionText.text = scriptNode.name;

            foreach (var nestedExpressions in scriptNode.parameters) {

                Pad();

                CreateNode(nestedExpressions);

            }

            backgroundImage.color = expressionColor;

        }


        endBracket.transform.SetSiblingIndex(transform.childCount - 1);

    }


}