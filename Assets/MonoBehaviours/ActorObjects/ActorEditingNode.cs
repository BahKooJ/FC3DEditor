
using FCopParser;
using System.Collections.Generic;
using UnityEngine;

public class ActorEditingNode : MonoBehaviour {

    // - Unity Refs -
    public Collider nodeCollider;

    // - Parameters -
    public ActorEditMode controller;
    public FCopActor actor;
    public List<ActorProperty> controlledProperties = new();

    virtual public void SetToPosition() {

    }

    virtual public void OnPositionChange(Vector3 position, AxisControl.Axis axis) {

    }

}