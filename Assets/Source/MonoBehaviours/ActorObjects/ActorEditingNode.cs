
using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class ActorEditingNode : MonoBehaviour {

    // - Unity Refs -
    public Collider nodeCollider;

    // - Parameters -
    public ActorEditMode controller;
    public FCopActor actor;
    [HideInInspector]
    public ActorObject actorObject;
    public List<ActorProperty> controlledProperties = new();

    virtual public void SetToPosition() {

    }

    virtual public void OnPositionChange(Vector3 position, AxisControl.Axis axis) {

    }

    virtual public void Refresh() {


    }

}