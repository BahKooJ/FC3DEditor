
using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class NavMeshEditMode : EditMode {

    public Main main { get; set; }

    public NavMeshEditPanel view;

    public int selectedNavMesh = 0;

    public List<NavNodePoint> navNodes = new();

    public List<GameObject> lines = new();

    public AxisControl selectedNavNode = null;

    public NavNodePoint navNodeToAdd = null;
    public (NavNodePoint, LineRenderer)? pathToAdd = null;

    public NavMeshEditMode(Main main) {
        this.main = main;
    }

    public void Update() {

        if (navNodeToAdd != null) {

            if (Input.GetMouseButtonDown(0)) {

                var node = new NavNode(navNodes.Count,
                    Mathf.RoundToInt(navNodeToAdd.transform.position.x * 32f),
                    Mathf.RoundToInt(navNodeToAdd.transform.position.z * -32f), false);

                main.level.navMeshes[selectedNavMesh].nodes.Add(node);

                navNodeToAdd.node = node;

                navNodeToAdd.controller = this;

                navNodeToAdd.Create();

                navNodes.Add(navNodeToAdd);

                navNodeToAdd = null;

            } else {

                var hitPos = main.CursorOnLevelMesh();

                if (hitPos != null) {

                    navNodeToAdd.transform.position = (Vector3)hitPos;

                }

            }

            return;

        }

        if (pathToAdd != null) {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 8)) {

                foreach (var node in navNodes) {

                    if (hit.colliderInstanceID == node.sphereCollider.GetInstanceID()) {

                        pathToAdd.Value.Item2.SetPosition(1, node.transform.position);

                        if (Input.GetMouseButtonDown(0)) {

                            var index = Array.IndexOf(pathToAdd.Value.Item1.node.nextNode, NavNode.invalid);

                            pathToAdd.Value.Item1.node.nextNode[index] = node.node.index;

                            pathToAdd.Value.Item1.RefreshLines();
                            node.RefreshLines();

                            node.previousPoints.Add(pathToAdd.Value.Item1);

                            Object.Destroy(pathToAdd.Value.Item2.gameObject);

                            pathToAdd = null;

                        }

                        break;

                    }

                }

            } else {

                var hitPos = main.CursorOnLevelMesh();

                if (hitPos != null) {

                    pathToAdd.Value.Item2.SetPosition(1, (Vector3)hitPos);

                }

            }

            return;

        }

        if (selectedNavNode != null) {

            if (Input.GetKey(KeyCode.LeftShift)) {

                var hitPos = main.CursorOnLevelMesh();

                if (hitPos != null) {

                    selectedNavNode.moveCallback((Vector3)hitPos);

                    selectedNavNode.transform.position = selectedNavNode.controlledObject.transform.position;

                }

            } else if (Input.GetKey(KeyCode.Delete)) {
                DeleteNode();
            }

        }

        if (Input.GetMouseButtonDown(0)) {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 8)) {

                foreach (var node in navNodes) {

                    if (hit.colliderInstanceID == node.sphereCollider.GetInstanceID()) {

                        UnselectNode();

                        var axisControl = Object.Instantiate(main.axisControl);
                        var script = axisControl.GetComponent<AxisControl>();

                        script.controlledObject = node.gameObject;

                        script.moveCallback = (newPos) => {

                            node.ChangePosition(newPos);

                            return true;
                        };

                        selectedNavNode = script;

                        view.RefeshCheck();

                        break;

                    }

                }

            }

        }

        if (Input.GetMouseButtonDown(1)) {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 8)) {

                foreach (var node in navNodes) {

                    if (hit.colliderInstanceID == node.sphereCollider.GetInstanceID()) {

                        UnselectNode();

                        if (node.nextNodeLines.Last() != null) {
                            break;
                        }

                        var lineObject = Object.Instantiate(main.line3d);

                        var line = lineObject.GetComponent<LineRenderer>();

                        line.SetPosition(0, node.transform.position);

                        pathToAdd = (node, line);

                        break;

                    }

                }

            }

        }

    }

    void DeleteNode() {

        var indexOfDeletedNode = selectedNavNode.controlledObject.GetComponent<NavNodePoint>().node.index;

        OnDestroy();

        main.level.navMeshes[selectedNavMesh].nodes.RemoveAt(indexOfDeletedNode);

        foreach (var node in main.level.navMeshes[selectedNavMesh].nodes) {

            if (node.index > indexOfDeletedNode) {

                node.index--;

            }

            foreach (var index in Enumerable.Range(0, node.nextNode.Count())) {

                if (node.nextNode[index] == indexOfDeletedNode) {
                    node.nextNode[index] = NavNode.invalid;
                } else if (node.nextNode[index] > indexOfDeletedNode && node.nextNode[index] != NavNode.invalid) {
                    node.nextNode[index]--;
                }

            }

        }

        OnCreateMode();

    }

    void ClearNavMesh() {

        main.level.navMeshes[selectedNavMesh].nodes.Clear();

        OnDestroy();

        OnCreateMode();

    }

    public void UnselectNode() {

        if (selectedNavNode != null) {
            Object.Destroy(selectedNavNode.gameObject);
        }

    }

    public void OnCreateMode() {

        foreach (var node in main.level.navMeshes[selectedNavMesh].nodes) {

            var nodeObject = Object.Instantiate(main.NavMeshPoint);

            var script = nodeObject.GetComponent<NavNodePoint>();

            script.node = node;

            script.controller = this;

            script.Create();

            navNodes.Add(script);

        }

        foreach (var node in navNodes) {
            node.RefreshLines();

            foreach (var index in Enumerable.Range(0, node.nextNodeLines.Count())) {

                if (node.node.nextNode[index] != NavNode.invalid) {
                    var nextNode = navNodes[node.node.nextNode[index]];

                    nextNode.previousPoints.Add(node);
                }

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

        UnselectNode();

    }

    public void LookTile(Tile tile, TileColumn column, LevelMesh section) { }

    public void SelectTile(Tile tile, TileColumn column, LevelMesh section) { }

}