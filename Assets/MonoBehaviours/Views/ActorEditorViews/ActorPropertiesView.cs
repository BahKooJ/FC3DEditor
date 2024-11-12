

using FCopParser;
using TMPro;
using UnityEngine;

public class ActorPropertiesView : MonoBehaviour {

    //Prefabs
    public GameObject valueActorPropertyItem;
    public GameObject toggleActorPropertyItem;
    public GameObject rotationActorPropertyItem;
    public GameObject enumDataActorPropertyItem;
    public GameObject actorScriptCallItem;
    public GameObject actorAssetRefItem;

    //View refs
    public Transform propertiesContent;
    public TMP_Text actorName;
    public TMP_Text idText;
    public TMP_Text actorTypeText;
    public SceneActorsView sceneActorsView;

    public ActorEditMode controller;

    void Start() {

        sceneActorsView.level = controller.main.level;
        sceneActorsView.controller = controller;

        Refresh();

    }

    public void Refresh() {

        foreach (Transform transform in propertiesContent.transform) {
            Destroy(transform.gameObject);
        }

        if (controller.selectedActor == null) {
            actorName.text = "N/A";
            idText.text = "N/A";
            actorTypeText.text = "N/A";
            return;
        }

        actorName.text = controller.selectedActor.name;
        idText.text = controller.selectedActor.DataID.ToString();
        actorTypeText.text = controller.selectedActor.behaviorType.ToString();

        if (controller.selectedActor.behavior != null && controller.selectedActor.behavior.properties != null) {

            foreach (var property in controller.selectedActor.behavior.properties) {

                if (property is ValueActorProperty) {

                    var view = Instantiate(valueActorPropertyItem);

                    view.GetComponent<ValueActorPropertyItemView>().controller = controller;
                    view.GetComponent<ValueActorPropertyItemView>().property = (ValueActorProperty)property;
                    view.GetComponent<ValueActorPropertyItemView>().actor = controller.selectedActor;

                    view.transform.SetParent(propertiesContent, false);

                }
                else if (property is ToggleActorProperty) {

                    var view = Instantiate(toggleActorPropertyItem);

                    view.GetComponent<ToggleActorPropertyItemView>().controller = controller;
                    view.GetComponent<ToggleActorPropertyItemView>().property = (ToggleActorProperty)property;
                    view.GetComponent<ToggleActorPropertyItemView>().actor = controller.selectedActor;

                    view.transform.SetParent(propertiesContent, false);

                }
                else if (property is RotationActorProperty) {

                    var view = Instantiate(rotationActorPropertyItem);

                    view.GetComponent<RotationActorPropertyItemView>().controller = controller;
                    view.GetComponent<RotationActorPropertyItemView>().property = (RotationActorProperty)property;
                    view.GetComponent<RotationActorPropertyItemView>().actor = controller.selectedActor;

                    view.transform.SetParent(propertiesContent, false);

                }
                else if (property is EnumDataActorProperty) {

                    var view = Instantiate(enumDataActorPropertyItem);

                    view.GetComponent<EnumDataActorPropertyItemView>().controller = controller;
                    view.GetComponent<EnumDataActorPropertyItemView>().property = (EnumDataActorProperty)property;
                    view.GetComponent<EnumDataActorPropertyItemView>().actor = controller.selectedActor;

                    view.transform.SetParent(propertiesContent, false);

                }

            }

        }


        var actorAssetRefView = Instantiate(actorAssetRefItem);

        actorAssetRefView.GetComponent<ActorAssetReferencesView>().controller = controller;
        actorAssetRefView.GetComponent<ActorAssetReferencesView>().actor = controller.selectedActor;

        actorAssetRefView.transform.SetParent(propertiesContent, false);

        var actorScriptView = Instantiate(actorScriptCallItem);

        actorScriptView.GetComponent<ActorScriptCallItemView>().controller = controller;
        actorScriptView.GetComponent<ActorScriptCallItemView>().actor = controller.selectedActor;

        actorScriptView.transform.SetParent(propertiesContent, false);

    }

    public void RefreshName() {

        actorName.text = controller.selectedActor.name;

    }

}