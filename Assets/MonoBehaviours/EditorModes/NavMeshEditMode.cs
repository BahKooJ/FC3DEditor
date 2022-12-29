
using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshEditMode : EditMode {
    public Main main { get; set; }

    public List<NavNodePoint> navNodes = new();

    public List<GameObject> lines = new();

    public NavMeshEditMode(Main main) {
        this.main = main;
    }

    public void Update() {

    }

    public void OnCreateMode() {

        foreach (var node in main.level.navMeshes[0].nodes) {

            var nodeObject = Object.Instantiate(main.NavMeshPoint);

            var script = nodeObject.GetComponent<NavNodePoint>();

            script.node = node;

            script.controller = this;

            if (node.nextNodeA != NavNode.invalid) {

                var lineObject = Object.Instantiate(main.line3d);

                var line = lineObject.GetComponent<LineRenderer>();

                script.nextNodeLineA = line;

                lines.Add(lineObject);

            }
            if (node.nextNodeB != NavNode.invalid) {

                var lineObject = Object.Instantiate(main.line3d);

                var line = lineObject.GetComponent<LineRenderer>();

                script.nextNodeLineB = line;

                lines.Add(lineObject);

            }
            if (node.nextNodeC != NavNode.invalid) {

                var lineObject = Object.Instantiate(main.line3d);

                var line = lineObject.GetComponent<LineRenderer>();

                script.nextNodeLineC = line;

                lines.Add(lineObject);

            }

            script.Create();

            navNodes.Add(script);

        }

        foreach (var node in navNodes) {
            node.RefreshLines();
        }

    }

    public void OnDestroy() {

        foreach (var obj in navNodes) {
            Object.Destroy(obj.gameObject);
        }

        navNodes.Clear();

        foreach (var line in lines) {
            Object.Destroy(line);
        }

        lines.Clear();

    }

    public void LookTile(Tile tile, TileColumn column, LevelMesh section) {

    }

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section) {

    }

}