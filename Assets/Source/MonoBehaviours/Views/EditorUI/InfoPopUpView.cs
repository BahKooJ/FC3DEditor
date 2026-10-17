using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopUpView : MonoBehaviour {

    public TMP_Text text;
    public ContentSizeFitter fitter;

    void Update() {

        if (Main.draggingElement != null) {
            Destroy(gameObject);
        }

        var rectTrans = (RectTransform)transform;

        var pos = Input.mousePosition / Main.uiScaleFactor;
        var scaledScreenWidth = Screen.width / Main.uiScaleFactor;

        var width = ((RectTransform)fitter.transform).sizeDelta.x;
        var height = ((RectTransform)fitter.transform).sizeDelta.y;


        if (pos.x + width > scaledScreenWidth) {
            var dif = pos.x + width - scaledScreenWidth;
            pos.x -= dif;
        }

        rectTrans.anchoredPosition = pos;


    }

}
