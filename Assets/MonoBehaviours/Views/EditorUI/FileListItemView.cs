
using TMPro;
using UnityEngine;

public class FileListItemView: MonoBehaviour {

    // View refs
    public TMP_Text fileName;

    // Paramaters
    public OpenFileWindowView view;
    public string filePath;

    void Start() {
        
        fileName.text = filePath;

    }

    // Callbacks
    public void OnClick() {

        view.SelectFile(filePath);

    }

}