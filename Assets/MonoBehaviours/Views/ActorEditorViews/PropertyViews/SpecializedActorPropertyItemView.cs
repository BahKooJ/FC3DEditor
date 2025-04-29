using TMPro;
using FCopParser;
using System.Linq;
using System.Collections.Generic;

public class SpecializedActorPropertyItemView : ActorPropertyItemView {

    // - Unity Refs -
    public TMP_Text nameText;
    public TMP_Text assetNameText;

    public SpecializedActorRefActorProperty specialProperty;

    bool refuseCallback = false;

    private void Start() {

        specialProperty = (SpecializedActorRefActorProperty)property;

        Refresh();

    }

    public override void Refresh() {

        refuseCallback = true;

        nameText.text = property.name;

        var actors = controller.main.level.sceneActors.actors.Where(a => a.behaviorType == specialProperty.behaviorType).ToList();

        if (actors.Count == 0) {
            assetNameText.text = "Missing";
            refuseCallback = false;
            return;
        }

        if (specialProperty.id == 0) {
            assetNameText.text = "None";
            refuseCallback = false;
            return;
        }

        foreach (var actor in actors) {

            if (specialProperty.specializedID && actor.behavior is SpecializedID specializedID) {

                if (specializedID.GetID() == specialProperty.id) {
                    assetNameText.text = actor.name;
                    refuseCallback = false;
                    return;
                }

            }
            else {

                if (actor.DataID == specialProperty.id) {
                    assetNameText.text = actor.name;
                    refuseCallback = false;
                    return;
                }

            }

        }

        assetNameText.text = "Missing";

        refuseCallback = false;

    }

    public void OnClickAsset() {

        if (refuseCallback) return;

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

            ActorEditMode.AddPropertyChangeCounterAction(property, actor);

            specialProperty.id = id;
            Refresh();

        });

    }

}