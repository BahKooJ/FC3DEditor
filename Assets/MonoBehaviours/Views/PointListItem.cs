using FCopParser;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PointListItem : MonoBehaviour {


    public TileVertex vertex;
    public Main controller;
    public int index;

    void Start() {
        transform.GetChild(0).GameObject().GetComponent<TextMeshProUGUI>().text = vertex.heightChannel.ToString() + " : " + vertex.vertexPosition;
    }

    void Update() {
        
    }

    public void Selected() {
        controller.selectedListItem = index;
    }


}
