using FCopParser;
using UnityEngine;



class TestMeshButton : MonoBehaviour {

    public Main controller;

    public void OnClick() {

        if (controller.selectedTile != null) {
            var meshID = MeshType.IDFromVerticies(controller.selectedTile.verticies);

            if (meshID != null) {
                Debug.Log(meshID);
            } else {
                Debug.Log("Mesh has no ID");
            }

        }

    }

}