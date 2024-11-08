


using FCopParser;
using System;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class ToggleActorPropertyItemView : MonoBehaviour {

    //View refs
    public TMP_Text nameText;
    public Toggle toggle;

    public ToggleActorProperty property;
    public FCopActor actor;
    public ActorEditMode controller;

    void Start() {

        nameText.text = property.name;
        toggle.isOn = property.value;

    }

    public void OnToggle() {

        ActorEditMode.AddActorPropertyCounterAction(property);

        property.value = toggle.isOn;

    }

}