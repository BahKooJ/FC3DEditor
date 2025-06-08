
using UnityEngine;
using FCopParser;
using TMPro;
using UnityEngine.UI;

public class ActorExcludeItemView : MonoBehaviour {

    // - Unity Refs -
    public Toggle enableToggle;
    public TMP_Text label;

    // - Parameters -
    [HideInInspector]
    public ActorBehavior type;
    [HideInInspector]
    public ActorExcludeView view;

    bool refuseCallback = false;
    private void Start() {

        refuseCallback = true;
        label.text = Utils.AddSpacesToString(type.ToString());

        enableToggle.isOn = SettingsManager.allowedActorsInSceneView[type];
        refuseCallback = false;

    }

    public void OnToggle() {

        if (refuseCallback) return;

        SettingsManager.allowedActorsInSceneView[type] = enableToggle.isOn;

        view.sceneActorsView.Validate();

    }

}