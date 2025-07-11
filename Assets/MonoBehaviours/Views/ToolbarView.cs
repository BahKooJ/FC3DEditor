﻿
using System;
using UnityEngine;


public class ToolbarView: MonoBehaviour {

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

        controller.toolbar = this;

        SelectHeightMapEditMode();

    }

    void Update() {

        if (Main.ignoreAllInputs) { return; }

        if (Controls.OnDown("HeightMapEditMode")) {
            SelectHeightMapEditMode();
        }
        if (Controls.OnDown("TileEditMode")) {
            SelectTileEditMode();
        }
        if (Controls.OnDown("TileAddMode")) {
            SelectTileAddMode();
        }
        if (Controls.OnDown("TextureEditMode")) {
            SelectTextureEditMode();
        }
        if (Controls.OnDown("SectionEditMode")) {
            SelectSectionEditMode();
        }
        if (Controls.OnDown("ShaderEditMode")) {
            SelectShaderEditMode();
        }
        if (Controls.OnDown("NavMeshEditMode")) {
            SelectNavMeshEdit();
        }
        if (Controls.OnDown("ActorEditMode")) {
            SelectActorEditMode();
        }

    }

    void SwitchEditMode() {

        var existingAssetManager = FindAnyObjectByType<MiniAssetManagerView>();

        if (existingAssetManager != null) {
            Destroy(existingAssetManager.gameObject);
        }

        var existingDataManager = FindAnyObjectByType<UniversialMiniDataManagerView>();

        if (existingDataManager != null) {
            Destroy(existingDataManager.gameObject);
        }

    }

    public void SelectHeightMapEditMode() {

        if (Main.editMode == null) {

            Main.AddCounterAction(new ChangeEditModeCounterAction(this, typeof(HeightMapEditMode)));

        }
        else {

            Main.AddCounterAction(new ChangeEditModeCounterAction(this, Main.editMode.GetType()));

        }

        Destroy(activePanel);

        SwitchEditMode();

        var editMode = new HeightMapEditMode(controller);

        var obj = Instantiate(heightMapEditPanel);

        var script = obj.GetComponent<HeightMapEditPanelView>();

        script.controller = editMode;

        obj.transform.SetParent(this.transform.parent, false);

        activePanel = obj;

        controller.ChangeEditMode(editMode);

    }

    public void SelectTileEditMode() {

        Main.AddCounterAction(new ChangeEditModeCounterAction(this, Main.editMode.GetType()));

        var editMode = new TileEditMode(controller);
        SwitchEditMode();
        Destroy(activePanel);

        var obj = Instantiate(tileEditPanel);

        var script = obj.GetComponent<TileEditPanel>();

        script.controller = editMode;

        obj.transform.SetParent(this.transform.parent, false);
        activePanel = obj;

        controller.ChangeEditMode(editMode);

    }

    public void SelectTileAddMode() {

        Main.AddCounterAction(new ChangeEditModeCounterAction(this, Main.editMode.GetType()));

        var editMode = new TileAddMode(controller);
        SwitchEditMode();
        Destroy(activePanel);

        var obj = Instantiate(tileAddPanel);

        var script = obj.GetComponent<TileAddPanel>();

        script.controller = editMode;

        obj.transform.SetParent(this.transform.parent, false);
        activePanel = obj;

        controller.ChangeEditMode(editMode);

    }

    public void SelectNavMeshEdit() {

        Main.AddCounterAction(new ChangeEditModeCounterAction(this, Main.editMode.GetType()));

        var editMode = new NavMeshEditMode(controller);
        SwitchEditMode();
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

        Main.AddCounterAction(new ChangeEditModeCounterAction(this, Main.editMode.GetType()));

        var editMode = new ActorEditMode(controller);
        SwitchEditMode();
        Destroy(activePanel);

        var obj = Instantiate(actorEditPanel);

        var script = obj.GetComponent<ActorEditPanelView>();

        script.controller = editMode;

        obj.transform.SetParent(this.transform.parent, false);

        activePanel = obj;

        controller.ChangeEditMode(editMode);

    }

    public void SelectSectionEditMode() {

        Main.AddCounterAction(new ChangeEditModeCounterAction(this, Main.editMode.GetType()));

        var editMode = new SectionEditMode(controller);
        SwitchEditMode();
        Destroy(activePanel);

        var obj = Instantiate(sectionEditPanel);

        var script = obj.GetComponent<SectionEditView>();

        script.controller = editMode;

        obj.transform.SetParent(this.transform.parent, false);

        activePanel = obj;

        controller.ChangeEditMode(editMode);

    }

    public void SelectTextureEditMode() {

        Main.AddCounterAction(new ChangeEditModeCounterAction(this, Main.editMode.GetType()));

        var editMode = new TextureEditMode(controller);
        SwitchEditMode();
        Destroy(activePanel);

        var obj = Instantiate(textureEditPanel);

        var script = obj.GetComponent<TextureEditView>();

        script.controller = editMode;

        obj.transform.SetParent(this.transform.parent, false);

        activePanel = obj;

        controller.ChangeEditMode(editMode);

    }

    public void SelectShaderEditMode() {

        Main.AddCounterAction(new ChangeEditModeCounterAction(this, Main.editMode.GetType()));

        var editMode = new ShaderEditMode(controller);
        SwitchEditMode();
        Destroy(activePanel);

        var obj = Instantiate(shaderEditPanel);

        var script = obj.GetComponent<ShaderEditPanelView>();

        script.controller = editMode;

        obj.transform.SetParent(this.transform.parent, false);

        activePanel = obj;

        controller.ChangeEditMode(editMode);

    }

    public void OpenScriptingPanel() {
        SwitchEditMode();
        var existingScriptingPanel = FindAnyObjectByType<ScriptingPanelView>();

        if (existingScriptingPanel != null) {
            Destroy(existingScriptingPanel.gameObject);
        }

        var obj = Instantiate(controller.scriptingPanelPrefab, controller.canvas.transform, false);

        var scriptingPanel = obj.GetComponent<ScriptingPanelView>();
        scriptingPanel.level = controller.level;

    }

    public void OpenAssetManager() {
        SwitchEditMode();
        var existingAssetManager = FindAnyObjectByType<AssetManagerView>();

        if (existingAssetManager != null) {
            Destroy(existingAssetManager.gameObject);
        }

        var obj = Instantiate(controller.assetManagerPrefab);
        obj.transform.SetParent(controller.canvas.transform, false);

        var assetManager = obj.GetComponent<AssetManagerView>();

        assetManager.level = controller.level;
        assetManager.main = controller;

    }

    public void SelectPlayMode() {

        Main.AddCounterAction(new ChangeEditModeCounterAction(this, Main.editMode.GetType()));

        var editMode = new PlayMode(controller);
        SwitchEditMode();
        Destroy(activePanel);

        controller.ChangeEditMode(editMode);

    }

    public class ChangeEditModeCounterAction : CounterAction {

        public string name { get; set; }

        Type editModeType;
        ToolbarView toolbar;

        public ChangeEditModeCounterAction(ToolbarView toolbar, Type type) {

            editModeType = type;
            this.toolbar = toolbar;

            name = "Edit Mode Change";

        }

        public void Action() {

            // typeof doesn't work with switch statements soooo...
            if (editModeType == typeof(HeightMapEditMode)) {
                toolbar.SelectHeightMapEditMode();
            } else if (editModeType == typeof(TileEditMode)) {
                toolbar.SelectTileEditMode();
            } else if (editModeType == typeof(TileAddMode)) {
                toolbar.SelectTileAddMode();
            } else if (editModeType == typeof(TextureEditMode)) {
                toolbar.SelectTextureEditMode();
            } else if (editModeType == typeof(ShaderEditMode)) {
                toolbar.SelectShaderEditMode();
            } else if (editModeType == typeof(SectionEditMode)) {
                toolbar.SelectSectionEditMode();
            } else if (editModeType == typeof(NavMeshEditMode)) {
                toolbar.SelectNavMeshEdit();
            } else if (editModeType == typeof(ActorEditMode)) {
                toolbar.SelectActorEditMode();
            }

        }

    }

}