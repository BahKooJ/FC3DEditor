

using FCopParser;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class NavNodePoint : MonoBehaviour {

    static float yPadding = 0.3f;

    public SphereCollider sphereCollider;

    public NavNode node;

    public NavMeshEditMode controller;

    public LineRenderer[] nextNodeLines = new LineRenderer[3] { null, null, null };

    public List<NavNodePoint> previousPoints = new();

    void Start() {

    }

    public void Create() {

        transform.position = new Vector3(node.x / 32f, 100f, -(node.y / 32f));

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, 1)) {

            var pos = transform.position;

            pos.y = hit.point.y + yPadding;

            transform.position = pos;

        } else {
            print("No floor found: " + transform.position.x.ToString() + " " + transform.position.z.ToString());
        }

    }

    public void ChangePosition(Vector3 pos) {

        node.x = Mathf.RoundToInt(pos.x * 32f);
        node.y = Mathf.RoundToInt(pos.z * -32f);

        Create();

        RefreshLines();

        foreach (var node in previousPoints) {
            node.RefreshLines();
        }

    }

    public void RefreshLines() {

        foreach (var index in Enumerable.Range(0, nextNodeLines.Count())) {

            if (node.nextNode[index] == NavNode.invalid && nextNodeLines[index] != null) {
                Destroy(nextNodeLines[index].gameObject);
                nextNodeLines[index] = null;
            }

            if (node.nextNode[index] != NavNode.invalid && nextNodeLines[index] == null) {

                var lineObject = Object.Instantiate(controller.main.line3d);

                var lineRenderer = lineObject.GetComponent<LineRenderer>();

                nextNodeLines[index] = lineRenderer;

                controller.lines.Add(lineObject);

            }

            var line = nextNodeLines[index];

            if (line != null) {

                var nextNode = controller.navNodes[node.nextNode[index]];

                line.SetPosition(0, transform.position);

                line.SetPosition(1, nextNode.transform.position);

                switch (index) {
                    case 0:
                        line.startColor = Color.blue;
                        line.endColor = Color.blue;
                        break;
                    case 1:
                        line.startColor = Color.green;
                        line.endColor = Color.green;
                        break;
                    case 2:
                        line.startColor = Color.red;
                        line.endColor = Color.red;
                        break;
                }

            }

        }

    }

    public void ClearPaths() {

        foreach (var index in Enumerable.Range(0, node.nextNode.Count())) {

            node.nextNode[index] = NavNode.invalid;

        }

        RefreshLines();

    }

}