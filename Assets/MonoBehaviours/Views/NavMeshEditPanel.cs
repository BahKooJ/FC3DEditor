

using FCopParser;
using System.Linq;
using UnityEngine;

class NavMeshEditPanel : MonoBehaviour {

    public NavMeshEditMode controller;

    public GameObject navMeshButton;

    public GameObject listView;

    void Start() {

        foreach (var index in Enumerable.Range(0, controller.main.level.navMeshes.Count)) {

            var button = Instantiate(navMeshButton);
            var script = button.GetComponent<NavMeshButton>();
            script.index = index;
            script.controller = this;

            button.transform.SetParent(listView.transform, false);

        }

    }

    public void AddNode() {

        controller.UnselectNode();

        var obj = Instantiate(controller.main.NavMeshPoint);
        var script = obj.GetComponent<NavNodePoint>();
        controller.navNodeToAdd = script;

    }

    public void ClearPaths() {

        if (controller.selectedNavNode == null) {
            return;
        }

        var script = controller.selectedNavNode.controlledObject.GetComponent<NavNodePoint>();
        script.ClearPaths();

    }

    public void ChangeNavMesh(int index) {

        controller.selectedNavMesh = index;
        controller.OnDestroy();
        controller.OnCreateMode();

    }

}