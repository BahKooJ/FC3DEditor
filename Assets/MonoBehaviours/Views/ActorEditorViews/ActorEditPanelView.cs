using UnityEngine;

public class ActorEditPanelView : MonoBehaviour {

    public ActorEditMode controller;

    //Prefabs
    public GameObject actorPropertiesView;

    // - Unity Refs -
    public ContextMenuHandler addActorContextMenu;

    public ActorPropertiesView activeActorPropertiesView;

    void Start() {

        controller.view = this;
        OpenActorPropertiesView();

        addActorContextMenu.items = new() {
            ("Dynamic Prop", () => { controller.StartAdd(FCopParser.ActorBehavior.DynamicProp); })
        };

    }

    public void PasteNavNodeCoords() {

        controller.PasteNavNodeCoords();

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

    public void RefreshActorPropertiesView() {

        if (activeActorPropertiesView != null) {
            activeActorPropertiesView.Refresh();
        }

    }

    public void CloseActorPorpertiesView() {

        Destroy(activeActorPropertiesView.gameObject);
        activeActorPropertiesView = null;

    }

}