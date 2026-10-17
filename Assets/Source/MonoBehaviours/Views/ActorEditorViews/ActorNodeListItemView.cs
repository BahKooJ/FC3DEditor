
using FCopParser;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ActorNodeListItemView : MonoBehaviour {

    // - Prefab -
    GameObject actorNodeListItemFab;

    // - Unity Asset -
    public Sprite folderTexture;
    public Sprite actorTexture;

    // - Unity Refs -
    public TMP_Text nameText;
    public TMP_InputField nameField;
    public ContextMenuHandler contextMenu;
    public Image icon;
    public Image background;
    public GameObject pad;
    public DragableUIElement dragableElement;

    // - Parameters -
    public ActorNode node = null;
    public FCopActor actor = null;
    public ActorNode parent = null;
    [HideInInspector]
    public SceneActorsView view;
    [HideInInspector]
    public bool forceGroup = false;

    [HideInInspector]
    public List<ActorNodeListItemView> actorNodes = new();
    public Dictionary<int, ActorNodeListItemView> actorNodesByID = null;


    int clickTimer = 0;

    private void Start() {

        dragableElement.onDragCallback = () => {
            CloseGroup();
        };

        foreach (var gobj in transform.GetComponentsInChildren<ReceiveDragable>()) {
            gobj.expectedTransform = transform.parent;
        }

        contextMenu.items = new() {
            ("Rename", Rename),
            ("Group", StartGroup),
            ("Ungroup", Ungroup),
            ("Delete", Delete),
        };

        actorNodeListItemFab = view.actorNodeListItemFab;

        if (node != null) {

            if (node.nestedActors.Count == 1 && node.groupType == ActorGroupType.Position) {
                nameText.text = node.nestedActors[0].name;
            }
            else {
                nameText.text = node.name;

            }

            if (node.nestedActors.Count > 1 || forceGroup) {
                icon.sprite = folderTexture;
            }

        }
        else {
            nameText.text = actor.name;

            pad.SetActive(true);
        }

        RefreshSelection(true);

    }

    private void Update() {
        
        if (clickTimer > 0) {
            clickTimer--;
        }

    }

    public void RefreshDisplay() {

        if (node != null) {

            if (node.nestedActors.Count == 1 && node.groupType == ActorGroupType.Position) {
                nameText.text = node.nestedActors[0].name;
            }
            else {
                nameText.text = node.name;
            }

            if (node.nestedActors.Count > 1 || forceGroup) {
                icon.sprite = folderTexture;
            }
            else {
                icon.sprite = actorTexture;
            }

        }
        else {
            nameText.text = actor.name;

            pad.SetActive(true);
        }

    }

    public void RefreshName() {

        if (node != null) {

            if (node.nestedActors.Count == 1) {
                nameText.text = node.nestedActors[0].name;
            }
            else {
                nameText.text = node.name;

            }
        }
        else {
            nameText.text = actor.name;
        }

    }

    public void RefreshSelection(bool jump) {

        if (node != null) {

            if (node.nestedActors.Count > 1) {
                background.color = Main.mainColor;
                return;
            }

        }

        if (forceGroup) {
            background.color = Main.mainColor;
            return; 
        }

        if (view.controller.selectedActor != null) {

            if (node != null) {

                if (view.controller.selectedActor.DataID == node.nestedActors[0].DataID) {
                    background.color = Main.selectedColor;

                    if (jump) {
                        JumpViewToListItem();
                    }

                    return;
                }

            }
            else {

                if (view.controller.selectedActor.DataID == actor.DataID) {
                    background.color = Main.selectedColor;

                    if (jump) {
                        JumpViewToListItem();
                    }

                    return;
                }

            }

        }

        background.color = Main.mainColor;

    }

    public void ClearSelection() {

        background.color = Main.mainColor;

    }

    void JumpViewToListItem() {

        try {
            var normalizedPos = ((decimal)transform.GetSiblingIndex()) / ((decimal)transform.parent.childCount - 10);

            view.contentScrollview.verticalNormalizedPosition = (float)(1 - normalizedPos);
        }
        catch { }

    }

    void Rename() {

        nameText.gameObject.SetActive(false);
        nameField.gameObject.SetActive(true);

        nameField.text = nameText.text;
        nameField.Select();

    }

    void Ungroup() {

        if (actor != null) {

            view.controller.UngroupActor(actor);

        }


    }

    void Delete() {

        if (node != null) {

            if (node.nestedActors.Count == 1 && !forceGroup) {
                view.controller.DeleteByID(node.nestedActors[0].DataID);
            }

        }
        else {

            view.controller.DeleteByID(actor.DataID);

        }

    }

    void StartGroup() {

        if (node != null) {

            if (node.nestedActors.Count == 1 && !forceGroup) {
                view.controller.StartGroup(node.nestedActors[0]);
            }

        }
        else {

            view.controller.StartGroup(actor);

        }

    }

    void ReOrder() {

        if (node == null) {
            return;
        }

        if (node.groupType != ActorGroupType.Position) {
            return;
        }

        if (view.lookingToReorder == null) {
            view.lookingToReorder = this;
            return;
        }

        var indexStart = view.actorNodes.IndexOf(view.lookingToReorder);
        var indexOfThis = view.actorNodes.IndexOf(this);

        view.level.sceneActors.ReorderPositionalGroup(indexStart, indexOfThis);

        view.lookingToReorder = null;

        view.Refresh();
        view.RequestDelayedScrollPosUpdate();

    }

    public void CloseGroup() {

        if (actorNodes.Count > 0) {

            foreach (var actorNode in actorNodes) {
                Destroy(actorNode.gameObject);
            }

            actorNodes.Clear();
            actorNodesByID = null;

        }

    }

    public void OpenGroup() {

        if (actorNodes.Count > 0) {

            foreach (var actorNode in actorNodes) {
                Destroy(actorNode.gameObject);
            }

            actorNodes.Clear();
            actorNodesByID = null;

        }
        else {

            actorNodesByID = new();

            foreach (var actor in node.nestedActors) {

                var obj = Instantiate(actorNodeListItemFab);
                obj.transform.SetParent(view.listContent, false);


                obj.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);

                var nodeListItem = obj.GetComponent<ActorNodeListItemView>();
                nodeListItem.actor = actor;
                nodeListItem.parent = node;
                nodeListItem.view = view;

                actorNodes.Add(nodeListItem);

                actorNodesByID[actor.DataID] = nodeListItem;

            }

        }

    }

    // - Unity Events -

    public void OnClick() {

        if (view.controller.actorToGroup != null) {

            if (forceGroup) {
                return;
            }

            if (node != null) {

                view.controller.GroupActor(node);

            }
            else {

                if (parent != null) {

                    view.controller.GroupActor(parent);

                }

            }

            return;

        }

        if (actor != null) {

            view.controller.SelectActorByID(actor.DataID);

            if (clickTimer != 0) {
                view.controller.MoveToActor(actor.DataID);
            }

            clickTimer = 30;

            return;

        }

        if (node.nestedActors.Count == 1 && !forceGroup) {

            view.controller.SelectActorByID(node.nestedActors[0].DataID);

            if (clickTimer != 0) {
                view.controller.MoveToActor(node.nestedActors[0].DataID);
            }

        }
        else {

            OpenGroup();

        }

        clickTimer = 30;

    }

    public void OnFinishNameType() {

        if (node != null) {

            if (node.nestedActors.Count == 1) {
                view.controller.RenameActor(node.nestedActors[0], nameField.text);
            }
            else {

                node.name = nameField.text;

                nameText.text = node.name;

            }
        }
        else {

            view.controller.RenameActor(actor, nameField.text);

        }

        nameText.gameObject.SetActive(true);
        nameField.gameObject.SetActive(false);
        Main.ignoreAllInputs = false;

    }

    public void OnStartNameType() {

        Main.ignoreAllInputs = true;

    }

    public void OnReceiveInsertDrag() {

        ActorNodeListItemView nodeView;

        // If this passes, then node view is nested inside a group
        if (node == null) {

            if (Main.draggingElement.TryGetComponent(out nodeView)) {

                // If this passes, then the node is in the root.
                if (nodeView.node != null) {

                    // This checks to make sure it is not a group in the root.
                    if (nodeView.node.nestedActors.Count == 1 && !nodeView.forceGroup) {
                        view.controller.GroupActor(nodeView.node.nestedActors[0], parent);
                    }

                }
                // Both are not in root, makes sure they're under the same parent for reordering.
                // By the way, for the view, nested nodes go from last first.
                else if (parent == nodeView.parent) {

                    var indexOfDraged = parent.nestedActors.IndexOf(nodeView.actor);
                    var indexOfThis = parent.nestedActors.IndexOf(actor);

                    if (indexOfDraged > indexOfThis) {
                        indexOfThis++;
                    }

                    view.level.sceneActors.ReorderInsideNode(nodeView.actor, indexOfThis, parent);

                    view.Validate();

                }

            }

            return;
        }

        if (node.groupType != ActorGroupType.Position) {
            return;
        }

        if (Main.draggingElement.TryGetComponent(out nodeView)) {

            var indexOfDraged = view.level.sceneActors.positionalGroupedActors.IndexOf(nodeView.node);
            var indexOfThis = view.level.sceneActors.positionalGroupedActors.IndexOf(node);

            // This node is not in root
            if (nodeView.node == null) {
                view.controller.UngroupActor(nodeView.actor);
                
                // When ungrouping, the dragged node got destroyed. It refinds the node.
                var newNodeView = view.level.sceneActors.positionalGroupedActors.FirstOrDefault(n => n.nestedActors[0] == nodeView.actor);
                indexOfDraged = view.level.sceneActors.positionalGroupedActors.IndexOf(newNodeView);

            }

            if (indexOfDraged < indexOfThis) {
                indexOfThis--;
            }

            view.level.sceneActors.ReorderPositionalGroup(indexOfDraged, indexOfThis);

            view.Validate();

        }

    }

    public void OnReceiveGroupDrag() {

        ActorNodeListItemView nodeView;

        // If this passes, then the receiving node view is nested inside a group
        if (node == null) {

            if (Main.draggingElement.TryGetComponent(out nodeView)) {

                // If this passes, then the dragged node is in the root.
                if (nodeView.node != null) {

                    // This checks to make sure it is not a group in the root.
                    if (nodeView.node.nestedActors.Count == 1 && !nodeView.forceGroup) {
                        view.controller.GroupActor(nodeView.node.nestedActors[0], parent);
                    }

                }
                // Both are not in root, checks to see if they're under different parents.
                // (If they are it doesn't matter)
                else if (parent != nodeView.parent) {

                    view.controller.UngroupActor(nodeView.actor);

                    // When ungrouping, the dragged node got destroyed. It refinds the node.
                    var newNodeView = view.level.sceneActors.positionalGroupedActors.FirstOrDefault(n => n.nestedActors[0] == nodeView.actor);

                    view.controller.GroupActor(newNodeView.nestedActors[0], parent);

                }

            }

            return;

        }

        if (node.groupType != ActorGroupType.Position) {
            return;
        }

        if (Main.draggingElement.TryGetComponent(out nodeView)) {

            // Dragged node is not in root
            if (nodeView.node == null) {
                view.controller.UngroupActor(nodeView.actor);

                // When ungrouping, the dragged node got destroyed. It refinds the node.
                var newNodeView = view.level.sceneActors.positionalGroupedActors.FirstOrDefault(n => n.nestedActors[0] == nodeView.actor);

                view.controller.GroupActor(newNodeView.nestedActors[0], node);

            }
            else if (nodeView.node.nestedActors.Count == 1) {

                view.controller.GroupActor(nodeView.node.nestedActors[0], node);

            }

            view.Validate();

        }

    }

}
