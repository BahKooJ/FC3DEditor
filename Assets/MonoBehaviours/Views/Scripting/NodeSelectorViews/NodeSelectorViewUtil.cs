

using FCopParser;
using System.Collections.Generic;

public abstract class NodeSelectorViewUtil {

    public static List<ByteCode> keyWordCodes = new() { 
        ByteCode.CONDITIONAL_JUMP, 
        ByteCode.JUMP 
    };
    public static List<ByteCode> expressionCodes = new() { 
        ByteCode.RANDOM,
        ByteCode.EQUAL,
        ByteCode.NOT_EQUAL,
        ByteCode.GREATER_THAN,
        ByteCode.GREATER_THAN_OR_EQUAL,
        ByteCode.LESS_THAN,
        ByteCode.LESS_THAN_OR_EQUAL,
        ByteCode.ADD,
        ByteCode.SUBTRACT,
        ByteCode.MULTIPLY,
        ByteCode.DIVIDE,
        ByteCode.MOD,
        ByteCode.AND,
        ByteCode.OR
    };
    public static List<ByteCode> variableCodes = new() { 
        ByteCode.GET_16 
    };
    public static List<ByteCode> literalCodes = new() { 
        ByteCode.LITERAL 
    };


    public static Dictionary<NodeSelectorTab, List<NodeCreatorData>> nodeCreatorData = new() {
        { NodeSelectorTab.System, new() {
            new NodeCreatorData(ByteCode.CONDITIONAL_JUMP, "If", ScriptDataType.Void, "Will run the provided code if the given condition is true"),
            new NodeCreatorData(ByteCode.JUMP, "Else", ScriptDataType.Void, "Will run the provided code if the given \"If\" condition is false"),
            new NodeCreatorData(ByteCode.QUEUE_STREAM, "Queue Stream", ScriptDataType.Void, "Adds a stream file to a queue to be played"),
            new NodeCreatorData(ByteCode.PLAY_STREAM, "Play Stream", ScriptDataType.Void, "Plays the stream file immediately"),
            new NodeCreatorData(ByteCode.RANDOM, "Random", ScriptDataType.Int, "Generates a random number from 0 until the given range, RANGE MUST BE NEGATIVE"),
        } },
        { NodeSelectorTab.Values, new() {
            new NodeCreatorData(ByteCode.LITERAL, "Number", ScriptDataType.Int, "A whole numeric value"),
            new NodeCreatorData(ByteCode.LITERAL, "Bool", ScriptDataType.Bool, "True or False"),
        } },
        { NodeSelectorTab.Variable, new() {
            new NodeCreatorData(ByteCode.GET_16, "Get Variable", ScriptDataType.Any, "Returns the value of the selected variable"),
            //new NodeCreatorData(ByteCode.BYTE47, "47", ScriptDataType.Any, "47"),
            new NodeCreatorData(ByteCode.SET_16, "Set Variable", ScriptDataType.Void, "Sets the value of the selected variable"),
            new NodeCreatorData(ByteCode.INCREMENT_16, "Increment Variable", ScriptDataType.Void, "Adds 1 and sets the selected variable"),
            new NodeCreatorData(ByteCode.DECREMENT_16, "Decrement Variable", ScriptDataType.Void, "Subtracts 1 and sets the selected variable"),
            new NodeCreatorData(ByteCode.ADD_16_SET, "Add Set Variable", ScriptDataType.Void, "Adds and Sets the value of the selected variable"),
            new NodeCreatorData(ByteCode.SUB_16_SET, "Subtract Set Variable", ScriptDataType.Void, "Subtracts and Sets the value of the selected variable"),
        } },
        { NodeSelectorTab.Logic, new() {
            new NodeCreatorData(ByteCode.EQUAL, "Equal (==)", ScriptDataType.Bool, "If the two values are equal to each other"),
            new NodeCreatorData(ByteCode.NOT_EQUAL, "Not Equal (!=)", ScriptDataType.Bool, "If the two values are not equal to each other"),
            new NodeCreatorData(ByteCode.GREATER_THAN, "Greater Than (>)", ScriptDataType.Bool, "If the left value is greater than the right"),
            new NodeCreatorData(ByteCode.GREATER_THAN_OR_EQUAL, "Greater Than Or Equal (>=)", ScriptDataType.Bool, "If the left value is greater than the right or equal"),
            new NodeCreatorData(ByteCode.LESS_THAN, "Less Than (<)", ScriptDataType.Bool, "If the left value is less than the right"),
            new NodeCreatorData(ByteCode.LESS_THAN_OR_EQUAL, "Less Than Or Equal (<=)", ScriptDataType.Bool, "If the left value is less than the right or equal"),
            new NodeCreatorData(ByteCode.ADD, "Add (+)", ScriptDataType.Int, "Adds the two values and returns the result"),
            new NodeCreatorData(ByteCode.SUBTRACT, "Subtract (-)", ScriptDataType.Int, "Subtracts the two values and returns the result"),
            new NodeCreatorData(ByteCode.MULTIPLY, "Multiply (*)", ScriptDataType.Int, "Multiplies the two values and returns the result"),
            new NodeCreatorData(ByteCode.DIVIDE, "Divide (/)", ScriptDataType.Int, "Divides the two values and returns the result. Does not include the remainder"),
            new NodeCreatorData(ByteCode.MOD, "Mod (%)", ScriptDataType.Int, "Divides the two values and returns the remainder"),
            new NodeCreatorData(ByteCode.AND, "And (&&)", ScriptDataType.Bool, "Returns true if both sides are true"),
            new NodeCreatorData(ByteCode.OR, "And (||)", ScriptDataType.Bool, "Returns true if either side is true"),
        } },
        { NodeSelectorTab.Functions, new() {
            new NodeCreatorData(ByteCode.SET_17, "System Function", ScriptDataType.Void, "Runs a function that will affect the game"),
            new NodeCreatorData(ByteCode.ACTOR_FUNC, "Actor Function", ScriptDataType.Void, "Runs a function that will affect the provided actor"),
            new NodeCreatorData(ByteCode.NAV_MESH_STATE_CHANGE, "Nav Mesh State Change", ScriptDataType.Void, "Changes the state of a provided nav mesh node"),
            new NodeCreatorData(ByteCode.SPAWNING_FUNC, "Spawning Function", ScriptDataType.Void, "Runs a function that will affect the provided actor's spawning properties"),
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