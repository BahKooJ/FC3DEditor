
using FCopParser;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ScriptingPanelView : MonoBehaviour {


    // - Unity Refs -
    public Transform scriptListContent;
    public VisualScriptingScriptWindowView scriptingWindow;
    public VisualScriptingFuncWindowView funcScriptingWindow;
    public AssetManagerView assetManager;

    // - Prefabs -
    public GameObject ScriptListItem;

    public bool isScriptTab = true;
    public int actorRPNSRefIndex;
    public FCopActor actor;
    public FCopLevel level;

    List<ScriptingButtonItemView> scriptButtons = new();

    void Start() {

        assetManager.level = level;
        assetManager.main = FindAnyObjectByType<Main>();

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

        try {
            if (script.failed) {
                //script.failed = false;
                script.Disassemble(script.compiledBytes);
                script.DeCompile();
            }
        }
        catch (Exception e) {

            var error = "";

            foreach (var b in script.compiledBytes) {
                error += b.ToString() + " ";
            }
            error += "\n";

            Debug.Log(error);
            throw e;

        }

        funcScriptingWindow.Clear();

        scriptingWindow.script = script;
        scriptingWindow.Init();

    }

    public void SelectFunc(FCopFunction func) {

        try {
            if (func.code.failed) {
                //script.failed = false;
                func.code.Disassemble(func.code.compiledBytes);
                func.code.DeCompile();
            }
        }
        catch (Exception e) {

            var error = "";

            foreach (var b in func.code.compiledBytes) {
                error += b.ToString() + " ";
            }
            error += "\n";

            Debug.Log(error);
            throw e;

        }

        scriptingWindow.Clear();

        funcScriptingWindow.func = func;
        funcScriptingWindow.Init();

    }

    public void OnClickScriptTab() {

        isScriptTab = true;
        Refresh();

    }

    public void OnClickFuncTab() {

        isScriptTab = false;
        Refresh();

    }

    public void OnDone() {
        Destroy(gameObject);
    }

    public void OnApply() {

    }

}