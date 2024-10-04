
using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class NavMeshEditMode : EditMode {

    public static Vector3? copiedNavNodeCoords = null;

    public Main main { get; set; }

    public NavMeshEditPanel view;

    public int selectedNavMeshIndex = 0;
    public FCopNavMesh SelectedNavMesh {
        get => main.level.navMeshes [selectedNavMeshIndex];
    }

    public List<NavNodePoint> navNodes = new();

    public AxisControl selectedNavNode = null;

    public NavNodePoint navNodeToAdd = null;
    public (NavNodePoint, LineRenderer)? pathToAdd = null;

    public NavMeshEditMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {

        foreach (var node in SelectedNavMesh.nodes) {

            var nodeObject = Object.Instantiate(main.NavMeshPoint);

            var script = nodeObject.GetComponent<NavNodePoint>();

            script.node = node;

            script.controller = this;

            script.Create();

            navNodes.Add(script);

        }

        // Gives nodes their previous node.
        foreach (var nodeObj in navNodes) {

            foreach (var nextNodeI in nodeObj.node.nextNodeIndexes) {

                if (nextNodeI != NavNode.invalid) {

                    var nextNodeObj = navNodes[nextNodeI];

                    nextNodeObj.previousPoints.Add(nodeObj);

                }

            }

            nodeObj.RefreshLines();

        }

    }

    public void OnDestroy() {

        foreach (var obj in navNodes) {

            foreach (var line in obj.nextNodeLines) {

                if (line != null) {
                    Object.Destroy(line.gameObject);
                }

            }

            Object.Destroy(obj.gameObject);

        }

        NodeNotAdded();

        if (pathToAdd != null) {

            Object.Destroy(pathToAdd.Value.Item2.gameObject);

            pathToAdd = null;

        }

        navNodes.Clear();

        UnselectNode();

    }

    public void Update() {

        if (Main.ignoreAllInputs) {
            return;
        }

        if (Controls.OnDown("AddNavNode")) {
            StartNodeToAdd();
        }

        if (Controls.OnDown("ClearNavPaths")) {
            ClearPaths();
        }

        if (Controls.OnDown("Unselect")) {
            UnselectNode();
        }

        if (pathToAdd != null) {

            if (Controls.OnDown("Interact")) {

                Object.Destroy(pathToAdd.Value.Item2.gameObject);

                pathToAdd = null;

                NodeNotAdded();

                return;
            }

            var castResult = TestRayOnNavNode();

            if (Controls.OnDown("Select")) {

                if (castResult != null) {
                    
                    CreateNewPath(castResult);
                    NodeNotAdded();

                }
                else {

                    AddNode();

                    var newlyAddedNode = navNodes.Last();
                    CreateNewPath(newlyAddedNode);

                }

            }
            else {

                if (castResult != null) {

                    pathToAdd.Value.Item2.SetPosition(1, castResult.transform.position);

                    navNodeToAdd.gameObject.SetActive(false);

                }
                else {

                    navNodeToAdd.gameObject.SetActive(true);

                    var hitPos = main.CursorOnLevelMesh();

                    if (hitPos != null) {

                        pathToAdd.Value.Item2.SetPosition(1, (Vector3)hitPos);

                        navNodeToAdd.transform.position = (Vector3)hitPos;

                    }

                }

            }

            return;

        }

        if (navNodeToAdd != null) {

            if (Controls.OnDown("Select")) {

                AddNode();

            }
            else {

                var hitPos = main.CursorOnLevelMesh();

                if (hitPos != null) {

                    navNodeToAdd.transform.position = (Vector3)hitPos;

                }

            }

            return;

        }

        if (selectedNavNode != null) {

            if (Controls.IsDown("MoveToCursor")) {

                var hitPos = main.CursorOnLevelMesh();

                if (hitPos != null) {

                    selectedNavNode.moveCallback((Vector3)hitPos);

                    selectedNavNode.transform.position = selectedNavNode.controlledObject.transform.position;

                }

            } 
            else if (Controls.OnDown("Delete")) {
                DeleteNode();
            }

        }

        if (Controls.OnDown("Select")) {

            var castResult = TestRayOnNavNode();

            if (castResult != null) {

                SelectNavNode(castResult);

            }

        }

        if (Controls.OnDown("Interact")) {

            var castResult = TestRayOnNavNode();

            if (castResult != null) {

                if (!castResult.AreAllPathsUsed()) {

                    UnselectNode();

                    var lineObject = Object.Instantiate(main.line3d);

                    var line = lineObject.GetComponent<LineRenderer>();

                    line.SetPosition(0, castResult.transform.position);

                    pathToAdd = (castResult, line);
                    StartNodeToAdd();

                }

            }

        }

    }

    void AddNode() {

        var node = new NavNode(navNodes.Count, -64, 0,
            Mathf.RoundToInt(navNodeToAdd.transform.position.x * 32f),
            Mathf.RoundToInt(navNodeToAdd.transform.position.z * -32f), false);

        SelectedNavMesh.nodes.Add(node);

        navNodeToAdd.node = node;

        navNodeToAdd.controller = this;

        navNodeToAdd.Create();

        navNodes.Add(navNodeToAdd);

        navNodeToAdd = null;

    }

    void NodeNotAdded() {

        if (navNodeToAdd != null) {
            Object.Destroy(navNodeToAdd.gameObject);
        }

        navNodeToAdd = null;

    }

    NavNodePoint TestRayOnNavNode() {

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 8)) {

            foreach (var node in navNodes) {

                if (hit.colliderInstanceID == node.sphereCollider.GetInstanceID()) {

                    return node;

                }

            }

        }

        return null;

    }

    void SelectNavNode(NavNodePoint node) {

        UnselectNode();

        var axisControl = Object.Instantiate(main.axisControl);
        var script = axisControl.GetComponent<AxisControl>();

        script.controlledObject = node.gameObject;
        script.moveCallback = (newPos) => {

            node.ChangePosition(newPos);

            return true;
        };

        selectedNavNode = script;

        // TODO: Why not just refresh view?
        view.RefeshCheck();

    }

    void DeleteNode() {

        var indexOfDeletedNode = selectedNavNode.controlledObject.GetComponent<NavNodePoint>().node.index;

        OnDestroy();

        SelectedNavMesh.nodes.RemoveAt(indexOfDeletedNode);

        foreach (var node in SelectedNavMesh.nodes) {

            if (node.index > indexOfDeletedNode) {

                node.index--;

            }

            foreach (var index in Enumerable.Range(0, node.nextNodeIndexes.Count())) {

                if (node.nextNodeIndexes[index] == indexOfDeletedNode) {
                    node.nextNodeIndexes[index] = NavNode.invalid;
                } else if (node.nextNodeIndexes[index] > indexOfDeletedNode && node.nextNodeIndexes[index] != NavNode.invalid) {
                    node.nextNodeIndexes[index]--;
                }

            }

        }

        OnCreateMode();

    }

    void CreateNewPath(NavNodePoint navNode) {

        var startingNodeObj = pathToAdd.Value.Item1;

        if (startingNodeObj.AlreadyContainsPath(navNode.node.index)) {
            return;
        }

        var index = Array.IndexOf(startingNodeObj.node.nextNodeIndexes, NavNode.invalid);

        startingNodeObj.node.nextNodeIndexes[index] = navNode.node.index;

        startingNodeObj.RefreshLines();
        navNode.RefreshLines();

        navNode.previousPoints.Add(startingNodeObj);

        Object.Destroy(pathToAdd.Value.Item2.gameObject);

        pathToAdd = null;

    }

    public void ClearPaths() {

        if (selectedNavNode == null) {
            return;
        }

        var script = selectedNavNode.controlledObject.GetComponent<NavNodePoint>();
        script.ClearPaths();

    }

    public void RenameNavMesh(string newName, int index) {

        main.level.navMeshes[index].name = newName;

    }

    public void StartNodeToAdd() {

        if (navNodeToAdd != null) {
            return;
        }

        UnselectNode();

        var obj = Object.Instantiate(main.NavMeshPoint);
        var script = obj.GetComponent<NavNodePoint>();
        navNodeToAdd = script;

    }

    public void CopyNavNodeCoords() {

        if (selectedNavNode != null) {

            copiedNavNodeCoords = selectedNavNode.controlledObject.transform.position;

        }

    }

    public void ClearNavMesh(int index) {

        DialogWindowUtil.Dialog("Clear Nav Mesh", "Are you sure you would like to clear this nav mesh? This cannot be undone.", () => {

            main.level.navMeshes[index].nodes.Clear();

            OnDestroy();

            OnCreateMode();

            return true;

        });

    }

    public void UnselectNode() {

        if (selectedNavNode != null) {

            selectedNavNode.ClearOutlineOnObject();

            Object.Destroy(selectedNavNode.gameObject);

        }

        selectedNavNode = null;

    }

}