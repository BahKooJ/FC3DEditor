
using FCopParser;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ActorEditMode : EditMode {

    public Main main { get; set; }

    public ActorEditPanelView view;

    List<ActorObject> actors = new();

    public AxisControl selectedActorObject = null;
    public FCopActor selectedActor = null;

    ActorObject actorToAdd = null;

    public ActorEditMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {

        void CreateActor(FCopActor actor, ActorObject script) {

            script.actor = actor;

            script.controller = this;

            actors.Add(script);

        }

        foreach (var actor in main.level.actors) {

            var nodeObject = Object.Instantiate(main.BlankActor);

            CreateActor(actor, nodeObject.GetComponent<ActorObject>());

        }
        
    }

    public void OnDestroy() {

        UnselectActor();

        foreach (var actor in actors) {
            Object.Destroy(actor.gameObject);
        }

        actors.Clear();

        view.CloseActorPorpertiesView();

    }

    public void Update() {

        if (selectedActorObject != null) {

            // Moves object to cursor
            if (Controls.IsDown("ModifierMoveToCursor")) {

                var hitPos = main.CursorOnLevelMesh();

                if (hitPos != null) {

                    selectedActorObject.moveCallback((Vector3)hitPos);

                    selectedActorObject.transform.position = selectedActorObject.controlledObject.transform.position;

                }

            } 

        }

        if (Controls.OnDown("Select")) {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;


            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 8)) {

                foreach (var act in actors) {

                    var didHit = false;


                    if (act.actCollider != null) {

                        if (hit.colliderInstanceID == act.actCollider.GetInstanceID()) {
                            didHit = true;
                        }

                    } else {

                        foreach (var obj in act.objects) {

                            if (hit.colliderInstanceID == obj.meshCollider.GetInstanceID()) {
                                didHit = true;
                            }

                        }

                    }

                    if (didHit) {

                        SelectActor(act);

                        break;

                    }

                }

            }


        }

        if (Input.GetButtonDown("Delete")) {
            deleteActor();
        }

    }

    public void SelectActor(ActorObject actorObject) {

        UnselectActor();

        var axisControl = Object.Instantiate(main.axisControl);
        var script = axisControl.GetComponent<AxisControl>();

        script.controlledObject = actorObject.gameObject;

        script.moveCallback = (newPos) => {

            actorObject.ChangePosition(newPos);

            return true;
        };

        selectedActorObject = script;
        selectedActor = actorObject.actor;

        if (view.activeActorPropertiesView != null) {
            view.RefreshActorPropertiesView();
        } else {
            view.OpenActorPropertiesView();
        }

    }

    public void UnselectActor() {

        if (selectedActorObject != null) {
            Object.Destroy(selectedActorObject.gameObject);
        }

        selectedActor = null;
        selectedActorObject = null;

    }

    void deleteActor() {

        if (selectedActorObject == null) {
            return;
        }

        var actorObject = selectedActorObject.controlledObject.GetComponent<ActorObject>();

        main.level.actors.Remove(actorObject.actor);

        actors.Remove(actorObject);
        actorObject.actor.rawFile.ignore = true;

        Object.Destroy(actorObject.gameObject);

        UnselectActor();

    }

    public void PasteNavNodeCoords() {

        if (selectedActorObject != null) {

            if (NavMeshEditMode.copiedNavNodeCoords != null) {

                selectedActorObject.transform.position = (Vector3)NavMeshEditMode.copiedNavNodeCoords;
                selectedActorObject.moveCallback((Vector3)NavMeshEditMode.copiedNavNodeCoords);

            }

        }

    }

    public void LookTile(Tile tile, TileColumn column, LevelMesh section) { }

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section) { }


}