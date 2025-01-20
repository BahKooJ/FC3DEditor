using UnityEngine;

public class NavMeshEditPanel : MonoBehaviour {

    public CustomDropdown navMeshDropdown;

    public NavMeshEditMode controller;

    public NavNodeDebugPanel debugPanel;


    void Start() {

        foreach (var navMesh in controller.main.level.navMeshes) {

            navMeshDropdown.AddItem(navMesh.name);

        }

    }

    public void AddNode() {

        controller.StartNodeToAdd();

    }

    public void ClearPaths() {

        controller.ClearPaths();

    }

    public void ChangeNavMesh(int index) {

        Main.ClearCounterActions();

        controller.selectedNavMeshIndex = index;
        controller.OnDestroy();
        controller.OnCreateMode();

    }

    public void CopyNavNodeCoords() {

        controller.CopyNavNodeCoords();

    }

    public void PasteNavNodeCoords() {
        controller.PasteNavNodeCoords();
    }

    public void OnDropdownChange() {

        ChangeNavMesh(navMeshDropdown.value);

    }

}