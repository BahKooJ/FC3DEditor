
using FCopParser;
using TMPro;
using UnityEngine;

public class CustomExplosionItemView : MonoBehaviour {

    // - Unity Refs -
    public ExplosionSelectorView view;
    public TMP_Text title;

    // - Parameters -
    [HideInInspector]
    public string actorName;
    [HideInInspector]
    public int id;

    void Start() {
        
        title.text = actorName;

    }

    public void OnClick() {

        view.Select(id);

    }

}