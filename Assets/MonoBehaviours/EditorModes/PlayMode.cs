

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

        playerCamera.transform.position -= playerCamera.transform.forward * 9f;
        playerCamera.transform.position += playerCamera.transform.up * 7f;

        playerCamera.transform.localEulerAngles = new Vector3(30f, playerCamera.transform.localEulerAngles.y, playerCamera.transform.localEulerAngles.z);


    }

}