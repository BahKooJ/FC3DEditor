


using FCopParser;
using System;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class ToggleActorPropertyItemView : ActorPropertyItemView {

    //View refs
    public TMP_Text nameText;
    public Toggle toggle;

    void Start() {

        var toggleProperty = (ToggleActorProperty)property;

        nameText.text = toggleProperty.name;
        toggle.isOn = toggleProperty.value;

    }

    public void OnToggle() {

        var toggleProperty = (ToggleActorProperty)property;


        ActorEditMode.AddActorPropertyCounterAction(property);

        toggleProperty.value = toggle.isOn;

    }

}