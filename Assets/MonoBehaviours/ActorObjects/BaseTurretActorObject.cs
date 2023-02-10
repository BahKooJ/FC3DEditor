

using FCopParser;
using UnityEngine;

public class BaseTurretActorObject : ActorObject {

    public MeshRenderer sphereRender;

    private void Start() {

        Create();

        var baseTurret = (FCopBaseTurretActor)actor;

        sphereRender.material.color = baseTurret.hostileTowards == Team.BLUE ? Color.red : Color.blue;

        transform.rotation = Quaternion.Euler(0f, (baseTurret.rotation / 4096f) * 360, 0f);

    }

}