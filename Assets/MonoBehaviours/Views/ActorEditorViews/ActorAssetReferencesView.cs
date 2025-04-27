

using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class ActorAssetReferencesView : MonoBehaviour {

    // - Prefab -
    public GameObject listItem;

    // - View Refs -
    public Transform listContent;

    // - Parameters -
    public ActorEditMode controller;
    public FCopActor actor;

    public List<ActorAssetReferencesItemView> items = new();

    private void Start() {

        Refresh();

    }

    public void Refresh() {

        foreach (var item in items) {
            Destroy(item.gameObject);
        }

        items.Clear();

        var i = 0;
        foreach (var r in actor.resourceReferences) {

            var obj = Instantiate(listItem);
            obj.transform.SetParent(listContent, false);
            obj.SetActive(true);
            var item = obj.GetComponent<ActorAssetReferencesItemView>();
            item.view = this;
            item.main = controller.main;
            item.fcopActor = actor;
            item.refIndex = i;

            items.Add(item);

            i++;
        }

    }

}