

using System;
using UnityEngine;
using FCopParser;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class VariableSelectorView : MonoBehaviour {

    // - Prefabs -
    public GameObject variableListItem;

    // - Unity Refs -
    public Transform fileContent;
    public TMP_InputField searchBar;
    public Image globalTab;
    public Image systemTab;
    public Image timerTab;
    public Image userTab;

    // - Parameters -
    public Action<int, ScriptVariableType> onDataSelected = (arg0, arg1) => { };
    [HideInInspector]
    public List<ScriptVariableType> allowedVars;

    List<VariableSelectorItemView> items = new();

    ScriptVariableType tabSelected = ScriptVariableType.Global;

    private void Start() {
        Refresh();
    }

    public void Refresh() {

        void InitListItem(ScriptVariableType type, int id, string name) {
            var obj = Instantiate(variableListItem, fileContent.transform, false);
            obj.SetActive(true);
            var item = obj.GetComponent<VariableSelectorItemView>();
            item.view = this;
            item.id = id;
            item.type = type;
            item.varName = name;
            items.Add(item);
        }

        foreach (var item in items) {
            Destroy(item.gameObject);
        }

        items.Clear();

        globalTab.gameObject.SetActive(allowedVars.Contains(ScriptVariableType.Global));
        systemTab.gameObject.SetActive(allowedVars.Contains(ScriptVariableType.System));
        timerTab.gameObject.SetActive(allowedVars.Contains(ScriptVariableType.Timer));
        userTab.gameObject.SetActive(allowedVars.Contains(ScriptVariableType.User));

        globalTab.color = Main.mainColor;
        systemTab.color = Main.mainColor;
        timerTab.color = Main.mainColor;
        userTab.color = Main.mainColor;

        switch (tabSelected) {
            case ScriptVariableType.Global:
                globalTab.color = Main.selectedColor;
                foreach (var pair in FCopScriptingProject.globalVariables) {
                    InitListItem(tabSelected, pair.Key, pair.Value.name);
                }
                break;
            case ScriptVariableType.System:
                systemTab.color = Main.selectedColor;
                foreach (var pair in FCopScriptingProject.systemVariables) {
                    InitListItem(tabSelected, pair.Key, pair.Value.name);
                }
                break;
            case ScriptVariableType.Timer:
                timerTab.color = Main.selectedColor;
                foreach (var pair in FCopScriptingProject.timerVariables) {
                    InitListItem(tabSelected, pair.Key, pair.Value.name);
                }
                break;
            case ScriptVariableType.User:
                userTab.color = Main.selectedColor;
                foreach (var pair in FCopScriptingProject.userVariables) {
                    InitListItem(tabSelected, pair.Key, pair.Value.name);
                }
                break;
        }

    }

    public void OnSelectItem(VariableSelectorItemView item) {

        onDataSelected(item.id, item.type);
        Destroy(this.gameObject);

    }

    public void OnClickGlobal() {
        tabSelected = ScriptVariableType.Global;
        Refresh();
    }

    public void OnClickSystem() {
        tabSelected = ScriptVariableType.System;
        Refresh();
    }

    public void OnClickTimer() {
        tabSelected = ScriptVariableType.Timer;
        Refresh();
    }

    public void OnClickUser() {
        tabSelected = ScriptVariableType.User;
        Refresh();
    }

    public void StartType() {
        Main.ignoreAllInputs = true;
    }

    public void StopType() {
        Main.ignoreAllInputs = false;
    }

    public void OnTypeInSearch() {

        foreach (var item in items) {

            if (item.varName.Contains(searchBar.text) || searchBar.text == "") {
                item.gameObject.SetActive(true);
            }
            else {
                item.gameObject.SetActive(false);
            }

        }

    }

}