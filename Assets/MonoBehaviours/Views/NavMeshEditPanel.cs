using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NavMeshEditPanel : MonoBehaviour {

    public NavMeshEditMode controller;

    public GameObject navMeshButton;

    public GameObject listView;

    public Toggle isStartToggle;

    void Start() {

        foreach (var index in Enumerable.Range(0, controller.main.level.navMeshes.Count)) {

            var button = Instantiate(navMeshButton);
            var script = button.GetComponent<NavMeshButton>();
            script.index = index;
            script.controller = this;

            button.transform.SetParent(listView.transform, false);

        }

    }

    public void RefeshCheck() {
        isStartToggle.isOn = controller.selectedNavNode.controlledObject.GetComponent<NavNodePoint>().node.isStartingPoint;
    }

    public void AddNode() {

        controller.StartNodeToAdd();

    }

    public void ClearPaths() {

        if (controller.selectedNavNode == null) {
            return;
        }

        var script = controller.selectedNavNode.controlledObject.GetComponent<NavNodePoint>();
        script.ClearPaths();

    }

    public void ChangeNavMesh(int index) {

        controller.selectedNavMeshIndex = index;
        controller.OnDestroy();
        controller.OnCreateMode();

    }

    public void OnValueChangeStartToggle() {

        if (controller == null) { return; }

        if (controller.selectedNavNode == null) { return; }

        controller.selectedNavNode.controlledObject.GetComponent<NavNodePoint>().node.isStartingPoint = isStartToggle.isOn;

    }

    public void ClearNavMesh() {

        controller.ClearNavMesh();

    }

    public void CopyNavNodeCoords() {

        controller.CopyNavNodeCoords();

    }

}