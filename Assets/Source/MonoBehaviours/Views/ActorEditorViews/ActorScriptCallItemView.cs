

using FCopParser;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActorScriptCallItemView : MonoBehaviour {

    // - Unity Refs -
    public List<TMP_Text> callbackNames;
    public List<TMP_Text> scriptNames;


    // - Parameters -
    public ActorEditMode controller;
    public FCopActor actor;

    void Start() {

        Refresh();

    }

    void Refresh() {

        for (int i = 0; i < 3; i++) {

            var rpnsRef = actor.rawFile.rpnsReferences[i];

            var script = controller.main.level.scripting.rpns.codeByOffset.GetValueOrDefault(rpnsRef);

            if (script == null) {
                scriptNames[i].text = "Missing";
            }
            else {
                scriptNames[i].text = script.name;
            }

            callbackNames[i].text = actor.behavior.callbackNames[i];

        }

    }

    void SelectScript(int index) {

        var requestingScripts = new Dictionary<int, string>();

        foreach (var code in controller.main.level.scripting.rpns.code) {
            requestingScripts[code.offset] = code.name;
        }

        MiniAssetManagerUtil.RequestUniversalData(requestingScripts, controller.main, id => {
            actor.rawFile.rpnsReferences[index] = id;
            Refresh();
        });

    }

    void EditScript(int index) {

        if (actor.rawFile.rpnsReferences[index] == controller.main.level.scripting.emptyOffset) {
            return;
        }

        var existingScriptingPanel = FindAnyObjectByType<ScriptingPanelView>();

        if (existingScriptingPanel != null) {
            Destroy(existingScriptingPanel.gameObject);
        }

        var obj = Instantiate(controller.main.scriptingPanelPrefab, controller.main.canvas.transform, false);

        var scriptingPanel = obj.GetComponent<ScriptingPanelView>();
        scriptingPanel.level = controller.main.level;
        scriptingPanel.SelectScript(actor.rawFile.rpnsReferences[index]);

    }

    public void OnClickRPNSRef1() {
        SelectScript(0);
    }

    public void OnClickRPNSRef2() {
        SelectScript(1);

    }

    public void OnClickRPNSRef3() {
        SelectScript(2);
    }

    public void OnClickEditRPNSRef1() {
        EditScript(0);
    }

    public void OnClickEditRPNSRef2() {
        EditScript(1);
    }

    public void OnClickEditRPNSRef3() {
        EditScript(2);
    }

}