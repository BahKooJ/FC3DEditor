using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HomeScreenView : MonoBehaviour {

    public FileManagerMain main;

    public GameObject FileListItem;

    public GameObject listView;

    private void Start() {

        // TODO: Hardcoded directory, refactor to settings file
        foreach (var file in Directory.GetFiles("MissionFiles")) {

            var item = Instantiate(FileListItem);

            item.transform.SetParent(listView.transform, false);

            var script = item.GetComponent<FileListButton>();

            script.file = file;

            script.main = main;

        }

    }

}
