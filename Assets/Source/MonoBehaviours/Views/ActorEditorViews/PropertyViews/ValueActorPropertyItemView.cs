

using FCopParser;
using TMPro;

public class ValueActorPropertyItemView : ActorPropertyItemView {

    //View refs
    public TMP_Text nameText;
    public TMP_InputField valueField;

    ValueActorProperty valueActorProperty;

    bool refuseCallback = false;

    void Start() {

        valueActorProperty = (ValueActorProperty)property;

        Refresh();

    }

    public override void Refresh() {

        refuseCallback = true;

        nameText.text = property.name;
        valueField.text = property.GetCompiledValue().ToString();

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