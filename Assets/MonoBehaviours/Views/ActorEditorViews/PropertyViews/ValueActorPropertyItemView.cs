﻿

using FCopParser;
using System;
using TMPro;

public class ValueActorPropertyItemView : ActorPropertyItemView {

    //View refs
    public TMP_Text nameText;
    public TMP_InputField valueField;

    ValueActorProperty valueActorProperty;

    void Start() {

        valueActorProperty = (ValueActorProperty)property;

        Refresh();

    }

    public override void Refresh() {

        nameText.text = property.name;
        valueField.text = property.GetCompiledValue().ToString();

    }

    public void OnStartTyping() {

        Main.ignoreAllInputs = true;

    }

    public void OnFinishTyping() {

        Main.ignoreAllInputs = false;

        ActorEditMode.AddActorPropertyCounterAction(property);

        try {

            var value = int.Parse(valueField.text);

            valueActorProperty.Set(value);

            valueField.text = property.GetCompiledValue().ToString();

            if (ActorPropertyChangeEvent.changeEventsByPropertyName.ContainsKey(property.name)) {

                ActorPropertyChangeEvent.changeEventsByPropertyName[property.name](controller, property);

            }

        }
        catch {

            valueField.text = valueActorProperty.value.ToString();

        }

    }

}