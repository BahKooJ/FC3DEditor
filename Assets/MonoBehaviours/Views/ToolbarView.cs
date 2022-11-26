
using UnityEditor;
using UnityEngine;

class ToolbarView: MonoBehaviour {

    public GameObject geometryEditorPanel;
    public GameObject addGeometryPanel;

    public Main controller;

    public GameObject activePanel;

    void Start() {

        var obj = Instantiate(geometryEditorPanel);

        obj.transform.SetParent(this.transform.parent,false);

        activePanel = obj;

    }

    public void SelectGeometryEditorTool() {

        Destroy(activePanel);

        var obj = Instantiate(geometryEditorPanel);

        obj.transform.SetParent(this.transform.parent, false);

        activePanel = obj;

    }

    public void AddGeometryPanel() {

        Destroy(activePanel);

        var obj = Instantiate(addGeometryPanel);

        obj.transform.SetParent(this.transform.parent, false);

        activePanel = obj;

    }


}