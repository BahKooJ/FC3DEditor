using TMPro;
using UnityEngine;

public class InfoPopUpView : MonoBehaviour {

    public TMP_Text text;

    void Update() {

        if (Main.draggingElement != null) {
            Destroy(gameObject);
        }
        
        ((RectTransform)transform).anchoredPosition = Input.mousePosition / Main.uiScaleFactor;


    }

}
