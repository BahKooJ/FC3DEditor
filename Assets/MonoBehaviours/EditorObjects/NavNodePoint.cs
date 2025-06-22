

using FCopParser;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class NavNodePoint : MonoBehaviour {

    static float heightMultiplier = 64f;

    static float yPadding = 0.1f;
    // - Unity Refs -
    public SphereCollider sphereCollider;
    public GameObject heightOffsetSphere;

    // - Parameters -
    public NavNode node;
    public NavMeshEditMode controller;

    [HideInInspector]
    public List<LineRenderer> nextNodeLines = new();
    [HideInInspector]
    public List<NavNodePoint> previousPoints = new();

    void Start() {

        heightOffsetSphere.GetComponent<MeshRenderer>().material.color = Color.yellow;

    }

    void Update() {
        
        if (Input.GetMouseButtonUp(0)) {
            savedHeightOffsetPos = -1f;
        }

    }

    public void Create() {

        SetToCurrentPosition();

    }

    float savedHeightOffsetPos = -1f;
    public void ChangePosition(Vector3 pos, AxisControl.Axis axis) {

        controller.ChangePosition(node, pos);

        if (axis == AxisControl.Axis.AxisY) {

            if (Input.GetMouseButton(0)) {

                if (savedHeightOffsetPos == -1f) {

                    savedHeightOffsetPos = heightOffsetSphere.transform.localPosition.y;

                }

            }

            controller.ChangeHeightOffset(Mathf.RoundToInt(pos.y * 32f));

            controller.view.propertyPanel.Refresh();

        }

        SetToCurrentPosition();

        RefreshLines();

        foreach (var node in previousPoints) {
            node.RefreshLines();
        }

    }

    public float GroundCast() {

        switch (node.groundCast) {
            case NavNodeGroundCast.Highest:
                return ObjectUtil.GroundCast(ActorGroundCast.Highest, new Vector2(node.x / 32f, -(node.y / 32f)));
            case NavNodeGroundCast.Lowest:
                return ObjectUtil.GroundCast(ActorGroundCast.Lowest, new Vector2(node.x / 32f, -(node.y / 32f)));
            case NavNodeGroundCast.LowestDisableHeight:
                return ObjectUtil.GroundCast(ActorGroundCast.Lowest, new Vector2(node.x / 32f, -(node.y / 32f)));
            case NavNodeGroundCast.Middle:
                return ObjectUtil.GroundCast(ActorGroundCast.Middle, new Vector2(node.x / 32f, -(node.y / 32f)));
        }

        return 6f;

    }

    public void SetToCurrentPosition() {

        var pos = new Vector3(node.x / 32f, GroundCast(), -(node.y / 32f));

        pos.y += yPadding;

        transform.position = pos;

        RefreshHeightOffsetSphere();

        if (node.state == NavNodeState.Disabled) {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else {
            GetComponent<MeshRenderer>().material.color = Color.white;
        }

    }

    public void RefreshHeightOffsetSphere() {

        var heightSpherePos = heightOffsetSphere.transform.position;

        heightSpherePos.y = node.heightOffset / 32f;

        heightOffsetSphere.transform.position = heightSpherePos;
        
        heightOffsetSphere.SetActive(node.readHeightOffset);

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
            case 3:
                lineRenderer.startColor = Color.magenta;
                lineRenderer.endColor = Color.magenta;
                break;
        }

        return lineRenderer;

    }

    public void ClearPaths() {

        foreach (var line in nextNodeLines) {

            if (line != null) {

                Destroy(line.gameObject);

            }

        }

        nextNodeLines.Clear();

        RefreshLines();

    }

}