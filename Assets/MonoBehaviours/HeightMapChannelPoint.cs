using FCopParser;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Collections.Specialized.BitVector32;

public class HeightMapChannelPoint : MonoBehaviour {

    public GameObject setHeightTextField;

    public HeightPoint heightPoint;
    public int channel;
    public LevelMesh section;
    Ray ray;
    RaycastHit hit;
    BoxCollider boxCollider;

    bool click = false;
    Vector3 previousMousePosition;

    void Start() {

        boxCollider = GetComponent<BoxCollider>();

    }


    void Update() {
        
        if (FreeMove.looking) {
            return;
        }

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {

            if (Input.GetMouseButtonDown(0)) {

                if (hit.colliderInstanceID == boxCollider.GetInstanceID()) {

                    click = true;
                    previousMousePosition = Input.mousePosition;

                }

            } else if (Input.GetMouseButtonDown(1)) {

                if (hit.colliderInstanceID == boxCollider.GetInstanceID()) {


                    var mainUI = FindObjectOfType<MainUI>();

                    var obj = Instantiate(setHeightTextField);

                    obj.transform.SetParent(mainUI.transform);

                    var script = obj.GetComponent<SetHeightValueTextField>();

                    script.heightPoint = heightPoint;
                    script.channel = channel;
                    script.geoPoint = this;
                    script.section = section;

                }

            }

        }

        if (click) {

            if (Input.GetMouseButtonUp(0)) {

                click = false;
                section.RefreshMesh();

            }

            var distance = (Input.mousePosition.y - previousMousePosition.y) / 40f;

            transform.position = new Vector3(transform.position.x, heightPoint.GetPoint(channel), transform.position.z);

            heightPoint.AddToPoint(distance, channel);

            previousMousePosition = Input.mousePosition;

        }

    }
}
