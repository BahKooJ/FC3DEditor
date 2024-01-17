using FCopParser;
using UnityEngine;

public class HeightMapChannelPoint : MonoBehaviour {

    public GameObject setHeightTextField;

    public bool isStatic = false;
    public HeightMapEditMode controller;
    public HeightPoints heightPoints;
    public int channel;
    public LevelMesh section;

    public bool isSelected = false;

    public BoxCollider boxCollider;
    Material material;

    public bool preInitSelect = false;

    bool click = false;
    Vector3 previousMousePosition;

    void Start() {

        boxCollider = GetComponent<BoxCollider>();

        material = GetComponent<MeshRenderer>().material;

        if (preInitSelect) {
            material.color = Color.white;
        } else {
            ResetColors();
        }

    }


    void Update() {

        if (isStatic) {
            return;
        }
        
        if (click) {

            if (Controls.OnUp("Select")) {

                click = false;
                section.RefreshMesh();
                controller.RefreshSelectedOverlays();

            }

            var distance = (Input.mousePosition.y - previousMousePosition.y) / 40f;

            if (isSelected) {
                controller.MoveAllHeights(distance);
            } else {
                MoveHeight(distance);
            }

            previousMousePosition = Input.mousePosition;

        }

    }

    public void MoveHeight(float distance) {

        heightPoints.AddToPoint(distance, channel);

        transform.position = new Vector3(transform.position.x, heightPoints.GetPoint(channel), transform.position.z);

        if (HeightMapEditMode.keepHeightsOnTop) {

            KeepHigherChannelsOnTop();

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

    public void KeepHigherChannelsOnTop() {

        var padding = 8;
        var gameCoordsPadding = padding / HeightPoints.multiplyer;

        if (channel == 3) {
            return;
        }

        if (channel == 1) {

            if (heightPoints.GetPoint(channel) + gameCoordsPadding > heightPoints.GetPoint(2)) {
                heightPoints.SetPoint(heightPoints.GetTruePoint(channel) + padding, 2);
            }

            if (heightPoints.GetPoint(2) + gameCoordsPadding > heightPoints.GetPoint(3)) {
                heightPoints.SetPoint(heightPoints.GetTruePoint(2) + padding, 3);
            }

        }
        if (channel == 2) {

            if (heightPoints.GetPoint(channel) + gameCoordsPadding > heightPoints.GetPoint(3)) {
                heightPoints.SetPoint(heightPoints.GetTruePoint(channel) + padding, 3);
            }

        }

        foreach (var point in controller.heightPointObjects) {
            point.RefreshHeight();
        }

    }

    public void SelectOrDeSelect() {

        if (isSelected) {
            DeSelect();
        } else {
            Select();
        }

    }

    public void Select() {

        isSelected = true;

        if (material == null) {
            preInitSelect = true;
        } else {
            material.color = Color.white;
        }

        controller.lastSelectedHeightChannel = this;

    }

    public void DeSelect() {

        isSelected = false;

        if (material == null) {
            preInitSelect = false;
        } else {
            ResetColors();
        }

    }

    public void MoveTileChannelUpOrDown() {

        //TODO: Change how single vertex change is made
        //float axis = Input.GetAxis("Mouse ScrollWheel");
        //if (axis != 0) {

        //    if (controller.selectedTiles.Count == 1) {

        //        var tile = controller.selectedTiles[0];

        //        var vertexIndex = tile.verticies.IndexOf(new TileVertex(channel, corner));

        //        if (vertexIndex != -1) {

        //            if (axis > 0) {

        //                if (tile.verticies[vertexIndex].heightChannel < 3) {

        //                    var vertex = tile.verticies[vertexIndex];

        //                    vertex.heightChannel++;

        //                    if (tile.verticies.Contains(vertex)) {
        //                        return;
        //                    }

        //                    tile.verticies[vertexIndex] = vertex;

        //                    controller.selectedSection.RefreshMesh();
        //                    controller.RefreshSelectedOverlays();

        //                }

        //            } else {

        //                if (tile.verticies[vertexIndex].heightChannel > 1) {

        //                    var vertex = tile.verticies[vertexIndex];

        //                    vertex.heightChannel--;

        //                    if (tile.verticies.Contains(vertex)) {
        //                        return;
        //                    }

        //                    tile.verticies[vertexIndex] = vertex;

        //                    controller.selectedSection.RefreshMesh();
        //                    controller.RefreshSelectedOverlays();

        //                }

        //            }

        //        }

        //    }

        //}

    }

    public void Click() {

        click = true;
        previousMousePosition = Input.mousePosition;

    }

    public void ChangeExactHeight() {

        Select();

        var mainUI = FindObjectOfType<HeightMapEditPanelView>();

        var obj = Instantiate(setHeightTextField);

        obj.transform.SetParent(mainUI.GetComponentInParent<Canvas>().rootCanvas.transform, false);

        var script = obj.GetComponent<SetHeightValueTextField>();

        script.controller = controller;

        script.selelctedHeightObject = this;

    }

}
