using FCopParser;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Collections.Specialized.BitVector32;

public class HeightMapChannelPoint : MonoBehaviour {

    public GameObject setHeightTextField;

    public GeometryEditMode controller;
    public HeightPoint heightPoint;
    public int channel;
    public LevelMesh section;

    public bool isSelected = false;

    Ray ray;
    RaycastHit hit;
    BoxCollider boxCollider;
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
        
        if (FreeMove.looking) {
            return;
        }

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {

            if (hit.colliderInstanceID == boxCollider.GetInstanceID()) {

                if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift)) {
                    Select();
                }

                if (Input.GetMouseButtonDown(0)) {

                    click = true;
                    previousMousePosition = Input.mousePosition;

                } else if (Input.GetMouseButtonDown(1)) {

                    Select();

                    var mainUI = FindObjectOfType<GeometryEditorUI>();

                    var obj = Instantiate(setHeightTextField);

                    obj.transform.SetParent(mainUI.transform);

                    var script = obj.GetComponent<SetHeightValueTextField>();

                    script.controller = controller;

                }

            }

        }

        if (click) {

            if (Input.GetMouseButtonUp(0)) {

                click = false;
                section.RefreshMesh();
                controller.RefreshSelectedOverlays();

            }

            var distance = (Input.mousePosition.y - previousMousePosition.y) / 40f;

            transform.position = new Vector3(transform.position.x, heightPoint.GetPoint(channel), transform.position.z);

            heightPoint.AddToPoint(distance, channel);

            previousMousePosition = Input.mousePosition;

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

    public void Select() {
        isSelected = true;
        if (material == null) {
            preInitSelect = true;
        } else {
            material.color = Color.white;
        }
    }

}
