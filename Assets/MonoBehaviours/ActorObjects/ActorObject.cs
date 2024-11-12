

using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActorObject : MonoBehaviour {

    // - Unity Refs -
    public Collider actCollider;
    public TextFieldPopupHandler renameTextFeild;
    public GameObject placeholderCube;

    // - Parameters -
    public ActorEditMode controller;
    public FCopActor actor;
    [HideInInspector]
    public ActorGroupObject group = null;

    public List<ObjectMesh> objects = new();

    public List<(string, System.Action)> contextMenuItems = new();

    private void Start() {

        renameTextFeild.finishCallback = text => {
            controller.RenameActor(actor, text);
        };

        contextMenuItems.Add(("Rename", () => { 
            renameTextFeild.OpenPopupTextField(actor.name); 
        }));
        contextMenuItems.Add(("Group", () => {
            controller.StartGroup(actor);
        }));
        contextMenuItems.Add(("Ungroup", () => { 
            controller.UngroupActor(actor);
        }
        ));
        contextMenuItems.Add(("Delete", () => { 
            controller.DeleteByID(actor.DataID);
        }));

    }

    public void ChangePosition(Vector3 pos) {

        controller.ChangeActorPosition(actor, pos);

        SetToCurrentPosition();

    }

    public void RefreshRotation() {

        if (actor.behavior == null) {
            return;
        }

        SetRotation();

    }

    public void Create() {

        SetToCurrentPosition();

        foreach (var resRef in actor.resourceReferences) {

            if (resRef.fourCC == FCopActor.FourCC.Cobj) {

                var obj = controller.main.level.objects.First(obj => { return obj.rawFile.dataID == resRef.id; });

                var gameObject = Instantiate(controller.main.ObjectMesh);
                gameObject.transform.SetParent(transform, false);

                var script = gameObject.GetComponent<ObjectMesh>();
                script.levelTexturePallet = controller.main.levelTexturePallet;
                script.fCopObject = obj;
                script.textureOffset = SetTextureOffset();
                script.Create();

                if (!script.failed) {

                    placeholderCube.SetActive(false);
                    actCollider = null;

                    objects.Add(script);

                } else {

                    Debug.LogWarning("Object on Actor: " + actor.behaviorType + " failed to create");

                    objects.Add(null);

                }

            }
            else {

                objects.Add(null);

            }

        }

        if (actor.behavior is FCopBehavior36 || actor.behavior is FCopBehavior8) {

            var headPos = objects[0].transform.localPosition;
            headPos.y = objects[2].maxY;
            objects[0].transform.localPosition = headPos;

        }

        SetRotation();

    }

    public void Refresh() {

        foreach (Transform tran in transform) {

            if (tran.gameObject != placeholderCube) {
                DestroyImmediate(tran.gameObject);
            }

        }

        objects.Clear();

        placeholderCube.SetActive(true);

        Create();

    }

    public void SetToCurrentPosition() {

        Vector3 castDirection = Vector3.down;
        float startingHeight = 100f;


        if (actor.behavior is FCopHeightOffseting groundCast) {

            switch (groundCast.GetGroundCast()) {
                case ActorGroundCast.Highest:
                    break;
                case ActorGroundCast.Lowest:
                    castDirection = Vector3.up;
                    startingHeight = -100f;
                    break;
                case ActorGroundCast.Default:
                    break;
            }

        }

        transform.position = new Vector3(actor.x / 8192f, startingHeight, -(actor.y / 8192f));

        if (Physics.Raycast(transform.position, castDirection, out RaycastHit hit, Mathf.Infinity, 1)) {

            var pos = transform.position;

            pos.y = hit.point.y;

            if (actor.behavior is FCopHeightOffseting offset) {
                pos.y += offset.GetHeight() / 8192f;
            }
            
            transform.position = pos;

        }
        else {
            print("No floor found");
        }

    }

    void SetRotation() {

        if (actor.behavior is FCopObjectMutating objectMutating) {

            var rotations = objectMutating.GetRotations();

            foreach (var rot in rotations) {

                foreach (var i in rot.affectedRefIndexes) {

                    var potentialObject = objects[i];

                    if (potentialObject != null) {
                        potentialObject.transform.localRotation = Quaternion.Euler(0f, rot.value.parsedRotation, 0f);
                    }

                }

            }

        }

    }

    int SetTextureOffset() {

        if (actor.behavior is FCopEntity e) {
            return e.uvOffset.value;
        }
        else {
            return 0;
        }
    
    }

}