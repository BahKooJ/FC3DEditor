
using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class ActorEditMode : EditMode {

    public static List<ActorBehavior> unsupportedBehaviors = new() {
        ActorBehavior.UniversalTrigger,
        ActorBehavior.Aircraft,
        ActorBehavior.Behavior26,
        ActorBehavior.Behavior27,
        ActorBehavior.Behavior30,
        ActorBehavior.Behavior31,
        ActorBehavior.Behavior33,
        ActorBehavior.Behavior34,
        ActorBehavior.Behavior37,
        ActorBehavior.Behavior38,
        ActorBehavior.VisualEffects87,
        ActorBehavior.VisualEffects88,
        ActorBehavior.VisualEffects89,
        ActorBehavior.VisualEffects90,
        ActorBehavior.VisualEffects92,
        ActorBehavior.VisualEffects94,
        ActorBehavior.ActorExplosion,
        ActorBehavior.ParticleEmitter,
        ActorBehavior.PlayerWeapon
    };

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
    ActorObject schematicObjectToAdd = null;
    ActorSchematic schematicToAdd = null;

    public static bool preventCounterAction = false;

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
            case FCopBehavior87:
                nodeObject = Object.Instantiate(main.VisualEffectsActorFab);
                break;
            case FCopBehavior88:
                nodeObject = Object.Instantiate(main.VisualEffectsActorFab);
                break;
            case FCopBehavior89:
                nodeObject = Object.Instantiate(main.VisualEffectsActorFab);
                break;
            case FCopBehavior90:
                nodeObject = Object.Instantiate(main.VisualEffectsActorFab);
                break;
            case FCopBehavior91:
                nodeObject = Object.Instantiate(main.ActorExplosionActorFab);
                break;
            case FCopBehavior92:
                nodeObject = Object.Instantiate(main.VisualEffectsActorFab);
                break;
            case FCopBehavior93:
                nodeObject = Object.Instantiate(main.ParticleEmitterFab);
                break;
            case FCopBehavior94:
                nodeObject = Object.Instantiate(main.VisualEffectsActorFab);
                break;
            case FCopBehavior95:
                nodeObject = Object.Instantiate(main.TriggerActorFab);
                break;
            case FCopBehavior97:
                nodeObject = Object.Instantiate(main.TextureActorFab);
                break;
            case FCopBehavior98:
                nodeObject = Object.Instantiate(main.WeaponActorFab);
                break;
            case FCopBehavior99:
                nodeObject = Object.Instantiate(main.PlayerWeaponActorFab);
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

        if (Controls.OnUp("MoveToCursor") || Input.GetMouseButtonUp(0)) {
            preventCounterAction = false;
        }

        if (schematicObjectToAdd != null) {

            var hitPos = main.CursorOnLevelMesh();

            if (hitPos != null) {

                if (Controls.IsDown("SnapActorPosition")) {

                    hitPos = new Vector3(MathF.Round(hitPos.Value.x * 2) / 2, MathF.Round(hitPos.Value.y * 2) / 2, MathF.Round(hitPos.Value.z * 2) / 2);

                }

                schematicObjectToAdd.transform.position = hitPos.Value;

            }

            if (Input.GetMouseButtonDown(0)) {

                if (hitPos != null) {

                    AddActor(new FCopActor(new IFFDataFile(3, new(schematicToAdd.actorData), "Cact", main.level.sceneActors.FindNextID(), main.level.scripting.emptyOffset)), hitPos.Value);

                    if (!Input.GetKey(KeyCode.LeftShift)) {

                        EndAddSchematic();

                    }

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

                return a.rawFile.rpnsReferences.Contains(39);

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

        schematicToAdd = schematic;

        var newAct = new FCopActor(new IFFDataFile(3, new(schematic.actorData), "Cact", main.level.sceneActors.FindNextID(), main.level.scripting.emptyOffset));

        schematicObjectToAdd = CreateActorObject(newAct);

        UnselectActorCompletely();

    }

    public void EndAddSchematic() {

        if (schematicObjectToAdd == null) {
            return;
        }

        Object.Destroy(schematicObjectToAdd.gameObject);

        schematicObjectToAdd = null;
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

            if (!ActorEditMode.preventCounterAction) {
                ActorEditMode.AddMultiPropertyChangeCounterAction(node.controlledProperties, node.actor);
            }

            ActorEditMode.preventCounterAction = true;

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

    public ActorGroupObject FindActorGroupByID(int id) {

        return actorGroupObjects.FirstOrDefault(ag => {

            foreach (var actObj in ag.actObjects) {

                if (actObj.actor.DataID == id) return true;

            }

            return false;

        });

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

            if (selectedActorObject.controlledObject.TryGetComponent<ActorObject>(out var actorObj)) {
                actorObj.SetToCurrentPosition();
            }

            if (selectedActorObject.controlledObject.TryGetComponent<ActorGroupObject>(out var groupObj)) {
                groupObj.SetToCurrentPosition();
            }

            selectedActorObject.transform.position = selectedActorObject.controlledObject.transform.position;

        }

    }

    public void RefreshActorPosition(int id) {

        var actorObj = actorObjectsByID[id];

        if (actorObj.actor == selectedActor) {
            RefreshSelectedActorPosition();
        }
        else {
            actorObj.SetToCurrentPosition();
        }

    }

    public void SelectActor(ActorObject actorObject, bool jump = true) {

        if (!preventCounterAction) {

            AddActorSelectCounterAction(selectedActor?.DataID);

        }

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

    public void ChangeActorPosition(FCopActor actor, Vector3 pos) {

        if (!preventCounterAction) {

            AddActorPositionCounterAction(actor.x, actor.y, actor);

        }

        preventCounterAction = true;

        actor.x = Mathf.RoundToInt(pos.x * 8192f);
        actor.y = Mathf.RoundToInt(pos.z * -8192f);

    }

    public void ChangeActorsPositionFromGroup(List<FCopActor> actors, Vector3 pos) {

        if (!preventCounterAction) {

            AddMultiActorPositionCounterAction(actors);

        }

        preventCounterAction = true;

        foreach (var actor in actors) {

            actor.x = Mathf.RoundToInt(pos.x * 8192f);
            actor.y = Mathf.RoundToInt(pos.z * -8192f);

        }

    }

    public void CreateActor(ActorBehavior behavior, Vector3 pos) {

        var newActor = new FCopActor(
            main.level.sceneActors.FindNextID(), 
            main.level.scripting.emptyOffset,
            behavior,
            Mathf.RoundToInt(pos.x * 8192f),
            Mathf.RoundToInt(pos.z * -8192f)
            );

        AddActor(newActor, Vector3.zero);

    }

    public void AddActor(FCopActor actor, Vector3 pos, bool refuseSceneRefresh = false) {

        if (!preventCounterAction) {
            AddActorAddCounterAction(actor);
        }

        if (pos != Vector3.zero) {
            actor.x = Mathf.RoundToInt(pos.x * 8192f);
            actor.y = Mathf.RoundToInt(pos.z * -8192f);
        }

        if (actor.behaviorType == ActorBehavior.Weapon) {
            main.level.sceneActors.AddActor(actor, null, true);

            var weaponActors = main.level.sceneActors.actors.Where(a => a.behaviorType == ActorBehavior.Weapon).ToList();

            if (weaponActors.Count > 16) {
                QuickLogHandler.Log("The maximum amount of weapon actors have been exceeded (16). All shooter actors will no longer work!", LogSeverity.Warning);
            }

        }
        else {
            main.level.sceneActors.AddActor(actor, null);
        }

        AddNewActorObject(actor);

        if (!refuseSceneRefresh) {
            view.activeActorPropertiesView.sceneActorsView.Validate();
        }

    }

    public void DeleteActor() {

        if (selectedActorObject == null) {
            return;
        }

        DeleteByID(selectedActor.DataID);

    }

    public void DeleteByID(int id) {

        var actor = main.level.sceneActors.actorsByID[id];
        
        if (!preventCounterAction) {
            AddActorDeleteCounterAction(actor, main.level.sceneActors.CreatePositionSaveState());
        }

        main.level.sceneActors.DeleteActor(id);

        var actObj = actorObjectsByID[id];

        actorObjects.Remove(actObj);
        actorObjectsByID.Remove(id);

        Object.Destroy(actObj.gameObject);

        UnselectActor();

        ValidateGrouping();

        view.activeActorPropertiesView.Refresh();
        view.activeActorPropertiesView.sceneActorsView.RemoveNode(actor);

    }

    public void PasteNavNodeCoords() {

        if (selectedActorObject != null) {

            if (NavMeshEditMode.copiedNavNodeCoords != null) {

                // Guess this is not needed, callbacks do the counter-actions automatically
                //if (selectedActorObject.controlledObject.TryGetComponent<ActorObject>(out var actorObj)) {
                //    AddActorPositionCounterAction(actorObj.actor.x, actorObj.actor.y, actorObj.actor);
                //}

                //if (selectedActorObject.controlledObject.TryGetComponent<ActorGroupObject>(out var groupObj)) {

                //    var actors = new List<FCopActor>();

                //    foreach (var actObj in groupObj.actObjects) {
                //        actors.Add(actObj.actor);
                //    }

                //    AddMultiActorPositionCounterAction(actors);

                //}

                selectedActorObject.transform.position = (Vector3)NavMeshEditMode.copiedNavNodeCoords;
                selectedActorObject.moveCallback((Vector3)NavMeshEditMode.copiedNavNodeCoords, AxisControl.Axis.IgnoreY);

            }

        }

    }

    public void GroupActor(ActorNode toGroup) {

        GroupActor(actorToGroup, toGroup);

        actorToGroup = null;

    }

    public void GroupActor(FCopActor actorToGroup, ActorNode toGroup) {

        HeadsUpTextUtil.End();

        if (actorToGroup == null) return;

        // Has to create a state before grouping
        var counterAction = new GroupCounterAction(main.level.sceneActors.CreatePositionSaveState(), actorToGroup);

        var didGroup = main.level.sceneActors.PositionalGroupActor(actorToGroup, toGroup);

        if (didGroup) {

            Main.AddCounterAction(counterAction);

            ValidateGrouping();

            var actorObj = actorObjectsByID[actorToGroup.DataID];

            actorObj.SetToCurrentPosition();

            view.activeActorPropertiesView.sceneActorsView.Validate();

        }
        else {
            QuickLogHandler.Log("Unable to group actors", LogSeverity.Error);
        }

    }

    public void UngroupActor(FCopActor actor) {

        // Has to create a state before grouping
        var counterAction = new GroupCounterAction(main.level.sceneActors.CreatePositionSaveState(), actor);

        var didUngroup = main.level.sceneActors.PositionalUngroupActor(actor);

        if (didUngroup) {

            Main.AddCounterAction(counterAction);

            ValidateGrouping();

            var actorObj = actorObjectsByID[actor.DataID];

            actorObj.SetToCurrentPosition();

            view.activeActorPropertiesView.sceneActorsView.Validate();

        }
        else {
            QuickLogHandler.Log("Actor is not in group.", LogSeverity.Info);
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

    public void ChangeActorResourceRef(FCopActor actor, int refIndex, FCopActor.Resource resource, AssetType assetType) {

        AddActorResourceRefCounterAction(actor, refIndex, actor.resourceReferences[refIndex], assetType);

        actor.resourceReferences[refIndex] = resource;

        if (assetType == AssetType.Object) {
            UnselectActor();
            RequestActorRefresh(actor.DataID);
            preventCounterAction = true;
            SelectActorByID(actor.DataID);
            preventCounterAction = false;
        }

    }

    #endregion

    #region Counter-Actions

    public class ActorPositionCounterAction : CounterAction {

        public string name { get; set; }

        public int savedX;
        public int savedY;
        public FCopActor modifiedActor;

        public ActorPositionCounterAction(int savedX, int savedY, FCopActor modifiedActor) {
            this.savedX = savedX;
            this.savedY = savedY;
            this.modifiedActor = modifiedActor;

            name = "Actor Position Change";
        }

        public void Action() {

            if (Main.editMode is not ActorEditMode) {
                return;
            }

            var editMode = (ActorEditMode)Main.editMode;

            modifiedActor.x = savedX; 
            modifiedActor.y = savedY;

            editMode.RefreshActorPosition(modifiedActor.DataID);

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
                ActorEditMode.preventCounterAction = true;
                editMode.SelectActor(editMode.actorObjectsByID[(int)selectedActorID]);
                ActorEditMode.preventCounterAction = false;

            }
            else {
                editMode.view.RefreshActorPropertiesView();
                editMode.view.activeActorPropertiesView.sceneActorsView.RefreshSelection(false);
            }

        }

    }

    public class ActorDeleteCounterAction : CounterAction {

        public string name { get; set; }

        public FCopActor deletedActor;
        List<ActorNode> positionGroupSaveState;

        public ActorDeleteCounterAction(FCopActor deletedActor, List<ActorNode> positionGroupSaveState) {
            this.deletedActor = deletedActor;
            this.positionGroupSaveState = positionGroupSaveState;
            name = "Actor Deletion";

        }

        public void Action() {

            if (Main.editMode is not ActorEditMode) {
                return;
            }

            var editMode = (ActorEditMode)Main.editMode;

            ActorEditMode.preventCounterAction = true;
            editMode.AddActor(deletedActor, Vector3.zero, true);
            editMode.main.level.sceneActors.positionalGroupedActors = positionGroupSaveState;
            editMode.view.activeActorPropertiesView.sceneActorsView.Refresh();
            ActorEditMode.preventCounterAction = false;

            // Delayed action because it takes another frame for the node to built so it can ungroup
            // if node is inside group.
            // JK this causes so many problems I'm leaving it alone for now.
            //Main.delayedActions.Add(new DelayedAction(1, () => {
            //    editMode.preventCounterAction = true;
            //    editMode.SelectActorByID(deletedActor.DataID);
            //    editMode.preventCounterAction = false;

            //}));

        }

    }

    public class ActorAddCounterAction : CounterAction {

        public string name { get; set; }

        FCopActor addedActor;

        public ActorAddCounterAction(FCopActor addedActor) {
            this.addedActor = addedActor;
            name = "Actor Addition";
        }

        public void Action() {

            if (Main.editMode is not ActorEditMode) {
                return;
            }

            var editMode = (ActorEditMode)Main.editMode;

            ActorEditMode.preventCounterAction = true;
            editMode.DeleteByID(addedActor.DataID);
            ActorEditMode.preventCounterAction = false;

        }
    }

    public class GroupCounterAction : CounterAction {

        public string name { get; set; }

        List<ActorNode> positionGroupSaveState;
        FCopActor groupedActor;
        int savedX;
        int savedY;

        public GroupCounterAction(List<ActorNode> positionGroupSaveState, FCopActor groupedActor) {
            this.positionGroupSaveState = positionGroupSaveState;
            this.groupedActor = groupedActor;
            savedX = groupedActor.x;
            savedY = groupedActor.y;

            name = "Actor Grouping";
        }

        public void Action() {

            if (Main.editMode is not ActorEditMode) {
                return;
            }

            var editMode = (ActorEditMode)Main.editMode;

            groupedActor.x = savedX;
            groupedActor.y = savedY;

            editMode.main.level.sceneActors.positionalGroupedActors = positionGroupSaveState;
            editMode.view.activeActorPropertiesView.sceneActorsView.Refresh();
            editMode.RefreshActorPosition(groupedActor.DataID);
            editMode.ValidateGrouping();

        }

    }

    public class PropertyChangeCounterAction : CounterAction {

        public string name { get; set; }

        int compiledValue;
        ActorProperty property;
        FCopActor actor;

        public PropertyChangeCounterAction(int compiledValue, ActorProperty property, FCopActor actor) {
            this.compiledValue = compiledValue;
            this.property = property;
            this.actor = actor;

            name = "Actor Property Change";
        }

        public void Action() {

            if (Main.editMode is not ActorEditMode) {
                return;
            }

            var editMode = (ActorEditMode)Main.editMode;

            property.SetCompiledValue(compiledValue);

            editMode.view.RefreshActorPropertiesView();
            editMode.RefreshActorPosition(actor.DataID);

            if (ActorPropertyChangeEvent.changeEventsByPropertyName.ContainsKey(property.name)) {

                ActorPropertyChangeEvent.changeEventsByPropertyName[property.name](editMode, property);

            }

        }

    }

    public class ActorResourceRefCounterAction : CounterAction {

        public string name { get; set; }

        FCopActor actor;
        int refIndex;
        FCopActor.Resource resource;
        AssetType assetType;

        public ActorResourceRefCounterAction(FCopActor actor, int refIndex, FCopActor.Resource resource, AssetType assetType) {
            this.actor = actor;
            this.refIndex = refIndex;
            this.resource = resource;
            this.assetType = assetType;

            name = "Actor Resource Change";
        }

        public void Action() {

            if (Main.editMode is not ActorEditMode) {
                return;
            }

            var editMode = (ActorEditMode)Main.editMode;

            editMode.ChangeActorResourceRef(actor, refIndex, resource, assetType);
            editMode.view.activeActorPropertiesView.activeActorAssetRefView?.Refresh();
        }

    }

    public class ActorSpawningPropertiesCounterAction : CounterAction {
        public string name { get; set; }

        FCopActor actor;
        public List<byte> spawningPropertiesSaveState;

        public ActorSpawningPropertiesCounterAction(FCopActor actor, List<byte> spawningPropertiesSaveState) {
            this.actor = actor;
            this.spawningPropertiesSaveState = spawningPropertiesSaveState;

            name = "Actor Spawn Property Change";
        }

        public void Action() {

            if (Main.editMode is not ActorEditMode) {
                return;
            }

            var editMode = (ActorEditMode)Main.editMode;

            if (spawningPropertiesSaveState == null) {
                actor.spawningProperties = null;
            }
            else {
                actor.spawningProperties = new FCopActorSpawning(spawningPropertiesSaveState);
            }

            editMode.view.activeActorPropertiesView.activeSpawningProperties?.Refresh();

        }

    }

    static void AddActorPositionCounterAction(int savedX, int savedY, FCopActor modifiedActor) {
        Main.AddCounterAction(new ActorPositionCounterAction(savedX, savedY, modifiedActor));
    }

    static void AddMultiActorPositionCounterAction(List<FCopActor> modifiedActors) {

        var counterActions = new List<CounterAction>();

        foreach (var actor in modifiedActors) {
            counterActions.Add(new ActorPositionCounterAction(actor.x, actor.y, actor));
        }

        Main.AddCounterAction(new MultiCounterAction(counterActions, () => {

            if (Main.editMode is not ActorEditMode) {
                return;
            }

            var editMode = (ActorEditMode)Main.editMode;

            // The ActorPositionCounterAction object already does the reverting of the actor position
            // along with refreshing any ActorObjects. Since this is a multi position change, we need
            // to refresh the groups as well.
            var groupsToRefresh = new HashSet<ActorGroupObject>();

            foreach (var action in counterActions.Cast<ActorPositionCounterAction>()) {

                var group = editMode.FindActorGroupByID(action.modifiedActor.DataID);

                if (group != null) {
                    groupsToRefresh.Add(group);
                }

            }

            foreach (var group in groupsToRefresh) {
                group.SetToCurrentPosition();
            }

            editMode.RefreshSelectedActorPosition();

        }));

    }

    static void AddActorSelectCounterAction(int? selectedActorID) {

        Main.AddCounterAction(new ActorSelectCounterAction(selectedActorID));

    }

    static void AddActorDeleteCounterAction(FCopActor deletedActor, List<ActorNode> positionGroupSaveState) {

        Main.AddCounterAction(new ActorDeleteCounterAction(deletedActor, positionGroupSaveState));

    }

    static void AddActorAddCounterAction(FCopActor addedActor) {
        Main.AddCounterAction(new ActorAddCounterAction(addedActor));
    }

    // This breaks architecture as the property view items change the value.
    public static void AddPropertyChangeCounterAction(ActorProperty property, FCopActor actor) {
        Main.AddCounterAction(new PropertyChangeCounterAction(property.GetCompiledValue(), property, actor));
    }

    public static void AddMultiPropertyChangeCounterAction(List<ActorProperty> properties, FCopActor actor) {

        var counterActions = new List<CounterAction>();

        foreach (var property in properties) {
            counterActions.Add(new PropertyChangeCounterAction(property.GetCompiledValue(), property, actor));
        }

        Main.AddCounterAction(new MultiCounterAction(counterActions, () => { }));
    }

    static void AddActorResourceRefCounterAction(FCopActor actor, int refIndex, FCopActor.Resource resource, AssetType assetType) {
        Main.AddCounterAction(new ActorResourceRefCounterAction(actor, refIndex, resource, assetType));
    }

    // This breaks arhcitexture.
    public static void AddActorSpawningPropertiesCounterAction(FCopActor actor, List<byte> spawningPropertiesSaveState) {
        Main.AddCounterAction(new ActorSpawningPropertiesCounterAction(actor, spawningPropertiesSaveState));
    }

    #endregion

}