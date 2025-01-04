

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

    public void OnFinishTyping() {

        ActorEditMode.AddActorPropertyCounterAction(property);

        var value = Int32.Parse(valueField.text);

        var valueProperty = (ValueActorProperty)property;

        valueProperty.SafeSetSigned(value);
        valueField.text = valueProperty.value.ToString();
        
        if (ActorPropertyChangeEvent.changeEventsByPropertyName.ContainsKey(property.name)) {

            ActorPropertyChangeEvent.changeEventsByPropertyName[property.name](controller);

        }

    }

}