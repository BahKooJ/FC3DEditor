using UnityEngine;
using UnityEngine.EventSystems;

public class InfoBoxHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GameObject InfoPopUp;

    public string message;

    private GameObject infoBox;

    public void OnPointerEnter(PointerEventData eventData) {
        
        var view = Instantiate(InfoPopUp);

        var script = view.GetComponent<InfoPopUpView>();

        script.text.text = message;

        infoBox = view;

        view.transform.SetParent(GetComponentInParent<Canvas>().rootCanvas.transform, false);

    }

    public void OnPointerExit(PointerEventData eventData) {

        if (infoBox != null) {
            Destroy(infoBox);
            infoBox = null;
        }

    }

    public void OnDestroy() {

        if (infoBox != null) {
            Destroy(infoBox);
            infoBox = null;
        }

    }

}
