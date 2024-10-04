

using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class ActorGroupObject : MonoBehaviour {


    // - Parameters -
    public ActorEditMode controller;
    public List<ActorObject> actObjects = new();

    public void Init() {
        SetToCurrentPosition();

        foreach (var obj in actObjects) {
            obj.transform.localPosition = Vector3.zero;
        }

    }

    public void ChangePosition(Vector3 pos) {

        var actors = new List<FCopActor>();

        foreach (var obj in actObjects) {
            actors.Add(obj.actor);
            obj.transform.localPosition = Vector3.zero;
        }

        controller.ChangeActorsPosition(actors, pos);

        // The other actor objects are children to the group,
        // which means we don't need to update their position, just this one.
        SetToCurrentPosition();

    }

    public void SetToCurrentPosition() {

        var firstObj = actObjects[0];

        foreach (var obj in actObjects) {

            if (obj.actor.x != firstObj.actor.x && obj.actor.y != firstObj.actor.y) {
                Debug.LogWarning("Grouped actor positions are desynced");
            }

        }

        transform.position = new Vector3(firstObj.actor.x / 8192f, 100f, -(firstObj.actor.y / 8192f));


        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, 1)) {

            var pos = transform.position;

            pos.y = hit.point.y;

            transform.position = pos;

        }
        else {
            print("No floor found");
        }

    }

}