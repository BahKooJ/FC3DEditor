﻿

using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class VisualScriptingScriptWindowView : MonoBehaviour {

    // - Prefabs -
    public GameObject linePrefab;
    public GameObject statementNodePrefab;
    public GameObject endCodeBracket;

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

        AddLines(script.scriptNodes);

    }

    public void Clear() {

        foreach (var line in lines) {
            Destroy(line.gameObject);
        }

        lines.Clear();

    }

}