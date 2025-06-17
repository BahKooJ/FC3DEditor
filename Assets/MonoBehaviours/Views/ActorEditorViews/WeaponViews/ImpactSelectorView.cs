

using FCopParser;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ImpactSelectorView : MonoBehaviour {

    // - Prefabs -
    public GameObject impactItemFab;

    // - Unity Refs -
    public List<VideoClip> videos = new();
    public List<int> ids = new();
    public Transform listContent;

    // - Parameters -
    public ImpactActorPropertyItemView impactPropertyView;
    public ActorEditMode controller;

    List<ImpactSelectorItemView> items = new();

    void Start() {

        for (int i = 0; i < videos.Count; i++) {

            var obj = Instantiate(impactItemFab, listContent, false);
            obj.SetActive(true);
            var item = obj.GetComponent<ImpactSelectorItemView>();
            item.video = videos[i];
            item.id = ids[i];
            item.view = this;

            items.Add(item);

        }

    }

    public void Select(int id) {

        ActorEditMode.AddPropertyChangeCounterAction((ActorProperty)impactPropertyView.impactProperty, controller.selectedActor);

        impactPropertyView.impactProperty.id = id;
        impactPropertyView.Refresh();
        Destroy(gameObject);

    }

}