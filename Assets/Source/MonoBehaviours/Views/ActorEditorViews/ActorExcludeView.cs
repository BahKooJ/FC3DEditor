
using UnityEngine;
using FCopParser;
using System;
using UnityEngine.EventSystems;

public class ActorExcludeView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    #region Context Menu Handeler

    bool insideItem = false;

    public void OnPointerEnter(PointerEventData eventData) {
        insideItem = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        insideItem = false;
    }

    void Update() {

        if (Main.draggingElement != null) {
            gameObject.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0)) {

            if (!insideItem) {
                gameObject.SetActive(false);
            }

        }

    }

    #endregion

    // - Prefabs -
    public GameObject excludeListItem;

    // - Unity Refs -
    public Transform listContent;
    public SceneActorsView sceneActorsView;

    void Start() {

        foreach (var type in Enum.GetValues(typeof(ActorBehavior))) {

            var obj = Instantiate(excludeListItem, listContent, false);
            obj.SetActive(true);
            var item = obj.GetComponent<ActorExcludeItemView>();
            item.type = (ActorBehavior)type;
            item.view = this;

        }

    }


}