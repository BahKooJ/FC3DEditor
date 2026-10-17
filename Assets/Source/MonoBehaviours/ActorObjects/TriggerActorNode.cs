

using FCopParser;
using UnityEngine;

public class TriggerActorNode : ActorEditingNode {

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

        var widthProp = (NormalizedValueProperty)actor.behavior.propertiesByName["Width Area"];
        var lengthProp = (NormalizedValueProperty)actor.behavior.propertiesByName["Length Area"];
        var heightProp = (NormalizedValueProperty)actor.behavior.propertiesByName["Height Area"];

        triggerAreaTrans.localScale = new Vector3(widthProp.value, heightProp.value, lengthProp.value);

    }

    public override void SetToPosition() {

        var pos = actorObject.transform.position;
        pos.y -= actorObject.yPadding;
        //pos.y += triggerAreaTrans.localScale.y / 2f;
        transform.position = pos;

    }

}