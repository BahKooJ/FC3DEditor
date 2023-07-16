
using FCopParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class OpenFileWindowView : MonoBehaviour {

    // Prefabs
    public GameObject fileListItem;

    // View refs
    public RectTransform fileList;
    public TMP_Text buttonText;
    public TMP_InputField fileField;

    // Paramaters
    public bool isOpen = true;
    public string directory;
    public string defaultFileName;
    public Action<string> confirmAction;

    public List<FileListItemView> fileListItems = new();

    void Start() {

        Main.ignoreAllInputs = true;

        if (!isOpen) {
            buttonText.text = "Save";
        }

        foreach (var file in Directory.GetFiles(directory)) {

            var item = Instantiate(fileListItem);

            item.transform.SetParent(fileList, false);

            var script = item.GetComponent<FileListItemView>();

            script.view = this;

            script.filePath = file;

            fileListItems.Add(script);

        }

        fileField.text = defaultFileName;

    }

    public void SelectFile(string filePath) {

        fileField.text = Utils.RemovePathingFromFilePath(filePath);

    }

    // Callbacks

    public void OnClickOpenSave() {

        if ((!File.Exists(directory + "\\" + fileField.text) && isOpen) || fileField.text.Count() == 0) {

            DialogWindowUtil.Dialog("Invalid File", "Please select a valid file");

            return;
        }

        confirmAction(directory + "\\" + fileField.text);

        Destroy(gameObject);

        Main.ignoreAllInputs = false;

    }

    public void OnClickCancel() {

        Destroy(gameObject);

        Main.ignoreAllInputs = false;

    }

}