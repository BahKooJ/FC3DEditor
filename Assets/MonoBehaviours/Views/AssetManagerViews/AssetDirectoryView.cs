
using TMPro;
using UnityEngine;

public class AssetDirectoryView : MonoBehaviour {

    // - Unity Refs -
    public TMP_Text title;

    // - Parameters -
    public AssetDirectory directory;
    public AssetManagerView view;
    public bool isParent;

    void Start () {

        if (isParent) {
            title.text = "...";
        }
        else {
            title.text = directory.name;
        }

    }

    public void OnClick() {

        view.MoveDirectory(directory);

    }


}