

using UnityEngine;

class NavMeshEditPanel : MonoBehaviour {

    public NavMeshEditMode controller;

    public void AddNode() {

        var obj = Instantiate(controller.main.NavMeshPoint);
        var script = obj.GetComponent<NavNodePoint>();
        controller.navNodeToAdd = script;

    }

    public void ClearPaths() {

    }

}