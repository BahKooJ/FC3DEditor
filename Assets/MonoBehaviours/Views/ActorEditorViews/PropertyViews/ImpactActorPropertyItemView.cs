
using FCopParser;
using System.Linq;
using TMPro;
using UnityEngine;

public class ImpactActorPropertyItemView : ActorPropertyItemView {

    // - Prefabs -
    public GameObject impactPicker;

    // - Unity Refs -
    public TMP_Text nameText;
    public TMP_Text impactNameText;

    public ImpactActorProperty impactProperty;

    bool refuseCallback = false;

    void Start() {

        impactProperty = (ImpactActorProperty)property;

        Refresh();

    }

    public override void Refresh() {

        refuseCallback = true;

        nameText.text = property.name;
        
        if (FCopExplosion.globalWeaponImpacts.ContainsKey(impactProperty.id)) {
            impactNameText.text = FCopExplosion.globalWeaponImpacts[impactProperty.id];
        }
        else if (impactProperty.id == 0) {
            impactNameText.text = "None";
        }
        else {
            impactNameText.text = "Missing";
        }

        refuseCallback = false;

    }

    public void OnClick() {

        if (refuseCallback) return;

        // Counter-action is added by the explosion selector view

        var obj = Instantiate(impactPicker);
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

        obj.GetComponent<ImpactSelectorView>().impactPropertyView = this;
        obj.GetComponent<ImpactSelectorView>().controller = controller;

    }

}