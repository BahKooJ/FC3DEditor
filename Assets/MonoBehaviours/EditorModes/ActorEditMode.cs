
using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class ActorEditMode : EditMode {

    public Main main { get; set; }

    public ActorEditPanelView view;

    List<ActorObject> actorObjects = new();
    public Dictionary<int, ActorObject> actorObjectsByID = new();
    List<ActorGroupObject> actorGroupObjects = new();

    public AxisControl selectedActorObject = null;
    public FCopActor selectedActor = null;
    public FCopActor actorToGroup = null;

    ActorObject actorToAdd = null;

    public ActorEditMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {

        ActorObject CreateActor(FCopActor actor, ActorGroupObject group = null) {

            var nodeObject = Object.Instantiate(main.BlankActor);

            var script = nodeObject.GetComponent<ActorObject>();

            script.actor = actor;
            script.controller = this;
            script.group = group;

            script.Create();

            return script;

        }

        void CreateGroup(List<FCopActor> actors) {

            var groupObj = Object.Instantiate(main.actorGroupObjectFab);

            var script = groupObj.GetComponent<ActorGroupObject>();
            script.controller = this;

            foreach (var actor in actors) {

                var createdActObj = CreateActor(actor, script);
                
                createdActObj.transform.SetParent(script.transform, false);

                script.actObjects.Add(createdActObj);

                actorObjects.Add(createdActObj);
                actorObjectsByID[actor.id] = createdActObj;

            }

            script.Init();

            actorGroupObjects.Add(script);

        }

        foreach (var node in main.level.sceneActors.positionalGroupedActors) {

            if (node.nestedActors.Count == 1) {

                var createdActObj = CreateActor(node.nestedActors[0]);

                actorObjects.Add(createdActObj);
                actorObjectsByID[createdActObj.actor.id] = createdActObj;

            }
            else {

                CreateGroup(node.nestedActors);

            }

        }
        
    }

    public void OnDestroy() {

        UnselectActor();

        foreach (var actor in actorObjects) {
            Object.Destroy(actor.gameObject);
        }

        actorObjects.Clear();
        actorObjectsByID.Clear();

        foreach (var group in actorGroupObjects) {
            Object.Destroy(group.gameObject);
        }

        actorGroupObjects.Clear();

        view.CloseActorPorpertiesView();

    }

    public void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {

            HeadsUpTextUtil.End();
            actorToGroup = null;

        }

        if (selectedActorObject != null) {

            // Moves object to cursor
            if (Controls.IsDown("MoveToCursor")) {

                var hitPos = main.CursorOnLevelMesh();

                if (hitPos != null) {

                    selectedActorObject.moveCallback((Vector3)hitPos);

                    selectedActorObject.transform.position = selectedActorObject.controlledObject.transform.position;

                }

            } 

        }

        if ((Controls.OnDown("Select") || Input.GetMouseButtonDown(1)) && !Main.IsMouseOverUI()) {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 8)) {

                foreach (var act in actorObjects) {

                    var didHit = false;


                    if (act.actCollider != null) {

                        if (hit.colliderInstanceID == act.actCollider.GetInstanceID()) {
                            didHit = true;
                        }

                    }
                    else {

                        foreach (var obj in act.objects) {

                            if (hit.colliderInstanceID == obj.meshCollider.GetInstanceID()) {
                                didHit = true;
                            }

                        }

                    }

                    if (didHit) {

                        if (actorToGroup != null) {
                            GroupActor(main.level.sceneActors.ActorNodeByIDPositional(act.actor.DataID));
                            actorToGroup = null;
                        }
                        else if (Input.GetMouseButtonDown(1)) {
                            ContextMenuUtil.CreateContextMenu(act.contextMenuItems);
                        }
                        else {
                            SelectActor(act);
                            view.activeActorPropertiesView.sceneActorsView.RefreshSelection(true);
                        }

                        break;

                    }

                }

            }


        }

        if (Input.GetButtonDown("Delete")) {
            DeleteActor();
        }

    }

    void ValidateGrouping() {

        UnselectActor();

        foreach (var actObj in actorObjects) {
            actObj.transform.parent = null;
            actObj.group = null;
        }

        foreach (var group in actorGroupObjects) {
            Object.Destroy(group.gameObject);
        }

        actorGroupObjects.Clear();

        foreach (var node in main.level.sceneActors.positionalGroupedActors) {

            if (node.nestedActors.Count != 1) {

                var groupObj = Object.Instantiate(main.actorGroupObjectFab);

                var script = groupObj.GetComponent<ActorGroupObject>();
                script.controller = this;

                foreach (var nestNode in node.nestedActors) {

                    var actObj = actorObjectsByID[nestNode.DataID];

                    actObj.transform.SetParent(script.transform, false);
                    actObj.group = script;

                    script.actObjects.Add(actObj);

                }

                script.Init();

                actorGroupObjects.Add(script);

            }

        }

    }

    public void SelectActor(ActorObject actorObject) {

        UnselectActor();

        var axisControl = Object.Instantiate(main.axisControl);
        var script = axisControl.GetComponent<AxisControl>();

        if (actorObject.group != null) {

            script.controlledObject = actorObject.group.gameObject;

            script.moveCallback = (newPos) => {

                actorObject.group.ChangePosition(newPos);

                return true;
            };

        }
        else {

            script.controlledObject = actorObject.gameObject;

            script.moveCallback = (newPos) => {

                actorObject.ChangePosition(newPos);

                return true;
            };

        }

        selectedActorObject = script;
        selectedActor = actorObject.actor;
        
        view.RefreshActorPropertiesView();

    }

    public void SelectActorByID(int id) {

        var actorObj = actorObjects.First(obj => obj.actor.DataID == id);

        SelectActor(actorObj);
        view.activeActorPropertiesView.sceneActorsView.RefreshSelection(false);

    }

    public void MoveToActor(int id) {

        ActorObject actorObj;
        
        actorObj = actorObjects.First(obj => obj.actor.DataID == id);

        Camera.main.transform.position = actorObj.transform.position;

        Camera.main.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * -4);

    }

    public void UnselectActor() {

        if (selectedActorObject != null) {
            Object.Destroy(selectedActorObject.gameObject);
        }

        selectedActor = null;
        selectedActorObject = null;

    }

    public void RenameActor(FCopActor actor, string newName) {

        actor.name = newName;

        if (selectedActor != null) {

            if (selectedActor.DataID == actor.DataID) {

                if (view.activeActorPropertiesView != null) {
                    view.activeActorPropertiesView.RefreshName();
                }

            }

        }

        var nodeView = view.activeActorPropertiesView.sceneActorsView.GetNodeItemByID(actor.DataID);

        if (nodeView != null) {
            nodeView.RefreshName();
        }

    }

    public void UngroupActor(FCopActor actor) {

        var didUngroup = main.level.sceneActors.PositionalUngroupActor(actor);

        if (didUngroup) {

            ValidateGrouping();

            var actorObj = actorObjectsByID[actor.DataID];

            actorObj.SetToCurrentPosition();

            view.activeActorPropertiesView.sceneActorsView.Refresh(true);

        }

    }

    public void StartGroup(FCopActor actor) {
        actorToGroup = actor;
        HeadsUpTextUtil.HeadsUp("Select Actor to Group...");
    }

    public void GroupActor(ActorNode toGroup) {

        HeadsUpTextUtil.End();

        if (actorToGroup == null) return;

        var didGroup = main.level.sceneActors.PositionalGroupActor(actorToGroup, toGroup);

        if (didGroup) {

            ValidateGrouping();

            var actorObj = actorObjectsByID[actorToGroup.DataID];

            actorObj.SetToCurrentPosition();

            view.activeActorPropertiesView.sceneActorsView.Refresh(true);

        }
        else {
            QuickLogHandler.Log("Unable to group actors", LogSeverity.Error);
        }

        actorToGroup = null;

    }

    public void DeleteActor() {

        if (selectedActorObject == null) {
            return;
        }

        var actorObject = selectedActorObject.controlledObject.GetComponent<ActorObject>();

        main.level.sceneActors.DeleteActor(actorObject.actor);

        actorObjects.Remove(actorObject);
        actorObjectsByID.Remove(actorObject.actor.DataID);

        Object.Destroy(actorObject.gameObject);

        UnselectActor();

        ValidateGrouping();

        view.activeActorPropertiesView.Refresh();
        view.activeActorPropertiesView.sceneActorsView.Refresh(true);

    }

    public void DeleteByID(int id) {

        main.level.sceneActors.DeleteActor(id);

        var actObj = actorObjectsByID[id];

        actorObjects.Remove(actObj);
        actorObjectsByID.Remove(id);

        Object.Destroy(actObj.gameObject);

        UnselectActor();

        ValidateGrouping();

        view.activeActorPropertiesView.Refresh();
        view.activeActorPropertiesView.sceneActorsView.Refresh(true);

    }

    public void PasteNavNodeCoords() {

        if (selectedActorObject != null) {

            if (NavMeshEditMode.copiedNavNodeCoords != null) {

                selectedActorObject.transform.position = (Vector3)NavMeshEditMode.copiedNavNodeCoords;
                selectedActorObject.moveCallback((Vector3)NavMeshEditMode.copiedNavNodeCoords);

            }

        }

    }


}