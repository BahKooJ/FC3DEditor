

using TMPro;
using UnityEngine;

public class TextFieldPopupView : MonoBehaviour {

    // - Unity View Refs -
    public TMP_InputField textField;

    // - Parameters -
    public TextFieldPopupHandler handler;

    void Start() {

        Main.ignoreAllInputs = true;

        ((RectTransform)transform).anchoredPosition = Input.mousePosition / Main.uiScaleFactor;

        textField.Select();
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Main.ignoreAllInputs = false;
            handler.DestroyActiveView();
        }

    }

    public void OnFinish() {

        Main.ignoreAllInputs = false;

        handler.finishCallback(textField.text);
        handler.DestroyActiveView();

    }

}