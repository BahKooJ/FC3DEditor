using UnityEngine;

public class ActorEditPanelView : MonoBehaviour {

    public ActorEditMode controller;

    //Prefabs
    public GameObject actorPropertiesView;

    public ActorPropertiesView activeActorPropertiesView;

    void Start() {

        controller.view = this;
        OpenActorPropertiesView();

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
            activeActorPropertiesView.GetComponent<ActorPropertiesView>().Refresh();
        }

    }

    public void CloseActorPorpertiesView() {

        Destroy(activeActorPropertiesView);
        activeActorPropertiesView = null;

    }

}