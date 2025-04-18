
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReceiveDragable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    // - Parameter -
    public Image highlightImage;
    public Transform expectedTransform;
    public UnityEvent onReceive;

    public void OnPointerEnter(PointerEventData eventData) {

        if (highlightImage == null) return;
        
        if (Main.draggingElement != null) {
            highlightImage.gameObject.SetActive(true);
        }

    }

    public void OnPointerExit(PointerEventData eventData) {

        if (highlightImage == null) return;

        highlightImage.gameObject.SetActive(false);

    }

    public void ReceiveDrag(DragableUIElement dragable) {

        if (dragable.transform.parent == expectedTransform) {

            onReceive.Invoke();

        }

        if (highlightImage == null) return;

        highlightImage.gameObject.SetActive(false);

    }

}