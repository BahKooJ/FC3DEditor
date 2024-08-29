

using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class VisualScriptingFuncWindowView : MonoBehaviour {

    // - Prefabs -
    public GameObject linePrefab;
    public GameObject statementNodePrefab;
    public GameObject endCodeBracket;
    public GameObject funcDataPrefab;
    public GameObject thickSeparatorPrefab;
    public GameObject runConditionHeaderPrefab;
    public GameObject codeHeaderPrefab;

    // - Parameters -
    public FCopFunction func;

    FuncDataView funcData;
    List<VisualScriptingLineView> conditionalLines = new();
    List<VisualScriptingLineView> codeLines = new();
    List<GameObject> uiFillerObjects = new();

    public void Init() {

        Clear();

        if (func.runCondition.failed || func.code.failed) {
            return;
        }

        InitFuncData();
        InitUIGameObject(thickSeparatorPrefab);
        InitUIGameObject(runConditionHeaderPrefab);
        InitCode(func.runCondition.scriptNodes, conditionalLines);
        InitUIGameObject(thickSeparatorPrefab);
        InitUIGameObject(codeHeaderPrefab);
        InitCode(func.code.scriptNodes, codeLines);

    }

    public void Clear() {

        foreach (var line in conditionalLines) {
            Destroy(line.gameObject);
        }

        conditionalLines.Clear();

        foreach (var line in codeLines) {
            Destroy(line.gameObject);
        }

        codeLines.Clear();

        foreach (var line in uiFillerObjects) {
            Destroy(line);
        }

        uiFillerObjects.Clear();

        if (funcData != null) {
            Destroy(funcData.gameObject);
        }

        funcData = null;

    }

    void InitCode(List<FCopScript.ScriptNode> script, List<VisualScriptingLineView> lines) {

        var i = 1;

        var nestCount = 0;

        void AddEndBracket() {

            var lineObj = Instantiate(linePrefab);

            lineObj.transform.SetParent(transform, false);

            var line = lineObj.GetComponent<VisualScriptingLineView>();
            line.number = i;
            i++;

            var endObj = Instantiate(endCodeBracket);
            endObj.transform.SetParent(line.lineContent, false);

            endObj.transform.SetParent(line.lineContent, false);

            var pos = ((RectTransform)endObj.transform).anchoredPosition;

            pos.x += nestCount * 24;

            ((RectTransform)endObj.transform).anchoredPosition = pos;

            lines.Add(line);

        }

        void AddLines(List<FCopScript.ScriptNode> nodes) {

            foreach (var node in nodes) {

                var lineObj = Instantiate(linePrefab);

                lineObj.transform.SetParent(transform, false);

                var line = lineObj.GetComponent<VisualScriptingLineView>();
                line.number = i;
                i++;

                if (node is FCopScript.StatementNode statementNode) {

                    var visualNodeObj = Instantiate(statementNodePrefab);

                    visualNodeObj.transform.SetParent(line.lineContent, false);

                    var pos = ((RectTransform)visualNodeObj.transform).anchoredPosition;

                    pos.x += nestCount * 24;

                    ((RectTransform)visualNodeObj.transform).anchoredPosition = pos;

                    var visualNode = visualNodeObj.GetComponent<StatementNodeView>();

                    visualNode.scriptNode = statementNode;
                    line.scriptNodes.Add(visualNodeObj);

                    if (node.nestedNodes.Count > 0) {

                        nestCount++;
                        AddLines(node.nestedNodes);
                        nestCount--;
                        AddEndBracket();

                    }

                }
                else {
                    Debug.LogError("what?");
                }

                lines.Add(line);


            }

        }

        AddLines(script);

    }

    void InitFuncData() {

        var obj = Instantiate(funcDataPrefab);
        obj.transform.SetParent(transform, false);
        funcData = obj.GetComponent<FuncDataView>();

    }

    void InitUIGameObject(GameObject prefab) {

        var obj = Instantiate(prefab);
        obj.transform.SetParent(transform, false);
        uiFillerObjects.Add(obj);

    }

}