using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualScriptingLineView : MonoBehaviour {

    // - Unity Refs -
    public TMP_Text numberText;
    public Transform lineContent;

    // - Parameters -
    public int number;
    public List<GameObject> scriptNodes = new();

    private void Start() {
        numberText.text = number.ToString();
    }

}