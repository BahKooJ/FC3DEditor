
using UnityEngine;
using FCopParser;
using UnityEngine.UI;
using System.Collections;
using System;

public class BoolNodeView : ExpressionNodeView {

    LiteralNode literalNode;

    int GetNodeValue() {

        if (parameterNode.bitCount != BitCount.NA) {
            var bitField = new BitArray(new byte[] { (byte)literalNode.value });
            var bits = Utils.CopyBitsOfRange(bitField, parameterNode.bitPosition, parameterNode.bitPosition + (byte)parameterNode.bitCount);
            return Utils.BitsToInt(bits);
        }
        else {
            return literalNode.value;
        }

    }

    void SetNodeValue(int value) {

        if (parameterNode.bitCount != BitCount.NA) {
            var shiftedValue = value << parameterNode.bitPosition;

            var andBits = ~(((int)Math.Pow(2, (int)parameterNode.bitCount) - 1) << parameterNode.bitPosition);

            literalNode.value = (literalNode.value & andBits) + shiftedValue;
        }
        else {
            literalNode.value = value;
        }

    }

    public override void Init() {

        literalNode = parameterNode.scriptNode as LiteralNode;

        expressionText.text = (GetNodeValue() == 1).ToString();

    }

    public void OnClick() {

        SetNodeValue((GetNodeValue() == 1) ? 0 : 1);

        expressionText.text = (GetNodeValue() == 1).ToString();

    }

}