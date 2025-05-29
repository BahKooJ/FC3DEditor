using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FCopParser;
using UnityEngine.UI;

public class VisualScriptingLineView : MonoBehaviour {

    // - Unity Refs -
    public TMP_Text numberText;
    public Transform lineContent;

    // - Parameters -
    [HideInInspector]
    public int number;
    [HideInInspector]
    public int localIndex;
    [HideInInspector]
    public StatementNodeView scriptNode;
    [HideInInspector]
    public StatementNodeView parentNodeView;
    [HideInInspector]
    public VisualScriptingScriptWindowView view;

    private void Start() {
        numberText.text = number.ToString();
    }

    public void RebuildScriptNode() {
        scriptNode.Rebuild();
    }

    // Love how I have to yell at unity 22 different times just to get the damn layouts to do their job.
    public void RefreshLayout() {

        void ReloadLayout(Transform transform) {

            Canvas.ForceUpdateCanvases();

            if (transform.gameObject.TryGetComponent<HorizontalLayoutGroup>(out var layouts)) {
                layouts.enabled = false;
                layouts.enabled = true;
            }

            foreach (Transform transform1 in transform) {
                ReloadLayout(transform1);
            }

        }

        Canvas.ForceUpdateCanvases();

        foreach (Transform transform in lineContent) {
            ReloadLayout(transform);
        }


    }

    public void OnReceiverDrag() {

        ScriptNode scriptNode = null;

        if (Main.draggingElement.TryGetComponent<StatementNodeView>(out var viewItem)) {

            scriptNode = viewItem.scriptNode;

        }
        else if (Main.draggingElement.TryGetComponent<ScriptNodeCreatorItemView>(out var creatorItem)) {

            scriptNode = creatorItem.Create(null);

        }

        if (scriptNode == null) {
            return;
        }

        var parentNodeOfDragged = scriptNode.parent;

        // Can't nest a node in itself.
        if (parentNodeView != null && scriptNode == parentNodeView.scriptNode) {
            return;
        }

        // Finds the scripting node and removes it so it can be added else where.
        int indexOfRemoved = view.script.RemoveNode(scriptNode);

        if (parentNodeView != null && parentNodeView.scriptNode is ScriptNestingNode parentNestingNode) {

            if (parentNestingNode == parentNodeOfDragged) {

                var indexOfThis = localIndex;

                if (indexOfRemoved != - 1 && indexOfRemoved < indexOfThis) {
                    indexOfThis--;
                }
                parentNestingNode.NestNode(scriptNode, indexOfThis);

            }
            else {
                parentNestingNode.NestNode(scriptNode, localIndex);
            }


        }
        else if (scriptNode.byteCode != ByteCode.JUMP) {

            if (parentNodeOfDragged == null) {

                var indexOfThis = localIndex;

                if (indexOfRemoved != -1 && indexOfRemoved < indexOfThis) {
                    indexOfThis--;
                }

                view.script.code.Insert(indexOfThis, scriptNode);

            }
            else {

                scriptNode.parent = null;
                view.script.code.Insert(localIndex, scriptNode);

            }

        }

        view.Init();

    }

}