

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

    void Start() {

        nameText.text = property.name;
        valueText.text = property.value.parsedRotation.ToString();
        slider.value = property.value.parsedRotation;

    }

    public void OnSliderChange() {

        property.value.SetRotationDegree(slider.value);

        valueText.text = property.value.parsedRotation.ToString();

        controller.selectedActorObject.controlledObject.GetComponent<ActorObject>().RefreshRotation();

    }

}