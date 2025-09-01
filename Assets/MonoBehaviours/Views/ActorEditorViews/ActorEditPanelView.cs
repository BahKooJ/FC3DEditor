using UnityEngine;

public class ActorEditPanelView : MonoBehaviour {

    public ActorEditMode controller;

    // - Prefabs -
    public GameObject actorPropertiesView;
    public GameObject actorSchematicViewFab;
    public GameObject supportingActorDataViewFab;

    // - Unity Refs -
    public ContextMenuHandler addActorContextMenu;

    public ActorPropertiesView activeActorPropertiesView;
    public ActorSchematicView activeActorSchematicView;
    public SupportingActorDataView activeSupportingActorDataView;

    void Start() {

        controller.view = this;
        OpenActorPropertiesView();

        addActorContextMenu.items = new() {
            ("Dynamic Prop", () => { controller.StartAdd(FCopParser.ActorBehavior.DynamicProp); }),
            ("Static Prop", () => { controller.StartAdd(FCopParser.ActorBehavior.StaticProp); }),
            ("Walkable Prop", () => { controller.StartAdd(FCopParser.ActorBehavior.WalkableProp); }),
            ("Movable Prop", () => { controller.StartAdd(FCopParser.ActorBehavior.MovableProp); }),
            ("Texture", () => { controller.StartAdd(FCopParser.ActorBehavior.Texture); }),
            ("Elevator", () => { controller.StartAdd(FCopParser.ActorBehavior.Elevator); }),
            ("Teleporter", () => { controller.StartAdd(FCopParser.ActorBehavior.Teleporter); }),
            ("Pathed Entity", () => { controller.StartAdd(FCopParser.ActorBehavior.PathedEntity); }),
            ("Stationary Entity", () => { controller.StartAdd(FCopParser.ActorBehavior.StationaryEntity); }),
            ("Stationary Turret", () => { controller.StartAdd(FCopParser.ActorBehavior.StationaryTurret); }),
            ("Interchanging Entity", () => { controller.StartAdd(FCopParser.ActorBehavior.InterchangingEntity); }),
            ("Aircraft", () => { controller.StartAdd(FCopParser.ActorBehavior.Aircraft); }),
            ("Pathed Turret", () => { controller.StartAdd(FCopParser.ActorBehavior.PathedTurret); }),
            ("Pathed Multi-Turret", () => { controller.StartAdd(FCopParser.ActorBehavior.PathedMultiTurret); }),
            ("Claimable Turret", () => { controller.StartAdd(FCopParser.ActorBehavior.ClaimableTurret); }),
            ("Weapon", () => { controller.StartAdd(FCopParser.ActorBehavior.Weapon); }),
            ("Floating Item", () => { controller.StartAdd(FCopParser.ActorBehavior.FloatingItem); }),
            ("Reloader", () => { controller.StartAdd(FCopParser.ActorBehavior.Reloader); }),
            ("Trigger", () => { controller.StartAdd(FCopParser.ActorBehavior.Trigger); }),

        };

    }

    public void PasteNavNodeCoords() {

        controller.PasteNavNodeCoords();

    }

    public void OpenActorSchematicsView() {

        if (activeActorSchematicView != null) {
            Destroy(activeActorSchematicView.gameObject);
            activeActorSchematicView = null;
        }
        else {
            var obj = Instantiate(actorSchematicViewFab);

            activeActorSchematicView = obj.GetComponent<ActorSchematicView>();
            activeActorSchematicView.controller = controller;
            activeActorSchematicView.view = this;

            obj.transform.SetParent(transform.parent, false);
        }

    }

    public void SaveActorSchematic() {

        if (controller.selectedActor != null) {
            Presets.actorSchematics.schematics.Add(new ActorSchematic(controller.selectedActor, controller.main.level));
            QuickLogHandler.Log("Actor Schematic Added", LogSeverity.Success);
        }
        else {
            QuickLogHandler.Log("No Actor Selected", LogSeverity.Info);
        }

    }

    public void OpenActorPropertiesView() {

        if (activeActorPropertiesView != null) {
            CloseActorPorpertiesView();
        }
        else {

            var obj = Instantiate(actorPropertiesView);

            activeActorPropertiesView = obj.GetComponent<ActorPropertiesView>();
            activeActorPropertiesView.controller = controller;

            obj.transform.SetParent(transform.parent, false);

        }

    }

    public void CloseActorSchematicsView() {

        if (activeActorSchematicView != null) {
            Destroy(activeActorSchematicView.gameObject);
            activeActorSchematicView = null;
        }

    }

    public void RefreshActorPropertiesView() {

        if (activeActorPropertiesView != null) {
            activeActorPropertiesView.Refresh();
        }

    }

    public void CloseActorPorpertiesView() {

        Destroy(activeActorPropertiesView.gameObject);
        activeActorPropertiesView = null;

    }

    public void OpenSupportingActorDataPanel() {

        if (activeSupportingActorDataView != null) {
            CloseSupportingActorDataPanel();
        }
        else {

            var obj = Instantiate(supportingActorDataViewFab);

            activeSupportingActorDataView = obj.GetComponent<SupportingActorDataView>();
            activeSupportingActorDataView.level = controller.main.level;
            activeSupportingActorDataView.main = controller.main;

            obj.transform.SetParent(transform.parent, false);

        }

    }

    public void CloseSupportingActorDataPanel() {

        if (activeSupportingActorDataView != null) {
            Destroy(activeSupportingActorDataView.gameObject);
            activeSupportingActorDataView = null;
        }

    }
}