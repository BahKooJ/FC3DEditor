
using System;
using UnityEngine;

public class TextFieldPopupHandler : MonoBehaviour {

    // - Prefab -
    public GameObject prefab;

    // - Parameters -
    public Action<string> finishCallback;

    TextFieldPopupView activeView = null;

    void OnDestroy() {
        DestroyActiveView();
    }

    void OnDisable() {
        DestroyActiveView();
    }

    public void OpenPopupTextField(string existingText) {

        DestroyActiveView();

        var obj = Instantiate(prefab);
        obj.transform.SetParent(GetComponentInParent<Canvas>().rootCanvas.transform, false);

        activeView = obj.GetComponent<TextFieldPopupView>();

        activeView.handler = this;

        activeView.textField.text = existingText;

    }

    public void DestroyActiveView() {

        if (activeView != null) {
            Destroy(activeView.gameObject);
        }

        activeView = null;

    }

}