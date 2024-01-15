using System.Collections;
using TMPro;
using UnityEngine;


public class PositionTextView : MonoBehaviour {

    public TMP_Text text;
    public Camera mainCamera;

    void Update() {

        text.text =
            "X: " + mainCamera.transform.position.x + "\n" +
            "Y: " + mainCamera.transform.position.y + "\n" +
            "Z: " + mainCamera.transform.position.z + "\n" +
            "Facing: " + mainCamera.transform.rotation.eulerAngles.y;

    }

}
