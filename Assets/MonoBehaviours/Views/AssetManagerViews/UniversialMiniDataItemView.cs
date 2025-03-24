

using FCopParser;
using TMPro;
using UnityEngine;

public class UniversialMiniDataItemView : MonoBehaviour {

    // - Unity Refs -
    public TMP_Text title;

    // - Parameters -
    [HideInInspector]
    public string dataName;
    [HideInInspector]
    public int id;
    [HideInInspector]
    public UniversialMiniDataManagerView view;

    void Start() {

        title.text = dataName;

    }

    public void OnClick() {

        view.OnSelectData(id);

    }

}