

using FCopParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileEffectChannelView : MonoBehaviour {

    public static OrderedDictionary dropdownValues = new() {
        { TileEffectType.Normal , "Normal" },
        { TileEffectType.Liquid , "Liquid" },
        { TileEffectType.InstantKill , "Instant Death" },
        { TileEffectType.Slipper0 , "Slippery 0" },
        { TileEffectType.Slipper1 , "Slippery 1" },
        { TileEffectType.Slipper2 , "Slippery 2" },
        { TileEffectType.Damage_Both_Medium , "Damage Both: Medium" },
        { TileEffectType.Damage_Both_High , "Damage Both: High" },
        { TileEffectType.Damage_Walker_Medium_Hover_Low , "Damage Walker: Medium, Hover: Low" },
        { TileEffectType.Damage_Walker_High_Hover_Medium , "Damage Walker: High, Hover: Medium" },
        { TileEffectType.Damage_Walker_Instant_Hover_Low , "Damage Walker: Instant, Hover: Low" },
        { TileEffectType.Damage_Walker_Instant_Hover_Medium , "Damage Walker: Instant, Hover: Medium" },
        { TileEffectType.Damage_Walker_Low_Hover_None , "Damage Walker: Low, Hover: None" },
        { TileEffectType.Damage_Walker_Medium_Hover_None , "Damage Walker: Medium, Hover: None" },
        { TileEffectType.Damage_Red , "Damage Red" },
        { TileEffectType.Damage_Blue , "Damage Blue" },
        { TileEffectType.Move_PosX_Medium , "Move Right Medium" },
        { TileEffectType.Move_PosX_High , "Move Right High" },
        { TileEffectType.Move_NegX_Medium , "Move Left Medium" },
        { TileEffectType.Move_NegX_High , "Move Left High" },
        { TileEffectType.Move_PosY_Medium , "Move Back Medium" },
        { TileEffectType.Move_PosY_High , "Move Back High" },
        { TileEffectType.Move_NegY_Medium , "Move Forward Medium" },
        { TileEffectType.Move_NegY_High , "Move Forward High" },
        { TileEffectType.Move_PosX_Low , "Move Right Low" },
        { TileEffectType.Move_NegX_Low , "Move Left Low" },
        { TileEffectType.Move_PosY_Low , "Move Back Low" },
        { TileEffectType.Move_NegY_Low , "Move Forward Low" },
        { TileEffectType.Dupe_Move_PosX_Low , "(Dupe) Move Right Low" },
        { TileEffectType.Dupe_Move_NegX_Low , "(Dupe) Move Left Low" },
        { TileEffectType.Dupe_Move_PosY_Low , "(Dupe) Move Back Low" },
        { TileEffectType.Dupe_Move_NegY_Low , "(Dupe) Move Forward Low" },
        { TileEffectType.No_Collision , "No Collision" },
        { TileEffectType.Other , "Other" },


    };

    // - View Refs -
    public Toggle toggle;
    public TMP_Dropdown dropdown;
    public int channel;

    // - Pars -
    public TileEffectsView parentView;

    void Start() {



    }

    public void CreateDropdown() {

        dropdown.ClearOptions();

        foreach (DictionaryEntry effect in dropdownValues) {
            dropdown.options.Add(new TMP_Dropdown.OptionData((string)effect.Value));
        }

    }

    public void Check() {

        toggle.isOn = true;

    }

    public void UnCheck() {

        toggle.isOn = false;

    }

    public void SetDropdown(int rawCase) {

        var doesCaseExist = Enum.IsDefined(typeof(TileEffectType), rawCase);

        if (doesCaseExist) {

            var effectCase = (TileEffectType)rawCase;

            dropdown.value = DropdownIndexOf(effectCase);

        } else {

            dropdown.value = DropdownIndexOf(TileEffectType.Other);

        }

    }

    public void OnToggle() {

        if (parentView.refuseCallbacks) {
            return;
        }

        parentView.controller.ChangeTileEffectIndex(channel);

        parentView.Refresh();

    }

    public void OnChangeDropdown() {

        if (parentView.refuseCallbacks) {
            return;
        }

        parentView.controller.ChaneTileEffect(channel, GetDropdownEffectValue(dropdown.value));

    }

    int DropdownIndexOf(TileEffectType type) {

        var i = 0;
        foreach (DictionaryEntry dropdownValue in dropdownValues) {

            if ((TileEffectType)(dropdownValue.Key) == type) {
                return i;
            }

            i++;

        }
        return -1;

    }

    TileEffectType GetDropdownType(int index) {

        var array = new TileEffectType[dropdownValues.Count];

        dropdownValues.Keys.CopyTo(array, 0);

        return array[index];

    }

    int GetDropdownEffectValue(int index) {

        return (int)GetDropdownType(index);

    }

}