

using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActorObject : MonoBehaviour {

    // - Prefabs -
    public GameObject missingObjectPrefab;

    // - Unity Refs -
    public Collider actCollider;
    public TextFieldPopupHandler renameTextFeild;
    public GameObject placeholderObject;

    // - Parameters -
    public ActorEditMode controller;
    public FCopActor actor;
    [HideInInspector]
    public ActorGroupObject group = null;

    [HideInInspector]
    public List<ObjectMesh> objects = new();
    [HideInInspector]
    public GameObject missingObjectGameobj;

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

        if (actor.behavior is FCopObjectMutating) {

            if (missingObjectGameobj == null) {
                var obj = Instantiate(missingObjectPrefab);
                obj.transform.SetParent(transform, false);
                missingObjectGameobj = obj;
                missingObjectGameobj.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            }

            missingObjectGameobj.SetActive(false);

        }

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

                    placeholderObject.SetActive(false);
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

        if (actor.behavior is FCopObjectMutating) {

            var hasObj = false;
            foreach (var obj in objects) {

                if (obj != null) {
                    hasObj = true;
                }

            }

            if (!hasObj) {

                placeholderObject.SetActive(false);
                actCollider = null;

                missingObjectGameobj.SetActive(true);

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

        foreach (var obj in objects) {

            if (obj != null) {

                DestroyImmediate(obj.gameObject);

            }

        }

        objects.Clear();

        placeholderObject.SetActive(true);

        Create();

    }

    public void SetToCurrentPosition() {

        Vector3 castDirection = Vector3.down;
        float startingHeight = 100f;


        if (actor.behavior is FCopHeightOffsetting groundCast) {

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

            if (actor.behavior is FCopHeightOffsetting offset) {
                pos.y += offset.GetHeight() / (float)offset.heightMultiplier;
            }
            
            transform.position = pos;

        }
        else {
            print("No floor found");
        }

    }

    void SetRotation() {

        foreach (var obj in objects) {

            if (obj != null) {
                obj.transform.localRotation = Quaternion.identity;
            }

        }

        if (actor.behavior is FCopObjectMutating objectMutating) {

            var rotations = objectMutating.GetRotations();

            foreach (var rot in rotations) {

                foreach (var i in rot.affectedRefIndexes) {

                    var potentialObject = objects[i];

                    if (potentialObject != null) {

                        var objRot = potentialObject.transform.localRotation.eulerAngles;

                        switch (rot.axis) {
                            case Axis.X:
                                objRot.x = -rot.value.parsedRotation;
                                break;
                            case Axis.Y:
                                objRot.y = rot.value.parsedRotation;
                                break;
                            case Axis.Z:
                                objRot.z = -rot.value.parsedRotation;
                                break;
                        }

                        potentialObject.transform.localRotation = Quaternion.Euler(objRot);

                    }

                }

            }

        }

    }

    int SetTextureOffset() {

        if (actor.behavior is FCopEntity e) {
            return e.GetUVOffset();
        }
        else {
            return 0;
        }
    
    }

}