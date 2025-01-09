
using UnityEngine;

public class ObjectEditorCamera : MonoBehaviour {

    Vector3 orbitPoint = Vector3.zero;

    float zoomDistance = 10f;

    private void Start() {
        
        orbitPoint = transform.position + Vector3.forward * zoomDistance;

    }

    Vector3 previousMousePos = Vector3.zero;
    private void Update() {

        if (Input.GetMouseButton(2)) {

            if (previousMousePos == Vector3.zero) {
                previousMousePos = Input.mousePosition;
            }

            transform.RotateAround(orbitPoint, transform.up, Input.mousePosition.x - previousMousePos.x);
            transform.RotateAround(orbitPoint, -transform.right, Input.mousePosition.y - previousMousePos.y);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);


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

            transform.position = orbitPoint + -transform.forward * zoomDistance;

        }


    }

}