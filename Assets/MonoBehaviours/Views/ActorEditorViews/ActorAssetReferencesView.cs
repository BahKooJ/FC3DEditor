

using FCopParser;
using UnityEngine;

public class ActorAssetReferencesView : MonoBehaviour {

    // - Prefab -
    public GameObject listItem;

    // - View Refs -
    public Transform listContent;

    // - Parameters -
    public ActorEditMode controller;
    public FCopActor actor;


    private void Start() {

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
            i++;
        }


    }

}