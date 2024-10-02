using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavMeshEditPanel : MonoBehaviour {

    public Toggle isStartToggle;
    public CustomDropdown navMeshDropdown;

    public NavMeshEditMode controller;


    void Start() {

        foreach (var navMesh in controller.main.level.navMeshes) {

            navMeshDropdown.AddItem(navMesh.name);

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

    public void OnDropdownChange() {

        ChangeNavMesh(navMeshDropdown.value);

    }

}