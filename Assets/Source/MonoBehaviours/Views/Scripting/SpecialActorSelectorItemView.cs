

using FCopParser;
using UnityEngine;
using TMPro;

public class SpecialActorSelectorItemView : MonoBehaviour {

    // - Unity Refs -
    public TMP_Text text;

    // - Parameters -
    [HideInInspector]
    public SpecialActorSelectorView view;
    [HideInInspector]
    public ScriptDataType type;
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