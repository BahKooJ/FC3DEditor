
using FCopParser;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatementNodeView : DragableUIElement {

    static List<ByteCode> keyWords = new() { ByteCode.CONDITIONAL_JUMP, ByteCode.JUMP, ByteCode.RUN };
    static Color keyWordColor = new Color(0x65 / 255f, 0x19 / 255f, 0x5E / 255f);

    // - Unity Prefabs -
    public GameObject expressionNodePrefab;
    public GameObject literalNodePrefab;
    public GameObject varNodePrefab;
    public GameObject paddingPrefab;
    public GameObject boolNodePrefab;
    public GameObject enumNodePrefab;
    public GameObject assetNodePrefab;
    public GameObject specialActorNodePrefab;
    public GameObject parameterNodeFab;

    // - Unity Refs -
    public TMP_Text statementText;
    public Transform endBracket;
    public Image backgroundImage;

    // - Parameters -
    public ScriptNode scriptNode;
    [HideInInspector]
    public VisualScriptingLineView currentLine;
    [HideInInspector]
    public List<VisualScriptingLineView> associatedLines = new();

    public List<ExpressionNodeView> expressionNodes;
    ParameterNodeView parameterNodeView;
    List<GameObject> activePads = new();

    private void Start() {

        Init();

    }

    public void Init() {

        void Pad() {
            var padObj = Instantiate(paddingPrefab);
            padObj.transform.SetParent(transform, false);
            padObj.SetActive(true);
            activePads.Add(padObj);
        }

        void CreateNode(ParameterNode parameter) {

            GameObject nodeObj = null;

            if (parameter.scriptNode is LiteralNode) {

                var set = false;
                if (parameter.scriptNode is VariableNode varNode) {

                    if (varNode.isGet) {
                        nodeObj = Instantiate(varNodePrefab);
                        set = true;
                    }

                }

                if (!set) {
                    nodeObj = parameter.dataType switch {
                        ScriptDataType.GlobalVar => Instantiate(varNodePrefab),
                        ScriptDataType.SystemVar => Instantiate(varNodePrefab),
                        ScriptDataType.TimerVar => Instantiate(varNodePrefab),
                        ScriptDataType.UserVar => Instantiate(varNodePrefab),
                        ScriptDataType.Int => Instantiate(literalNodePrefab),
                        ScriptDataType.Bool => Instantiate(boolNodePrefab),
                        ScriptDataType.Enum => Instantiate(enumNodePrefab),
                        ScriptDataType.Cwav => Instantiate(assetNodePrefab),
                        ScriptDataType.Cnet => Instantiate(assetNodePrefab),
                        ScriptDataType.Stream => Instantiate(assetNodePrefab),
                        ScriptDataType.TeamDirect => Instantiate(assetNodePrefab),
                        ScriptDataType.Actor => Instantiate(specialActorNodePrefab),
                        ScriptDataType.Group => Instantiate(specialActorNodePrefab),
                        ScriptDataType.Team => Instantiate(specialActorNodePrefab),
                        ScriptDataType.Any => Instantiate(literalNodePrefab),
                        _ => Instantiate(expressionNodePrefab),
                    };
                }

            }
            else {
                nodeObj = Instantiate(expressionNodePrefab);
            }

            var node = nodeObj.GetComponent<ExpressionNodeView>();
            node.parameterNode = parameter;
            node.currentLine = currentLine;

            expressionNodes.Add(node);

            // For whatever reason, Unity changes the prefab to the instance of the object rather than the prefab in the assets
            // So I have to use the prefab ref from this class and give it to the expression nodes via the inspecter.
            // oh Unity you sweet child
            node.expressionNodePrefab = expressionNodePrefab;

        }


        statementText.text = scriptNode.name;

        foreach (var parameter in scriptNode.GetParameters()) {

            CreateNode(parameter);

        }

        if (expressionNodes.Count != 0) {

            if (scriptNode is OperatorNode) {

                expressionNodes[0].transform.SetParent(transform, false);
                expressionNodes[0].Init();

                if (expressionNodes.Count > 1) {

                    Pad();

                    statementText.transform.SetSiblingIndex(transform.childCount - 1);

                    Pad();

                    expressionNodes[1].transform.SetParent(transform, false);
                    expressionNodes[1].Init();

                }
                else {
                    statementText.transform.SetSiblingIndex(transform.childCount - 1);
                }

            }
            else {

                Pad();
                var paraNode = Instantiate(parameterNodeFab, transform, false);
                parameterNodeView = paraNode.GetComponent<ParameterNodeView>();
                parameterNodeView.expressionNodes = expressionNodes;

            }

        }

        if (scriptNode is ScriptNestingNode) {
            endBracket.gameObject.SetActive(false);
            Pad();
        }
        else {
            endBracket.transform.SetSiblingIndex(transform.childCount - 1);
        }

        if (keyWords.Contains(scriptNode.byteCode)) {
            backgroundImage.color = keyWordColor;
        }

        if (scriptNode.byteCode == ByteCode.RUN) {
            refuseDrag = true;
        }

    }

    public void Rebuild() {

        foreach (var expression in expressionNodes) {
            Destroy(expression.gameObject);
        }

        expressionNodes.Clear();

        if (parameterNodeView != null) {
            Destroy(parameterNodeView.gameObject);
        }

        parameterNodeView = null;

        foreach (var pad in activePads) {
            Destroy(pad);
        }

        activePads.Clear();

        Init();

    }

    public override void OnBeginDrag(PointerEventData eventData) {
        base.OnBeginDrag(eventData);

        foreach (var line in associatedLines) {
            line.gameObject.SetActive(false);
        }

    }

    public override void OnEndDrag(PointerEventData eventData) {
        base.OnEndDrag(eventData);

        foreach (var line in associatedLines) {
            line.gameObject.SetActive(true);
        }

    }

}