﻿
using TMPro;
using UnityEngine;

public class ScriptingButtonItemView : MonoBehaviour {

    //View ref
    public TMP_Text title;

    public ScriptingPanelView view;
    public int value;

    void Start() {
        
        title.text = value.ToString();

    }


}