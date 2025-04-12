

using FCopParser;
using UnityEngine;

public class TeleporterLocationEditingNode : ActorEditingNode {

    // - Prefab -
    public GameObject editingNodeLine;

    // - Parameters -
    public NormalizedValueProperty propertyX;
    public NormalizedValueProperty propertyY;

    LineRenderer lineToActor;

    void Start() {
        SetToPosition();

        var obj = Instantiate(editingNodeLine);
        obj.transform.SetParent(transform, false);

        lineToActor = obj.GetComponent<LineRenderer>();

        lineToActor.SetPosition(0, transform.position);
        lineToActor.SetPosition(1, actorObject.transform.position);

    }

    void Update() {

        lineToActor.SetPosition(0, transform.position);
        lineToActor.SetPosition(1, actorObject.transform.position);

    }

    public override void SetToPosition() {

        var pos = new Vector3(
            propertyX.value,
            ObjectUtil.GroundCast(ActorGroundCast.Highest, new Vector2(propertyX.value, -propertyY.value)) + 0.5f,
            -propertyY.value);

        transform.position = pos;
    }

    public override void OnPositionChange(Vector3 position, AxisControl.Axis axis) {

        propertyX.Set(position.x);
        propertyY.Set(-position.z);

        controller.view.activeActorPropertiesView.RequestPropertyRefresh(propertyX);
        controller.view.activeActorPropertiesView.RequestPropertyRefresh(propertyY);

        SetToPosition();

    }

}