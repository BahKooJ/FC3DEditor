

using FCopParser;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActorPropertiesView : MonoBehaviour {

    //Prefabs
    public GameObject valueActorPropertyItem;
    public GameObject toggleActorPropertyItem;
    public GameObject rangeActorPropertyItem;
    public GameObject enumDataActorPropertyItem;
    public GameObject actorScriptCallItem;
    public GameObject actorAssetRefItem;
    public GameObject groupActorPropertyItem;
    public GameObject overloadActorPropertyItem;
    public GameObject spawningPropertiesItem;
    public GameObject assetActorPropertyItem;
    public GameObject normalizedValueActorPorpertyItem;

    //View refs
    public Transform propertiesContent;
    public ScrollRect contentScrollview;
    public TMP_Text actorName;
    public TMP_Text idText;
    public TMP_Text actorTypeText;
    public SceneActorsView sceneActorsView;

    [HideInInspector]
    public List<ActorPropertyItemView> initedProperties = new();
    [HideInInspector]
    public List<GroupActorPropertyItemView> initedGroups = new();

    public ActorEditMode controller;

    [HideInInspector]
    public HashSet<string> openedGroups = new();

    void Start() {

        sceneActorsView.level = controller.main.level;
        sceneActorsView.controller = controller;

        Refresh();

    }

    public ActorPropertyItemView InitProperty(ActorProperty property) {

        GameObject obj = null;

        switch (property) {
            case ValueActorProperty:
                obj = Instantiate(valueActorPropertyItem);
                break;
            case ToggleActorProperty:
                obj = Instantiate(toggleActorPropertyItem);
                break;
            case RangeActorProperty:
                obj = Instantiate(rangeActorPropertyItem);
                break;
            case EnumDataActorProperty:
                obj = Instantiate(enumDataActorPropertyItem);
                break;
            case AssetActorProperty:
                obj = Instantiate(assetActorPropertyItem);
                break;
            case OverloadedProperty:
                obj = Instantiate(overloadActorPropertyItem);
                break;
            case NormalizedValueProperty:
                obj = Instantiate(normalizedValueActorPorpertyItem);
                break;
        }

        if (obj == null) {
            return null;
        }

        var view = obj.GetComponent<ActorPropertyItemView>();

        view.controller = controller;
        view.property = property;
        view.actor = controller.selectedActor;
        view.view = this;

        obj.transform.SetParent(propertiesContent, false);

        return view;

    }

    public void Refresh() {

        foreach (Transform transform in propertiesContent.transform) {
            Destroy(transform.gameObject);
        }

        initedGroups.Clear();
        initedProperties.Clear();

        if (controller.selectedActor == null) {
            actorName.text = "N/A";
            idText.text = "N/A";
            actorTypeText.text = "N/A";
            return;
        }

        actorName.text = controller.selectedActor.name;
        idText.text = controller.selectedActor.DataID.ToString();
        actorTypeText.text = controller.selectedActor.behaviorType.ToString();

        List<ActorProperty> floatingProperties = new();

        if (controller.selectedActor.behavior != null && controller.selectedActor.behavior.properties != null) {

            foreach (var property in controller.selectedActor.behavior.propertiesByCommonName) {

                if (property.Key == "") {
                    floatingProperties = property.Value;
                    continue;
                }

                var view = Instantiate(groupActorPropertyItem);

                var propertyView = view.GetComponent<GroupActorPropertyItemView>();

                propertyView.controller = controller;
                propertyView.view = this;
                propertyView.commonName = property.Key;
                propertyView.properties = property.Value;
                propertyView.actor = controller.selectedActor;

                view.transform.SetParent(propertiesContent, false);

                initedGroups.Add(propertyView);

            }

        }

        foreach (var property in floatingProperties) {
            var obj = InitProperty(property);

            if (obj != null) {
                initedProperties.Add(obj);
            }

        }


        var actorAssetRefView = Instantiate(actorAssetRefItem);

        actorAssetRefView.GetComponent<ActorAssetReferencesView>().controller = controller;
        actorAssetRefView.GetComponent<ActorAssetReferencesView>().actor = controller.selectedActor;

        actorAssetRefView.transform.SetParent(propertiesContent, false);

        var actorScriptView = Instantiate(actorScriptCallItem);

        actorScriptView.GetComponent<ActorScriptCallItemView>().controller = controller;
        actorScriptView.GetComponent<ActorScriptCallItemView>().actor = controller.selectedActor;

        actorScriptView.transform.SetParent(propertiesContent, false);

        var actorSpawningView = Instantiate(spawningPropertiesItem);

        actorSpawningView.GetComponent<ActorSpawningPropertiesItemView>().controller = controller;
        actorSpawningView.GetComponent<ActorSpawningPropertiesItemView>().actor = controller.selectedActor;

        actorSpawningView.transform.SetParent(propertiesContent, false);

    }

    public void RequestPropertyRefresh(ActorProperty property) {

        foreach (var initedProp in initedProperties) {

            if (initedProp.property == property) {
                initedProp.Refresh();
                return;
            }

        }

        foreach (var group in initedGroups) {

            foreach (var initedProp in group.initedProperties) {

                if (initedProp.property == property) {
                    initedProp.Refresh();
                    return;
                }

            }

        }

    }

    public void JumpToPropety(ActorProperty property) {

        foreach (var group in initedGroups) {

            if (group.properties.Contains(property)) {

                if (group.initedProperties.Count == 0) {

                    group.OnClick();

                }

            }

        }

        var propertyView = initedProperties.FirstOrDefault(v => v.property == property);

        if (propertyView == null) {
            
            foreach (var group in initedGroups) {

                propertyView = group.initedProperties.FirstOrDefault(v => v.property == property);

                if (propertyView != null) { break; }

            }

        }

        if (propertyView == null) { return; }

        var normalizedPos = ((decimal)propertyView.transform.GetSiblingIndex()) / ((decimal)propertyView.transform.parent.childCount - 0);

        contentScrollview.verticalNormalizedPosition = (float)(1 - normalizedPos);

    }

    public void HighlightProperty(ActorProperty property) {

        var propertyView = initedProperties.FirstOrDefault(v => v.property == property);

        if (propertyView == null) {

            foreach (var group in initedGroups) {

                propertyView = group.initedProperties.FirstOrDefault(v => v.property == property);

                if (propertyView != null) { break; }

            }

        }

        if (propertyView == null) { return; }

        propertyView.backgroundImage.color = new Color(0f, 0.8f, 0f);

    }

    public void RefreshName() {

        actorName.text = controller.selectedActor.name;

    }

}