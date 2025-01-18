
using UnityEngine;

public class ObjectEditorCamera : MonoBehaviour {

    public Transform orbitPoint;

    float zoomDistance = 10f;

    private void Start() {
       

    }

    Vector3 previousMousePos = Vector3.zero;
    private void Update() {

        if (Input.GetMouseButton(2)) {

            if (previousMousePos == Vector3.zero) {
                previousMousePos = Input.mousePosition;
            }

            if (Input.GetKey(KeyCode.LeftShift)) {

                // Why 600? idk that was just the best multiplier I found
                var xMove = (Input.mousePosition.x - previousMousePos.x) * (zoomDistance / 600f);
                var yMove = (Input.mousePosition.y - previousMousePos.y) * (zoomDistance / 600f);

                orbitPoint.transform.position += -transform.right * xMove;
                orbitPoint.transform.position -= transform.up * yMove;

                transform.position = orbitPoint.position + -transform.forward * zoomDistance;

            }
            else {

                transform.RotateAround(orbitPoint.position, transform.up, Input.mousePosition.x - previousMousePos.x);
                transform.RotateAround(orbitPoint.position, -transform.right, Input.mousePosition.y - previousMousePos.y);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

            }




            previousMousePos = Input.mousePosition;

        }

        if (Input.GetMouseButtonUp(2)) {
            previousMousePos = Vector3.zero;
        }

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0) {

            zoomDistance += -axis * 4f;

            if (zoomDistance < 0) {
                zoomDistance = 0;
            }

            transform.position = orbitPoint.position + -transform.forward * zoomDistance;

        }


    }

}