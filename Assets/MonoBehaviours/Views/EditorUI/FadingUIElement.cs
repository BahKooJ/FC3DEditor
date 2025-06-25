

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadingUIElement : MonoBehaviour {

    // - Unity Refs -
    public Image image;
    public TMP_Text text;
    public float increment;
    public float delay;

    float alphaValue = 0f;

    private void Start() {
        if (image != null) {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
        }

        if (text != null) {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
        }
    }

    private void Update() {

        if (delay > 0f) {
            delay -= Time.deltaTime;
            return;
        }

        if (alphaValue > 1f) {
            alphaValue = 1f;
        }

        if (alphaValue == 1f) {
            return;
        }

        if (image != null) {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alphaValue);
        }

        if (text != null ) {
            text.color = new Color(text.color.r, text.color.g, text.color.b, alphaValue);
        }

        alphaValue += increment;

    }

}