

using FCopParser;
using System;
using System.Collections.Generic;
using TMPro;

public class EnumDataActorPropertyItemView: ActorPropertyItemView {

    //View refs
    public TMP_Text nameText;
    public TMP_Dropdown caseDropDown;

    string[] cases;
    Array values;

    void Start() {

        var enumProperty = (EnumDataActorProperty)property;

        nameText.text = property.name;

        cases = Enum.GetNames(enumProperty.caseValue.GetType());
        values = Enum.GetValues(enumProperty.caseValue.GetType());

        caseDropDown.ClearOptions();

        var value = 0;
        foreach (var c in cases) {

            caseDropDown.AddOptions(new List<string>() { Utils.AddSpacesToString(c) });

            if (Enum.GetName(enumProperty.caseValue.GetType(), enumProperty.caseValue) == c) {
                caseDropDown.value = value;
            }

            value++;
        }


    }

    public void OnChange() {

        var enumProperty = (EnumDataActorProperty)property;

        enumProperty.caseValue = (Enum)values.GetValue(caseDropDown.value);

        if (ActorPropertyChangeEvent.changeEventsByPropertyName.ContainsKey(property.name)) {

            ActorPropertyChangeEvent.changeEventsByPropertyName[property.name](controller, property);

        }
        if (enumProperty.dictatesOverload != null) {

            foreach (var name in enumProperty.dictatesOverload) {

                controller.view.activeActorPropertiesView.RequestPropertyRefresh(actor.behavior.propertiesByName[name]);

            }

        }

    }

}