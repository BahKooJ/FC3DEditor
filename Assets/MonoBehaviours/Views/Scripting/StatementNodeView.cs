
using FCopParser;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatementNodeView : MonoBehaviour {

    static List<ByteCode> keyWords = new() { ByteCode.CONDITIONAL_JUMP, ByteCode.JUMP };
    static Color keyWordColor = new Color(0x65 / 255f, 0x19 / 255f, 0x5E / 255f);

    // - Unity Prefabs -
    public GameObject expressionNodePrefab;
    public GameObject paddingPrefab;

    // - Unity Refs -
    public TMP_Text statementText;
    public Transform endBracket;
    public Image backgroundImage;

    // - Parameters -
    public ScriptNode scriptNode;

    public List<ExpressionNodeView> expressionNodes;

    private void Start() {

        void Pad() {
            var padObj = Instantiate(paddingPrefab);
            padObj.transform.SetParent(transform, false);
            padObj.SetActive(true);
        }

        void CreateNode(ParameterNode parameter) {

            var nodeObj = Instantiate(expressionNodePrefab);
            nodeObj.transform.SetParent(transform, false);

            var node = nodeObj.GetComponent<ExpressionNodeView>();
            node.parameterNode = parameter;

            expressionNodes.Add(node);

            // For whatever reason, Unity changes the prefab to the instance of the object rather than the prefab in the assets
            // So I have to use the prefab ref from this class and give it to the expression nodes via the inspecter.
            // oh Unity you sweet child
            node.expressionNodePrefab = expressionNodePrefab;

            node.Init();

        }


        statementText.text = scriptNode.name;

        foreach (var parameter in scriptNode.GetParameters()) {

            Pad();

            CreateNode(parameter);

        }

        if (scriptNode.nestedNodes.Count > 0) {
            endBracket.gameObject.SetActive(false);
            Pad();
        }
        else {
            endBracket.transform.SetSiblingIndex(transform.childCount - 1);
        }

        if (keyWords.Contains(scriptNode.byteCode)) {
            backgroundImage.color = keyWordColor;
        }

    }

}