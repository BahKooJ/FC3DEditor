
using FCopParser;
using System.Linq;
using UnityEngine;

public class MovablePropStop : ActorEditingNode {

    // - Unity Refs -
    public GameObject placeHolderObject;

    // - Parameter-
    [HideInInspector]
    public EnumDataActorProperty moveAxis;
    public NormalizedValueProperty moveOffset;
    public RangeActorProperty rotationOffset;
    public RangeActorProperty rotation;


    GameObject objectMesh;

    void Start() {

        try {
            var obj = controller.main.level.objects.First(obj => { return obj.rawFile.dataID == actor.resourceReferences[0].id; });

            objectMesh = Instantiate(controller.main.ObjectMesh);
            objectMesh.transform.SetParent(transform, false);

            var objMesh = objectMesh.GetComponent<ObjectMesh>();
            objMesh.levelTexturePallet = controller.main.levelTexturePallet;
            objMesh.fCopObject = obj;
            objMesh.ForceMake();

            nodeCollider = objMesh.meshCollider;
            placeHolderObject.SetActive(false);
        }
        catch {

        }

        SetToPosition();

    }

    private void Update() {

        // YES YES I KNOW THIS SHOULD NOT BE UPDATING EVERY FRAME BUT IM DOING IT ANYWAYS
        SetToPosition();

    }

    public override void OnPositionChange(Vector3 position, AxisControl.Axis axis) {

        switch ((MoveablePropMoveAxis)moveAxis.GetCompiledValue()) {

            case MoveablePropMoveAxis.PositionZ:
                moveOffset.Set(position.z - actorObject.transform.position.z);
                controller.view.activeActorPropertiesView.RequestPropertyRefresh(moveOffset);
                break;
            case MoveablePropMoveAxis.PositionX:
                moveOffset.Set(position.x - actorObject.transform.position.x);
                controller.view.activeActorPropertiesView.RequestPropertyRefresh(moveOffset);
                break;
            case MoveablePropMoveAxis.PositionY:
                moveOffset.Set(position.y - actorObject.transform.position.y);
                controller.view.activeActorPropertiesView.RequestPropertyRefresh(moveOffset);
                break;

        }

    }

    public override void SetToPosition() {

        var pos = actorObject.transform.position;

        switch ((MoveablePropMoveAxis)moveAxis.GetCompiledValue()) {
            case MoveablePropMoveAxis.RotationY:
                if (objectMesh != null) {
                    var objRot = objectMesh.transform.localRotation.eulerAngles;

                    objRot.y = rotationOffset.value + rotation.value;

                    objectMesh.transform.localRotation = Quaternion.Euler(objRot);
                }
                break;
            case MoveablePropMoveAxis.PositionZ:

                if (actorObject.objects[0] != null) {
                    pos -= actorObject.objects[0].transform.right * moveOffset.value;
                }

                if (objectMesh != null) {
                    var objRot = objectMesh.transform.localRotation.eulerAngles;

                    objRot.y = rotation.value;

                    objectMesh.transform.localRotation = Quaternion.Euler(objRot);
                }

                break;
            case MoveablePropMoveAxis.PositionX:

                if (actorObject.objects[0] != null) {
                    pos -= actorObject.objects[0].transform.forward * moveOffset.value;
                }

                if (objectMesh != null) {
                    var objRot = objectMesh.transform.localRotation.eulerAngles;

                    objRot.y = rotation.value;

                    objectMesh.transform.localRotation = Quaternion.Euler(objRot);
                }

                break;
            case MoveablePropMoveAxis.PositionY:
                pos.y += moveOffset.value;

                if (objectMesh != null) {
                    var objRot = objectMesh.transform.localRotation.eulerAngles;

                    objRot.y = rotation.value;

                    objectMesh.transform.localRotation = Quaternion.Euler(objRot);
                }

                break;
            case MoveablePropMoveAxis.RotationX:
                if (objectMesh != null) {
                    var objRot = objectMesh.transform.localRotation.eulerAngles;

                    objRot.x = rotationOffset.value + rotation.value;

                    objectMesh.transform.localRotation = Quaternion.Euler(objRot);
                }
                break;
            case MoveablePropMoveAxis.RotationZ:
                if (objectMesh != null) {
                    var objRot = objectMesh.transform.localRotation.eulerAngles;

                    objRot.z = rotationOffset.value + rotation.value;

                    objectMesh.transform.localRotation = Quaternion.Euler(objRot);
                }
                break;
        }

        transform.position = pos;

    }

}