
using UnityEditor;
using UnityEngine;

class ToolbarView: MonoBehaviour {

    public GameObject geometryEditorPanel;
    public GameObject addGeometryPanel;

    public Main controller;

    public GameObject activePanel;

    void Start() {

        AddGeometryPanel();

    }

    public void SelectGeometryEditorTool() {

        Destroy(activePanel);

        var obj = Instantiate(geometryEditorPanel);

        var script = obj.GetComponent<GeometryEditorUI>();

        script.controller = controller;

        obj.transform.SetParent(this.transform.parent, false);

        activePanel = obj;

        controller.ChangeEditMode(new GeometryEditMode(controller));

    }

    public void AddGeometryPanel() {

        Destroy(activePanel);

        var obj = Instantiate(addGeometryPanel);

        var script = obj.GetComponent<AddGeometryPanel>();

        script.controller = controller;

        obj.transform.SetParent(this.transform.parent, false);

        activePanel = obj;

        controller.ChangeEditMode(new GeometryAddMode(controller));

    }


}