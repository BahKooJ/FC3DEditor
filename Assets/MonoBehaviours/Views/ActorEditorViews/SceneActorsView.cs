

using FCopParser;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class SceneActorsView : MonoBehaviour {

    // - Prefabs -
    public GameObject actorNodeListItemFab;

    // - Unity Refs -
    public Transform listContent;
    public TMP_Dropdown sortDropdown;
    public ActorPropertiesView view;

    // - Parameters -
    public FCopLevel level;
    public ActorEditMode controller;

    List<ActorNodeListItemView> actorNodes = new();

    void Start() {

        Refresh();

    }

    public void ClearList() {

        foreach (var node in actorNodes) {

            foreach (var nestNode in node.actorNodes) {
                Destroy(nestNode.gameObject);
            }

            Destroy(node.gameObject);

        }

        actorNodes.Clear();

    }

    public void Refresh() {

        ClearList();

        switch (sortDropdown.value) {
            case 0:
                
                foreach (var node in level.sceneActors.positionalGroupedActors) {

                    InitListNode(node.Value, false);

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

    }

    void InitListNode(ActorNode node, bool forceGroup) {

        var obj = Instantiate(actorNodeListItemFab);
        obj.transform.SetParent(listContent, false);

        var nodeListItem = obj.GetComponent<ActorNodeListItemView>();
        nodeListItem.node = node;
        nodeListItem.view = this;
        nodeListItem.forceGroup = forceGroup;

        actorNodes.Add(nodeListItem);

    }

    public void OnGroupDrowdownChange() {

        Refresh();

    }

}