using System;
using TMPro;
using UnityEngine;

public class DialogWindow : MonoBehaviour {

    public TMP_Text title;
    public TMP_Text message;

    public GameObject cancelButton;

    public Func<bool> confirmAction;

    public void OnClickOK() {

        if (confirmAction != null) {

            confirmAction();

        }

        Destroy(gameObject);

    }

    public void OnClickCancel() {

        Destroy(gameObject);

    }

}
