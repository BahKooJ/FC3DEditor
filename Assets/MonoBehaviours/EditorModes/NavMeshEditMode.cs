
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

    public static int selectedNavMeshIndex = 0;
    public FCopNavMesh SelectedNavMesh {
        get => main.level.navMeshes[selectedNavMeshIndex];
    }

    public List<NavNodePoint> navNodes = new();

    public AxisControl selectedNavNode = null;

    public NavNodePoint navNodeToAdd = null;
    public (NavNodePoint, LineRenderer)? pathToAdd = null;

    bool preventCounterAction = false;

    public NavMeshEditMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {

        if (selectedNavMeshIndex >= main.level.navMeshes.Count) {
            selectedNavMeshIndex = 0;
        }

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

    public void Refresh() {

        OnDestroy();
        OnCreateMode();

    }

    public void Update() {

        if (Main.ignoreAllInputs) {
            return;
        }

        if (Controls.OnUp("MoveToCursor") || Input.GetMouseButtonUp(0)) {
            preventCounterAction = false;
        }

        if (selectedNavNode != null && !Main.IsMouseOverUI()) {

            if (selectedNavNode.TestCollision()) {
                return;
            }

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

                    selectedNavNode.moveCallback((Vector3)hitPos, AxisControl.Axis.None);

                    selectedNavNode.transform.position = selectedNavNode.controlledObject.transform.position;

                }

            } 
            else if (Controls.OnDown("Delete")) {
                DeleteNode();
            }

        }

        if (Controls.OnDown("Select") && !Main.IsMouseOverUI()) {

            var castResult = TestRayOnNavNode();

            if (castResult != null) {

                SelectNavNode(castResult);

            }

        }

        if (Controls.OnDown("Interact") && !Main.IsMouseOverUI()) {

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

                if (hit.colliderInstanceID == node.heightOffsetSphere.GetComponent<SphereCollider>().GetInstanceID()) {
                    return node;
                }

            }

        }

        return null;

    }

    public void SelectNavNode(NavNodePoint node) {

        UnselectNode();

        var axisControl = Object.Instantiate(main.axisControl);
        var script = axisControl.GetComponent<AxisControl>();

        script.controlledObject = node.gameObject;
        script.moveCallback = (newPos, axis) => {

            if (Controls.IsDown("SnapActorPosition")) {

                var lockPos = new Vector3(MathF.Round(newPos.x * 2) / 2, MathF.Round(newPos.y * 2) / 2, MathF.Round(newPos.z * 2) / 2);
                node.ChangePosition(lockPos, axis);

            }
            else {

                node.ChangePosition(newPos, axis);

            }


            return true;
        };

        selectedNavNode = script;

        view.propertyPanel.Refresh();

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

    public void PasteNavNodeCoords() {

        if (selectedNavNode == null) {

            QuickLogHandler.Log("No Nav Node selected.", LogSeverity.Info);

        }
        else if (copiedNavNodeCoords == null) {

            QuickLogHandler.Log("No Nav Node coordinates Copied.", LogSeverity.Info);

        }
        else {

            selectedNavNode.moveCallback((Vector3)copiedNavNodeCoords, AxisControl.Axis.None);

            selectedNavNode.transform.position = selectedNavNode.controlledObject.transform.position;

            preventCounterAction = false;

            QuickLogHandler.Log("Nav Node coordinates pasted.", LogSeverity.Success);

        }

    }

    public void CopyNavNodeCoords() {

        if (selectedNavNode != null) {

            QuickLogHandler.Log("Nav Node coordinates copied.", LogSeverity.Success);

            copiedNavNodeCoords = selectedNavNode.controlledObject.transform.position;

        }
        else {

            QuickLogHandler.Log("No Nav Node selected", LogSeverity.Info);


        }

    }

    public void ClearNavMesh(int index) {

        DialogWindowUtil.Dialog("Clear Nav Mesh", "Are you sure you would like to clear this nav mesh? This cannot be undone.", () => {

            main.level.navMeshes[index].nodes.Clear();

            OnDestroy();

            OnCreateMode();

            Main.ClearCounterActions();

            return true;

        });

    }

    public void UnselectNode() {

        if (selectedNavNode != null) {

            selectedNavNode.ClearOutlineOnObject();

            Object.Destroy(selectedNavNode.gameObject);

        }

        selectedNavNode = null;

        view.propertyPanel.Refresh();

    }

    public NavNode GetSeletedNavNode() {

        if (selectedNavNode == null) {
            return null;
        }

        return selectedNavNode.controlledObject.GetComponent<NavNodePoint>().node;

    }

    public void RefreshNavNode() {

        if (selectedNavNode != null) {
            selectedNavNode.controlledObject.GetComponent<NavNodePoint>().SetToCurrentPosition();
            selectedNavNode.controlledObject.GetComponent<NavNodePoint>().RefreshLines();
            selectedNavNode.RefreshPosition();
        }

    }

    #region Model Mutating

    void CreateNewPath(NavNodePoint navNode) {

        AddNavMeshSaveState(SelectedNavMesh);

        var startingNodeObj = pathToAdd.Value.Item1;

        if (startingNodeObj.AlreadyContainsPath(navNode.node.index)) {
            Object.Destroy(pathToAdd.Value.Item2.gameObject);
            pathToAdd = null;
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

        AddNavMeshSaveState(SelectedNavMesh);

        if (selectedNavNode == null) {
            return;
        }

        var script = selectedNavNode.controlledObject.GetComponent<NavNodePoint>();

        foreach (var index in Enumerable.Range(0, script.node.nextNodeIndexes.Count())) {

            script.node.nextNodeIndexes[index] = NavNode.invalid;

        }

        script.ClearPaths();
        view.propertyPanel.Refresh();

    }

    public void ChangePosition(NavNode node, Vector3 pos) {

        if (!preventCounterAction) {

            AddNavNodeSaveState(node);

        }

        preventCounterAction = true;

        node.x = Mathf.RoundToInt(pos.x * 32f);
        node.y = Mathf.RoundToInt(pos.z * -32f);

    }

    public void AddNewNavMesh() {

        main.level.navMeshes.Add(new FCopNavMesh(main.level.CreateEmptyAssetFile(AssetType.NavMesh)));

    }

    public void RemoveNavMesh(int index) {

        DialogWindowUtil.Dialog("Delete Nav Mesh", "Are you sure you would like to delete this nav mesh? Some actors maybe dependent on this nav mesh and may not work correctly. This cannot be undone.", () => {

            main.level.DeleteAsset(AssetType.NavMesh, main.level.navMeshes[index].DataID);

            OnDestroy();

            OnCreateMode();

            view.RefreshDropdown();

            Main.ClearCounterActions();

            return true;

        });

    }

    void AddNode() {

        AddNavMeshSaveState(SelectedNavMesh);

        var node = new NavNode(navNodes.Count,
            Mathf.RoundToInt(navNodeToAdd.transform.position.x * 32f),
            Mathf.RoundToInt(navNodeToAdd.transform.position.z * -32f));

        SelectedNavMesh.nodes.Add(node);

        navNodeToAdd.node = node;

        navNodeToAdd.controller = this;

        navNodeToAdd.Create();

        navNodes.Add(navNodeToAdd);

        navNodeToAdd = null;

    }

    void DeleteNode() {

        AddNavMeshSaveState(SelectedNavMesh);

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
                }
                else if (node.nextNodeIndexes[index] > indexOfDeletedNode && node.nextNodeIndexes[index] != NavNode.invalid) {
                    node.nextNodeIndexes[index]--;
                }

            }

        }

        OnCreateMode();

    }

    public void ChangeState(NavNodeState state) {

        AddNavNodeSaveState(GetSeletedNavNode());

        GetSeletedNavNode().state = state;
    }

    public void ChangeGroundCast(NavNodeGroundCast groundCast) {

        AddNavNodeSaveState(GetSeletedNavNode());

        GetSeletedNavNode().groundCast = groundCast;
    }

    public void ChangeReadHeight(bool value) {

        AddNavNodeSaveState(GetSeletedNavNode());

        GetSeletedNavNode().readHeightOffset = value;
    }

    public void ChangeHeightOffset(int value) {

        AddNavNodeSaveState(GetSeletedNavNode());

        GetSeletedNavNode().SafeSetHeight(value);

    }

    #endregion

    // Using a save state for everything seems a little excessive
    #region Counter-Actions

    class NavMeshSaveState : CounterAction {

        public string name { get; set; }

        List<NavNode> savedMesh = new();
        FCopNavMesh navMesh;

        public NavMeshSaveState(FCopNavMesh navMesh) {

            foreach (var node in navMesh.nodes) {

                savedMesh.Add(node.Clone());

            }

            this.navMesh = navMesh;

            name = "Nav Mesh Changes";

        }

        public void Action() {

            navMesh.nodes = savedMesh;

            if (Main.editMode is not NavMeshEditMode) {
                return;
            }

            var editMode = (NavMeshEditMode)Main.editMode;

            editMode.Refresh();

        }

    }

    class NavNodeSaveState : CounterAction {

        public string name { get; set; }

        NavNode saveNodeState;
        public NavNode navNode;
        Action additionalAction;

        public NavNodeSaveState(NavNode node, Action additionalAction) {
            this.navNode = node;
            this.saveNodeState = node.Clone();

            this.additionalAction = additionalAction;

            name = "Nav Node Changes";

        }

        public void Action() {
            navNode.ReciveData(saveNodeState);
            additionalAction();
        }

    }

    static void AddNavMeshSaveState(FCopNavMesh navMesh) {

        Main.AddCounterAction(new NavMeshSaveState(navMesh));
    }

    static void AddNavNodeSaveState(NavNode navNode) {

        Main.AddCounterAction(new NavNodeSaveState(navNode, () => {

            if (Main.editMode is not NavMeshEditMode) {
                return;
            }

            var editMode = (NavMeshEditMode)Main.editMode;

            editMode.Refresh();

            editMode.view.propertyPanel.Refresh();

        }));

    }

    #endregion

}