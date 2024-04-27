
using UnityEngine;


class ToolbarView: MonoBehaviour {

    public GameObject heightMapEditPanel;
    public GameObject tileEditPanel;
    public GameObject tileAddPanel;
    public GameObject sectionEditPanel;
    public GameObject navMeshEditPanel;
    public GameObject actorEditPanel;
    public GameObject textureEditPanel;
    public GameObject shaderEditPanel;

    public Main controller;

    public GameObject activePanel;

    void Start() {

        SelectHeightMapEditMode();

    }

    void Update() {

        if (Main.ignoreAllInputs) { return; }

        if (Controls.OnDown("GeometryEditingMode")) {
            SelectHeightMapEditMode();
        }
        if (Controls.OnDown("TileBuildingMode")) {
            SelectTileEditMode();
        }
        if (Controls.OnDown("SectionEditingMode")) {
            SelectSectionEditMode();
        }
        if (Controls.OnDown("TextureEditingMode")) {
            SelectTextureEditMode();
        }
        if (Controls.OnDown("NavMeshEditingMode")) {
            SelectNavMeshEdit();
        }
        if (Controls.OnDown("ActorEditingMode")) {
            SelectActorEditMode();
        }

    }

    public void SelectHeightMapEditMode() {

        Destroy(activePanel);

        var editMode = new HeightMapEditMode(controller);

        var obj = Instantiate(heightMapEditPanel);

        var script = obj.GetComponent<HeightMapEditPanelView>();

        script.controller = editMode;

        obj.transform.SetParent(this.transform.parent, false);

        activePanel = obj;

        controller.ChangeEditMode(editMode);

    }

    public void SelectTileEditMode() {

        var editMode = new TileEditMode(controller);

        Destroy(activePanel);

        var obj = Instantiate(tileEditPanel);

        var script = obj.GetComponent<TileEditPanel>();

        script.controller = editMode;

        obj.transform.SetParent(this.transform.parent, false);
        activePanel = obj;

        controller.ChangeEditMode(editMode);

    }

    public void SelectTileAddMode() {

        var editMode = new TileAddMode(controller);

        Destroy(activePanel);

        var obj = Instantiate(tileAddPanel);

        var script = obj.GetComponent<TileAddPanel>();

        script.controller = editMode;

        obj.transform.SetParent(this.transform.parent, false);
        activePanel = obj;

        controller.ChangeEditMode(editMode);

    }

    public void SelectNavMeshEdit() {

        var editMode = new NavMeshEditMode(controller);

        Destroy(activePanel);

        var obj = Instantiate(navMeshEditPanel);

        var script = obj.GetComponent<NavMeshEditPanel>();

        script.controller = editMode;

        obj.transform.SetParent(this.transform.parent, false);

        activePanel = obj;

        editMode.view = script;

        controller.ChangeEditMode(editMode);

    }

    public void SelectActorEditMode() {

        var editMode = new ActorEditMode(controller);

        Destroy(activePanel);

        var obj = Instantiate(actorEditPanel);

        var script = obj.GetComponent<ActorEditPanelView>();

        script.controller = editMode;

        obj.transform.SetParent(this.transform.parent, false);

        activePanel = obj;

        controller.ChangeEditMode(editMode);

    }

    public void SelectSectionEditMode() {

        var editMode = new SectionEditMode(controller);

        Destroy(activePanel);

        var obj = Instantiate(sectionEditPanel);

        var script = obj.GetComponent<SectionEditView>();

        script.controller = editMode;

        obj.transform.SetParent(this.transform.parent, false);

        activePanel = obj;

        controller.ChangeEditMode(editMode);

    }

    public void SelectTextureEditMode() {

        var editMode = new TextureEditMode(controller);

        Destroy(activePanel);

        var obj = Instantiate(textureEditPanel);

        var script = obj.GetComponent<TextureEditView>();

        script.controller = editMode;

        obj.transform.SetParent(this.transform.parent, false);

        activePanel = obj;

        controller.ChangeEditMode(editMode);

    }

    public void SelectShaderEditMode() {

        var editMode = new ShaderEditMode(controller);

        Destroy(activePanel);

        var obj = Instantiate(shaderEditPanel);

        var script = obj.GetComponent<ShaderEditPanelView>();

        script.controller = editMode;

        obj.transform.SetParent(this.transform.parent, false);

        activePanel = obj;

        controller.ChangeEditMode(editMode);

    }

}