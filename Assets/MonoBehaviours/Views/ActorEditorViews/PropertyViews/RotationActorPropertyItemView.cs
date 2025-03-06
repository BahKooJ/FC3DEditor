

using FCopParser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RotationActorPropertyItemView : ActorPropertyItemView {

    //View Refs
    public TMP_Text valueText;
    public TMP_Text nameText;
    public Slider slider;

    bool refuseCallback = true;

    void Start() {

        var rotProperty = (RotationActorProperty)property;

        nameText.text = rotProperty.name;
        valueText.text = rotProperty.value.parsedRotation.ToString();
        slider.value = rotProperty.value.parsedRotation;

        refuseCallback = false;
    }

    public void OnSliderChange() {

        var rotProperty = (RotationActorProperty)property;

        if (!Main.counterActionAddedOnCurrentSelectHold) {
            ActorEditMode.AddActorPropertyCounterAction(property);
        }

        if (refuseCallback) return;

        if (Input.GetKey(KeyCode.LeftShift)) {
            slider.value -= slider.value % 5;
        }

        rotProperty.value.SetRotationDegree(slider.value);

        valueText.text = rotProperty.value.parsedRotation.ToString();

        controller.actorObjectsByID[actor.DataID].RefreshRotation();

    }

}