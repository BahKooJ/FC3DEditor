
using FCopParser;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavNodePropertyPanel : MonoBehaviour {

    // - Unity View Refs -
    public NavMeshEditPanel view;

    public TMP_Text indexText;
    public TMP_Dropdown stateDropdown;
    public TMP_Dropdown groundcastDropdown;
    public Toggle readHeightToggle;
    public TMP_InputField heightOffsetInput;
    public List<TMP_Text> nextNodeButtonTexts;

    NavNode node;

    private void Start() {

        view.propertyPanel = this;

        refuseCallback = true;

        indexText.text = "---";
        stateDropdown.interactable = false;
        groundcastDropdown.interactable = false;
        readHeightToggle.interactable = false;
        heightOffsetInput.text = "---";
        heightOffsetInput.interactable = false;

        foreach (var text in nextNodeButtonTexts) {
            text.text = "None";
        }

        refuseCallback = false;

    }

    bool refuseCallback = false;
    public void Refresh() {

        refuseCallback = true;

        node = view.controller.GetSeletedNavNode();

        if (node != null) {

            stateDropdown.interactable = true;
            groundcastDropdown.interactable = true;
            readHeightToggle.interactable = true;
            heightOffsetInput.interactable = true;

            indexText.text = node.index.ToString();
            stateDropdown.value = (int)node.state;
            groundcastDropdown.value = (int)node.groundCast;
            readHeightToggle.isOn = node.readHeightOffset;
            heightOffsetInput.text = node.heightOffset.ToString();

            for (int i = 0; i < nextNodeButtonTexts.Count; i++) {

                if (node.nextNodeIndexes[i] == NavNode.invalid) {
                    nextNodeButtonTexts[i].text = "None";
                }
                else {
                    nextNodeButtonTexts[i].text = node.nextNodeIndexes[i].ToString();
                }

            }

        }
        else {

            indexText.text = "---";
            stateDropdown.interactable = false;
            groundcastDropdown.interactable = false;
            readHeightToggle.interactable= false;
            heightOffsetInput.text = "---";
            heightOffsetInput.interactable = false;

            foreach (var text in nextNodeButtonTexts) {
                text.text = "None";
            }

        }

        refuseCallback = false;


    }

    public void OnChangeStateDropdown() {

        if (refuseCallback) return;

        view.controller.ChangeState((NavNodeState)stateDropdown.value);

        view.controller.RefreshNavNode();

    }

    public void OnChangeGroundCastDropdown() {

        if (refuseCallback) return;

        view.controller.ChangeGroundCast((NavNodeGroundCast)groundcastDropdown.value);

        view.controller.RefreshNavNode();

    }

    public void OnChangeReadHeightToggle() {

        if (refuseCallback) return;

        view.controller.ChangeReadHeight(readHeightToggle.isOn);

        view.controller.RefreshNavNode();


    }

    public void OnFinishHeightOffset() {

        if (refuseCallback) return;

        try {
            view.controller.ChangeHeightOffset(Int32.Parse(heightOffsetInput.text));
        }
        catch {

        }
        
        view.controller.RefreshNavNode();

        heightOffsetInput.text = node.heightOffset.ToString();

    }

    public void StartType() {
        Main.ignoreAllInputs = true;
    }

    public void StopType() {
        Main.ignoreAllInputs = false;
    }

    public void OnClickNextIndex1() {

        if (this.node == null) return;

        if (this.node.nextNodeIndexes[0] >= view.controller.navNodes.Count) {
            return;
        }

        var node = view.controller.navNodes[this.node.nextNodeIndexes[0]];

        if (this.node.nextNodeIndexes[0] == NavNode.invalid) return;

        Camera.main.transform.position = node.transform.position;
        Camera.main.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * -4);
        view.controller.SelectNavNode(node);
    }

    public void OnClickNextIndex2() {

        if (this.node == null) return;

        if (this.node.nextNodeIndexes[1] >= view.controller.navNodes.Count) {
            return;
        }

        var node = view.controller.navNodes[this.node.nextNodeIndexes[1]];

        if (this.node.nextNodeIndexes[1] == NavNode.invalid) return;

        Camera.main.transform.position = node.transform.position;
        Camera.main.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * -4);
        view.controller.SelectNavNode(node);
    }

    public void OnClickNextIndex3() {

        if (this.node == null) return;

        if (this.node.nextNodeIndexes[2] >= view.controller.navNodes.Count) {
            return;
        }

        var node = view.controller.navNodes[this.node.nextNodeIndexes[2]];

        if (this.node.nextNodeIndexes[2] == NavNode.invalid) return;

        Camera.main.transform.position = node.transform.position;
        Camera.main.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * -4);
        view.controller.SelectNavNode(node);
    }

    public void OnClickNextIndex4() {

        if (this.node == null) return;

        if (this.node.nextNodeIndexes[3] >= view.controller.navNodes.Count) {
            return;
        }

        var node = view.controller.navNodes[this.node.nextNodeIndexes[3]];

        if (this.node.nextNodeIndexes[3] == NavNode.invalid) return;

        Camera.main.transform.position = node.transform.position;
        Camera.main.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * -4);
        view.controller.SelectNavNode(node);
    }

}