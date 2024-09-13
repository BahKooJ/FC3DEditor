using TMPro;
using UnityEngine;

public class InfoPopUpView : MonoBehaviour {

    public TMP_Text text;

    void Update() {
        
        ((RectTransform)transform).anchoredPosition = Input.mousePosition / Main.uiScaleFactor;


    }

}
