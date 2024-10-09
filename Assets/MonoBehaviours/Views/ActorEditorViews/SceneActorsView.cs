

using FCopParser;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneActorsView : MonoBehaviour {

    // - Prefabs -
    public GameObject actorNodeListItemFab;

    // - Unity Refs -
    public Transform listContent;
    public ScrollRect contentScrollview;
    public TMP_Dropdown sortDropdown;
    public ActorPropertiesView view;

    // - Parameters -
    public FCopLevel level;
    public ActorEditMode controller;

    public List<ActorNodeListItemView> actorNodes = new();
    public Dictionary<int, ActorNodeListItemView> actorNodesByID = null;

    public ActorNodeListItemView lookingToReorder = null;
    public float delayedScrollPos = 0f;
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

                    InitListNode(node, false);

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

    void InitListNode(ActorNode node, bool forceGroup) {

        var obj = Instantiate(actorNodeListItemFab);
        obj.transform.SetParent(listContent, false);

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

}