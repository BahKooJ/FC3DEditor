

using UnityEngine;
using TMPro;

public class NodeSelectorTabView : MonoBehaviour {

    // - Unity Refs -
    public TMP_Text label;

    // - Parameters -
    [HideInInspector]
    public NodeSelectorViewUtil.NodeSelectorTab tab;
    [HideInInspector]
    public NodeSelectorView view;

    void Start() {

        label.text = tab.ToString();

    }

    public void OnClick() {

        view.SelectTab(tab);

    }

}