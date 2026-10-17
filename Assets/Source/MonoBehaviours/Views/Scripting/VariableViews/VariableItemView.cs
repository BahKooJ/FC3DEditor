

using UnityEngine;
using TMPro;
using FCopParser;
using System;

public class VariableItemView : MonoBehaviour {

    // - Unity Refs -
    public TMP_InputField nameText;
    public TMP_Dropdown typeDropdown;
    public TMP_InputField descriptionText;

    // - Parameters -
    [HideInInspector]
    public ScriptVariable scriptVariable;
    [HideInInspector]
    public bool canChangeType = false;
    [HideInInspector]
    public bool canChangeName = false;
    [HideInInspector]
    public bool canChangeDescription = false;

    bool refusedCallback = false;

    void Start() {

        refusedCallback = true;

        var cases = Enum.GetNames(typeof(ScriptDataType));

        nameText.text = scriptVariable.name;
        descriptionText.text = scriptVariable.description;
        var value = 0;
        foreach (var c in typeDropdown.options) {

            if (Enum.GetName(typeof(ScriptDataType), scriptVariable.dataType) == c.text) {
                typeDropdown.value = value;
            }

            value++;
        }

        if (!canChangeType) {
            typeDropdown.interactable = false;
        }
        if (!canChangeName) {
            nameText.interactable = false;
        }
        if (!canChangeDescription) { 
            descriptionText.interactable = false; 
        }

        refusedCallback = false;

    }

    public void StartType() {
        Main.ignoreAllInputs = true;
    }

    public void StopType() {
        Main.ignoreAllInputs = false;
    }

    public void OnChangeType() {

        if (refusedCallback) return;

        Enum.TryParse(typeof(ScriptDataType), typeDropdown.options[typeDropdown.value].text, out var type);
        
        scriptVariable.dataType = (ScriptDataType)type;

        switch (scriptVariable.varibleType) {
            case ScriptVariableType.Global:
                FCopScriptingProject.globalVariables[scriptVariable.id] = scriptVariable;
                break;
            case ScriptVariableType.System:
                FCopScriptingProject.systemVariables[scriptVariable.id] = scriptVariable;
                break;
            case ScriptVariableType.Timer:
                FCopScriptingProject.timerVariables[scriptVariable.id] = scriptVariable;
                break;
            case ScriptVariableType.User:
                FCopScriptingProject.userVariables[scriptVariable.id] = scriptVariable;
                break;
        }

    }

    public void OnFinishNameType() {

        if (refusedCallback) return;

        scriptVariable.name = nameText.text;

        switch (scriptVariable.varibleType) {
            case ScriptVariableType.Global:
                FCopScriptingProject.globalVariables[scriptVariable.id] = scriptVariable;
                break;
            case ScriptVariableType.System:
                FCopScriptingProject.systemVariables[scriptVariable.id] = scriptVariable;
                break;
            case ScriptVariableType.Timer:
                FCopScriptingProject.timerVariables[scriptVariable.id] = scriptVariable;
                break;
            case ScriptVariableType.User:
                FCopScriptingProject.userVariables[scriptVariable.id] = scriptVariable;
                break;
        }

    }

    public void OnFinishDescriptionType() {

        if (refusedCallback) return;

        scriptVariable.description = descriptionText.text;

        switch (scriptVariable.varibleType) {
            case ScriptVariableType.Global:
                FCopScriptingProject.globalVariables[scriptVariable.id] = scriptVariable;
                break;
            case ScriptVariableType.System:
                FCopScriptingProject.systemVariables[scriptVariable.id] = scriptVariable;
                break;
            case ScriptVariableType.Timer:
                FCopScriptingProject.timerVariables[scriptVariable.id] = scriptVariable;
                break;
            case ScriptVariableType.User:
                FCopScriptingProject.userVariables[scriptVariable.id] = scriptVariable;
                break;
        }

    }


}