

using FCopParser;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SceneActorsView : MonoBehaviour {

    // - Prefabs -
    public GameObject actorNodeListItemFab;

    // - Unity Refs -
    public Transform listContent;
    public ScrollRect contentScrollview;
    public TMP_Dropdown sortDropdown;
    public TMP_InputField searchBar;
    public ActorPropertiesView view;
    public ActorExcludeView excludeView;

    // - Parameters -
    public FCopLevel level;
    public ActorEditMode controller;

    [HideInInspector]
    public List<ActorNodeListItemView> actorNodes = new();
    [HideInInspector]
    public Dictionary<int, ActorNodeListItemView> actorNodesByID = null;

    [HideInInspector]
    public ActorNodeListItemView lookingToReorder = null;
    [HideInInspector]
    public float delayedScrollPos = 0f;
    [HideInInspector]
    public int delayScrollPosCount = -1;

    void Start() {

        delayScrollPosCount = -1;
        Refresh();

    }

    private void Update() {

        if (delayScrollPosCount != -1) {

            if (delayScrollPosCount == 0) {
                contentScrollview.verticalNormalizedPosition = delayedScrollPos;
            }

            delayScrollPosCount--;

        }
        
    }

    public void ClearList() {

        foreach (var node in actorNodes) {

            foreach (var nestNode in node.actorNodes) {
                Destroy(nestNode.gameObject);
            }

            Destroy(node.gameObject);

        }

        actorNodes.Clear();
        actorNodesByID = null;

    }

    public void Refresh(bool preserveScrollPosition = false) {

        var scrollPos = contentScrollview.verticalNormalizedPosition;

        ClearList();

        switch (sortDropdown.value) {
            case 0:
                actorNodesByID = new();
                foreach (var node in level.sceneActors.positionalGroupedActors) {

                    bool isAllowed = false;
                    foreach (var actor in node.nestedActors) {

                        if (SettingsManager.allowedActorsInSceneView[actor.behaviorType]) {
                            isAllowed = true;
                            break;
                        }

                    }

                    if (isAllowed) {
                        InitListNode(node, false);
                    }

                }
                break;
            case 1:

                foreach (var node in level.sceneActors.behaviorGroupedActors) {

                    InitListNode(node.Value, true);

                }
                break;
            case 2:
                break;
        }

        if (preserveScrollPosition) {
            contentScrollview.verticalNormalizedPosition = scrollPos;
        }

    }

    public void Validate() {

        ClearSearch();

        var openedNodes = new List<ActorNodeListItemView>();

        foreach (var node in actorNodes) {

            if (node.actorNodes.Count > 0) {
                openedNodes.Add(node);
            }

            node.CloseGroup();
        }

        var unvalidateNodes = new List<ActorNodeListItemView>(actorNodes);
        var forceGrouping = sortDropdown.value == 1;

        List<ActorNode> nodes = new();

        switch (sortDropdown.value) {
            case 0:
                nodes = level.sceneActors.positionalGroupedActors;
                break;
            case 1:
                nodes = level.sceneActors.behaviorGroupedActors.Values.ToList();
                break;
            case 2:

                break;
        }

        var pi = 0;
        foreach (var node in nodes) {

            bool isAllowed = false;
            foreach (var actor in node.nestedActors) {

                if (SettingsManager.allowedActorsInSceneView[actor.behaviorType]) {
                    isAllowed = true;
                    break;
                }

            }

            var nodeView = unvalidateNodes.FirstOrDefault(n => n.node == node);

            if (nodeView != null) {

                if (isAllowed) {

                    // Node view is existing and validated.
                    nodeView.transform.SetSiblingIndex(pi);
                    unvalidateNodes.Remove(nodeView);
                    nodeView.RefreshDisplay();

                }

            }
            else {

                if (isAllowed) {
                    InitListNode(node, forceGrouping, pi);
                }

            }

            pi++;

        }

        foreach (var openedNode in openedNodes) {

            if (!unvalidateNodes.Contains(openedNode)) {

                if (openedNode.node.nestedActors.Count > 1) {
                    openedNode.OpenGroup();
                }

            }

        }

        foreach (var node in unvalidateNodes) {

            foreach (var nestNode in node.actorNodes) {
                Destroy(nestNode.gameObject);
            }

            Destroy(node.gameObject);

            actorNodes.Remove(node);

            if (actorNodesByID != null && node.node.nestedActors.Count > 0) {
                actorNodesByID.Remove(node.node.nestedActors[0].DataID);
            }

        }

    }

    public void StartType() {
        Main.ignoreAllInputs = true;
    }

    public void StopType() {
        Main.ignoreAllInputs = false;
    }

    public void Search() {

        if (searchBar.text == "") {
            ClearSearch();
            return;
        }

        foreach (var node in actorNodes) {

            if (node.node.nestedActors.Count > 1) {

                var hasName = false;
                foreach (var actor in node.node.nestedActors) {

                    if (actor.name.ToUpper().Contains(searchBar.text.ToUpper()) || searchBar.text == "") {
                        hasName = true;
                        break;
                    }

                }

                if (hasName) {

                    node.gameObject.SetActive(true);

                    foreach (var nestNode in node.actorNodes) {

                        if (nestNode.actor.name.ToUpper().Contains(searchBar.text.ToUpper()) || searchBar.text == "") {
                            nestNode.gameObject.SetActive(true);
                        }
                        else {
                            nestNode.gameObject.SetActive(false);
                        }

                    }

                }
                else {
                    node.gameObject.SetActive(false);

                    foreach (var nestNode in node.actorNodes) {
                        
                        nestNode.gameObject.SetActive(false);

                    }

                }

            }
            else {

                if (node.node.name.ToUpper().Contains(searchBar.text.ToUpper()) || searchBar.text == "") {
                    node.gameObject.SetActive(true);
                }
                else {
                    node.gameObject.SetActive(false);
                }

            }

        }

    }

    public void ClearSearch() {

        foreach (var node in actorNodes) {

            node.gameObject.SetActive(true);

            foreach (var nestNode in node.actorNodes) {
                nestNode.gameObject.SetActive(true);
            }

        }

    }

    public void RequestDelayedScrollPosUpdate() {
        delayedScrollPos = contentScrollview.verticalNormalizedPosition;
        delayScrollPosCount = 5;
    }

    public void RefreshSelection(bool jump) {

        foreach (var actorNode in actorNodes) {
            
            if (actorNode.node.nestedActors.Count > 1 || actorNode.forceGroup) {

                if (actorNode.actorNodes.Count == 0 && controller.selectedActor != null) {

                    foreach (var act in actorNode.node.nestedActors) {

                        if (act.DataID == controller.selectedActor.DataID) {
                            actorNode.OpenGroup();
                            break;
                        }

                    }

                }

                foreach (var actorNodeN in actorNode.actorNodes) {
                    actorNodeN.RefreshSelection(jump);
                }

            }
            else {
                actorNode.RefreshSelection(jump);
            }

        }

    }

    public void ClearSelection() {

        foreach (var actorNode in actorNodes) {

            if (actorNode.node.nestedActors.Count > 1 || actorNode.forceGroup) {

                foreach (var actorNodeN in actorNode.actorNodes) {
                    actorNodeN.ClearSelection();
                }

            }
            else {

                actorNode.ClearSelection();

            }

        }

    }

    public void RemoveNode(FCopActor actor) {

        if (actorNodesByID != null && actorNodesByID.ContainsKey(actor.DataID)) {

            var node = actorNodesByID[actor.DataID];

            actorNodesByID.Remove(actor.DataID);
            actorNodes.Remove(node);
            Destroy(node.gameObject);

        }
        else {

            ActorNodeListItemView nodeToDestroy = null;

            foreach (var node in actorNodes) {

                foreach (var nestedNode in node.actorNodes) {

                    if (nestedNode.actor == actor) {

                        nodeToDestroy = nestedNode;
                        break;

                    }

                }

                if (nodeToDestroy != null) {

                    node.actorNodesByID?.Remove(actor.DataID);
                    node.actorNodes.Remove(nodeToDestroy);
                    Destroy(nodeToDestroy.gameObject);

                    if (node.actorNodes.Count == 1 && !node.forceGroup) {

                        foreach (var nestedNode in node.actorNodes) {
                            Destroy(nestedNode.gameObject);
                        }

                        var fcopNode = node.node;
                        var siblingIndex = node.transform.GetSiblingIndex();

                        actorNodesByID?.Remove(actor.DataID);
                        actorNodes.Remove(node);
                        Destroy(node.gameObject);

                        InitListNode(fcopNode, false);
                        actorNodes.Last().transform.SetSiblingIndex(siblingIndex);

                    }
                    else if (node.actorNodes.Count == 0) {
                        actorNodesByID?.Remove(actor.DataID);
                        actorNodes.Remove(node);
                        Destroy(node.gameObject);
                    }

                    break;

                }

            }

        }

    }

    public ActorNodeListItemView GetNodeItemByID(int id) {

        if (actorNodesByID != null) {

            if (actorNodesByID.ContainsKey(id)) {
                return actorNodesByID[id];
            }

        }

        foreach (var node in actorNodes) {

            if (node.actorNodesByID != null) {

                if (node.actorNodesByID.ContainsKey(id)) {
                    return node.actorNodesByID[id];
                }

            }

        }

        return null;

    }

    void InitListNode(ActorNode node, bool forceGroup, int siblingIndex = -1) {

        var obj = Instantiate(actorNodeListItemFab);
        obj.transform.SetParent(listContent, false);

        if (siblingIndex != -1) {
            obj.transform.SetSiblingIndex(siblingIndex);
        }

        var nodeListItem = obj.GetComponent<ActorNodeListItemView>();
        nodeListItem.node = node;
        nodeListItem.view = this;
        nodeListItem.forceGroup = forceGroup;

        actorNodes.Add(nodeListItem);

        if (!forceGroup) {

            if (node.nestedActors.Count == 1) {
                actorNodesByID[node.nestedActors[0].DataID] = nodeListItem;
            }

        }

    }

    public void OnGroupDrowdownChange() {

        Refresh();

    }

    public void OnClickExcludeContext() {

        excludeView.gameObject.SetActive(!excludeView.gameObject.activeSelf);

    }

}