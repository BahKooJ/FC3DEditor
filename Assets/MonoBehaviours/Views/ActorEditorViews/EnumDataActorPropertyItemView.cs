

using FCopParser;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnumDataActorPropertyItemView: MonoBehaviour {

    //View refs
    public TMP_Text nameText;
    public TMP_Dropdown caseDropDown;

    public EnumDataActorProperty property;
    public FCopActor actor;
    public ActorEditMode controller;

    void Start() {

        nameText.text = property.name;

        var cases = Enum.GetNames(property.caseValue.GetType());

        caseDropDown.ClearOptions();

        caseDropDown.AddOptions(new List<string>(cases));

        var value = 0;
        foreach (var c in cases) {

            if (Enum.GetName(property.caseValue.GetType(), property.caseValue) == c) {
                break;
            }

            value++;
        }

        caseDropDown.value = value;

    }

}