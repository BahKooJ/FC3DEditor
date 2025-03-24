

using FCopParser;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UniversialMiniDataManagerView : MonoBehaviour {

    // - Unity Prefabs -
    public GameObject universialMiniDataItemViewPrefab;

    // - Unity Refs -
    public Transform fileContent;

    // - Parameters -
    public FCopLevel level;
    [HideInInspector]
    public Main main;
    [HideInInspector]
    public Dictionary<int, string> requestingData;
    public Action<int> onDataSelected;

    List<UniversialMiniDataItemView> files = new();

    private void Start() {

        void InitFile(KeyValuePair<int, string> data) {

            var obj = Instantiate(universialMiniDataItemViewPrefab);
            obj.transform.SetParent(fileContent.transform, false);

            var fileView = obj.GetComponent<UniversialMiniDataItemView>();
            fileView.dataName = data.Value;
            fileView.id = data.Key;
            fileView.view = this;

            files.Add(fileView);

        }

        foreach (var data in requestingData) {
            InitFile(data);
        }

    }

    public void OnSelectData(int id) {

        onDataSelected(id);
        Destroy(gameObject);

    }

}