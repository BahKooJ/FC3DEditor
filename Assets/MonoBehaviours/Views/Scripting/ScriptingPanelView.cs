
using FCopParser;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ScriptingPanelView : MonoBehaviour {

    // - Unity Refs -
    public Transform scriptListContent;
    public VisualScriptingScriptWindowView scriptingWindow;
    public VisualScriptingFuncWindowView funcScriptingWindow;
    public VariableManagerView variableManagerView;
    //public AssetManagerView assetManager;

    // - Prefabs -
    public GameObject ScriptListItem;

    public bool isScriptTab = true;
    public FCopLevel level;

    List<ScriptingSelectorItemView> selectorItems = new();

    void Start() {

        //assetManager.level = level;
        //assetManager.main = FindAnyObjectByType<Main>();

        Refresh();

    }

    ScriptingSelectorItemView AddScriptSelector(FCopScript script) {

        var listItem = Instantiate(ScriptListItem, scriptListContent, false);
        listItem.SetActive(true);

        var listItemScript = listItem.GetComponent<ScriptingSelectorItemView>();

        listItemScript.view = this;
        listItemScript.id = script.offset;
        listItemScript.script = script;

        selectorItems.Add(listItemScript);
        return listItemScript;

    }

    public void Refresh() {

        foreach (var item in selectorItems) {
            Destroy(item.gameObject);
        }

        selectorItems.Clear();

        if (isScriptTab) {

            foreach (var script in level.scripting.rpns.code) {

                if (script.offset == level.scripting.emptyOffset) {
                    continue;
                }

                AddScriptSelector(script);

            }

        }
        else {

            var i = 0;
            foreach (var script in level.scripting.functionParser.functions) {

                var listItem = Instantiate(ScriptListItem);

                listItem.gameObject.SetActive(true);

                var listItemScript = listItem.GetComponent<ScriptingSelectorItemView>();

                listItemScript.view = this;
                listItemScript.id = i;

                listItem.transform.SetParent(scriptListContent, false);

                selectorItems.Add(listItemScript);

                i++;
            }

        }

    }

    public void RefreshScriptSelection() {

        foreach (var item in selectorItems) {
            item.Unselect();
        }

    }

    public void SelectScript(int id) {

        RefreshScriptSelection();

        var script = level.scripting.rpns.codeByOffset[id];

        //funcScriptingWindow.Clear();

        scriptingWindow.script = script;
        scriptingWindow.Init();

    }

    public void SelectFunc(FCopFunction func) {

        scriptingWindow.Clear();

        scriptingWindow.script = func.code;
        scriptingWindow.script.code.AddRange(func.runCondition.code);
        scriptingWindow.Init();

    }

    public void ReOrderScript(int indexOfDragged, int indexOfReceiver) {

        var draggedScript = level.scripting.rpns.code[indexOfDragged];

        level.scripting.rpns.code.RemoveAt(indexOfDragged);

        if (indexOfReceiver > indexOfDragged) {
            level.scripting.rpns.code.Insert(indexOfReceiver - 1, draggedScript);
        }
        else {
            level.scripting.rpns.code.Insert(indexOfReceiver, draggedScript);
        }

    }

    public void OnClickScriptTab() {

        scriptingWindow.gameObject.SetActive(true);
        variableManagerView.gameObject.SetActive(false);

        isScriptTab = true;
        Refresh();

    }

    public void OnClickFuncTab() {

        scriptingWindow.gameObject.SetActive(true);
        variableManagerView.gameObject.SetActive(false);

        isScriptTab = false;
        Refresh();

    }

    public void OnClickVariableTab() {

        scriptingWindow.gameObject.SetActive(false);
        variableManagerView.gameObject.SetActive(true);

    }

    public void OnClickAddScript() {

        if (isScriptTab) {
            level.scripting.rpns.AddScript();

            var selector = AddScriptSelector(level.scripting.rpns.code[0]);

            selector.transform.SetSiblingIndex(0);
            selector.Rename();

        }

    }

    public void OnDone() {
        Destroy(gameObject);
    }

}