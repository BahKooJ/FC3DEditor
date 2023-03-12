

using FCopParser;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ActorObject : MonoBehaviour {

    public Collider actCollider;

    public ActorEditMode controller;

    public FCopActor actor;

    public void ChangePosition(Vector3 pos) {

        actor.x = Mathf.RoundToInt(pos.x * 8192f);
        actor.y = Mathf.RoundToInt(pos.z * -8192f);

        Create();

    }

    public void ChangeRotation() {

    }


    public void Create() {

        transform.position = new Vector3(actor.x / 8192f, 100f, -(actor.y / 8192f));

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, 1)) {

            var pos = transform.position;

            pos.y = hit.point.y;

            transform.position = pos;

        } else {
            print("No floor found");
        }

    }

    void Start() {

        Create();

    }

}