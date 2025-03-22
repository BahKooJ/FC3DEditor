

using FCopParser;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class ActorSpawningPropertiesItemView : MonoBehaviour {

    // - Unity Refs -
    public GameObject propertiesGObj;
    public TMP_Text addRemoveButtonLabel;
    public TMP_InputField respawnTimerField;
    public Toggle randomFirstSpawnToggle;
    public TMP_InputField maxActiveActorsField;
    public Toggle disableRespawnToggle;
    public TMP_InputField maxRespawnsField;
    public Toggle infiniteRespawnsToggle;

    // - Parameters -
    public ActorEditMode controller;
    public FCopActor actor;

    float defaultSize = 0f;

    void Start() {

        defaultSize = ((RectTransform)transform).sizeDelta.y;

        Refresh();

    }

    public void Refresh() {

        refuseCallback = true;

        if (actor.spawningProperties == null) {

            propertiesGObj.SetActive(false);
            addRemoveButtonLabel.text = "Add Spawning Properties";
            ((RectTransform)transform).sizeDelta = new Vector2(100, 56);

        }
        else {

            propertiesGObj.SetActive(true);
            addRemoveButtonLabel.text = "Remove Spawning Properties";
            ((RectTransform)transform).sizeDelta = new Vector2(100, defaultSize);

            respawnTimerField.text = actor.spawningProperties.respawnTime.ToString();
            randomFirstSpawnToggle.isOn = actor.spawningProperties.randomFirstSpawnTime;
            maxActiveActorsField.text = actor.spawningProperties.maxActiveActors.ToString();
            disableRespawnToggle.isOn = actor.spawningProperties.disableRespawn;

            if (actor.spawningProperties.infiniteRespawns) {
                maxRespawnsField.text = "N/A";
                maxRespawnsField.interactable = false;
            }
            else {
                maxRespawnsField.text = actor.spawningProperties.maxRespawns.ToString();
            }

            infiniteRespawnsToggle.isOn = actor.spawningProperties.infiniteRespawns;

        }

        refuseCallback = false;

    }

    // - Callbacks -
    bool refuseCallback = false;

    public void OnClickAddRemoveSpawnProperties() {

        if (refuseCallback) return;

        if (actor.spawningProperties != null) {

            actor.spawningProperties = null;
            Refresh();

        }
        else {

            actor.spawningProperties = new FCopActorSpawning(actor.DataID);
            Refresh();

        }

    }

    public void OnFinishRespawnTimer() {

        if (refuseCallback) return;

        if (actor.spawningProperties != null) {

            try {
                var value = float.Parse(respawnTimerField.text);

                if (value > 100) {
                    actor.spawningProperties.respawnTime = 100;
                }
                else if (value < 0) {
                    actor.spawningProperties.respawnTime = 0;
                }
                else {
                    actor.spawningProperties.respawnTime = value;
                }

            }
            catch { }

            respawnTimerField.text = actor.spawningProperties.respawnTime.ToString();


        }

    }

    public void OnToggleRandomFirstRespawn() {

        if (refuseCallback) return;

        if (actor.spawningProperties != null) {

            actor.spawningProperties.randomFirstSpawnTime = randomFirstSpawnToggle.isOn;

        }

    }

    public void OnFinishMaxActiveActors() {

        if (refuseCallback) return;

        if (actor.spawningProperties != null) {

            try {

                var value = Int32.Parse(maxActiveActorsField.text);

                if (value > 256) {
                    actor.spawningProperties.maxActiveActors = 256;
                }
                else if (value < 0) {
                    actor.spawningProperties.maxActiveActors = 0;
                }
                else {
                    actor.spawningProperties.maxActiveActors = value;
                }


            }
            catch { }

            respawnTimerField.text = actor.spawningProperties.maxActiveActors.ToString();

        }

    }

    public void OnToggleDisableRespawn() {

        if (refuseCallback) return;

        if (actor.spawningProperties != null) {

            actor.spawningProperties.disableRespawn = disableRespawnToggle.isOn;

        }

    }

    public void OnFinishMaxRespawns() {

        if (refuseCallback) return;

        refuseCallback = true;

        if (actor.spawningProperties != null) {

            try {

                var value = Int32.Parse(maxRespawnsField.text);

                if (value == -1) {

                    infiniteRespawnsToggle.isOn = true;
                    maxRespawnsField.text = "N/A";
                    maxRespawnsField.interactable = false;

                }
                else if (value > short.MaxValue) {
                    actor.spawningProperties.maxRespawns = short.MaxValue;
                    maxRespawnsField.text = actor.spawningProperties.maxRespawns.ToString();
                }
                else if (value < 0) {
                    actor.spawningProperties.maxRespawns = 0;
                    maxRespawnsField.text = actor.spawningProperties.maxRespawns.ToString();
                }
                else {
                    actor.spawningProperties.maxRespawns = value;
                    maxRespawnsField.text = actor.spawningProperties.maxRespawns.ToString();
                }


            }
            catch {
            
                if (actor.spawningProperties.infiniteRespawns) {
                    maxRespawnsField.text = "N/A";
                    maxRespawnsField.interactable = false;
                }
                else {
                    maxRespawnsField.text = actor.spawningProperties.maxRespawns.ToString();
                }

            }

        }

        refuseCallback = false;

    }

    public void OnToggleInfiniteRespawns() {

        if (refuseCallback) return;

        refuseCallback = true;

        if (actor.spawningProperties != null) {

            actor.spawningProperties.infiniteRespawns = infiniteRespawnsToggle.isOn;

            if (actor.spawningProperties.infiniteRespawns) {
                maxRespawnsField.text = "N/A";
                maxRespawnsField.interactable = false;
            }
            else {

                maxRespawnsField.interactable = true;
                maxRespawnsField.text = actor.spawningProperties.maxRespawns.ToString();

            }

        }

        refuseCallback = false;

    }

}