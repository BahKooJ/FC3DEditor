
using FCopParser;
using System.Collections.Generic;
using UnityEngine;

class NavMeshEditMode : EditMode {
    public Main main { get; set; }

    public List<GameObject> navNodes = new();

    public NavMeshEditMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {

        foreach (var node in main.level.navMeshes[0].nodes) {

            var nodeObject = Object.Instantiate(main.NavMeshPoint);

            var script = nodeObject.GetComponent<NavNodePoint>();

            script.node = node;

            navNodes.Add(nodeObject);

        }

    }

    public void OnDestroy() {

    }

    public void LookTile(Tile tile, TileColumn column, LevelMesh section) {

    }

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section) {

    }

}