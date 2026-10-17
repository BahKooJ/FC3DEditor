

using FCopParser;
using System.Collections.Generic;
using static NodeSelectorViewUtil;

public class ScriptNodeCreatorItemView : DragableUIElement {

    // - Parameter -
    public NodeCreatorData creatorData;

    public ScriptNode Create(ScriptNode nestParameterNode) {

        if (creatorData.byteCode == ByteCode.LITERAL) {
            return new LiteralNode(0);
        }

        if (creatorData.byteCode == ByteCode.JUMP) {
            return new ScriptNestingNode(ByteCode.JUMP, "Else", ScriptDataType.Void, new(), new());
        }

        List<byte> scriptByteChunk = new();

        var maxArgs = FCopScript.maxArgumentsByCode[creatorData.byteCode];

        for (int i = 0; i < maxArgs; i++) {
            scriptByteChunk.Add(128);
        }

        scriptByteChunk.Add((byte)creatorData.byteCode);

        if (creatorData.byteCode == ByteCode.CONDITIONAL_JUMP || creatorData.byteCode == ByteCode.JUMP) {
            scriptByteChunk.Add(1);
        }

        scriptByteChunk.Add(0);

        var scriptChunk = new FCopScript(0, scriptByteChunk);

        if (scriptChunk.code[0].parameters.Count > 0 && nestParameterNode != null) {
            scriptChunk.code[0].parameters[0] = nestParameterNode;
        }

        return scriptChunk.code[0];

    }

}