

using FCopParser;
using System.Collections.Generic;

public abstract class NodeSelectorViewUtil {

    public static Dictionary<NodeSelectorTab, List<NodeCreatorData>> nodeCreatorData = new() {
        { NodeSelectorTab.System, new() {
            new NodeCreatorData(ByteCode.CONDITIONAL_JUMP, "If", ScriptDataType.Void, "Will run the provided code if the given condition is true"),
            new NodeCreatorData(ByteCode.JUMP, "Else", ScriptDataType.Void, "Will run the provided code if the given \"If\" condition is false"),
            new NodeCreatorData(ByteCode.QUEUE_STREAM, "Queue Stream", ScriptDataType.Void, "Adds a stream file to a queue to be played"),
            new NodeCreatorData(ByteCode.PLAY_STREAM, "Play Stream", ScriptDataType.Void, "Plays the stream file immediately"),
            new NodeCreatorData(ByteCode.RANDOM, "Random", ScriptDataType.Int, "Generates a random number from 0 until the given range, RANGE MUST BE NEGATIVE"),


        } },

    };

    public enum NodeSelectorTab {
        System,
        Values,
        Variable,
        Logic,
        Functions
    }

    public struct NodeCreatorData {

        public ByteCode byteCode;
        public string name;
        public ScriptDataType returnType;
        public string description;

        public NodeCreatorData(ByteCode byteCode, string name, ScriptDataType returnType, string description) {
            this.byteCode = byteCode;
            this.name = name;
            this.returnType = returnType;
            this.description = description;
        }

    }

}