

using FCopParser;
using UnityEngine;

public class UniversalTriggerActorNode : ActorEditingNode {

    // - Unity Refs -
    public Transform triggerAreaTrans;

    void Start() {

        Refresh();

        SetToPosition();

    }

    private void Update() {

        SetToPosition();

    }

    public override void Refresh() {

        var radiusProp = (NormalizedValueProperty)actor.behavior.propertiesByName["Interact Radius"];

        triggerAreaTrans.localScale = new Vector3(radiusProp.value, radiusProp.value, radiusProp.value);

    }

    public override void SetToPosition() {

        var pos = actorObject.transform.position;
        pos.y -= actorObject.yPadding;
        //pos.y += triggerAreaTrans.localScale.y / 2f;
        transform.position = pos;

    }

}