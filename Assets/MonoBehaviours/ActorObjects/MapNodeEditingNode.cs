
using FCopParser;
using UnityEngine;

public class MapNodeEditingNode : ActorEditingNode {


    // - Parameters -
    public NormalizedValueProperty propertyX;
    public NormalizedValueProperty propertyY;

    private void Start() {
        SetToPosition();
    }

    void SetToPosition() {

        var pos = new Vector3(
            propertyX.value, 
            ObjectUtil.GroundCast(ActorGroundCast.Highest, new Vector2(propertyX.value, -propertyY.value)), 
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