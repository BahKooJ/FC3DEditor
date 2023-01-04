
using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshEditMode : EditMode {
    public Main main { get; set; }

    public List<NavNodePoint> navNodes = new();

    public List<GameObject> lines = new();

    public AxisControl selectedNavNode = null;

    public NavNodePoint navNodeToAdd = null;

    public NavMeshEditMode(Main main) {
        this.main = main;
    }

    public void Update() {

        if (navNodeToAdd != null) {

            if (Input.GetMouseButtonDown(0)) {

                var node = new NavNode(navNodes.Count, NavNode.invalid, NavNode.invalid, NavNode.invalid,
                    Mathf.RoundToInt(navNodeToAdd.transform.position.x * 32f),
                    Mathf.RoundToInt(navNodeToAdd.transform.position.z * -32f), false);

                navNodeToAdd.node = node;

                navNodeToAdd.Create();

                navNodes.Add(navNodeToAdd);

                navNodeToAdd = null;

            } else {

                var hitPos = main.CursorOnLevelMesh();

                if (hitPos != null) {

                    navNodeToAdd.transform.position = (Vector3)hitPos;

                }

            }

        } else if (selectedNavNode != null) {

            if (Input.GetKey(KeyCode.LeftShift)) {

                var hitPos = main.CursorOnLevelMesh();

                if (hitPos != null) {

                    selectedNavNode.action((Vector3)hitPos);

                    selectedNavNode.transform.position = selectedNavNode.controlledObject.transform.position;

                }

            }

        }

        if (Input.GetMouseButtonDown(0)) {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 8)) {

                foreach (var node in navNodes) {

                    if (hit.colliderInstanceID == node.sphereCollider.GetInstanceID()) {

                        if (selectedNavNode != null) {
                            Object.Destroy(selectedNavNode.gameObject);
                        }

                        var axisControl = Object.Instantiate(main.axisControl);
                        var script = axisControl.GetComponent<AxisControl>();

                        script.controlledObject = node.gameObject;

                        script.action = (newPos) => {

                            node.ChangePosition(newPos);

                            return true;
                        };

                        selectedNavNode = script;

                        break;

                    }

                }

            }

        }

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

            if (node.node.nextNodeA != NavNode.invalid) {

                var nextNode = navNodes[node.node.nextNodeA];

                nextNode.previousPoints.Add(node);

            }
            if (node.node.nextNodeB != NavNode.invalid) {

                var nextNode = navNodes[node.node.nextNodeB];

                nextNode.previousPoints.Add(node);

            }
            if (node.node.nextNodeC != NavNode.invalid) {

                var nextNode = navNodes[node.node.nextNodeC];

                nextNode.previousPoints.Add(node);

            }

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

        if (selectedNavNode != null) {
            Object.Destroy(selectedNavNode.gameObject);
        }

    }

    public void LookTile(Tile tile, TileColumn column, LevelMesh section) {

    }

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section) {

    }

}