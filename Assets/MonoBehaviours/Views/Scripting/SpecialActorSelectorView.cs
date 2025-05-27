
using System;
using UnityEngine;
using FCopParser;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpecialActorSelectorView : MonoBehaviour {

    // - Prefabs -
    public GameObject actorRefListItem;

    // - Unity Refs -
    public Transform fileContent;
    public TMP_InputField searchBar;
    public Image actorTab;
    public Image groupTab;
    public Image teamTab;

    // - Parameters -
    public Action<int, ScriptDataType> onDataSelected = (arg0, arg1) => { };
    [HideInInspector]
    public List<ScriptDataType> allowedActorRefs;

    List<SpecialActorSelectorItemView> items = new();

    ScriptDataType tabSelected = ScriptDataType.Actor;

    private void Start() {
        Refresh();
    }

    public void Refresh() {

        var main = FindAnyObjectByType<Main>();

        void InitListItem(ScriptDataType type, int id, string name) {
            var obj = Instantiate(actorRefListItem, fileContent.transform, false);
            obj.SetActive(true);
            var item = obj.GetComponent<SpecialActorSelectorItemView>();
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

        actorTab.gameObject.SetActive(allowedActorRefs.Contains(ScriptDataType.Actor));
        groupTab.gameObject.SetActive(allowedActorRefs.Contains(ScriptDataType.Group));
        teamTab.gameObject.SetActive(allowedActorRefs.Contains(ScriptDataType.Team));

        actorTab.color = Main.mainColor;
        groupTab.color = Main.mainColor;
        teamTab.color = Main.mainColor;

        switch (tabSelected) {
            case ScriptDataType.Actor:
                actorTab.color = Main.selectedColor;
                foreach (var actor in main.level.sceneActors.actors) {
                    InitListItem(tabSelected, actor.DataID, actor.name);
                }
                break;
            case ScriptDataType.Group:
                groupTab.color = Main.selectedColor;
                foreach (var group in main.level.sceneActors.scriptingGroupedActors) {
                    InitListItem(tabSelected, group.Key, group.Value.name);
                }
                break;
            case ScriptDataType.Team:
                teamTab.color = Main.selectedColor;
                foreach (var team in main.level.sceneActors.teams) {
                    InitListItem(tabSelected, team.Key, team.Value);
                }
                break;
        }

    }

    public void OnSelectItem(SpecialActorSelectorItemView item) {

        onDataSelected(item.id, item.type);
        Destroy(this.gameObject);

    }

    public void OnClickActor() {
        tabSelected = ScriptDataType.Actor;
        Refresh();
    }

    public void OnClickGroup() {
        tabSelected = ScriptDataType.Group;
        Refresh();
    }

    public void OnClickTeam() {
        tabSelected = ScriptDataType.Team;
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