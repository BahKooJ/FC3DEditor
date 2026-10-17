
using FCopParser;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScriptingPanelView : MonoBehaviour {

    // - Unity Refs -
    public Transform scriptListContent;
    public VisualScriptingScriptWindowView scriptingWindow;
    public VariableManagerView variableManagerView;
    public FuncDataView funcDataView;

    // - Prefabs -
    public GameObject ScriptListItem;

    public bool isScriptTab = true;
    public FCopLevel level;

    List<ScriptingSelectorItemView> selectorItems = new();

    void Start() {

        //assetManager.level = level;
        //assetManager.main = FindAnyObjectByType<Main>();

        Refresh();
        Main.requiredCounterActionType = typeof(ScriptSaveStateCounterAction);

    }

    private void OnDestroy() {
        Main.GarbageCollectCounterActions(typeof(ScriptSaveStateCounterAction));
        Main.requiredCounterActionType = null;
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

            foreach (var func in level.scripting.functionParser.functions) {

                AddScriptSelector(func.code);

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

        scriptingWindow.script = script;
        scriptingWindow.Init();

        funcDataView.gameObject.SetActive(false);
        var scriptWindowPos = ((RectTransform)scriptingWindow.transform).offsetMax;
        scriptWindowPos.y = -31;
        ((RectTransform)scriptingWindow.transform).offsetMax = scriptWindowPos;

        Main.GarbageCollectCounterActions(typeof(ScriptSaveStateCounterAction));

    }

    public void SelectFunc(FCopScript script) {

        RefreshScriptSelection();

        scriptingWindow.Clear();

        scriptingWindow.script = script;
        scriptingWindow.Init();

        funcDataView.gameObject.SetActive(true);
        var scriptWindowPos = ((RectTransform)scriptingWindow.transform).offsetMax;
        scriptWindowPos.y = -61;
        ((RectTransform)scriptingWindow.transform).offsetMax = scriptWindowPos;

        funcDataView.function = level.scripting.functionParser.functions.First(fun => fun.code == script);
        funcDataView.Refresh();

        Main.GarbageCollectCounterActions(typeof(ScriptSaveStateCounterAction));

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

    public void ReOrderFunc(int indexOfDragged, int indexOfReceiver) {

        var draggedFunc = level.scripting.functionParser.functions[indexOfDragged];

        level.scripting.functionParser.functions.RemoveAt(indexOfDragged);

        if (indexOfReceiver > indexOfDragged) {
            level.scripting.functionParser.functions.Insert(indexOfReceiver - 1, draggedFunc);
        }
        else {
            level.scripting.functionParser.functions.Insert(indexOfReceiver, draggedFunc);
        }

    }

    #region Unity Callbacks

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

        scriptingWindow.gameObject.SetActive(variableManagerView.gameObject.activeSelf);
        variableManagerView.gameObject.SetActive(!variableManagerView.gameObject.activeSelf);

    }

    public void OnClickAddScript() {

        if (isScriptTab) {
            level.scripting.rpns.AddScript();

            var selector = AddScriptSelector(level.scripting.rpns.code[0]);

            selector.transform.SetSiblingIndex(0);
            selector.Rename();

        }
        else {

            level.scripting.functionParser.AddFunc();

            var selector = AddScriptSelector(level.scripting.functionParser.functions[0].code);

            selector.transform.SetSiblingIndex(0);
            selector.Rename();

        }

    }

    public void OnDone() {
        Destroy(gameObject);
    }

    #endregion
    #region Counter-Actions

    public class ScriptSaveStateCounterAction : CounterAction {

        public string name { get; set; }

        public FCopScript script;
        public List<ScriptNode> saveState;
        public VisualScriptingScriptWindowView view;

        public ScriptSaveStateCounterAction(FCopScript script, VisualScriptingScriptWindowView view) {
            this.script = script;
            this.saveState = script.CloneScripts();
            this.name = "Script Change";
            this.view = view;
        }

        public void Action() {
            script.code = saveState;
            view.Init();
        }

    }

    public void AddCounterAction() {
        Main.AddCounterAction(new ScriptSaveStateCounterAction(scriptingWindow.script, scriptingWindow));
    }

    #endregion

}