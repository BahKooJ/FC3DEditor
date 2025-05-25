
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
    public int actorRPNSRefIndex;
    public FCopActor actor;
    public FCopLevel level;

    List<ScriptingButtonItemView> scriptButtons = new();

    void Start() {

        //assetManager.level = level;
        //assetManager.main = FindAnyObjectByType<Main>();

        Refresh();

    }

    void Refresh() {

        foreach (var item in scriptButtons) {
            Destroy(item.gameObject);
        }

        scriptButtons.Clear();

        if (isScriptTab) {

            foreach (var script in level.scripting.rpns.code) {

                var listItem = Instantiate(ScriptListItem);

                listItem.gameObject.SetActive(true);

                var listItemScript = listItem.GetComponent<ScriptingButtonItemView>();

                listItemScript.view = this;
                listItemScript.value = script.Key;

                listItem.transform.SetParent(scriptListContent, false);

                scriptButtons.Add(listItemScript);

            }

        }
        else {

            var i = 0;
            foreach (var script in level.scripting.functionParser.functions) {

                var listItem = Instantiate(ScriptListItem);

                listItem.gameObject.SetActive(true);

                var listItemScript = listItem.GetComponent<ScriptingButtonItemView>();

                listItemScript.view = this;
                listItemScript.value = i;

                listItem.transform.SetParent(scriptListContent, false);

                scriptButtons.Add(listItemScript);

                i++;
            }

        }

    }

    public void SelectScript(int id) {

        var script = level.scripting.rpns.code[id];

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

    public void OnDone() {
        Destroy(gameObject);
    }

}