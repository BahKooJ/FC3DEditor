

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StaticTextField : MonoBehaviour, IPointerClickHandler {

    TMP_InputField input;

    void Start() {
        
        input = GetComponent<TMP_InputField>();

        input.onEndEdit.AddListener(delegate { EndEdit(); });

    }

    public void OnPointerClick(PointerEventData eventData) {

        //input.interactable = true;

    }

    void EndEdit() {

        input.DeactivateInputField();

    }



}