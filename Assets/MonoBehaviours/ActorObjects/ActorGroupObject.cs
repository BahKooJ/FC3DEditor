

using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class ActorGroupObject : MonoBehaviour {


    // - Parameters -
    public ActorEditMode controller;
    public List<ActorObject> actObjects = new();

    public void Init() {

        foreach (var obj in actObjects) {
            obj.transform.localPosition = Vector3.zero;
        }

        SetToCurrentPosition();

    }

    public void ChangePosition(Vector3 pos, AxisControl.Axis axis) {

        var actors = new List<FCopActor>();

        foreach (var obj in actObjects) {
            actors.Add(obj.actor);
            obj.transform.localPosition = Vector3.zero;
        }

        controller.ChangeActorsPositionFromGroup(actors, pos);

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

            obj.SetToOnlyY();

        }

        transform.position = new Vector3(firstObj.actor.x / 8192f, 0, -(firstObj.actor.y / 8192f));


    }

}