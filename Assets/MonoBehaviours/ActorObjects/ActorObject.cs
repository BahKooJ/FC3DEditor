

using FCopParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActorObject : MonoBehaviour {

    public Collider actCollider;

    public ActorEditMode controller;

    public FCopActor actor;

    public List<ObjectMesh> objects = new();

    public void ChangePosition(Vector3 pos) {

        actor.x = Mathf.RoundToInt(pos.x * 8192f);
        actor.y = Mathf.RoundToInt(pos.z * -8192f);

        SetToCurrentPosition();

    }

    public void ChangeRotation(float y) {

        if (actor.script == null) {
            return;
        }

        actor.script.ChangeRotation(y);

        SetRotation();

    }


    public void Create() {

        SetToCurrentPosition();

        // 98 and 99 are the weapon models. They keep breaking the mesh collider in ObjectMesh so for now I'll just skip them.
        if (actor.objectType == 98 || actor.objectType == 99) {
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

                    Debug.LogWarning("Object on Actor: " + actor.objectType + " failed to create");

                }

                gameObject.transform.SetParent(transform, false);

            }

        }

        if (actor.script is FCopScript36 || actor.script is FCopScript8) {

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

        switch (actor.script) {

            case FCopScript8:
                var script8 = (FCopScript8)actor.script;

                objects[0].transform.localRotation = Quaternion.Euler(0f, script8.headRotation.parsedRotation, 0f);
                objects[1].transform.localRotation = Quaternion.Euler(0f, script8.baseRotation.parsedRotation, 0f);

                break;
            case FCopScript11:
                var script11 = (FCopScript11)actor.script;

                objects[0].transform.localRotation = Quaternion.Euler(0f, script11.rotation.parsedRotation, 0f);

                break;
            case FCopScript36:
                var script36 = (FCopScript36)actor.script;

                objects[0].transform.localRotation = Quaternion.Euler(0f, script36.headRotation.parsedRotation, 0f);
                objects[1].transform.localRotation = Quaternion.Euler(0f, script36.baseRotation.parsedRotation, 0f);

                break;

        }


    }

    int SetTextureOffset() {

        switch (actor.script) {

            case FCopScript5:
                return ((FCopScript5)actor.script).textureOffset;
            case FCopScript8:
                return ((FCopScript8)actor.script).textureOffset;
            case FCopScript9:
                return ((FCopScript9)actor.script).textureOffset;
            case FCopScript28:
                return ((FCopScript28)actor.script).textureOffset;
            default:
                return 0;
        }
    
    }

    void Start() {

        Create();

    }

}