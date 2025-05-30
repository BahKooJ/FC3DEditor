

using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class VariableManagerView : MonoBehaviour {

    // - Prefabs -
    public GameObject variableItem;

    // - Unity Refs -
    public Transform listContent;
    public Transform userHeader;
    public Transform globalHeader;
    public Transform systemHeader;
    public Transform timerHeader;

    List<VariableItemView> items = new();

    VariableItemView InitVarItem(ScriptVariable scriptVariable) {

        var obj = Instantiate(variableItem, listContent, false);
        obj.SetActive(true);
        var item = obj.GetComponent<VariableItemView>();
        item.scriptVariable = scriptVariable;

        items.Add(item);
        return item;

    }

    private void Start() {

        foreach (var userVar in FCopScriptingProject.userVariables) {

            var varView = InitVarItem(userVar.Value);
            varView.transform.SetSiblingIndex(globalHeader.transform.GetSiblingIndex());
            varView.canChangeType = true;
            varView.canChangeName = true;
            varView.canChangeDescription = true;

        }

        foreach (var gloVar in FCopScriptingProject.globalVariables) {

            var varView = InitVarItem(gloVar.Value);
            varView.transform.SetSiblingIndex(systemHeader.transform.GetSiblingIndex());
            varView.canChangeType = false;
            varView.canChangeName = false;
            varView.canChangeDescription = false;

        }

        foreach (var sysVar in FCopScriptingProject.systemVariables) {

            var varView = InitVarItem(sysVar.Value);
            varView.transform.SetSiblingIndex(timerHeader.transform.GetSiblingIndex());
            varView.canChangeType = false;
            varView.canChangeName = false;
            varView.canChangeDescription = false;

        }

        foreach (var timVar in FCopScriptingProject.timerVariables) {

            var varView = InitVarItem(timVar.Value);
            varView.canChangeType = false;
            varView.canChangeName = true;
            varView.canChangeDescription = true;
        }

    }

    public void AddUserVar() {

        var varId = FCopScriptingProject.AddUserVariable();

        if (varId == -1) {
            QuickLogHandler.Log("Maximum user variables added", LogSeverity.Error); 
            return;
        }

        var varView = InitVarItem(FCopScriptingProject.userVariables[varId]);
        varView.transform.SetSiblingIndex(2);
        varView.canChangeType = true;
        varView.canChangeName = true;
        varView.canChangeDescription = true;

    }

}