

using FCopParser;
using System;
using TMPro;

public class NormalizedActorPropertyView : ActorPropertyItemView {

    //View refs
    public TMP_Text nameText;
    public TMP_InputField valueField;

    NormalizedValueProperty normalizedProperty;

    void Start() {

        normalizedProperty = (NormalizedValueProperty)property;

        Refresh();

    }

    public override void Refresh() {

        nameText.text = property.name;
        valueField.text = normalizedProperty.value.ToString();

    }

    public void OnStartTyping() {

        Main.ignoreAllInputs = true;

    }

    public void OnFinishTyping() {

        Main.ignoreAllInputs = false;

        try {

            var value = float.Parse(valueField.text);

            normalizedProperty.Set(value);

            if (ActorPropertyChangeEvent.changeEventsByPropertyName.ContainsKey(property.name)) {

                ActorPropertyChangeEvent.changeEventsByPropertyName[property.name](controller, property);

            }

        }
        catch {

            valueField.text = normalizedProperty.value.ToString();

        }

    }

}