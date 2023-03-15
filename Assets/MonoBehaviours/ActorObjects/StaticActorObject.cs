

using FCopParser;
using UnityEngine;

public class StaticActorObject : ActorObject {


    void Start() {

        Create();

        var prop = (FCopStaticPropActor)actor;

        transform.rotation = Quaternion.Euler(0f, prop.rotation.parsedRotation, 0f);

    }

}