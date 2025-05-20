
using FCopParser;
using System;
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

        }
        else {

            indexText.text = "---";
            stateDropdown.interactable = false;
            groundcastDropdown.interactable = false;
            readHeightToggle.interactable= false;
            heightOffsetInput.text = "---";
            heightOffsetInput.interactable = false;
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

}