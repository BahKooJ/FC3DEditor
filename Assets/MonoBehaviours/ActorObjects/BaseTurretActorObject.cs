

using FCopParser;
using UnityEngine;

public class BaseTurretActorObject : ActorObject {

    public MeshRenderer sphereRender;

    private void Start() {

        Create();

        var baseTurret = (FCopBaseTurretActor)actor;

        sphereRender.material.color = baseTurret.hostileTowards == Team.BLUE ? Color.red : Color.blue;

        transform.rotation = Quaternion.Euler(0f, baseTurret.rotation.parsedRotation, 0f);

    }

    public override void ChangeRotation(float y) {

        var baseTurret = (FCopBaseTurretActor)actor;

        baseTurret.rotation += y;

        transform.rotation = Quaternion.Euler(0f, baseTurret.rotation.parsedRotation, 0f);

    }

}