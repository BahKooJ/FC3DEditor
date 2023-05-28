using System;
using UnityEngine;

public class AxisControl: MonoBehaviour {

    public GameObject axisX;
    public GameObject axisY;
    public GameObject axisZ;

    enum Axis {
        AxisX,
        AxisY,
        AxisZ,
        None
    }

    Axis click = Axis.None;
    Vector2 previousMouse = Vector2.zero;

    bool rotate = false;

    public GameObject controlledObject;
    public Func<Vector3, bool> moveCallback = (par) => { return false; };
    public Func<float, bool> rotateCallback = (par) => { return false; };


    void Start() {

        transform.position = controlledObject.transform.position;

    }

    void Update() {

        if (Controls.OnDown("Rotate")) {
            rotate = true;
        }
        if (Controls.OnUp("Rotate")) {
            rotate = false;
        }

        if (rotate) {

            rotateCallback(previousMouse.x - Input.mousePosition.x);

            previousMouse = Input.mousePosition;

        }

        if (Controls.IsDown("Select")) {

            if (click == Axis.None) {

                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 8)) {

                    if (hit.colliderInstanceID == axisX.GetComponent<CapsuleCollider>().GetInstanceID()) {
                        click = Axis.AxisX;
                    } else if (hit.colliderInstanceID == axisY.GetComponent<CapsuleCollider>().GetInstanceID()) {
                        click = Axis.AxisY;
                    } else if (hit.colliderInstanceID == axisZ.GetComponent<CapsuleCollider>().GetInstanceID()) {
                        click = Axis.AxisZ;
                    }

                }

                previousMouse = Input.mousePosition;

                return;

            }

            var pos = transform.position;

            //TODO: Move should scale and keep up with cursor
            switch (click) {
                case Axis.AxisX:

                    pos.x += (previousMouse.x - Input.mousePosition.x) / 50f;

                    break;
                case Axis.AxisY:

                    pos.y += (Input.mousePosition.y - previousMouse.y) / 50f;

                    break;
                case Axis.AxisZ:

                    pos.z += (Input.mousePosition.x - previousMouse.x) / 50f;

                    break;
            }

            transform.position = pos;

            if (moveCallback(pos)) {
                transform.position = controlledObject.transform.position;
            } else {
                controlledObject.transform.position = pos;
            }

            previousMouse = Input.mousePosition;

        } else {
            click = Axis.None;
        }

    }

}