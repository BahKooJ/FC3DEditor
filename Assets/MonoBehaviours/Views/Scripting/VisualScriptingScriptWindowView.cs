﻿

using FCopParser;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static ScriptingPanelView;

public class VisualScriptingScriptWindowView : MonoBehaviour {

    // - Prefabs -
    public GameObject linePrefab;
    public GameObject statementNodePrefab;
    public GameObject endCodeBracket;

    // - Unity Refs -
    public TMP_InputField debugInput;
    public TMP_InputField commentInput;
    public TMP_InputField commentWindow;
    public Transform codeScrollView;

    // - Parameters -
    public FCopScript script;

    List<VisualScriptingLineView> lines = new();

    public void Init() {

        Clear();

        if (script.failed) {
            return;
        }

        var i = 1;

        var nestCount = 0;

        VisualScriptingLineView CreateLineFab(StatementNodeView parentNode, StatementNodeView visualNode, int localIndex) {
            var lineObj = Instantiate(linePrefab);

            lineObj.transform.SetParent(codeScrollView, false);
            var line = lineObj.GetComponent<VisualScriptingLineView>();
            line.number = i;
            i++;

            line.view = this;
            line.parentNodeView = parentNode;
            line.scriptNode = visualNode;
            line.localIndex = localIndex;

            lines.Add(line);

            return line;
        }

        VisualScriptingLineView AddEndBracket(StatementNodeView parentNode) {

            var line = CreateLineFab(parentNode, null, ((ScriptNestingNode)parentNode.scriptNode).nestedNodes.Count);

            var endObj = Instantiate(endCodeBracket);
            endObj.transform.SetParent(line.lineContent, false);

            var pos = ((RectTransform)endObj.transform).anchoredPosition;

            pos.x += nestCount * 24;

            ((RectTransform)endObj.transform).anchoredPosition = pos;

            return line;

        }

        List<VisualScriptingLineView> AddLines(List<ScriptNode> nodes, StatementNodeView parentNode) {

            var linesAdded = new List<VisualScriptingLineView>();

            var localIndex = 0;
            foreach (var node in nodes) {

                var visualNodeObj = Instantiate(statementNodePrefab);
                var visualNode = visualNodeObj.GetComponent<StatementNodeView>();

                var line = CreateLineFab(parentNode, visualNode, localIndex);

                linesAdded.Add(line);

                visualNodeObj.transform.SetParent(line.lineContent, false);

                var pos = ((RectTransform)visualNodeObj.transform).anchoredPosition;

                pos.x += nestCount * 24;

                ((RectTransform)visualNodeObj.transform).anchoredPosition = pos;

                visualNode.scriptNode = node;
                visualNode.currentLine = line;

                if (node is ScriptNestingNode nestingNode) {

                    nestCount++;
                    var nestedLinesAdded = AddLines(nestingNode.nestedNodes, visualNode);
                    nestCount--;

                    nestedLinesAdded.Add(AddEndBracket(visualNode));
                    visualNode.associatedLines = nestedLinesAdded;
                    linesAdded.AddRange(nestedLinesAdded);

                }

                localIndex++;
            }

            return linesAdded;

        }

        AddLines(script.code, null);
        CreateLineFab(null, null, script.code.Count);

        refuseCallback = true;

        commentInput.text = script.comment;

        debugInput.text = "";
        foreach (var b in script.compiledBytes) {
            debugInput.text += b.ToString() + " ";
        }
        refuseCallback = false;

    }

    public void Clear() {

        foreach (var line in lines) {
            Destroy(line.gameObject);
        }

        lines.Clear();

        if (commentWindow.gameObject.activeSelf) {
            commentWindow.gameObject.SetActive(false);
        }

    }

    bool refuseCallback = false;
    public void OnFinishDebugType() {

        if (refuseCallback) return;

        var total = new List<byte>();

        var value = "";

        foreach (var c in debugInput.text) {

            if (c == ' ') {

                total.Add(byte.Parse(value));
                value = "";
                continue;
            }

            value += c;

        }
        if (value != "") {

            total.Add(byte.Parse(value));

        }

        script.compiledBytes = total;
        script.Refresh();
        Init();

    }

    public void OnStartType() {

        Main.ignoreAllInputs = true;

    }

    public void OnEndType() {

        Main.ignoreAllInputs = false;

    }

    public void OnClickComment() {

        if (script == null) return; 

        commentWindow.gameObject.SetActive(!commentWindow.gameObject.activeSelf);

        if (!commentWindow.gameObject.activeSelf) {
            script.comment = commentWindow.text;
        }

        commentWindow.text = script.comment;
        commentInput.text = script.comment;

    }

    public void OnReceiveTrash() {

        if (Main.draggingElement.TryGetComponent<StatementNodeView>(out var viewItem)) {

            Main.AddCounterAction(new ScriptSaveStateCounterAction(script, this));

            script.RemoveNode(viewItem.scriptNode);
            Init();

        }

    }

}