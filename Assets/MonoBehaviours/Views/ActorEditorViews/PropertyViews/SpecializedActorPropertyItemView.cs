
using UnityEngine;
using TMPro;
using FCopParser;
using System.Linq;
using System.Collections.Generic;

public class SpecializedActorPropertyItemView : ActorPropertyItemView {

    // - Unity Refs -
    public TMP_Text nameText;
    public TMP_Text assetNameText;

    public SpecializedActorRefActorProperty specialProperty;

    private void Start() {

        specialProperty = (SpecializedActorRefActorProperty)property;

        Refresh();

    }

    public override void Refresh() {

        nameText.text = property.name;

        var actors = controller.main.level.sceneActors.actors.Where(a => a.behaviorType == specialProperty.behaviorType).ToList();

        if (actors.Count == 0) {
            assetNameText.text = "Missing";
            return;
        }

        if (specialProperty.id == 0) {
            assetNameText.text = "None";
            return;
        }

        foreach (var actor in actors) {

            if (specialProperty.specializedID && actor.behavior is SpecializedID specializedID) {

                if (specializedID.GetID() == specialProperty.id) {
                    assetNameText.text = actor.name;
                    return;
                }

            }
            else {

                if (actor.DataID == specialProperty.id) {
                    assetNameText.text = actor.name;
                    return;
                }

            }

        }

        assetNameText.text = "Missing";

    }

    public void OnClickAsset() {

        var actorData = new Dictionary<int, string>() {
            { 0, "None" }
        };

        var actors = controller.main.level.sceneActors.actors.Where(a => a.behaviorType == specialProperty.behaviorType).ToList();

        foreach (var actor in actors) {

            if (specialProperty.specializedID && actor.behavior is SpecializedID specializedID) {

                actorData[specializedID.GetID()] = actor.name;

            }
            else {

                actorData[actor.DataID] = actor.name;

            }

        }

        MiniAssetManagerUtil.RequestUniversalData(actorData, controller.main, id => {

            specialProperty.id = id;
            Refresh();

        });

    }

}