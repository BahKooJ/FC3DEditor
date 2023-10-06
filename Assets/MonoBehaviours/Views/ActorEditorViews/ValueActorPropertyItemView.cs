

using FCopParser;
using System;
using TMPro;
using UnityEngine;

public class ValueActorPropertyItemView : MonoBehaviour {

    //View refs
    public TMP_Text nameText;
    public TMP_InputField valueField;

    public ValueActorProperty property;
    public FCopActor actor;
    public ActorEditMode controller;

    void Start() {
        
        nameText.text = property.name;
        valueField.text = property.value.ToString();

    }

    public void OnFinishTyping() {

        var value = Int32.Parse(valueField.text);

        if (value > Int16.MaxValue) {
            value = Int16.MaxValue;
        } else if (value < Int32.MinValue) {
            value = Int32.MinValue;
        }

        property.value = value;
        valueField.text = property.value.ToString();

    }

}