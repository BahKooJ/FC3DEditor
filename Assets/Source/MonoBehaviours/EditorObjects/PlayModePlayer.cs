

using UnityEngine;

public class PlayModePlayer : MonoBehaviour {

    public PlayMode controller;

    float moveSpeed = 8f;
    float turnSpeed = 90f;

    public bool placed = false;

    void Update() {

        if (!placed) {
            var pos = controller.main.CursorOnLevelMesh();

            if (pos != null) {
                transform.position = (Vector3)pos;
            }

            if (Input.GetMouseButtonDown(0)) {
                placed = true;
                controller.Place();
            }

            return;
        }

        if (Controls.IsDown("CameraForward")) {
            transform.position += moveSpeed * Time.deltaTime * transform.forward;
        }
        if (Controls.IsDown("CameraBack")) {
            transform.position -= moveSpeed * Time.deltaTime * transform.forward;
        }
        if (Controls.IsDown("CameraLeft")) {
            float newRotationX = transform.localEulerAngles.y - turnSpeed * Time.deltaTime;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, newRotationX, 0f);
        }
        if (Controls.IsDown("CameraRight")) {
            float newRotationX = transform.localEulerAngles.y + turnSpeed * Time.deltaTime;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, newRotationX, 0f);
        }
        if (Controls.IsDown("CameraUp")) {
            transform.position += moveSpeed * Time.deltaTime * transform.right;
        }
        if (Controls.IsDown("CameraDown")) {
            transform.position -= moveSpeed * Time.deltaTime * transform.right;
        }

        transform.position = new Vector3(transform.position.x, 100f, transform.position.z);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, 1)) {

            var pos = transform.position;

            pos.y = hit.point.y;

            transform.position = pos;

        }
        else {
            print("No floor found");
        }

    }

}