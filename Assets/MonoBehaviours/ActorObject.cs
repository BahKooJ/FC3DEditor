

using FCopParser;
using UnityEngine;

public class ActorObject : MonoBehaviour {

    public ActorEditMode controller;

    public FCopActor actor;

    void Start() {

        transform.position = new Vector3(actor.x / 8192f, 100f, -(actor.y / 8192f));

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, 1)) {

            var pos = transform.position;

            pos.y = hit.point.y;

            transform.position = pos;

        } else {
            print("No floor found");
        }

        if (actor is FCopTurretActor) {
            var turret = (FCopTurretActor)actor;

            transform.rotation = Quaternion.Euler(0f, (turret.rotation / 4096f) * 360, 0f);

        }

    }

}