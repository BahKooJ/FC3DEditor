

using FCopParser;
using TMPro;
using UnityEngine;

public class FileListButton : MonoBehaviour {

    public string file;
    public FileManagerMain main;

    public TMP_Text text;

    void Start() {

        text.text = Utils.RemovePathingFromFilePath(file);

    }

    public void OnClick() {
        main.OpenFile(file);
    }

}
