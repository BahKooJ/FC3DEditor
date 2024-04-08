using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PositionTextView : MonoBehaviour {

    public TMP_Text text;
    public Slider facingSlider;
    public Camera mainCamera;

    void Update() {

        var facing = mainCamera.transform.rotation.eulerAngles.y;

        text.text =
            "X: " + mainCamera.transform.position.x + "\n" +
            "Y: " + mainCamera.transform.position.y + "\n" +
            "Z: " + mainCamera.transform.position.z + "\n" +
            "Facing: " + facing;

        if (facing < 180) {
            facingSlider.value = 180f + facing;
        } else {
            facingSlider.value = facing - 180f;
        }

    }

}
