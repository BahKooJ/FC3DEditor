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

        if (controlledObject.GetComponent<Outline>() != null) {
            return;
        }

        var addComp = controlledObject.AddComponent<Outline>();

        addComp.OutlineMode = Outline.Mode.OutlineVisible;
        addComp.OutlineWidth = 6;
        addComp.OutlineColor = Color.green;

    }

    public void ClearOutlineOnObject() {

        if (controlledObject != null) {

            if (controlledObject.TryGetComponent<Outline>(out var comp)) {

                DestroyImmediate(comp);

            }

        }

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

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, Main.interfaceObjectsMask)) {

                    if (hit.colliderInstanceID == axisX.GetComponent<CapsuleCollider>().GetInstanceID()) {
                        click = Axis.AxisX;
                    }
                    else if (hit.colliderInstanceID == axisY.GetComponent<CapsuleCollider>().GetInstanceID()) {
                        click = Axis.AxisY;
                    }
                    else if (hit.colliderInstanceID == axisZ.GetComponent<CapsuleCollider>().GetInstanceID()) {
                        click = Axis.AxisZ;
                    }

                }

                previousMouse = Input.mousePosition;

                return;

            }

            var pos = transform.position;

            var mousePos = Input.mousePosition;
            mousePos.z = Camera.main.WorldToScreenPoint(axisX.transform.position).z;
            var dragPos = Camera.main.ScreenToWorldPoint(mousePos);

            switch (click) {
                case Axis.AxisX:


                    pos.x = dragPos.x;

                    break;
                case Axis.AxisY:

                    pos.y = dragPos.y;

                    break;
                case Axis.AxisZ:

                    pos.z = dragPos.z;

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