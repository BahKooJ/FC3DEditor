

using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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

        SetToCurrentPosition();

        foreach (var obj in actObjects) {
            obj.ChangePosition(pos);
            obj.transform.localPosition = Vector3.zero;
        }

    }

    void SetToCurrentPosition() {

        var firstObj = actObjects[0];

        transform.position = new Vector3(firstObj.actor.x / 8192f, 100f, -(firstObj.actor.y / 8192f));

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, 1)) {

            var pos = transform.position;

            pos.y = hit.point.y;

            transform.position = pos;

        }
        else {
            print("No floor found");
        }

    }

}