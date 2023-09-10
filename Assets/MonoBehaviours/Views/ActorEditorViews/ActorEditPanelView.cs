using UnityEngine;

public class ActorEditPanelView : MonoBehaviour {

    public ActorEditMode controller;

    //Prefabs
    public GameObject actorPropertiesView;

    public GameObject activeActorPropertiesView = null;

    void Start() {

        controller.view = this;

    }

    public void PasteNavNodeCoords() {

        controller.PasteNavNodeCoords();

    }

    public void OpenActorPropertiesView() {

        if (activeActorPropertiesView != null) {
            CloseActorPorpertiesView();
        }
        else {

            activeActorPropertiesView = Instantiate(actorPropertiesView);

            activeActorPropertiesView.GetComponent<ActorPropertiesView>().controller = controller;

            activeActorPropertiesView.transform.SetParent(transform.parent, false);

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