

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

        foreach (var codeByOffset in controller.main.level.scripting.rpns.codeByOffset) {
            requestingScripts[codeByOffset.Key] = codeByOffset.Value.name;
        }

        MiniAssetManagerUtil.RequestUniversalData(requestingScripts, controller.main, id => {
            actor.rawFile.rpnsReferences[index] = id;
            Refresh();
        });

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

}