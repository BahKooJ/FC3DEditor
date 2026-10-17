

using FCopParser;
using UnityEngine;
using TMPro;

public class VariableSelectorItemView : MonoBehaviour {

    // - Unity Refs -
    public TMP_Text text;

    // - Parameters -
    [HideInInspector]
    public VariableSelectorView view;
    [HideInInspector]
    public ScriptVariableType type;
    [HideInInspector]
    public int id;
    [HideInInspector]
    public string varName;

    private void Start() {
        
        text.text = varName;

    }

    public void OnClick() {
        view.OnSelectItem(this);
    }

}