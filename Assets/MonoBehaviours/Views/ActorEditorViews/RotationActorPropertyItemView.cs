

using FCopParser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RotationActorPropertyItemView : MonoBehaviour {

    //View Refs
    public TMP_Text valueText;
    public TMP_Text nameText;
    public Slider slider;

    public RotationActorProperty property;
    public FCopActor actor;
    public ActorEditMode controller;

    bool refuseCallback = true;

    void Start() {

        nameText.text = property.name;
        valueText.text = property.value.parsedRotation.ToString();
        slider.value = property.value.parsedRotation;

        refuseCallback = false;
    }

    public void OnSliderChange() {

        if (!Main.counterActionAddedOnCurrentSelectHold) {
            ActorEditMode.AddActorPropertyCounterAction(property);
        }

        if (refuseCallback) return;

        if (Input.GetKey(KeyCode.LeftShift)) {
            slider.value -= slider.value % 5;
        }

        property.value.SetRotationDegree(slider.value);

        valueText.text = property.value.parsedRotation.ToString();

        controller.actorObjectsByID[actor.DataID].RefreshRotation();

    }

}