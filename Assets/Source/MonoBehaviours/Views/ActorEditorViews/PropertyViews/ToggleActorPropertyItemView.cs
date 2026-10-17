


using FCopParser;
using TMPro;
using UnityEngine.UI;

public class ToggleActorPropertyItemView : ActorPropertyItemView {

    //View refs
    public TMP_Text nameText;
    public Toggle toggle;

    bool refuseCallback = false;

    void Start() {

        Refresh();

    }

    public override void Refresh() {

        refuseCallback = true;

        var toggleProperty = (ToggleActorProperty)property;

        nameText.text = toggleProperty.name;
        toggle.isOn = toggleProperty.value;

        refuseCallback = false;

    }

    public void OnToggle() {

        if (refuseCallback) return;

        ActorEditMode.AddPropertyChangeCounterAction(property, actor);

        var toggleProperty = (ToggleActorProperty)property;

        toggleProperty.value = toggle.isOn;

        if (ActorPropertyChangeEvent.changeEventsByPropertyName.ContainsKey(property.name)) {

            ActorPropertyChangeEvent.changeEventsByPropertyName[property.name](controller, property);

        }

    }

}