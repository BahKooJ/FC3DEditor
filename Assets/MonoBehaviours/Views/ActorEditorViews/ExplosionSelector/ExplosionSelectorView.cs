
using FCopParser;
using UnityEngine;
using UnityEngine.UI;

public class ExplosionSelectorView : MonoBehaviour {

    // - Ref Prefabs -
    public GameObject customExplosionItem;

    // - Unity Refs -
    public Transform customExplosionContent;
    public GameObject globalView;
    public GameObject customView;
    public Button globalTab;
    public Button customTab;

    // - Parameters -
    [HideInInspector]
    public ExplosionActorPropertyItemView explosionPropertyView;
    public ActorEditMode controller;

    void Start() {

        var explosionActors = controller.main.level.sceneActors.FindActorsByBehavior(ActorBehavior.ActorExplosion);

        foreach (var actor in explosionActors) {

            var obj = Instantiate(customExplosionItem);
            obj.transform.SetParent(customExplosionContent, false);
            obj.SetActive(true);

            var viewItem = obj.GetComponent<CustomExplosionItemView>();
            viewItem.actorName = actor.name;
            viewItem.id = ((SpecializedID)actor.behavior).GetID();

        }

    }

    public void Select(int id) {

        explosionPropertyView.explosionProperty.id = id;
        explosionPropertyView.Refresh();
        Destroy(gameObject);

    }

    // - Unity Callback -
    public void OnClickGlobalTab() {

        var globalColors = globalTab.colors;
        globalColors.normalColor = new Color(0.5f, 1f, 0.5f);
        globalTab.colors = globalColors;

        var customColors = customTab.colors;
        customColors.normalColor = new Color(1f, 1f, 1f);
        customTab.colors = customColors;

        globalView.SetActive(true);
        customView.SetActive(false);

    }

    public void OnClickCustomTab() {

        var globalColors = globalTab.colors;
        globalColors.normalColor = new Color(1f, 1f, 1f);
        globalTab.colors = globalColors;

        var customColors = customTab.colors;
        customColors.normalColor = new Color(0.5f, 1f, 0.5f);
        customTab.colors = customColors;

        globalView.SetActive(false);
        customView.SetActive(true);

    }

}