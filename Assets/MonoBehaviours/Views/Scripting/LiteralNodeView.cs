
using TMPro;
using FCopParser;

public class LiteralNodeView : ExpressionNodeView {

    // - Unity Refs -
    public TMP_InputField valueInput;

    LiteralNode literal;

    bool refuseCallback = false;
    public override void Init() {
        refuseCallback = true;
        literal = parameterNode.scriptNode as LiteralNode;

        valueInput.text = literal.value.ToString();
        refuseCallback = false;
    }

    public void OnFinishType() {

        if (refuseCallback) return;

        try {
            var value = int.Parse(valueInput.text);
            literal.value = value;
        }
        catch { }

        valueInput.text = literal.value.ToString();

    }

}