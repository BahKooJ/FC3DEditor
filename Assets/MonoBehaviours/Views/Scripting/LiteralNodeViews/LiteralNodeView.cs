
using TMPro;
using FCopParser;
using System.Collections;
using System;
using static ScriptingPanelView;

public class LiteralNodeView : ExpressionNodeView {

    // - Unity Refs -
    public TMP_InputField valueInput;

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

            if (value < 0) {
                return;
            }

            if (value > ((int)Math.Pow(2, (int)parameterNode.bitCount) - 1)) {
                return;
            }

            var shiftedValue = value << parameterNode.bitPosition;

            var andBits = ~(((int)Math.Pow(2, (int)parameterNode.bitCount) - 1) << parameterNode.bitPosition);

            literalNode.value = (literalNode.value & andBits) + shiftedValue;
        }
        else {
            literalNode.value = value;
        }

    }

    bool refuseCallback = false;
    public override void Init() {
        refuseCallback = true;
        literalNode = parameterNode.scriptNode as LiteralNode;

        valueInput.text = GetNodeValue().ToString();
        refuseCallback = false;
    }

    public void StartType() {
        Main.ignoreAllInputs = true;
    }

    public void StopType() {
        Main.ignoreAllInputs = false;
    }

    public void OnFinishType() {

        if (refuseCallback) return;

        Main.AddCounterAction(new ScriptSaveStateCounterAction(currentLine.view.script, currentLine.view));

        try {
            var value = int.Parse(valueInput.text);
            SetNodeValue(value);
        }
        catch { }

        valueInput.text = GetNodeValue().ToString();

    }

}