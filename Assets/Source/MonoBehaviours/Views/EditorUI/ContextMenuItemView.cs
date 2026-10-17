
using System;
using TMPro;
using UnityEngine;

class ContextMenuItemView: MonoBehaviour {

    // View refs
    public TMP_Text titleText;

    public string title;
    public Action clickAction;

    public GameObject container;

    void Start() {
        
        titleText.text = title;

    }

    public void OnClick() {

        clickAction();

        Destroy(container);

    }

}

