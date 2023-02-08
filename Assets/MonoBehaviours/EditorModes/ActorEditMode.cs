

using FCopParser;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ActorEditMode : EditMode {

    public Main main { get; set; }

    List<ActorObject> actors = new();

    ActorObject selectedActor = null;
    ActorObject actorToAdd = null;

    public ActorEditMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {

        foreach (var actor in main.level.actors) {

            var nodeObject = Object.Instantiate(main.ActorObject);

            var script = nodeObject.GetComponent<ActorObject>();

            script.actor = actor;

            script.controller = this;

            actors.Add(script);

        }
        
    }

    public void OnDestroy() {
        
    }

    public void Update() {

    }

    public void LookTile(Tile tile, TileColumn column, LevelMesh section) { }

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section) { }


}