using FCopParser;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PointListView : MonoBehaviour {


    RectTransform rectTransformer;
    public GameObject listItem;
    public Main controller;
    
    void Start() {
        
        rectTransformer = GetComponent<RectTransform>();
        

    }

    public void Clear() {

        foreach (Object child in transform) {
            Destroy(child.GameObject());
        }

    }

    public void AddItems(Tile tile) {

        var index = 0;
        foreach (var vertex in tile.verticies) {

            var item = Instantiate(listItem);

            item.transform.position = new Vector3(0, -16 - (index * 32), 0);

            item.transform.SetParent(this.transform, false);

            var script = item.GetComponent<PointListItem>();
            script.vertex = vertex;
            script.controller = controller;
            script.index = index;

            index++;

        }

    }

}
