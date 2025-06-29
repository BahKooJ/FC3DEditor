﻿

using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActorObject : MonoBehaviour {

    // - Prefabs -
    public GameObject missingObjectPrefab;

    // - Unity Refs -
    public Transform rootParent;
    public Collider actCollider;
    public TextFieldPopupHandler renameTextFeild;
    public GameObject placeholderObject;
    public float yPadding = 0f;

    // - Parameters -
    public ActorEditMode controller;
    public FCopActor actor;
    [HideInInspector]
    public ActorGroupObject group = null;

    [HideInInspector]
    public List<ObjectMesh> objects = new();
    [HideInInspector]
    public GameObject missingObjectGameobj;

    [HideInInspector]
    public bool missingObjects = false;

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

    public void ChangePosition(Vector3 pos, AxisControl.Axis axis) {

        if (axis == AxisControl.Axis.AxisY) {

            if (actor.behavior is FCopHeightOffsetting heightOffseting) {

                if (!ActorEditMode.preventCounterAction) {

                    ActorEditMode.AddPropertyChangeCounterAction(heightOffseting.GetHeightProperty(), actor);

                }

                ActorEditMode.preventCounterAction = true;

                // Height offset is relative to ground cast
                heightOffseting.SetHeight(pos.y - GroundCast());
                controller.view.activeActorPropertiesView.RequestPropertyRefresh(heightOffseting.GetHeightProperty());
            }

        }
        else {

            // Why call a method in the controller when the changes can be made here?:
            // For counter-actions, it also need to know whether the change was made with a group or not.
            controller.ChangeActorPosition(actor, pos);

        }

        SetToCurrentPosition();

    }

    public virtual void RefreshRotation() {

        if (actor.behavior == null) {
            return;
        }

        SetObjectMutations();

    }

    public virtual void Create() {

        void ShowMissingObject() {

            missingObjects = true;

            placeholderObject.SetActive(false);
            actCollider = null;

            missingObjectGameobj.SetActive(true);

        }

        if (missingObjectGameobj == null) {
            var obj = Instantiate(missingObjectPrefab);
            obj.transform.SetParent(transform, false);
            missingObjectGameobj = obj;
        }

        missingObjectGameobj.SetActive(false);

        SetToCurrentPosition();

        foreach (var resRef in actor.resourceReferences) {

            if (resRef.fourCC == FCopActor.FourCC.Cobj) {

                var obj = controller.main.level.objects.FirstOrDefault(obj => { return obj.rawFile.dataID == resRef.id; });

                if (obj == null) {

                    objects.Add(null);
                    ShowMissingObject();
                    continue;

                }

                var gameObject = Instantiate(controller.main.ObjectMesh);
                gameObject.transform.SetParent(transform, false);

                var script = gameObject.GetComponent<ObjectMesh>();
                script.levelTexturePallet = controller.main.levelTexturePallet;
                script.fCopObject = obj;
                script.textureOffset = SetTextureOffset();
                script.ForceMake();

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

        if (actor.behavior is FCopShooter) {

            var hasObj = false;
            foreach (var obj in objects) {

                if (obj != null) {
                    hasObj = true;
                }

            }

            if (!hasObj) {

                ShowMissingObject();

            }

        }

        SetObjectMutations();

    }

    public virtual void Refresh() {

        foreach (var obj in objects) {

            if (obj != null) {

                DestroyImmediate(obj.gameObject);

            }

        }

        objects.Clear();

        placeholderObject.SetActive(true);

        Create();

    }

    public float GroundCast() {

        if (actor.behavior is FCopHeightOffsetting groundCast) {

            return ObjectUtil.GroundCast(groundCast.GetGroundCast(), new Vector2(actor.x / 8192f, -(actor.y / 8192f)));

        }
        else {

            return ObjectUtil.GroundCast(ActorGroundCast.Highest, new Vector2(actor.x / 8192f, -(actor.y / 8192f)));

        }


    }

    void SetToCurrentObjPositions() {

        if (actor.behavior.assetReferences != null) {

            var ari = 0;
            foreach (var assetRef in actor.behavior.assetReferences) {

                if (assetRef.dependantRefIndex != -1 && assetRef.type == AssetType.Object) {

                    var obj = objects[ari];

                    var dependantObj = objects[assetRef.dependantRefIndex];

                    if (assetRef.positionIndex != -1 && obj != null) {

                        FCopObject.Vertex fCopVert;

                        try {
                            fCopVert = dependantObj.fCopObject.GetPosition(assetRef.positionIndex);

                            if (actor.behaviorType == ActorBehavior.PathedTurret && actor.behavior.propertiesByName["Thruster Behavior Override"].GetCompiledValue() == 1) {
                                fCopVert = dependantObj.fCopObject.GetPosition(2);
                            }
                        }
                        catch {
                            ari++;
                            continue;
                        }

                        obj.transform.SetParent(dependantObj.transform, true);

                        var pos = new Vector3(fCopVert.x / ObjectMesh.scale, fCopVert.y / ObjectMesh.scale, fCopVert.z / ObjectMesh.scale);

                        obj.transform.localPosition = pos;

                        obj.transform.SetParent(rootParent, true);

                    }

                }
                ari++;
            }

        }

    }

    public void SetToCurrentPosition() {

        var pos = new Vector3(actor.x / 8192f, GroundCast() + yPadding, -(actor.y / 8192f));

        if (actor.behavior is FCopHeightOffsetting offset) {
            pos.y += offset.GetHeight();
        }

        transform.position = pos;

    }

    public void SetToOnlyY() {

        var pos = new Vector3(0, GroundCast() + yPadding, 0);

        if (actor.behavior is FCopHeightOffsetting offset) {
            pos.y += offset.GetHeight();
        }

        transform.localPosition = pos;

    }

    void SetObjectMutations() {

        foreach (var obj in objects) {

            if (obj != null) {
                obj.transform.localRotation = Quaternion.identity;
            }

        }

        if (actor.behavior is FCopObjectMutating objectMutating) {

            foreach (var mutation in objectMutating.GetMutations()) {

                var potentialObject = objects[mutation.refIndex];

                if (potentialObject != null) {

                    var objRot = potentialObject.transform.localRotation.eulerAngles;

                    objRot.x = -mutation.rotationX;
                    objRot.y = mutation.rotationY;
                    objRot.z = -mutation.rotationZ;

                    potentialObject.transform.localRotation = Quaternion.Euler(objRot);

                    var objScale = potentialObject.transform.localScale;

                    objScale.x = -mutation.scaleX;
                    objScale.y = mutation.scaleY;
                    objScale.z = -mutation.scaleZ;

                    potentialObject.transform.localScale = objScale;

                }

            }

        }

        SetToCurrentObjPositions();

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