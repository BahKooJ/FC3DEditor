
using FCopParser;
using TMPro;
using UnityEngine;

public class FolderListItemView: MonoBehaviour {

    // View refs
    public TMP_Text folderName;

    // Paramaters
    public OpenFileWindowView view;
    public string path;
    public string displayName = "";

    void Start() {

        if (displayName == "") {
            folderName.text = Utils.RemovePathingFromFilePath(path);
        }
        else {
            folderName.text = displayName;
        }


    }

    // Callbacks
    public void OnClick() {

        view.SelectFolder(path);

    }

}