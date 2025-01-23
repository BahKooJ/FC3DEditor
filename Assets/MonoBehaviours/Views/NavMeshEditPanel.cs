using System.Linq;
using UnityEngine;

public class NavMeshEditPanel : MonoBehaviour {

    public CustomDropdown navMeshDropdown;

    public NavMeshEditMode controller;

    public NavNodePropertyPanel propertyPanel;


    void Start() {

        RefreshDropdown();

    }

    public void RefreshDropdown() {

        navMeshDropdown.Clear();

        foreach (var navMesh in controller.main.level.navMeshes) {

            navMeshDropdown.AddItem(navMesh.name);

        }

        navMeshDropdown.value = NavMeshEditMode.selectedNavMeshIndex;

    }

    public void AddNode() {

        controller.StartNodeToAdd();

    }

    public void ClearPaths() {

        controller.ClearPaths();

    }

    public void ChangeNavMesh(int index) {

        Main.ClearCounterActions();

        NavMeshEditMode.selectedNavMeshIndex = index;
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

    public void OnClickAddNavMesh() {

        controller.AddNewNavMesh();

        RefreshDropdown();

        navMeshDropdown.Open();

        navMeshDropdown.ScrollToBottom();

        navMeshDropdown.itemObjects.Last().GetComponent<NavMeshListItemView>().StartRenaming(controller.main.level.navMeshes.Last().name);

        // Delayed action because it takes a sec for the new item to be added.
        // Otherwise it would scroll just above the new item.
        Main.delayedActions.Add(new DelayedAction(1, () => navMeshDropdown.ScrollToBottom()));

    }

}