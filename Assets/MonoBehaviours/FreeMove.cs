using UnityEngine;

/// <summary>
/// A simple free camera to be added to a Unity game object.
/// 
/// Keys:
///	wasd / arrows	- movement
///	q/e 			- up/down (local space)
///	r/f 			- up/down (world space)
///	pageup/pagedown	- up/down (world space)
///	hold shift		- enable fast movement mode
///	right mouse  	- enable free look
///	mouse			- free look / rotation
///     
/// </summary>
public class FreeMove : MonoBehaviour {
    /// <summary>
    /// Normal speed of camera movement.
    /// </summary>
    public float movementSpeed = 10f;

    /// <summary>
    /// Speed of camera movement when shift is held down,
    /// </summary>
    public float fastMovementSpeed = 100f;

    /// <summary>
    /// Sensitivity for free look.
    /// </summary>
    public float freeLookSensitivity = 3f;

    /// <summary>
    /// Amount to zoom the camera when using the mouse wheel.
    /// </summary>
    public float zoomSensitivity = 10f;

    /// <summary>
    /// Amount to zoom the camera when using the mouse wheel (fast mode).
    /// </summary>
    public float fastZoomSensitivity = 50f;

    /// <summary>
    /// Set to true when free looking (on right mouse button).
    /// </summary>
    static public bool looking = false;

    void Update() {

        if (Main.ignoreAllInputs) { return; }

        var fastMode = false;
        var movementSpeed = fastMode ? this.fastMovementSpeed : this.movementSpeed;

        if (Controls.IsDown("CameraLeft")) {
            transform.position = transform.position + (-transform.right * movementSpeed * Time.deltaTime);
        }

        if (Controls.IsDown("CameraRight")) {
            transform.position = transform.position + (transform.right * movementSpeed * Time.deltaTime);
        }

        if (Controls.IsDown("CameraForward")) {
            transform.position = transform.position + (transform.forward * movementSpeed * Time.deltaTime);
        }

        if (Controls.IsDown("CameraBack")) {
            transform.position = transform.position + (-transform.forward * movementSpeed * Time.deltaTime);
        }

        if (Controls.IsDown("CameraUp")) {
            transform.position = transform.position + (transform.up * movementSpeed * Time.deltaTime);
        }

        if (Controls.IsDown("CameraDown")) {
            transform.position = transform.position + (-transform.up * movementSpeed * Time.deltaTime);
        }

        if (Controls.IsDown("CameraWorldUp")) {
            transform.position = transform.position + (Vector3.up * movementSpeed * Time.deltaTime);
        }

        if (Controls.IsDown("CameraWorldDown")) {
            transform.position = transform.position + (-Vector3.up * movementSpeed * Time.deltaTime);
        }

        if (looking) {
            float newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
            float newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;

            if (newRotationY > 90f && newRotationY < 180) {
                newRotationY = 90f;
            }
            if (newRotationY < 270f && newRotationY > 180) {
                newRotationY = 270f;
            }

            transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
        }

        //float axis = Input.GetAxis("Mouse ScrollWheel");
        //if (axis != 0) {
        //    var zoomSensitivity = fastMode ? this.fastZoomSensitivity : this.zoomSensitivity;
        //    transform.position = transform.position + transform.forward * axis * zoomSensitivity;
        //}

        if (Controls.OnDown("CameraEnable")) {

            if (looking) {
                StopLooking();
            } else {
                StartLooking();
            }

        }

    }

    void OnDisable() {
        StopLooking();
    }

    /// <summary>
    /// Enable free looking.
    /// </summary>
    public void StartLooking() {
        looking = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Disable free looking.
    /// </summary>
    public void StopLooking() {
        looking = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}