

using FCopParser;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Object = UnityEngine.Object;

public class ActorEditMode : EditMode {

    public Main main { get; set; }

    List<ActorObject> actors = new();

    AxisControl selectedActor = null;
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


            switch (actor) {

                case FCopBaseTurretActor:

                    var baseTurret = Object.Instantiate(main.BaseTurretActorObject);

                    CreateActor(actor, baseTurret.GetComponent<BaseTurretActorObject>());

                    break;
                case FCopTurretActor:

                    var turret = Object.Instantiate(main.TurretActorObject);

                    CreateActor(actor, turret.GetComponent<TurretActorObject>());

                    break;
                default:

                    var nodeObject = Object.Instantiate(main.BlankActor);

                    CreateActor(actor, nodeObject.GetComponent<ActorObject>());

                    break;
            }

        }
        
    }

    public void OnDestroy() {

        UnselectActor();

        foreach (var actor in actors) {
            Object.Destroy(actor.gameObject);
        }

        actors.Clear();

    }

    public void Update() {

        if (selectedActor != null) {

            if (Input.GetKey(KeyCode.LeftShift)) {

                var hitPos = main.CursorOnLevelMesh();

                if (hitPos != null) {

                    selectedActor.action((Vector3)hitPos);

                    selectedActor.transform.position = selectedActor.controlledObject.transform.position;

                }

            } 
            //else if (Input.GetKey(KeyCode.Delete)) {
            //    DeleteNode();
            //}

        }

        if (Input.GetMouseButtonDown(0)) {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;


            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 8)) {

                foreach (var act in actors) {

                    if (hit.colliderInstanceID == act.actCollider.GetInstanceID()) {

                        Debug.Log(act.actor.id + " : " + act.actor.objectType);

                        UnselectActor();

                        var axisControl = Object.Instantiate(main.axisControl);
                        var script = axisControl.GetComponent<AxisControl>();

                        script.controlledObject = act.gameObject;

                        script.action = (newPos) => {

                            act.ChangePosition(newPos);

                            return true;
                        };

                        selectedActor = script;

                        break;

                    }

                }

            }


        }


    }

    public void UnselectActor() {

        if (selectedActor != null) {
            Object.Destroy(selectedActor.gameObject);
        }

    }


    public void LookTile(Tile tile, TileColumn column, LevelMesh section) { }

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section) { }


}