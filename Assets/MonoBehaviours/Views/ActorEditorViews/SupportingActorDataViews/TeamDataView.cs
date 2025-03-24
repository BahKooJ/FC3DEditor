using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FCopParser;
using System.Linq;

public class TeamDataView : MonoBehaviour {

    // - Prefabs -
    public GameObject teamListItem;

    // - Unity Refs -
    public Transform listContent;
    public Transform addTeamButtonTrans;

    // - Parameters -
    public FCopLevel level;

    List<TeamDataListItemView> listItems = new();

    void Start() {

        Refresh();

    }

    public void Refresh() {

        foreach (var item in listItems) {
            Destroy(item.gameObject);
        }

        listItems.Clear();

        foreach (var team in level.sceneActors.teams) {

            var obj = Instantiate(teamListItem);
            obj.transform.SetParent(listContent.transform, false);
            obj.SetActive(true);

            var teamDataItem = obj.GetComponent<TeamDataListItemView>();
            teamDataItem.teamName = team.Value;
            teamDataItem.id = team.Key;
            teamDataItem.view = this;
            teamDataItem.level = level;

            listItems.Add(teamDataItem);

        }

        addTeamButtonTrans.SetAsLastSibling();

    }

    public void AddTeam() {

        level.sceneActors.AddTeam();

        Refresh();

        listItems.Last().Rename();

    }


}
