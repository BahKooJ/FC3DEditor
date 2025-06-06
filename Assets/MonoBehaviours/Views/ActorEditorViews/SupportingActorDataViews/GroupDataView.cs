

using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupDataView : MonoBehaviour {

    // - Prefabs -
    public GameObject groupListItem;

    // - Unity Refs -
    public Transform listContent;
    public Transform addGroupButtonTrans;

    // - Parameters -
    public FCopLevel level;

    List<GroupDataListItemView> listItems = new();

    void Start() {

        Refresh();

    }

    public void Refresh() {

        foreach (var item in listItems) {
            Destroy(item.gameObject);
        }

        listItems.Clear();

        foreach (var group in level.sceneActors.scriptGroup) {

            var obj = Instantiate(groupListItem);
            obj.transform.SetParent(listContent.transform, false);
            obj.SetActive(true);

            var groupDataItem = obj.GetComponent<GroupDataListItemView>();
            groupDataItem.groupName = group.Value;
            groupDataItem.id = group.Key;
            groupDataItem.view = this;
            groupDataItem.level = level;

            listItems.Add(groupDataItem);

        }

        addGroupButtonTrans.SetAsLastSibling();

    }

    public void AddGroup() {

        level.sceneActors.AddGroup();

        Refresh();

        listItems.Last().Rename();

    }


}