using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogWindow : MonoBehaviour {

    public TMP_Text title;
    public TMP_Text message;

    public GameObject cancelButton;

    public Func<bool> confirmAction;

    void Start() { 

        

    }

    public void OnClickOK() {

        confirmAction();

        Destroy(this.gameObject);

    }

    public void OnClickCancel() {

        Destroy(this.gameObject);

    }

}
