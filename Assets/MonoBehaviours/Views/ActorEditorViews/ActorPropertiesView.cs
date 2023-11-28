

using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ActorPropertiesView : MonoBehaviour {

    //Prefabs
    public GameObject valueActorPropertyItem;
    public GameObject rotationActorPropertyItem;
    public GameObject enumDataActorPropertyItem;
    public GameObject actorScriptCallItem;

    //View refs
    public Transform propertiesContent;
    public TMP_Text idText;
    public TMP_Text actorTypeText;

    public ActorEditMode controller;

    void Start() {

        Refresh();

    }

    public void Refresh() {

        foreach (Transform transform in propertiesContent.transform) {
            Destroy(transform.gameObject);
        }

        idText.text = controller.selectedActor.id.ToString();
        actorTypeText.text = controller.selectedActor.actorType.ToString();

        if (controller.selectedActor.behavior != null && controller.selectedActor.behavior.properties != null) {

            foreach (var property in controller.selectedActor.behavior.properties) {

                if (property is ValueActorProperty) {

                    var view = Instantiate(valueActorPropertyItem);

                    view.GetComponent<ValueActorPropertyItemView>().controller = controller;
                    view.GetComponent<ValueActorPropertyItemView>().property = (ValueActorProperty)property;
                    view.GetComponent<ValueActorPropertyItemView>().actor = controller.selectedActor;

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


        var actorScriptView = Instantiate(actorScriptCallItem);

        actorScriptView.GetComponent<ActorScriptCallItemView>().controller = controller;
        actorScriptView.GetComponent<ActorScriptCallItemView>().actor = controller.selectedActor;

        actorScriptView.transform.SetParent(propertiesContent, false);

    }


}