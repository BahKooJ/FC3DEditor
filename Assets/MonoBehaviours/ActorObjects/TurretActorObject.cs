
using FCopParser;
using UnityEngine;

public class TurretActorObject : ActorObject {


    private void Start() {

        Create();

        var turret = (FCopTurretActor)actor;

        transform.rotation = Quaternion.Euler(0f, (turret.rotation / 4096f) * 360, 0f);

    }

}