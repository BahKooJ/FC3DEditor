

using FCopParser;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class NavNodePoint : MonoBehaviour {

    static float yPadding = 0.1f;

    // - Unity Refs -
    public SphereCollider sphereCollider;

    // - Parameters -
    public NavNode node;
    public NavMeshEditMode controller;

    [HideInInspector]
    public List<LineRenderer> nextNodeLines = new();
    [HideInInspector]
    public List<NavNodePoint> previousPoints = new();

    void Start() {

    }

    public void Create() {

        transform.position = new Vector3(node.x / 32f, 100f, -(node.y / 32f));

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, 1)) {

            var pos = transform.position;

            pos.y = hit.point.y + yPadding;

            transform.position = pos;

        }
        else {
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

        ValidateCorrectNextNodes();

        if (nextNodeLines.Count == 0) {

            var nextNodeI = 0;
            foreach (var nodeIndex in node.nextNodeIndexes) {

                if (nodeIndex != NavNode.invalid) {

                    nextNodeLines.Add(InitLine(nodeIndex, nextNodeI));

                }
                else {

                    // Even though the next index is invalid, the space needs to be filled up so things line up.
                    nextNodeLines.Add(null);

                }

                nextNodeI++;
            }

        }
        else {

            var nextNodeI = 0;
            foreach (var nodeIndex in node.nextNodeIndexes) {

                if (nodeIndex != NavNode.invalid) {

                    var line = nextNodeLines[nextNodeI];

                    if (line == null) {
                        nextNodeLines[nextNodeI] = InitLine(nodeIndex, nextNodeI);
                    }
                    else {

                        line.SetPosition(0, transform.position);

                        line.SetPosition(1, controller.navNodes[nodeIndex].transform.position);

                    }

                }

                nextNodeI++;
            }

        }

    }

    void ValidateCorrectNextNodes() {

        foreach (var nodeIndex in node.nextNodeIndexes) {

            if (nodeIndex == node.index) {
                ClearPaths();
                return;
            }

        }

    }

    public bool AreAllPathsUsed() {

        foreach (var nodeIndex in node.nextNodeIndexes) {

            if (nodeIndex == NavNode.invalid) {
                return false;
            }

        }

        return true;

    }

    public bool AlreadyContainsPath(int index) {

        foreach (var nodeIndex in node.nextNodeIndexes) {

            if (nodeIndex == index) {
                return true;
            }

        }

        return false;

    }

    LineRenderer InitLine(int nodeIndex, int nextNodeIndex) {

        var nextNode = controller.navNodes[nodeIndex];

        var lineObject = Object.Instantiate(controller.main.line3d);

        var lineRenderer = lineObject.GetComponent<LineRenderer>();

        lineRenderer.material.SetTextureScale("_MainTex", new Vector2(3f ,1f));

        lineRenderer.SetPosition(0, transform.position);

        lineRenderer.SetPosition(1, nextNode.transform.position);

        switch (nextNodeIndex) {
            case 0:
                lineRenderer.startColor = Color.blue;
                lineRenderer.endColor = Color.blue;
                break;
            case 1:
                lineRenderer.startColor = Color.green;
                lineRenderer.endColor = Color.green;
                break;
            case 2:
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.red;
                break;
        }

        return lineRenderer;

    }

    public void ClearPaths() {

        foreach (var index in Enumerable.Range(0, node.nextNodeIndexes.Count())) {

            node.nextNodeIndexes[index] = NavNode.invalid;

        }

        foreach (var line in nextNodeLines) {

            if (line != null) {

                Object.Destroy(line.gameObject);

            }

        }

        nextNodeLines.Clear();

        RefreshLines();

    }

}