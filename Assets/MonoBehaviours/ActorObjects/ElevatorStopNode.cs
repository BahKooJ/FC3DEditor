
using FCopParser;
using System.Linq;
using UnityEngine;

public class ElevatorStopNode : ActorEditingNode {

    // - Unity Refs -
    public GameObject placeHolderObject;

    // - Parameter-
    [HideInInspector]
    public int stopIndex;
    public NormalizedValueProperty heightOffset;

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

        if (axis == AxisControl.Axis.AxisY) {

            // Height offset is relative to ground cast
            heightOffset.Set(position.y - ObjectUtil.GroundCast(ActorGroundCast.Highest, new Vector2(actor.x / 8192f, -actor.y / 8192f)));
            controller.view.activeActorPropertiesView.RequestPropertyRefresh(heightOffset);

        }


    }

    public override void SetToPosition() {

        var pos = new Vector3(
            actor.x / 8192f,
            ObjectUtil.GroundCast(ActorGroundCast.Highest, new Vector2(actor.x / 8192f, -actor.y / 8192f)),
            -actor.y / 8192f);

        pos.y += heightOffset.value;

        transform.position = pos;

        if (actor.behavior is FCopObjectMutating objectMutating) {

            foreach (var mutation in objectMutating.GetMutations()) {

                if (objectMesh != null) {

                    var objRot = objectMesh.transform.localRotation.eulerAngles;

                    objRot.x = -mutation.rotationX;
                    objRot.y = mutation.rotationY;
                    objRot.z = -mutation.rotationZ;

                    objectMesh.transform.localRotation = Quaternion.Euler(objRot);

                    var objScale = objectMesh.transform.localScale;

                    objScale.x = -mutation.scaleX;
                    objScale.y = mutation.scaleY;
                    objScale.z = -mutation.scaleZ;

                    objectMesh.transform.localScale = objScale;

                }

            }

        }

    }

}