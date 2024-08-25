
using FCopParser;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatementNodeView : MonoBehaviour {

    static List<FCopScript.Instruction> keyWords = new() { FCopScript.Instruction.ConditionalJump, FCopScript.Instruction.Jump };
    static Color keyWordColor = new Color(0x65 / 255f, 0x19 / 255f, 0x5E / 255f);

    // - Unity Prefabs -
    public GameObject expressionNodePrefab;
    public GameObject paddingPrefab;

    // - Unity Refs -
    public TMP_Text statementText;
    public Transform endBracket;
    public Image backgroundImage;

    // - Parameters -
    public FCopScript.StatementNode scriptNode;

    public List<ExpressionNodeView> expressionNodes;

    private void Start() {

        var metadata = FCopScript.instructionMetaData[scriptNode.instruction];

        void Pad() {
            var padObj = Instantiate(paddingPrefab);
            padObj.transform.SetParent(transform, false);
            padObj.SetActive(true);
        }

        void CreateNode(FCopScript.ScriptNode nestedExpressions) {

            var nodeObj = Instantiate(expressionNodePrefab);
            nodeObj.transform.SetParent(transform, false);

            var node = nodeObj.GetComponent<ExpressionNodeView>();
            node.scriptNode = nestedExpressions;

            expressionNodes.Add(node);

            // For whatever reason, Unity changes the prefab to the instance of the object rather than the prefab in the assets
            // So I have to use the prefab ref from this class and give it to the expression nodes via the inspecter.
            // oh Unity you sweet child
            node.expressionNodePrefab = expressionNodePrefab;

            node.Init();

        }

        if (metadata.leftRightOperation) {

            if (metadata.parameterCount != 2) {
                Debug.LogError(scriptNode.instruction.ToString() + " left right operator with a parameter count of " + metadata.parameterCount.ToString() + "?");
            }

            CreateNode(scriptNode.nestedExpressiveNodes[0]);

            Pad();

            statementText.text = metadata.topLevelOperatorString;
            statementText.transform.SetSiblingIndex(transform.childCount - 1);

            Pad();

            CreateNode(scriptNode.nestedExpressiveNodes[1]);

        }
        else if (metadata.topLevelOperatorString != "" && metadata.parameterCount == 1) {

            CreateNode(scriptNode.nestedExpressiveNodes[0]);

            statementText.text = metadata.topLevelOperatorString;
            statementText.transform.SetSiblingIndex(transform.childCount - 1);
        }
        else {

            statementText.text = metadata.topLevelName;

            foreach (var nestedExpressions in scriptNode.nestedExpressiveNodes) {

                Pad();

                CreateNode(nestedExpressions);

            }

        }

        if (scriptNode.nestedNodes.Count > 0) {
            endBracket.gameObject.SetActive(false);
            Pad();
        }
        else {
            endBracket.transform.SetSiblingIndex(transform.childCount - 1);
        }

        if (keyWords.Contains(scriptNode.instruction)) {
            backgroundImage.color = keyWordColor;
        }

    }

}