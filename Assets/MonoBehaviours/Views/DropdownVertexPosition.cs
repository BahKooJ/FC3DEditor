using FCopParser;
using TMPro;
using UnityEngine;

public class DropdownVertexPosition : MonoBehaviour {

    public int index;

    public Main controller;

    public TMP_Dropdown dropdown;

    void Start() {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    public void OnChange() {

        if (controller.selectedTile.verticies.Count - 1 < index) {
            return;
        }

        var vertex = controller.selectedTile.verticies[index];

        vertex.vertexPosition = (VertexPosition)dropdown.value + 1;

        controller.selectedTile.verticies[index] = vertex;

        controller.selectedSection.RefreshMesh();

    }

}