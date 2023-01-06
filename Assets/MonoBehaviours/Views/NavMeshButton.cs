
using TMPro;
using UnityEngine;

class NavMeshButton : MonoBehaviour {

    public int index;

    public NavMeshEditPanel controller;

    public TextMeshProUGUI text;

    void Start() {
        
        text.text = index.ToString();

    }

    public void OnClick() {

        controller.ChangeNavMesh(index);

    }


}