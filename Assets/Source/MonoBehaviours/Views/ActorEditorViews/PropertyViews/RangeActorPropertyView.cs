

using FCopParser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RangeActorPropertyView : ActorPropertyItemView {

    //View refs
    public TMP_Text nameText;
    public TMP_InputField valueField;
    public Slider slider;

    RangeActorProperty rangeProperty;

    bool refuseCallback = false;

    void Start() {

        refuseCallback = true;

        rangeProperty = (RangeActorProperty)property;

        slider.minValue = rangeProperty.min; 
        slider.maxValue = rangeProperty.max;

        Refresh();

    }

    public override void Refresh() {

        refuseCallback = true;

        nameText.text = property.name;
        valueField.text = rangeProperty.value.ToString();
        slider.value = rangeProperty.value;

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

            rangeProperty.Set(value);

            Refresh();

            if (ActorPropertyChangeEvent.changeEventsByPropertyName.ContainsKey(property.name)) {

                ActorPropertyChangeEvent.changeEventsByPropertyName[property.name](controller, property);

            }

        }
        catch {

            valueField.text = rangeProperty.value.ToString();

        }

    }

    public void OnSliderChange() {

        if (refuseCallback) return;

        ActorEditMode.AddPropertyChangeCounterAction(property, actor);

        if (Input.GetKey(KeyCode.LeftShift)) {
            slider.value -= slider.value % 5;
        }

        rangeProperty.Set(slider.value);

        Refresh();

        if (ActorPropertyChangeEvent.changeEventsByPropertyName.ContainsKey(property.name)) {

            ActorPropertyChangeEvent.changeEventsByPropertyName[property.name](controller, property);

        }

    }

}