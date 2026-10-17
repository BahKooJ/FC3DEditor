

using FCopParser;
using UnityEngine;
using UnityEngine.UI;

public class ActorPropertyItemView : MonoBehaviour {

    // - View Refs -
    public Image backgroundImage;

    // - Parameters -
    public ActorProperty property;
    public FCopActor actor;
    public ActorEditMode controller;
    [HideInInspector]
    public ActorPropertiesView view;
    public Color defaultColor = Color.white;

    float colorEaseTrackTime = 0f;

    void Update() {

        if (backgroundImage == null) {
            return;
        }
        
        if (defaultColor != backgroundImage.color) {

            var r = backgroundImage.color.r;
            var g = backgroundImage.color.g;
            var b = backgroundImage.color.b;

            var dr = defaultColor.r;
            var dg = defaultColor.g;
            var db = defaultColor.b;

            if (dr - r > 0) {
                r += 0.01f;
            }
            else {
                r -= 0.01f;
            }

            if (dg - g > 0) {
                g += 0.01f;
            }
            else {
                g -= 0.01f;
            }

            if (db - b > 0) {
                b += 0.01f;
            }
            else {
                b -= 0.01f;
            }

            backgroundImage.color = new Color(r, g, b);

            colorEaseTrackTime += Time.deltaTime;

            if (colorEaseTrackTime > 4f) {
                backgroundImage.color = defaultColor;
                colorEaseTrackTime = 0f;
            }
        }

    }


    virtual public void Refresh() {



    }

}