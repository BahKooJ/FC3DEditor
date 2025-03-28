

using FCopParser;
using System;
using TMPro;

public class ValueActorPropertyItemView : ActorPropertyItemView {

    //View refs
    public TMP_Text nameText;
    public TMP_InputField valueField;

    void Start() {

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

        var value = Int32.Parse(valueField.text);

        var valueProperty = (ValueActorProperty)property;

        //if (valueProperty.bitCount != BitCount.Bit3) {

            //valueProperty.SafeSetSigned(value);

        //}
        //else {

            valueProperty.value = value;

        //}

        valueField.text = valueProperty.value.ToString();
        
        if (ActorPropertyChangeEvent.changeEventsByPropertyName.ContainsKey(property.name)) {

            ActorPropertyChangeEvent.changeEventsByPropertyName[property.name](controller);

        }

    }

}