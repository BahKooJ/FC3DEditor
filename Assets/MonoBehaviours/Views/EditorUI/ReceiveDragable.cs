
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReceiveDragable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    // - Parameter -
    public Image highlightImage;
    public bool changeColor;
    public Color highlightColor;
    public Transform expectedTransform;
    public UnityEvent onReceive;

    Color originalColor;

    void Start() {
        originalColor = highlightImage.color;
    }

    public void OnPointerEnter(PointerEventData eventData) {

        if (highlightImage == null) return;
        
        if (Main.draggingElement != null) {

            if (changeColor) {
                highlightImage.color = highlightColor;
            }
            else {
                highlightImage.gameObject.SetActive(true);
            }

        }

    }

    public void OnPointerExit(PointerEventData eventData) {

        if (highlightImage == null) return;

        if (changeColor) {
            highlightImage.color = originalColor;
        }
        else {
            highlightImage.gameObject.SetActive(false);
        }

    }

    public void ReceiveDrag(DragableUIElement dragable) {

        if (expectedTransform != null) {

            if (dragable.transform.parent == expectedTransform) {

                onReceive.Invoke();

            }

        }
        else {

            onReceive.Invoke();

        }

        if (highlightImage == null) return;

        if (changeColor) {
            highlightImage.color = originalColor;
        }
        else {
            highlightImage.gameObject.SetActive(false);
        }

    }

}