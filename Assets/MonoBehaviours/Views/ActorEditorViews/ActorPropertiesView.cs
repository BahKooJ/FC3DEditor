

using TMPro;
using UnityEngine;

public class ActorPropertiesView : MonoBehaviour {

    //Prefabs
    public GameObject valueActorPropertyItem;

    //View refs
    public Transform propertiesContent;
    public TMP_Text idText;
    public TMP_Text actorTypeText;

    public ActorEditMode controller;

    void Start() {

        Refresh();

    }

    public void Refresh() {

        idText.text = controller.selectedActor.id.ToString();
        actorTypeText.text = controller.selectedActor.actorType.ToString();

    }


}