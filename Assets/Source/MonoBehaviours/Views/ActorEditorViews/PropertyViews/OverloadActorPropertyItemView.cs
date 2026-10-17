

using FCopParser;
using UnityEngine;

public class OverloadActorPropertyItemView : ActorPropertyItemView {

    ActorPropertyItemView activePropertyItem;

    void Start() {

        Refresh();

    }

    public override void Refresh() {

        if (activePropertyItem != null) {

            Destroy(activePropertyItem.gameObject);

        }

        var property = ((OverloadedProperty)this.property).GetOverloadProperty();

        activePropertyItem = this.view.InitProperty(property);

        if (activePropertyItem == null) {
            gameObject.SetActive(false);
            return;
        }
        else {
            gameObject.SetActive(true);
        }

        activePropertyItem.transform.SetParent(transform, false);

        ((RectTransform)transform).sizeDelta = new Vector2(100, ((RectTransform)activePropertyItem.transform).sizeDelta.y);

    }

}