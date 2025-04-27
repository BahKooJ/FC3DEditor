

using FCopParser;
using System;
using TMPro;

public class NormalizedActorPropertyView : ActorPropertyItemView {

    //View refs
    public TMP_Text nameText;
    public TMP_InputField valueField;

    NormalizedValueProperty normalizedProperty;

    bool refuseCallback = false;

    void Start() {

        normalizedProperty = (NormalizedValueProperty)property;

        Refresh();

    }

    public override void Refresh() {

        refuseCallback = true;

        nameText.text = property.name;
        valueField.text = normalizedProperty.value.ToString();

        refuseCallback = false;

    }

    public void OnStartTyping() {

        Main.ignoreAllInputs = true;

    }

    public void OnFinishTyping() {

        Main.ignoreAllInputs = false;

        if (refuseCallback) return;

        ActorEditMode.AddPropertyChangeCounterAction(property, actor);

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