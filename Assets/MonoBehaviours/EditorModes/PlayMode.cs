

using UnityEngine;

public class PlayMode : EditMode {

    public Main main { get; set; }

    PlayModePlayer player;
    Camera playerCamera;

    public PlayMode(Main main) {
        this.main = main;
    }

    public void OnCreateMode() {

        main.DisableMainCamera();

        var camObj = Object.Instantiate(main.playerCamera);
        playerCamera = camObj.GetComponent<Camera>();
        
        var playerObj = Object.Instantiate(main.playModePlayer);
        player = playerObj.GetComponent<PlayModePlayer>();

    }

    public void OnDestroy() {

        main.EnableMainCamera();

    }

    public void Update() {
        UpdateCamPos();
    }

    void UpdateCamPos() {

        playerCamera.transform.position = player.transform.position;
        playerCamera.transform.localEulerAngles = player.transform.localEulerAngles;

        //var maxHeight = 8.4999996f;
        var ratio = 0.9f;

        var yFromBottom = player.transform.position.y + 4.2666666f;

        playerCamera.transform.position -= playerCamera.transform.forward * 11.36f;
        playerCamera.transform.position += playerCamera.transform.up * 8.5f;
        playerCamera.transform.localEulerAngles = new Vector3(30f, playerCamera.transform.localEulerAngles.y, playerCamera.transform.localEulerAngles.z);
        playerCamera.transform.position += playerCamera.transform.forward * (yFromBottom * ratio);

    }

}