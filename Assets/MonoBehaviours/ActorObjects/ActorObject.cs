

using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActorObject : MonoBehaviour {

    public Collider actCollider;

    public EditMode controller;

    public FCopActor actor;

    public List<ObjectMesh> objects = new();

    public void ChangePosition(Vector3 pos) {

        actor.x = Mathf.RoundToInt(pos.x * 8192f);
        actor.y = Mathf.RoundToInt(pos.z * -8192f);

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

        // 98 and 99 are the weapon models. They keep breaking the mesh collider in ObjectMesh so for now I'll just skip them.
        if (actor.actorType == 98 || actor.actorType == 99) {
            return;
        }

        foreach (var resRef in actor.resourceReferences) {

            if (resRef.fourCC == FCopActor.FourCC.Cobj) {

                var obj = controller.main.level.objects.First(obj => { return obj.rawFile.dataID == resRef.id; });

                var gameObject = Instantiate(controller.main.ObjectMesh);

                var script = gameObject.GetComponent<ObjectMesh>();

                script.controller = controller.main;

                script.fCopObject = obj;

                script.textureOffset = SetTextureOffset();

                script.Create();

                if (!script.failed) {

                    var cube = transform.Find("Cube");

                    if (cube != null) {
                        Destroy(cube.gameObject);
                        actCollider = null;
                    }

                    objects.Add(script);

                } else {

                    Debug.LogWarning("Object on Actor: " + actor.actorType + " failed to create");

                }

                gameObject.transform.SetParent(transform, false);

            }

        }

        if (actor.behavior is FCopBehavior36 || actor.behavior is FCopBehavior8) {

            if (objects.Count != 2) {
                Debug.LogWarning("Actor script 36/8 doesn't have 2 models?");
                return;
            }

            var headPos = objects[0].transform.localPosition;
            headPos.y = objects[1].maxY;
            objects[0].transform.localPosition = headPos;

        }

        SetRotation();

    }

    void SetToCurrentPosition() {

        transform.position = new Vector3(actor.x / 8192f, 100f, -(actor.y / 8192f));

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, 1)) {

            var pos = transform.position;

            pos.y = hit.point.y;

            transform.position = pos;

        } else {
            print("No floor found");
        }

    }

    void SetRotation() {

        switch (actor.behavior) {

            case FCopBehavior8:
                var script8 = (FCopBehavior8)actor.behavior;

                objects[0].transform.localRotation = Quaternion.Euler(0f, script8.headRotation.value.parsedRotation, 0f);
                objects[1].transform.localRotation = Quaternion.Euler(0f, script8.baseRotation.value.parsedRotation, 0f);

                break;
            case FCopBehavior11:
                var script11 = (FCopBehavior11)actor.behavior;

                objects[0].transform.localRotation = Quaternion.Euler(0f, script11.rotation.value.parsedRotation, 0f);

                break;
            case FCopBehavior36:
                var script36 = (FCopBehavior36)actor.behavior;

                objects[0].transform.localRotation = Quaternion.Euler(0f, script36.headRotation.value.parsedRotation, 0f);
                objects[1].transform.localRotation = Quaternion.Euler(0f, script36.baseRotation.value.parsedRotation, 0f);

                break;

        }


    }

    int SetTextureOffset() {

        switch (actor.behavior) {

            case FCopBehavior5:
                return ((FCopBehavior5)actor.behavior).textureOffset;
            case FCopBehavior8:
                return ((FCopBehavior8)actor.behavior).textureOffset.value;
            case FCopBehavior9:
                return ((FCopBehavior9)actor.behavior).textureOffset;
            case FCopBehavior28:
                return ((FCopBehavior28)actor.behavior).textureOffset;
            default:
                return 0;
        }
    
    }

    void Start() {

        Create();

    }

}