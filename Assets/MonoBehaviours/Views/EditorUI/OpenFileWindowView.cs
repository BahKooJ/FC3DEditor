
using FCopParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class OpenFileWindowView : MonoBehaviour {

    // Prefabs
    public GameObject fileListItem;
    public GameObject folderListItem;

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
    public List<FolderListItemView> folderListItems = new();

    void Start() {

        Main.ignoreAllInputs = true;

        if (!isOpen) {
            buttonText.text = "Save";
        }

        Refresh();

    }

    void Refresh() {

        foreach (var obj in fileListItems) {
            Destroy(obj.gameObject);
        }

        foreach (var obj in folderListItems) {
            Destroy(obj.gameObject);
        }

        fileListItems.Clear();
        folderListItems.Clear();

        var rootItem = Instantiate(folderListItem);

        rootItem.transform.SetParent(fileList, false);

        var rootscript = rootItem.GetComponent<FolderListItemView>();

        rootscript.view = this;

        rootscript.path = Directory.GetParent(directory).FullName;
        rootscript.displayName = "...";

        folderListItems.Add(rootscript);

        foreach (var dir in Directory.GetDirectories(directory)) {

            var item = Instantiate(folderListItem);

            item.transform.SetParent(fileList, false);

            var script = item.GetComponent<FolderListItemView>();

            script.view = this;

            script.path = dir;

            folderListItems.Add(script);

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

    public void SelectFolder(string path) {
        directory = path;

        Refresh();
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