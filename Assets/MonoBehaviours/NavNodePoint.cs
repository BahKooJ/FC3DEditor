

using FCopParser;
using UnityEngine;

public class NavNodePoint : MonoBehaviour {

    static float yPadding = 0.3f;

    public NavNode node;

    public NavMeshEditMode controller;

    public LineRenderer nextNodeLineA = null;
    public LineRenderer nextNodeLineB = null;
    public LineRenderer nextNodeLineC = null ;

    void Start() {

    }

    public void Create() {

        transform.position = new Vector3(node.x / 32f, 100f, -(node.y / 32f));

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, 1)) {
            print("Found an object - distance: " + hit.distance);

            var pos = transform.position;

            pos.y = hit.point.y + yPadding;

            transform.position = pos;

        } else {
            print("No object found");
        }

    }

    public void RefreshLines() {

        if (nextNodeLineA != null) {

            var nextNode = controller.navNodes[node.nextNodeA];

            nextNodeLineA.SetPosition(0, transform.position);

            nextNodeLineA.SetPosition(1, nextNode.transform.position);

            nextNodeLineA.startColor = Color.blue;
            nextNodeLineA.endColor = Color.blue;

        }
        if (nextNodeLineB != null) {

            var nextNode = controller.navNodes[node.nextNodeB];

            nextNodeLineB.SetPosition(0, transform.position);

            nextNodeLineB.SetPosition(1, nextNode.transform.position);

            nextNodeLineB.startColor = Color.green;
            nextNodeLineB.endColor = Color.green;

        }
        if (nextNodeLineC != null) {

            var nextNode = controller.navNodes[node.nextNodeC];

            nextNodeLineC.SetPosition(0, transform.position);

            nextNodeLineC.SetPosition(1, nextNode.transform.position);

            nextNodeLineC.startColor = Color.red;
            nextNodeLineC.endColor = Color.red;

        }

    }

}