
using FCopParser;
using System;
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
    public List<ActorEditingNode> actorEditingNodes = new();

    public AxisControl selectedActorObject = null;
    public AxisControl selectedActorEditingNode = null;
    public FCopActor selectedActor = null;
    public FCopActor actorToGroup = null;

    GameObject arrowModel = null;
    ActorBehavior? actorToAdd = null;
    ActorObject schematicToAdd = null;

    static int counterActionID = 0;

    public ActorEditMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {

        foreach (var node in main.level.sceneActors.positionalGroupedActors) {

            if (node.nestedActors.Count == 1) {

                var createdActObj = CreateActorObject(node.nestedActors[0]);

                actorObjects.Add(createdActObj);
                actorObjectsByID[createdActObj.actor.DataID] = createdActObj;

            }
            else {

                CreateGroup(node.nestedActors);

            }

        }

    }

    public ActorObject CreateActorObject(FCopActor actor, ActorGroupObject group = null) {

        GameObject nodeObject;

        switch (actor.behavior) {
            case FCopBehavior29:
                nodeObject = Object.Instantiate(main.TeleporterActor);
                break;
            case FCopBehavior35:
                nodeObject = Object.Instantiate(main.MapObjectiveNodesActor);
                break;
            case FCopBehavior95:
                nodeObject = Object.Instantiate(main.TriggerActorFab);
                break;
            case FCopBehavior97:
                nodeObject = Object.Instantiate(main.TextureActorFab);
                break;
            default:
                nodeObject = Object.Instantiate(main.BlankActor);
                break;
        }


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

            var createdActObj = CreateActorObject(actor, script);

            createdActObj.transform.SetParent(script.transform, false);

            script.actObjects.Add(createdActObj);

            actorObjects.Add(createdActObj);
            actorObjectsByID[actor.DataID] = createdActObj;

        }

        script.Init();

        actorGroupObjects.Add(script);

    }

    public void OnDestroy() {

        UnselectActor();
        EndAddSchematic();

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
        view.CloseActorSchematicsView();
        view.CloseSupportingActorDataPanel();

    }

    public void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {

            HeadsUpTextUtil.End();
            actorToGroup = null;
            EndAdd();
            EndAddSchematic();

        }

        if (Main.ignoreAllInputs) {
            return;
        }

        if (schematicToAdd != null) {

            var hitPos = main.CursorOnLevelMesh();

            if (hitPos != null) {
                schematicToAdd.transform.position = hitPos.Value;
            }

            if (Input.GetMouseButtonDown(0)) {

                if (hitPos != null) {

                    AddActor(schematicToAdd.actor, hitPos.Value);

                    EndAddSchematic();

                }

            }

            return;
        }

        if (actorToAdd != null) {

            var hitPos = main.CursorOnLevelMesh();

            if (hitPos != null) {
                arrowModel.transform.position = hitPos.Value;
            }

            if (Input.GetMouseButtonDown(0)) {

                if (hitPos != null) {

                    CreateActor(actorToAdd.Value, hitPos.Value);

                    EndAdd();

                }

            }

            return;

        }

        if (Controls.OnUp("MoveToCursor") || Controls.OnUp("Select") || Input.GetMouseButtonUp(0)) {

            counterActionID++;

        }

        // Actor editing node take priority
        if (selectedActorEditingNode != null) {

            if (selectedActorEditingNode.TestCollision()) {
                return;
            }

            if (Controls.IsDown("MoveToCursor")) {

                MoveActorToCursor();

            }

        }
        else if (selectedActorObject != null) {

            if (selectedActorObject.TestCollision()) {
                return;
            }

            // Moves object to cursor
            if (Controls.IsDown("MoveToCursor")) {

                MoveActorToCursor();

            }

        }


        if ((Controls.OnDown("Select") || Input.GetMouseButtonDown(1)) && !Main.IsMouseOverUI()) {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 8)) {

                if (!TestEditingNodeSelection(hit)) {

                    TestActorSelection(hit);

                }

            }


        }

        if (Input.GetButtonDown("Delete")) {
            DeleteActor();
        }

        if (Controls.OnDown("Unselect")) {

            if (selectedActorEditingNode != null) {
                UnSelectActorEditingNode();
            }
            else {

                UnselectActorCompletely();

            }

        }

        if (Input.GetKeyDown(KeyCode.F10)) {

            var actors = main.level.sceneActors.actors.Where(a => {

                //return a.behavior is FCopShooter e;

                if (a.behavior is FCopBehavior96 es) {
                    return es.propertiesByName["Unknown2"].GetCompiledValue() == 1;
                }
                return false;

            }).ToList();

            //foreach (var actor in actors) {
            //    ((ToggleActorProperty)actor.behavior.propertiesByName["STag unknown4"]).value = true;
            //}

            MoveToActor(actors[UnityEngine.Random.Range(0, actors.Count)].DataID);

        }

    }

    bool TestEditingNodeSelection(RaycastHit hit) {

        foreach (var node in actorEditingNodes) {

            if (hit.colliderInstanceID == node.nodeCollider.GetInstanceID()) {

                SelectActorEditingNode(node);

                return true;

            }

        }

        return false;

    }

    bool TestActorSelection(RaycastHit hit) {

        foreach (var act in actorObjects) {

            var didHit = false;

            if (act.missingObjectGameobj != null && act.missingObjectGameobj.activeSelf) {

                if (hit.colliderInstanceID == act.missingObjectGameobj.GetComponentInChildren<Collider>().GetInstanceID()) {
                    didHit = true;
                }

            }
            else if (act.actCollider != null) {

                if (hit.colliderInstanceID == act.actCollider.GetInstanceID()) {
                    didHit = true;
                }

            }
            else {

                foreach (var obj in act.objects) {

                    if (obj == null) {
                        continue;
                    }

                    if (hit.colliderInstanceID == obj.meshCollider.GetInstanceID()) {
                        didHit = true;
                    }
                    else {

                        foreach (var specialObj in obj.specialPrimitives) {

                            if (hit.colliderInstanceID == specialObj.GetComponent<MeshCollider>().GetInstanceID()) {
                                didHit = true;
                                break;
                            }

                        }

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
                }

                return didHit;

            }

        }

        return false;

    }

    public void StartGroup(FCopActor actor) {
        actorToGroup = actor;
        HeadsUpTextUtil.HeadsUp("Select Actor to Group...");
    }

    public void StartAddSchematic(ActorSchematic schematic) {

        var newAct = new FCopActor(new IFFDataFile(3, new(schematic.actorData), "Cact", main.level.sceneActors.FindNextID(), main.level.scripting.emptyOffset));

        var obj = Object.Instantiate(main.BlankActor);
        obj.layer = 8; // UI Mesh
        var actorObject = obj.GetComponent<ActorObject>();
        actorObject.actor = newAct;
        actorObject.controller = this;

        actorObject.Create();

        schematicToAdd = actorObject;

        UnselectActorCompletely();

    }

    public void EndAddSchematic() {

        if (schematicToAdd == null) {
            return;
        }

        Object.Destroy(schematicToAdd.gameObject);

        schematicToAdd = null;

    }

    public void StartAdd(ActorBehavior behavior) {

        actorToAdd = behavior;
        arrowModel = Object.Instantiate(main.ArrowModelFab);

        var pos = main.CursorOnLevelMesh();

        if (pos != null) {
            arrowModel.transform.position = pos.Value;
        }

        UnselectActorCompletely();

    }

    public void EndAdd() {

        if (arrowModel == null) {
            return;
        }

        Object.Destroy(arrowModel);

        actorToAdd = null;

    }

    #region Selection And GameObjects

    void AddActorEditingNodes() {

        void InitMapNode(NormalizedValueProperty propX, NormalizedValueProperty propY) {

            var obj = Object.Instantiate(main.MapNodeFab);

            var editingNode = obj.GetComponent<MapNodeEditingNode>();
            editingNode.controller = this;
            editingNode.actor = selectedActor;
            editingNode.controlledProperties = new() { 
                propX, 
                propY 
            };
            editingNode.propertyX = propX;
            editingNode.propertyY = propY;
            editingNode.actorObject = actorObjectsByID[selectedActor.DataID];

            actorEditingNodes.Add(editingNode);

        }

        void InitElevatorStopNode(int stop, NormalizedValueProperty heightOffset) {

            var obj = Object.Instantiate(main.ElevatorStopNodeFab);

            var editingNode = obj.GetComponent<ElevatorStopNode>();
            editingNode.controller = this;
            editingNode.actor = selectedActor;
            editingNode.controlledProperties = new() {
                heightOffset
            };
            editingNode.stopIndex = stop;
            editingNode.heightOffset = heightOffset;

            actorEditingNodes.Add(editingNode);

        }

        void InitMoveablePropStop() {

            var obj = Object.Instantiate(main.MovablePropStopFab);

            var editingNode = obj.GetComponent<MovablePropStop>();
            editingNode.controller = this;
            editingNode.actor = selectedActor;

            var moveAxis = selectedActor.behavior.propertiesByName["Move Axis"];
            var moveOffset = selectedActor.behavior.propertiesByName["Ending Position Offset"];
            var rotationOffset = selectedActor.behavior.propertiesByName["Ending Rotation"];
            var rotation = selectedActor.behavior.propertiesByName["Rotation"];


            editingNode.controlledProperties = new() {
                moveOffset, rotationOffset
            };

            editingNode.actorObject = selectedActorObject.controlledObject.GetComponent<ActorObject>();
            editingNode.moveAxis = (EnumDataActorProperty)moveAxis;
            editingNode.moveOffset = (NormalizedValueProperty)moveOffset;
            editingNode.rotationOffset = (RangeActorProperty)rotationOffset;
            editingNode.rotation = (RangeActorProperty)rotation;

            actorEditingNodes.Add(editingNode);

        }

        void InitTeleporterNode(NormalizedValueProperty propX, NormalizedValueProperty propY) {

            var obj = Object.Instantiate(main.TeleporterLocationNodeFab);

            var editingNode = obj.GetComponent<TeleporterLocationEditingNode>();
            editingNode.controller = this;
            editingNode.actor = selectedActor;
            editingNode.controlledProperties = new() {
                propX,
                propY
            };
            editingNode.propertyX = propX;
            editingNode.propertyY = propY;
            editingNode.actorObject = actorObjectsByID[selectedActor.DataID];

            actorEditingNodes.Add(editingNode);

        }

        void InitTriggerNode() {

            var obj = Object.Instantiate(main.TriggerActorNodeFab);

            var editingNode = obj.GetComponent<TriggerActorNode>();
            editingNode.controller = this;
            editingNode.actor = selectedActor;
            editingNode.actorObject = actorObjectsByID[selectedActor.DataID];

            actorEditingNodes.Add(editingNode);

        }

        switch (selectedActor.behavior) {

            case FCopBehavior10:

                InitElevatorStopNode(2, (NormalizedValueProperty)selectedActor.behavior.propertiesByName["2nt Height Offset"]);
                InitElevatorStopNode(3, (NormalizedValueProperty)selectedActor.behavior.propertiesByName["3rd Height Offset"]);

                break;
            case FCopBehavior25:
                InitMoveablePropStop();
                break;
            case FCopBehavior29:
                InitTeleporterNode((NormalizedValueProperty)selectedActor.behavior.propertiesByName["X"], (NormalizedValueProperty)selectedActor.behavior.propertiesByName["Y"]);
                break;
            case FCopBehavior35:

                InitMapNode((NormalizedValueProperty)selectedActor.behavior.propertiesByName["Node 1 X"], (NormalizedValueProperty)selectedActor.behavior.propertiesByName["Node 1 Y"]);
                InitMapNode((NormalizedValueProperty)selectedActor.behavior.propertiesByName["Node 2 X"], (NormalizedValueProperty)selectedActor.behavior.propertiesByName["Node 2 Y"]);
                InitMapNode((NormalizedValueProperty)selectedActor.behavior.propertiesByName["Node 3 X"], (NormalizedValueProperty)selectedActor.behavior.propertiesByName["Node 3 Y"]);
                InitMapNode((NormalizedValueProperty)selectedActor.behavior.propertiesByName["Node 4 X"], (NormalizedValueProperty)selectedActor.behavior.propertiesByName["Node 4 Y"]);
                InitMapNode((NormalizedValueProperty)selectedActor.behavior.propertiesByName["Node 5 X"], (NormalizedValueProperty)selectedActor.behavior.propertiesByName["Node 5 Y"]);
                InitMapNode((NormalizedValueProperty)selectedActor.behavior.propertiesByName["Node 6 X"], (NormalizedValueProperty)selectedActor.behavior.propertiesByName["Node 6 Y"]);
                InitMapNode((NormalizedValueProperty)selectedActor.behavior.propertiesByName["Node 7 X"], (NormalizedValueProperty)selectedActor.behavior.propertiesByName["Node 7 Y"]);
                InitMapNode((NormalizedValueProperty)selectedActor.behavior.propertiesByName["Node 8 X"], (NormalizedValueProperty)selectedActor.behavior.propertiesByName["Node 8 Y"]);

                break;
            case FCopBehavior95:
                InitTriggerNode();

                break;

        }

    }

    void ClearEditingNodes() {

        UnSelectActorEditingNode();

        foreach (var obj in actorEditingNodes) {
            Object.Destroy(obj.gameObject);
        }

        actorEditingNodes.Clear();

    }

    void SelectActorEditingNode(ActorEditingNode node) {

        UnSelectActorEditingNode();

        var axisControl = Object.Instantiate(main.axisControl);
        var script = axisControl.GetComponent<AxisControl>();

        script.controlledObject = node.gameObject;
        script.moveCallback = (newPos, axis) => {

            if (Controls.IsDown("SnapActorPosition")) {

                var lockPos = new Vector3(MathF.Round(newPos.x * 2) / 2, MathF.Round(newPos.y * 2) / 2, MathF.Round(newPos.z * 2) / 2);
                node.OnPositionChange(lockPos, axis);
            }
            else {

                node.OnPositionChange(newPos, axis);

            }

            return true;
        };

        selectedActorEditingNode = script;
        view.activeActorPropertiesView.JumpToPropety(node.controlledProperties[0]);
        foreach (var prop in node.controlledProperties) {
            view.activeActorPropertiesView.HighlightProperty(prop);
        }

    }

    void UnSelectActorEditingNode() {

        if (selectedActorEditingNode != null) {

            selectedActorEditingNode.ClearOutlineOnObject();

            Object.Destroy(selectedActorEditingNode.gameObject);

        }

        selectedActorEditingNode = null;

    }

    public void RequestActorRefresh(int id) {

        actorObjectsByID[id].Refresh();

    }

    public void UnselectActorCompletely() {

        ClearEditingNodes();

        if (selectedActor == null) {
            return;
        }

        AddActorSelectCounterAction(selectedActor.DataID);

        if (selectedActorObject != null) {

            selectedActorObject.ClearOutlineOnObject();

            Object.Destroy(selectedActorObject.gameObject);

        }

        selectedActor = null;
        selectedActorObject = null;

        view.activeActorPropertiesView.Refresh();
        view.activeActorPropertiesView.sceneActorsView.ClearSelection();

    }

    public void UnselectActor() {

        ClearEditingNodes();

        if (selectedActorObject != null) {

            selectedActorObject.ClearOutlineOnObject();

            Object.Destroy(selectedActorObject.gameObject);

        }

        selectedActor = null;
        selectedActorObject = null;

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

    public void RefreshSelectedActorPosition() {

        if (selectedActorObject != null) {

            // WTF DO YOU WANT FROM ME UNITY???
            // "Unity objects should not use null propagation" Ok odd guess I'll do it the verbos wa- OH WAIT
            // "GetComponent allocates even if no component is found."
            // FINE GUESS I'LL DO WHATEVER TF THIS IS

            if (selectedActorObject.controlledObject.TryGetComponent<ActorObject>(out var actorObj)) {
                actorObj.SetToCurrentPosition();
            }

            if (selectedActorObject.controlledObject.TryGetComponent<ActorGroupObject>(out var groupObj)) {
                groupObj.SetToCurrentPosition();
            }

            selectedActorObject.transform.position = selectedActorObject.controlledObject.transform.position;

        }

    }

    public void SelectActor(ActorObject actorObject, bool jump = true) {

        AddActorSelectCounterAction(selectedActor?.DataID);

        UnselectActor();

        var axisControl = Object.Instantiate(main.axisControl);
        var script = axisControl.GetComponent<AxisControl>();

        if (actorObject.group != null) {

            script.controlledObject = actorObject.group.gameObject;

            script.moveCallback = (newPos, axis) => {

                if (Controls.IsDown("SnapActorPosition")) {

                    var lockPos = new Vector3(MathF.Round(newPos.x * 2) / 2, MathF.Round(newPos.y * 2) / 2, MathF.Round(newPos.z * 2) / 2);
                    actorObject.group.ChangePosition(lockPos, axis);

                }
                else {
                    actorObject.group.ChangePosition(newPos, axis);
                }

                return true;
            };

        }
        else {

            script.controlledObject = actorObject.gameObject;

            script.moveCallback = (newPos, axis) => {

                if (Controls.IsDown("SnapActorPosition")) {

                    var lockPos = new Vector3(MathF.Round(newPos.x * 2) / 2, MathF.Round(newPos.y * 2) / 2, MathF.Round(newPos.z * 2) / 2);
                    actorObject.ChangePosition(lockPos, axis);

                }
                else {

                    actorObject.ChangePosition(newPos, axis);

                }


                return true;
            };

        }

        selectedActorObject = script;
        selectedActor = actorObject.actor;

        view.RefreshActorPropertiesView();
        view.activeActorPropertiesView.sceneActorsView.RefreshSelection(jump);

        AddActorEditingNodes();

    }

    public void SelectActorByID(int id) {

        var actorObj = actorObjects.First(obj => obj.actor.DataID == id);

        SelectActor(actorObj, false);

    }

    public void MoveToActor(int id) {

        ActorObject actorObj;

        actorObj = actorObjects.First(obj => obj.actor.DataID == id);

        Camera.main.transform.position = actorObj.transform.position;

        Camera.main.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * -4);

    }

    // Used for making actors objects during edit mode runtime.
    public void AddNewActorObject(FCopActor actor) {

        var createdActObj = CreateActorObject(actor);

        actorObjects.Add(createdActObj);
        actorObjectsByID[createdActObj.actor.DataID] = createdActObj;

        ValidateGrouping();

    }

    // Calls "ChangeActorPosition" which is model mutating
    void MoveActorToCursor() {

        var hitPos = main.CursorOnLevelMesh();

        if (hitPos != null) {

            var nnHitPost = hitPos.Value;

            nnHitPost.y = 0;

            if (selectedActorEditingNode != null) {

                selectedActorEditingNode.moveCallback(nnHitPost, AxisControl.Axis.IgnoreY);

                selectedActorEditingNode.transform.position = selectedActorEditingNode.controlledObject.transform.position;

            }
            else if (selectedActorObject != null) {

                selectedActorObject.moveCallback(nnHitPost, AxisControl.Axis.IgnoreY);

                selectedActorObject.transform.position = selectedActorObject.controlledObject.transform.position;

            }

        }

    }

    #endregion

    #region Model Mutating

    public void CreateActor(ActorBehavior behavior, Vector3 pos) {

        var newActor = new FCopActor(
            main.level.sceneActors.FindNextID(), 
            main.level.scripting.emptyOffset,
            behavior,
            Mathf.RoundToInt(pos.x * 8192f),
            Mathf.RoundToInt(pos.z * -8192f)
            );

        main.level.sceneActors.AddActor(newActor, null);

        AddNewActorObject(newActor);

    }

    public void AddActor(FCopActor actor, Vector3 pos) {

        actor.x = Mathf.RoundToInt(pos.x * 8192f);
        actor.y = Mathf.RoundToInt(pos.z * -8192f);

        main.level.sceneActors.AddActor(actor, null);

        AddNewActorObject(actor);

    }

    public void DeleteActor() {

        if (selectedActorObject == null) {
            return;
        }

        DeleteByID(selectedActor.DataID);

    }

    public void DeleteByID(int id) {

        var actor = main.level.sceneActors.actorsByID[id];
        
        AddActorDeleteCounterAction(actor);

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
                selectedActorObject.moveCallback((Vector3)NavMeshEditMode.copiedNavNodeCoords, AxisControl.Axis.IgnoreY);

            }

        }

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

    public void UngroupActor(FCopActor actor) {

        var didUngroup = main.level.sceneActors.PositionalUngroupActor(actor);

        if (didUngroup) {

            ValidateGrouping();

            var actorObj = actorObjectsByID[actor.DataID];

            actorObj.SetToCurrentPosition();

            view.activeActorPropertiesView.sceneActorsView.Refresh(true);

        }

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

    #endregion

    // TODO: Convert to state-like saving
    // So many values would need to be saved in order to a nonsave state undo.
    // Many methods would need to change and data passed simply for undo which would also break
    // architecture. Undo will remain incomplete until a save state undo can be made.
    #region Counter-Actions

    public class ActorPositionCounterAction : CounterAction {

        public string name { get; set; }

        public int savedX;
        public int savedY;
        public FCopActor modifiedActor;
        // Because it runs a func every frame to move and it needs to add a counter action,
        // it needs a way to know if a counter action was already added.
        // Since a type can't be used, an ID will be provided instead.
        // This isn't super interchangable but it works.
        public int id;

        public ActorPositionCounterAction(int savedX, int savedY, FCopActor modifiedActor, int id) {
            this.savedX = savedX;
            this.savedY = savedY;
            this.modifiedActor = modifiedActor;
            this.id = id;

            name = "Actor Position Change";
        }

        public void Action() {

            if (Main.editMode is not ActorEditMode) {
                return;
            }

            var editMode = (ActorEditMode)Main.editMode;

            modifiedActor.x = savedX; 
            modifiedActor.y = savedY;

            editMode.RefreshSelectedActorPosition();

        }

    }

    public class MultiActorPositionCounterAction : CounterAction {

        public string name { get; set; }

        public int savedX;
        public int savedY;
        public List<FCopActor> modifiedActors;
        public int id;

        public MultiActorPositionCounterAction(int savedX, int savedY, List<FCopActor> modifiedActors, int id) {
            this.savedX = savedX;
            this.savedY = savedY;
            this.modifiedActors = modifiedActors;
            this.id = id;

            name = "Actor Position Change";
        }

        public void Action() {

            if (Main.editMode is not ActorEditMode) {
                return;
            }

            var editMode = (ActorEditMode)Main.editMode;

            foreach (var actor in modifiedActors) {
                actor.x = savedX;
                actor.y = savedY;
            }

            editMode.RefreshSelectedActorPosition();

        }

    }

    public class ActorSelectCounterAction : CounterAction {

        public string name { get; set; }

        public int? selectedActorID;

        public ActorSelectCounterAction(int? selectedActorID) {
            this.selectedActorID = selectedActorID;

            name = "Actor Selection";

        }

        public void Action() {

            if (Main.editMode is not ActorEditMode) {
                return;
            }

            var editMode = (ActorEditMode)Main.editMode;

            editMode.UnselectActor();

            if (selectedActorID != null) {
                editMode.SelectActor(editMode.actorObjectsByID[(int)selectedActorID]);
            }
            else {
                editMode.view.activeActorPropertiesView.sceneActorsView.RefreshSelection(false);
            }

        }

    }

    public class ActorPropertyCounterAction : CounterAction {

        public string name { get; set; }

        public ActorProperty property;
        public int value;

        public ActorPropertyCounterAction(ActorProperty property, int value) {
            this.property = property;
            this.value = value;

            name = "Actor Property Change";

        }

        public void Action() {

            var editMode = (ActorEditMode)Main.editMode;

            switch (property) {

                case ValueActorProperty:
                    var valueProp = (ValueActorProperty)property;
                    valueProp.value = value;
                    break;
                case EnumDataActorProperty:
                    var enumProp = (EnumDataActorProperty)property;
                    enumProp.caseValue = (Enum)Enum.ToObject(enumProp.caseValue.GetType(), value);
                    break;

            }


            editMode.view.RefreshActorPropertiesView();

        }

    }

    public class ActorDeleteCounterAction : CounterAction {

        public string name { get; set; }

        public FCopActor deletedActor;

        public ActorDeleteCounterAction(FCopActor deletedActor) {
            this.deletedActor = deletedActor;

            name = "Actor Deletion";

        }

        public void Action() {

            if (Main.editMode is not ActorEditMode) {
                return;
            }

            var editMode = (ActorEditMode)Main.editMode;

            editMode.main.level.sceneActors.AddActor(deletedActor, null);

            // No need to add group because group is validated for sceneActors.
            editMode.AddNewActorObject(deletedActor);

            editMode.view.activeActorPropertiesView.sceneActorsView.Refresh();

        }

    }

    static void AddActorPositionCounterAction(int savedX, int savedY, FCopActor modifiedActor, int id) {

        var last = Main.counterActions.Last();

        if (last is ActorPositionCounterAction posCounterAction) {

            if (posCounterAction.id != id) {

                Main.AddCounterAction(new ActorPositionCounterAction(savedX, savedY, modifiedActor, id));

            }

        }
        else {

            Main.AddCounterAction(new ActorPositionCounterAction(savedX, savedY, modifiedActor, id));

        }

    }

    static void AddMultiActorPositionCounterAction(int savedX, int savedY, List<FCopActor> modifiedActors, int id) {

        var last = Main.counterActions.Last();

        if (last is MultiActorPositionCounterAction posCounterAction) {

            if (posCounterAction.id != id) {

                Main.AddCounterAction(new MultiActorPositionCounterAction(savedX, savedY, modifiedActors, id));

            }

        }
        else {

            Main.AddCounterAction(new MultiActorPositionCounterAction(savedX, savedY, modifiedActors, id));

        }

    }

    static void AddActorSelectCounterAction(int? selectedActorID) {

        Main.AddCounterAction(new ActorSelectCounterAction(selectedActorID));

    }

    public static void AddActorPropertyCounterAction(ActorProperty property) {

        switch(property) {

            case ValueActorProperty:
                var valueProp = (ValueActorProperty)property;
                Main.AddCounterAction(new ActorPropertyCounterAction(valueProp, valueProp.value));
                break;
            case EnumDataActorProperty:
                var enumProp = (EnumDataActorProperty)property;
                Main.AddCounterAction(new ActorPropertyCounterAction(enumProp, Convert.ToInt32(enumProp.caseValue)));
                break;


        }

    }

    static void AddActorDeleteCounterAction(FCopActor deletedActor) {

        Main.AddCounterAction(new ActorDeleteCounterAction(deletedActor));

    }

    #endregion

}