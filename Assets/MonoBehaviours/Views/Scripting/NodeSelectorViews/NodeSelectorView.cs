
using System;
using System.Collections.Generic;
using UnityEngine;
using static NodeSelectorViewUtil;

public class NodeSelectorView : MonoBehaviour {

    // - Prefabs -
    public GameObject nodeSelectorTab;
    public GameObject nodeViewItem;

    // - Unity Refs -
    public Transform tabContent;
    public Transform listContent;

    [HideInInspector]
    public NodeSelectorTab selectorTab = NodeSelectorTab.System;
    [HideInInspector]
    public List<NodeSelectorTabView> tabs = new();
    [HideInInspector]
    public List<NodeSelectorViewItem> items = new();

    void Start() {
        
        foreach (NodeSelectorTab tabCase in Enum.GetValues(typeof(NodeSelectorTab))) {
            var tabObj = Instantiate(nodeSelectorTab, tabContent, false);
            tabObj.SetActive(true);
            var tabView = tabObj.GetComponent<NodeSelectorTabView>();
            tabView.tab = tabCase;
            tabView.view = this;
            tabs.Add(tabView);
        }

        var nodeData = nodeCreatorData[selectorTab];

        foreach (var data in nodeData) {

            var obj = Instantiate(nodeViewItem, listContent, false);
            obj.SetActive(true);
            var itemView = obj.GetComponent<NodeSelectorViewItem>();
            itemView.creatorData = data;
            itemView.view = this;
            items.Add(itemView);

        }

    }

}