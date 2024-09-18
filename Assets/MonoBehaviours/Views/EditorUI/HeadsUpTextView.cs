

using TMPro;
using UnityEngine;

public class HeadsUpTextView : MonoBehaviour {

    // - Unity Refs -
    public TMP_Text text;

    // - Parameters -
    public string message;

    private void Start() {

        text.text = message;

    }

}