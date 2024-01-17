

using FCopParser;
using UnityEngine;

public class TileHeightMapChannelPoint : MonoBehaviour {

    public TileEditMode controller;
    public HeightPoints heightPoints;
    public int channel;
    public LevelMesh section;

    public bool isSelected = false;

    public BoxCollider boxCollider;
    Material material;

    public bool preInitSelect = false;

    bool click = false;

    void Start() {

        boxCollider = GetComponent<BoxCollider>();

        material = GetComponent<MeshRenderer>().material;

        if (preInitSelect) {
            material.color = Color.white;
        }
        else {
            ResetColors();
        }

    }


    void Update() {

        if (click) {

        }

    }

    public void ResetColors() {
        switch (channel) {
            case 1:
                material.color = Color.blue;
                break;
            case 2:
                material.color = Color.green;
                break;
            case 3:
                material.color = Color.red;
                break;
        }
    }

    public void RefreshHeight() {
        transform.position = new Vector3(transform.position.x, heightPoints.GetPoint(channel), transform.position.z);
    }

    public void SelectOrDeSelect() {

        if (isSelected) {
            DeSelect();
        }
        else {
            Select();
        }

    }

    public void Select() {

    }

    public void DeSelect() {


    }

    public void Click() {

    }

}