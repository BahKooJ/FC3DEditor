
using FCopParser;
using TMPro;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ExplosionActorPropertyItemView : ActorPropertyItemView {

    // - Prefabs -
    public GameObject explosionPicker;

    // - Unity Refs -
    public TMP_Text nameText;
    public TMP_Text explosionNameText;

    public ExplosionActorProperty explosionProperty;

    bool refuseCallback = false;

    void Start() {

        explosionProperty = (ExplosionActorProperty)property;

        Refresh();

    }

    public override void Refresh() {

        refuseCallback = true;

        nameText.text = property.name;

        var explosionActors = controller.main.level.sceneActors.FindActorsByBehavior(ActorBehavior.ActorExplosion);

        var selectedActor = explosionActors.FirstOrDefault(a => ((SpecializedID)a.behavior).GetID() == explosionProperty.id);

        if (selectedActor != null) {
            explosionNameText.text = selectedActor.name;
        }
        else if (FCopExplosion.globalExplosions.ContainsKey(explosionProperty.id)) {
            explosionNameText.text = FCopExplosion.globalExplosions[explosionProperty.id];
        }
        else if (explosionProperty.id == 0) {
            explosionNameText.text = "None";
        }
        else {
            explosionNameText.text = "Missing";
        }

        refuseCallback = false;

    }

    public void OnClick() {

        if (refuseCallback) return;

        // Counter-action is added by the explosion selector view

        var obj = Instantiate(explosionPicker);
        obj.transform.SetParent(controller.main.canvas.transform, false);

        var rectTrans = (RectTransform)obj.transform;

        var pos = Input.mousePosition / Main.uiScaleFactor;
        var scaledScreenWidth = Screen.width / Main.uiScaleFactor;

        if (pos.x + 218f > scaledScreenWidth) {
            var dif = pos.x + 218f - scaledScreenWidth;
            pos.x -= dif;
        }

        if (pos.y - 300f < 0) {
            pos.y -= pos.y - 300f;

        }

        rectTrans.anchoredPosition = pos;

        obj.GetComponent<ExplosionSelectorView>().explosionPropertyView = this;
        obj.GetComponent<ExplosionSelectorView>().controller = controller;

    }

}