
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssetFileView : MonoBehaviour {

    // - Unity Refs -
    public TMP_Text title;
    public Image icon;

    // - Parameters -
    public AssetFile file;
    public AssetManagerView view;

    void Start () {

        title.text = file.asset.name;

    }

    public void OnClick() {

        view.OnSelectAsset(file);

    }

}