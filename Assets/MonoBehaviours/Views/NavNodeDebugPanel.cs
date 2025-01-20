
using FCopParser;
using System;
using TMPro;
using UnityEngine;

public class NavNodeDebugPanel : MonoBehaviour {

    // - Unity View Refs -
    public NavMeshEditPanel view;

    public TMP_InputField unknownBit2Input;
    public TMP_InputField groundCastInput;
    public TMP_InputField unknown1;
    public TMP_InputField heightOffsetInput;

    NavNode node;

    private void Start() {

        view.debugPanel = this;

    }

    public void Refresh() {

        node = view.controller.selectedNavNode.controlledObject.GetComponent<NavNodePoint>().node;

        unknownBit2Input.text = node.unknown2Bit.ToString();
        groundCastInput.text = ((int)node.groundCast).ToString();
        unknown1.text = node.unknown.ToString();
        heightOffsetInput.text = node.heightOffset.ToString();


    }

    public void OnFinishBit2() {

        node.unknown2Bit = Int32.Parse(unknownBit2Input.text);

    }

    public void OnFinishUnknown() {

        node.unknown = Int32.Parse(unknown1.text);

    }

    public void OnFinishHeightOffset() {

        node.SafeSetHeight(Int32.Parse(heightOffsetInput.text));

        heightOffsetInput.text = node.heightOffset.ToString();

    }

    public void OnFinishGroundCast() {

        node.groundCast = (NavNodeGroundCast)Int32.Parse(groundCastInput.text);

    }

}