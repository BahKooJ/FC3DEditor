

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

        Create();

    }

    virtual public void ChangeRotation(float y) {

    }


    public void Create() {

        transform.position = new Vector3(actor.x / 8192f, 100f, -(actor.y / 8192f));

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, 1)) {

            var pos = transform.position;

            pos.y = hit.point.y;

            transform.position = pos;

        } else {
            print("No floor found");
        }

        foreach (var resRef in actor.resourceReferences) {

            if (resRef.fourCC == FCopActor.FourCC.Cobj) {

                var obj = controller.main.level.objects.First(obj => { return obj.rawFile.dataID == resRef.id; });

                var gameObject = Instantiate(controller.main.ObjectMesh);

                var script = gameObject.GetComponent<ObjectMesh>();

                script.controller = controller.main;

                script.fCopObject = obj;

                if (!script.failed) {

                    var cube = transform.Find("Cube");

                    if (cube != null) {
                        Destroy(cube.gameObject);
                    }

                    objects.Add(script);

                }

                gameObject.transform.SetParent(transform, false);

            }

        }

    }

    void Start() {

        Create();

    }

}