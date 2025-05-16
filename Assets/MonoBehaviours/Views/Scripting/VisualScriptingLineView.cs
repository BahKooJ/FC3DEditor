using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FCopParser;

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

    public void OnReceiverDrag() {

        if (Main.draggingElement.TryGetComponent<StatementNodeView>(out var viewItem)) {

            var parentNodeOfDragged = viewItem.scriptNode.parent;

            // Can't nest a node in itself.
            if (parentNodeView != null && viewItem.scriptNode == parentNodeView.scriptNode) {
                return;
            }

            int indexOfRemoved;

            // Finds the scripting node and removes it so it can be added else where.
            if (parentNodeOfDragged == null) {
                indexOfRemoved = view.script.code.IndexOf(viewItem.scriptNode);
                view.script.code.Remove(viewItem.scriptNode);
            }
            else {
                indexOfRemoved = parentNodeOfDragged.nestedNodes.IndexOf(viewItem.scriptNode);
                parentNodeOfDragged.nestedNodes.Remove(viewItem.scriptNode);
            }

            if (parentNodeView != null && parentNodeView.scriptNode is ScriptNestingNode parentNestingNode) {

                if (parentNestingNode == parentNodeOfDragged) {

                    var indexOfThis = localIndex;

                    if (indexOfRemoved < indexOfThis) {
                        indexOfThis--;
                    }

                    parentNestingNode.nestedNodes.Insert(indexOfThis, viewItem.scriptNode);

                }
                else {
                    viewItem.scriptNode.parent = parentNestingNode;
                    parentNestingNode.nestedNodes.Insert(localIndex, viewItem.scriptNode);
                }


            }
            else {

                if (parentNodeOfDragged == null) {

                    var indexOfThis = localIndex;

                    if (indexOfRemoved < indexOfThis) {
                        indexOfThis--;
                    }

                    view.script.code.Insert(indexOfThis, viewItem.scriptNode);

                }
                else {

                    viewItem.scriptNode.parent = null;
                    view.script.code.Insert(localIndex, viewItem.scriptNode);

                }

            }

            view.Init();

        }

    }

}