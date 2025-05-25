
using System;
using System.Collections.Generic;
using System.Linq;

namespace FCopParser {

    public class FCopScriptingProject {

        public static Dictionary<int, ScriptVariable> globalVariables = new() {

            { 2, new ScriptVariable("Unknown2", 2, ScriptVariableType.Global, ScriptDataType.Any, "Unknown") },
            { 3, new ScriptVariable("Player Count", 3, ScriptVariableType.Global, ScriptDataType.Int, "The selected player count") },
            { 16, new ScriptVariable("Mission Completed", 16, ScriptVariableType.Global, ScriptDataType.Bool, "If the level was completed, opposite in Precinct Assault") },
            { 17, new ScriptVariable("Unknown17", 17, ScriptVariableType.Global, ScriptDataType.Any, "Unknown") },
            { 19, new ScriptVariable("Sky Captain Level", 19, ScriptVariableType.Global, ScriptDataType.Int, "Selected level of Sky Capatin starting from 0") },
            { 25, new ScriptVariable("Red Points", 25, ScriptVariableType.Global, ScriptDataType.Int, "Red team points in Precinct Assault") },
            { 26, new ScriptVariable("Blue Points", 26, ScriptVariableType.Global, ScriptDataType.Int, "Blue team points in Precinct Assault") },
            { 27, new ScriptVariable("Red Tank Count", 27, ScriptVariableType.Global, ScriptDataType.Int, "Red tank count in Precinct Assault") },
            { 28, new ScriptVariable("Red Chopper Count", 28, ScriptVariableType.Global, ScriptDataType.Int, "Red chopper count in Precinct Assault") },
            { 29, new ScriptVariable("Blue Tank Count", 29, ScriptVariableType.Global, ScriptDataType.Int, "Blue tank count in Precinct Assault") },
            { 30, new ScriptVariable("Blue Chopper Count", 30, ScriptVariableType.Global, ScriptDataType.Int, "Blue chopper count in Precinct Assault") },
            { 31, new ScriptVariable("Red Points Total", 31, ScriptVariableType.Global, ScriptDataType.Int, "The total acquired red points in Precinct Assault") },
            { 32, new ScriptVariable("Blue Points Total", 32, ScriptVariableType.Global, ScriptDataType.Int, "The total acquired blue points in Precinct Assault") },
            { 33, new ScriptVariable("Red Outposts Claimed", 33, ScriptVariableType.Global, ScriptDataType.Int, "The total claimed outposts by red team in Precinct Assault") },
            { 34, new ScriptVariable("Blue Outposts Claimed", 34, ScriptVariableType.Global, ScriptDataType.Int, "The total claimed outposts by blue team in Precinct Assault") },
            { 35, new ScriptVariable("Riot Shield Unlocked", 35, ScriptVariableType.Global, ScriptDataType.Bool, "If the riot shield has been unlocked") },
            { 36, new ScriptVariable("K-9 Drone Unlocked", 36, ScriptVariableType.Global, ScriptDataType.Bool, "If the K-9 Drone has been unlocked") },
            { 37, new ScriptVariable("Grenade Unlocked", 37, ScriptVariableType.Global, ScriptDataType.Bool, "If the Grenade has been unlocked") },
            { 39, new ScriptVariable("skycaptainBehavior39", 39, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 40, new ScriptVariable("skycaptainBehavior40", 40, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 41, new ScriptVariable("skycaptainBehavior41", 41, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 42, new ScriptVariable("skycaptainBehavior42", 42, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 43, new ScriptVariable("skycaptainBehavior43", 43, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 44, new ScriptVariable("skycaptainBehavior44", 44, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 45, new ScriptVariable("skycaptainBehavior45", 45, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 46, new ScriptVariable("skycaptainBehavior46", 46, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 47, new ScriptVariable("skycaptainBehavior47", 47, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 48, new ScriptVariable("skycaptainBehavior48", 48, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 49, new ScriptVariable("skycaptainBehavior49", 49, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 50, new ScriptVariable("skycaptainBehavior50", 50, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 51, new ScriptVariable("skycaptainBehavior51", 51, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 52, new ScriptVariable("skycaptainBehavior52", 52, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 53, new ScriptVariable("skycaptainBehavior53", 53, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 54, new ScriptVariable("skycaptainBehavior54", 54, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 55, new ScriptVariable("skycaptainBehavior55", 55, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") },
            { 56, new ScriptVariable("skycaptainBehavior56", 56, ScriptVariableType.Global, ScriptDataType.Any, "Unknown Sky Captain behavior variable") }

        };

        public static Dictionary<int, ScriptVariable> systemVariables = new() {
            { 0, new ScriptVariable("Frames Rendered", 0, ScriptVariableType.System, ScriptDataType.Int, "The total amount of frames rendered from start.") },
        };

        public static Dictionary<int, ScriptVariable> timerVariables = new() {
            { 0, new ScriptVariable("Timer Var 0", 0, ScriptVariableType.Timer, ScriptDataType.Int, "Timer Variable 0") },
            { 1, new ScriptVariable("Timer Var 1", 1, ScriptVariableType.Timer, ScriptDataType.Int, "Timer Variable 1") },
            { 2, new ScriptVariable("Timer Var 2", 2, ScriptVariableType.Timer, ScriptDataType.Int, "Timer Variable 2") },
            { 3, new ScriptVariable("Timer Var 3", 3, ScriptVariableType.Timer, ScriptDataType.Int, "Timer Variable 3") },
            { 4, new ScriptVariable("Timer Var 4", 4, ScriptVariableType.Timer, ScriptDataType.Int, "Timer Variable 4") },
            { 5, new ScriptVariable("Timer Var 5", 5, ScriptVariableType.Timer, ScriptDataType.Int, "Timer Variable 5") },
            { 6, new ScriptVariable("Timer Var 6", 6, ScriptVariableType.Timer, ScriptDataType.Int, "Timer Variable 6") },
            { 7, new ScriptVariable("Timer Var 7", 7, ScriptVariableType.Timer, ScriptDataType.Int, "Timer Variable 7") },
            { 8, new ScriptVariable("Timer Var 8", 8, ScriptVariableType.Timer, ScriptDataType.Int, "Timer Variable 8") },
            { 9, new ScriptVariable("Timer Var 9", 9, ScriptVariableType.Timer, ScriptDataType.Int, "Timer Variable 9") },
            { 10, new ScriptVariable("Timer Var 10", 10, ScriptVariableType.Timer, ScriptDataType.Int, "Timer Variable 10") },
            { 11, new ScriptVariable("Timer Var 11", 11, ScriptVariableType.Timer, ScriptDataType.Int, "Timer Variable 11") },
            { 12, new ScriptVariable("Timer Var 12", 12, ScriptVariableType.Timer, ScriptDataType.Int, "Timer Variable 12") },
            { 13, new ScriptVariable("Timer Var 13", 13, ScriptVariableType.Timer, ScriptDataType.Int, "Timer Variable 13") },
            { 14, new ScriptVariable("Timer Var 14", 14, ScriptVariableType.Timer, ScriptDataType.Int, "Timer Variable 14") },
            { 15, new ScriptVariable("Timer Var 15", 15, ScriptVariableType.Timer, ScriptDataType.Int, "Timer Variable 15") },
        };

        public static Dictionary<int, ScriptVariable> userVariables = new();

        public FCopRPNS rpns;
        public FCopFunctionParser functionParser;
        public int emptyOffset;

        public FCopScriptingProject(FCopRPNS rpns, FCopFunctionParser functionParser) {
            this.rpns = rpns;
            this.functionParser = functionParser;
            emptyOffset = rpns.code.Last().Key;
            FindAllUserVars();
        }

        public void FindAllUserVars() {

            void SearchScript(ScriptNode scriptNode) {

                if (scriptNode is VariableNode varNode) {

                    if (varNode.varibleType == ScriptVariableType.User) {

                        if (!userVariables.ContainsKey(varNode.value)) {
                            userVariables[varNode.value] = new ScriptVariable("Var" + varNode.value, varNode.value, ScriptVariableType.User, ScriptDataType.Any, "User Variable " + varNode.value);
                        }

                    }

                }

                foreach (var para in scriptNode.parameters) {
                    SearchScript(para);
                }

                if (scriptNode is ScriptNestingNode scriptNesting) {

                    foreach (var nestedScript in scriptNesting.nestedNodes) {
                        SearchScript(nestedScript);
                    }

                }

            }

            foreach (var code in rpns.code) {
                
                foreach (var script in code.Value.code) {
                    SearchScript(script);
                }

            }

            foreach (var fun in functionParser.functions) {

                foreach (var script in fun.runCondition.code) {
                    SearchScript(script);
                }

                foreach (var script in fun.code.code) {
                    SearchScript(script);
                }

            }

        }

        public List<IFFDataFile> Compile() {

            List<IFFDataFile> total = new() {
                rpns.Compile(),
                functionParser.Compile()
            };

            return total;
        }

        public void ResetIDAndOffsets() {
            rpns.ResetKeys();
            emptyOffset = rpns.code.Last().Value.offset;
        }

        // Added a debug code I forgot about and now mission files are messed up
        public void DebugScriptDupeFix() {

            var first = rpns.code.First().Value;
            var copy = new Dictionary<int, FCopScript>(rpns.code.Skip(1));
            foreach (var code in copy) {

                if (first.compiledBytes.SequenceEqual(code.Value.compiledBytes)) {
                    rpns.code.Remove(code.Key);
                }

            }

        }

    }

    public class FCopScript {

        public bool failed = false;

        public string name = "";
        public int id;

        public int offset;
        public int terminationOffset;
        public List<byte> compiledBytes = new();
        public List<ScriptNode> code = new();

        public FCopScript(int offset, List<byte> compiledBytes) {
            this.id = offset;
            this.offset = offset;
            code = Decompile(offset, compiledBytes, out terminationOffset);
            this.compiledBytes = compiledBytes.GetRange(offset, terminationOffset - offset);

        }

        public void Refresh() {

            code.Clear();
            code = Decompile(0, compiledBytes, out var _);

        }

        public static Dictionary<ByteCode, int> maxArgumentsByCode = new() {

            { ByteCode.RANDOM, 1 },
            { ByteCode.QUEUE_STREAM, 1 },
            { ByteCode.PLAY_STREAM, 1 },
            { ByteCode.BYTE14, 1 },
            { ByteCode.BYTE15, 1 },
            { ByteCode.GET_16, 1 },
            { ByteCode.GET_17, 1 },
            { ByteCode.GET_18, 1 },
            { ByteCode.GET_19, 1 },
            { ByteCode.CONDITIONAL_JUMP, 1 },
            { ByteCode.INCREMENT_16, 1 },
            { ByteCode.INCREMENT_19, 1 },
            { ByteCode.DECREMENT_16, 1 },
            { ByteCode.DECREMENT_19, 1 },
            { ByteCode.SET_16, 2 },
            { ByteCode.SET_17, 2 },
            { ByteCode.SET_18, 2 },
            { ByteCode.SET_19, 2 },
            { ByteCode.EQUAL, 2 },
            { ByteCode.NOT_EQUAL, 2 },
            { ByteCode.GREATER_THAN, 2 },
            { ByteCode.GREATER_THAN_OR_EQUAL, 2 },
            { ByteCode.LESS_THAN, 2 },
            { ByteCode.LESS_THAN_OR_EQUAL, 2 },
            { ByteCode.ADD, 2 },
            { ByteCode.SUBTRACT, 2 },
            { ByteCode.MULTIPLY, 2 },
            { ByteCode.DIVIDE, 2 },
            { ByteCode.MOD, 2 },
            { ByteCode.AND, 2 },
            { ByteCode.OR, 2 },
            { ByteCode.BYTE47, 2 },
            { ByteCode.ADD_16_SET, 2 },
            { ByteCode.ADD_19_SET, 2 },
            { ByteCode.SUB_16_SET, 2 },
            { ByteCode.SUB_19_SET, 2 },
            { ByteCode.ACTOR_FUNC, 3 },
            { ByteCode.GROUP_ACTOR_FUNC, 3 },
            { ByteCode.TEAM_ACTOR_FUNC, 3 },
            { ByteCode.NAV_MESH_STATE_CHANGE, 3 },
            { ByteCode.SPAWNING_FUNC, 3 },
            { ByteCode.GROUP_SPAWNING_FUNC, 3 },
            { ByteCode.STATIC_PROP_FUNC, 3 }

        };

        public static Dictionary<ScriptDataKey, ScriptOperationData> scriptNodeData = new() {

            { new ScriptDataKey(ByteCode.RANDOM, 1),
                new ScriptOperationData("Random", ScriptDataType.Int, new() { new ScriptParameter("Range", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.QUEUE_STREAM, 1),
                new ScriptOperationData("Queue Stream", ScriptDataType.Void, new() { new ScriptParameter("Stream Index", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.PLAY_STREAM, 1),
                new ScriptOperationData("Play Stream", ScriptDataType.Void, new() { new ScriptParameter("Stream Index", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.BYTE14, 1),
                new ScriptOperationData("14", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.BYTE15, 1),
                new ScriptOperationData("Get 15", ScriptDataType.Int, new() { new ScriptParameter("ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.GET_16, 1),
                new ScriptOperationData("Get 16", ScriptDataType.Int, new() { new ScriptParameter("ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.GET_17, 1),
                new ScriptOperationData("Get 17", ScriptDataType.Int, new() { new ScriptParameter("ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.GET_18, 1),
                new ScriptOperationData("Get 18", ScriptDataType.Int, new() { new ScriptParameter("ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.GET_19, 1),
                new ScriptOperationData("Get 19", ScriptDataType.Int, new() { new ScriptParameter("ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.CONDITIONAL_JUMP, 1),
                new ScriptOperationData("If", ScriptDataType.Void, new() { new ScriptParameter("Condition", ScriptDataType.Bool) })
            },
            { new ScriptDataKey(ByteCode.CONDITIONAL_JUMP, 0),
                new ScriptOperationData("If (True)", ScriptDataType.Void, new())
            },
            { new ScriptDataKey(ByteCode.INCREMENT_16, 1),
                new ScriptOperationData("++", ScriptDataType.Void, new() { new ScriptParameter("Var ID", ScriptDataType.GlobalVar) })
            },
            { new ScriptDataKey(ByteCode.INCREMENT_19, 1),
                new ScriptOperationData("++", ScriptDataType.Void, new() { new ScriptParameter("Var ID", ScriptDataType.UserVar) })
            },
            { new ScriptDataKey(ByteCode.DECREMENT_16, 1),
                new ScriptOperationData("--", ScriptDataType.Void, new() { new ScriptParameter("Var ID", ScriptDataType.GlobalVar) })
            },
            { new ScriptDataKey(ByteCode.DECREMENT_19, 1),
                new ScriptOperationData("--", ScriptDataType.Void, new() { new ScriptParameter("Var ID", ScriptDataType.UserVar) })
            },
            { new ScriptDataKey(ByteCode.SET_16, 2),
                new ScriptOperationData("=", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Int), new ScriptParameter("Var ID", ScriptDataType.GlobalVar) })
            },
            { new ScriptDataKey(ByteCode.SET_17, 2),
                new ScriptOperationData("30", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Int), new ScriptParameter("Var ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.SET_18, 2),
                new ScriptOperationData("=", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int), new ScriptParameter("Par1", ScriptDataType.TimerVar) })
            },
            { new ScriptDataKey(ByteCode.SET_18, 1),
                new ScriptOperationData("=(18) Temp Debug 1", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.TimerVar)})
            },
            { new ScriptDataKey(ByteCode.SET_19, 2),
                new ScriptOperationData("=", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int), new ScriptParameter("Par1", ScriptDataType.UserVar) })
            },
            { new ScriptDataKey(ByteCode.EQUAL, 2),
                new ScriptOperationData("==", ScriptDataType.Bool, new() { new ScriptParameter("Left", ScriptDataType.Int), new ScriptParameter("Right", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.EQUAL, 1),
                new ScriptOperationData("==(1)", ScriptDataType.Bool, new() { new ScriptParameter("Par0", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.NOT_EQUAL, 2),
                new ScriptOperationData("!=", ScriptDataType.Bool, new() { new ScriptParameter("Left", ScriptDataType.Int), new ScriptParameter("Right", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.GREATER_THAN, 2),
                new ScriptOperationData(">", ScriptDataType.Bool, new() { new ScriptParameter("Left", ScriptDataType.Int), new ScriptParameter("Right", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.GREATER_THAN_OR_EQUAL, 2),
                new ScriptOperationData(">=", ScriptDataType.Bool, new() { new ScriptParameter("Left", ScriptDataType.Int), new ScriptParameter("Right", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.LESS_THAN, 2),
                new ScriptOperationData("<", ScriptDataType.Bool, new() { new ScriptParameter("Left", ScriptDataType.Int), new ScriptParameter("Right", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.LESS_THAN_OR_EQUAL, 2),
                new ScriptOperationData("<=", ScriptDataType.Bool, new() { new ScriptParameter("Left", ScriptDataType.Int), new ScriptParameter("Right", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.ADD, 2),
                new ScriptOperationData("+", ScriptDataType.Int, new() { new ScriptParameter("Left", ScriptDataType.Int), new ScriptParameter("Right", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.SUBTRACT, 2),
                new ScriptOperationData("-", ScriptDataType.Int, new() { new ScriptParameter("Left", ScriptDataType.Int), new ScriptParameter("Right", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.MULTIPLY, 2),
                new ScriptOperationData("*", ScriptDataType.Int, new() { new ScriptParameter("Left", ScriptDataType.Int), new ScriptParameter("Right", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.DIVIDE, 2),
                new ScriptOperationData("/", ScriptDataType.Int, new() { new ScriptParameter("Left", ScriptDataType.Int), new ScriptParameter("Right", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.MOD, 2),
                new ScriptOperationData("%", ScriptDataType.Int, new() { new ScriptParameter("Left", ScriptDataType.Int), new ScriptParameter("Right", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.AND, 2),
                new ScriptOperationData("&&", ScriptDataType.Bool, new() { new ScriptParameter("Left", ScriptDataType.Bool), new ScriptParameter("Right", ScriptDataType.Bool) })
            },
            { new ScriptDataKey(ByteCode.OR, 2),
                new ScriptOperationData("||", ScriptDataType.Bool, new() { new ScriptParameter("Left", ScriptDataType.Bool), new ScriptParameter("Right", ScriptDataType.Bool) })
            },
            { new ScriptDataKey(ByteCode.BYTE47, 2),
                new ScriptOperationData("47", ScriptDataType.Int, new() { new ScriptParameter("Par0", ScriptDataType.Int), new ScriptParameter("Par1", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.ADD_16_SET, 2),
                new ScriptOperationData("+=", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Int), new ScriptParameter("Var ID", ScriptDataType.GlobalVar) })
            },
            { new ScriptDataKey(ByteCode.ADD_19_SET, 2),
                new ScriptOperationData("+=", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Int), new ScriptParameter("Var ID", ScriptDataType.UserVar) })
            },
            { new ScriptDataKey(ByteCode.SUB_16_SET, 2),
                new ScriptOperationData("-=", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Int), new ScriptParameter("Var ID", ScriptDataType.GlobalVar) })
            },
            { new ScriptDataKey(ByteCode.SUB_19_SET, 2),
                new ScriptOperationData("-=", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Int), new ScriptParameter("Var ID", ScriptDataType.UserVar) })
            },
            { new ScriptDataKey(ByteCode.ACTOR_FUNC, 3),
                new ScriptOperationData("Actor Func", ScriptDataType.Void, new() { new ScriptParameter("Actor", ScriptDataType.Int), new ScriptParameter("Method", ScriptDataType.Int), new ScriptParameter("Parameter", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.ACTOR_FUNC, 2),
                new ScriptOperationData("Actor Func(2)", ScriptDataType.Void, new() { new ScriptParameter("Method", ScriptDataType.Int), new ScriptParameter("Parameter", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.GROUP_ACTOR_FUNC, 3),
                new ScriptOperationData("Group Actor Func", ScriptDataType.Void, new() { new ScriptParameter("Actor Group", ScriptDataType.Int), new ScriptParameter("Method", ScriptDataType.Int), new ScriptParameter("Parameter", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.TEAM_ACTOR_FUNC, 3),
                new ScriptOperationData("Team Actor Func", ScriptDataType.Void, new() { new ScriptParameter("Team", ScriptDataType.Int), new ScriptParameter("Method", ScriptDataType.Int), new ScriptParameter("Parameter", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.NAV_MESH_STATE_CHANGE, 3),
                new ScriptOperationData("Nav Mesh State Change", ScriptDataType.Void, new() { new ScriptParameter("Nav Mesh Index", ScriptDataType.Int), new ScriptParameter("Disabled", ScriptDataType.Int), new ScriptParameter("Nav Mesh Node Index", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.SPAWNING_FUNC, 3),
                new ScriptOperationData("Spawning Func", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int), new ScriptParameter("Par1", ScriptDataType.Int), new ScriptParameter("Par2", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.GROUP_SPAWNING_FUNC, 3),
                new ScriptOperationData("Group Spawning Func", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int), new ScriptParameter("Par1", ScriptDataType.Int), new ScriptParameter("Par2", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.STATIC_PROP_FUNC, 3),
                new ScriptOperationData("62", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int), new ScriptParameter("Par1", ScriptDataType.Int), new ScriptParameter("Par2", ScriptDataType.Int) })
            },

        };

        public static List<ByteCode> doubleOperators = new() {
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

        public static List<ByteCode> varAssignmentOperators = new() {
            ByteCode.INCREMENT_16,
            ByteCode.INCREMENT_19,
            ByteCode.DECREMENT_16,
            ByteCode.DECREMENT_19,
            ByteCode.SET_16,
            ByteCode.SET_18,
            ByteCode.SET_19,
            ByteCode.ADD_16_SET,
            ByteCode.ADD_19_SET,
            ByteCode.SUB_16_SET,
            ByteCode.SUB_19_SET,
        };

        public static List<ScriptDataType> varDataTypes = new() {
            ScriptDataType.GlobalVar,
            ScriptDataType.SystemVar,
            ScriptDataType.TimerVar,
            ScriptDataType.UserVar
        };

        public List<ScriptNode> Decompile(int startingOffset, List<byte> code, out int terminationOffset) {

            var statements = new List<ScriptNode>();
            var floatingExpressions = new List<ScriptNode>();
            List<(ScriptNestingNode node, int byteCount)> nodesToNest = new();

            void AddScriptingNode(ScriptNode node, int bytesProcessed, bool isExpression) {

                if (nodesToNest.Count != 0) {

                    var nestingNode = nodesToNest.Last();

                    nestingNode.byteCount -= bytesProcessed;

                    if (!isExpression) {

                        node.parent = nestingNode.node;
                        nestingNode.node.nestedNodes.Add(node);

                    }

                    if (nestingNode.byteCount == 0) {

                        // Expression wasn't used and now outside the jump statement.
                        if (isExpression) {

                            node.parent = nestingNode.node;
                            nestingNode.node.nestedNodes.Add(node);

                            // Todo: Ternary Check

                        }

                    }
                    else {

                        if (isExpression) {
                            floatingExpressions.Add(node);
                        }

                    }

                    foreach (var i in Enumerable.Range(0, nodesToNest.Count)) {

                        var n = nodesToNest[i];
                        n.byteCount -= bytesProcessed;
                        nodesToNest[i] = n;

                        // Data is misaligned
                        if (n.byteCount < 0) {
                            throw new Exception();
                        }

                    }

                    nodesToNest.RemoveAll(n => n.byteCount == 0);

                }
                else {

                    if (isExpression) {
                        floatingExpressions.Add(node);
                    }
                    else {
                        statements.Add(node);
                    }

                }

            }

            var i = startingOffset;
            while (i < code.Count) {

                var b = code[i];

                // Key Byte found!
                if (Enum.IsDefined(typeof(ByteCode), (int)b)) {

                    var byteCode = (ByteCode)b;

                    if (byteCode == ByteCode.BIT_FLIP) {

                        int value = code[i + 1];

                        value ^= -1;

                        AddScriptingNode(new LiteralNode(value), 2, true);

                        i += 2;
                        continue;

                    }
                    if (byteCode == ByteCode.BIT_SHIFT_RIGHT) {

                        int value = code[i + 1];

                        // I can't figure this out so adding 128 does the same thing I guess.
                        value += 128;

                        AddScriptingNode(new LiteralNode(value), 2, true);

                        i += 2;
                        continue;

                    }
                    if (byteCode == ByteCode.LITERAL_16) {

                        // Big Endian
                        int value = BitConverter.ToInt16(new byte[] { code[i + 2], code[i + 1] });

                        AddScriptingNode(new LiteralNode(value), 3, true);

                        i += 3;
                        continue;

                    }
                    if (byteCode == ByteCode.JUMP) {

                        foreach (var expression in floatingExpressions) {

                            // Why 0?
                            // The expression was already counted for, because nothing used it and we hit a jump statement.
                            // This is pretty much always going to be a ternary.
                            AddScriptingNode(expression, 0, false);

                        }

                        floatingExpressions.Clear();

                        var jumpNode = new ScriptNestingNode(ByteCode.JUMP, "Else", ScriptDataType.Void, new(), new());

                        AddScriptingNode(jumpNode, 2, false);

                        // If one it's an empty jump statement.
                        // This is only possible by the editor.
                        if (code[i + 1] != 1) {
                            nodesToNest.Add((jumpNode, code[i + 1] - 1));
                        }

                        i += 2;
                        continue;

                    }
                    if (byteCode == ByteCode.END) {

                        i++;
                        break;

                    }

                    var maxParaCount = maxArgumentsByCode[byteCode];
                    var paraCount = floatingExpressions.Count >= maxParaCount ? maxParaCount : floatingExpressions.Count;

                    var scriptData = scriptNodeData[new ScriptDataKey(byteCode, paraCount)];
                    var parameters = floatingExpressions.GetRange(floatingExpressions.Count - paraCount, paraCount);
                    floatingExpressions.RemoveRange(floatingExpressions.Count - paraCount, paraCount);

                    ScriptNode node;

                    if (Enum.IsDefined(typeof(ScriptVariableType), (int)byteCode) && parameters.Count == 1 && parameters[0].GetType() == typeof(LiteralNode)) {
                        var litNode = (LiteralNode)parameters[0];
                        node = new VariableNode(true, (ScriptVariableType)b, litNode.value);
                    }
                    else if (doubleOperators.Contains(byteCode) && paraCount == 2) {
                        node = new OperatorNode(byteCode, scriptData.name, scriptData.defaultReturnType, new(scriptData.parameterData), parameters, false);
                    }
                    else if (varAssignmentOperators.Contains(byteCode)) {
                        node = new VariableAssignmentNode(byteCode, scriptData.name, scriptData.defaultReturnType, new(scriptData.parameterData), parameters, true);
                    }
                    else if (byteCode == ByteCode.CONDITIONAL_JUMP) {
                        node = new ScriptNestingNode(byteCode, scriptData.name, scriptData.defaultReturnType, new(scriptData.parameterData), parameters);
                    }
                    else {
                        node = new ScriptNode(byteCode, scriptData.name, scriptData.defaultReturnType, new(scriptData.parameterData), parameters);
                    }

                    if (byteCode == ByteCode.CONDITIONAL_JUMP) {

                        AddScriptingNode(node, 2, false);

                        // If one it's an empty jump statement.
                        // This is only possible by the editor.
                        if (code[i + 1] != 1) {
                            nodesToNest.Add(((ScriptNestingNode)node, code[i + 1] - 1));
                        }

                        i += 2;
                        continue;

                    }

                    AddScriptingNode(node, 1, node.defaultReturnType != ScriptDataType.Void);

                }
                else {

                    if (b < 128) {
                        throw new Exception();
                    }

                    AddScriptingNode(new LiteralNode(b - 128), 1, true);

                }

                i++;

            }

            if (floatingExpressions.Count > 0) {

                foreach (var floatingExpression in floatingExpressions) {

                    statements.Add(floatingExpression);

                }

            }
            terminationOffset = i;
            return statements;

        }

        public List<byte> Compile(int newOffset) {

            var total = new List<byte>();

            var nestedBytesStack = new List<List<byte>>();

            void CompileNode(ScriptNode node) {

                foreach (var parameter in node.parameters) {
                    CompileNode(parameter);
                }

                if (nestedBytesStack.Count != 0) {
                    nestedBytesStack.Last().AddRange(node.Compile());
                }
                else {
                    total.AddRange(node.Compile());
                }

                if (node.byteCode == ByteCode.JUMP) {
                    // The jump byte hasn't been added yet.
                    PopByteStack(2);
                }

                if (node is ScriptNestingNode nestingNode) {

                    nestedBytesStack.Add(new());

                    bool alreadyPopped = false;

                    foreach (var nestedNode in nestingNode.nestedNodes) {

                        if (nestedNode.byteCode == ByteCode.JUMP) {
                            alreadyPopped = true;
                        }

                        CompileNode(nestedNode);
                    }

                    if (!alreadyPopped) {
                        PopByteStack();
                    }

                }


            }

            // It always counts the jump byte so 1 needs to be added
            void PopByteStack(int futureByteCount = 1) {

                if (nestedBytesStack.Count > 1) {
                    nestedBytesStack[^2].Add((byte)(nestedBytesStack.Last().Count + futureByteCount));
                    nestedBytesStack[^2].AddRange(nestedBytesStack.Last());
                    nestedBytesStack.RemoveAt(nestedBytesStack.Count - 1);
                }
                else {
                    total.Add((byte)(nestedBytesStack.Last().Count + futureByteCount));
                    total.AddRange(nestedBytesStack.Last());
                    nestedBytesStack.RemoveAt(nestedBytesStack.Count - 1);
                }

            }

            foreach (var node in code) {

                CompileNode(node);

            }

            total.Add(0);

            offset = newOffset;

            if (!compiledBytes.SequenceEqual(total)) {
                //throw new Exception("skill issue");
            }

            compiledBytes = total;
            return compiledBytes;
        }

    }

    public enum ByteCode {
        NONE = -2,
        LITERAL = -1,
        END = 0,
        BIT_FLIP = 1,
        BIT_SHIFT_RIGHT = 2,
        LITERAL_16 = 3,
        JUMP = 8,
        RANDOM = 11,
        QUEUE_STREAM = 12,
        PLAY_STREAM = 13,
        BYTE14 = 14,
        BYTE15 = 15,
        GET_16 = 16,
        GET_17 = 17,
        GET_18 = 18,
        GET_19 = 19,
        CONDITIONAL_JUMP = 20,
        INCREMENT_16 = 21,
        INCREMENT_19 = 24,
        DECREMENT_16 = 25,
        DECREMENT_19 = 28,
        SET_16 = 29,
        SET_17 = 30,
        SET_18 = 31,
        SET_19 = 32,
        EQUAL = 33,
        NOT_EQUAL = 34,
        GREATER_THAN = 35,
        GREATER_THAN_OR_EQUAL = 36,
        LESS_THAN = 37,
        LESS_THAN_OR_EQUAL = 38,
        ADD = 39,
        SUBTRACT = 40,
        MULTIPLY = 41,
        DIVIDE = 42,
        MOD = 43,
        AND = 44,
        OR = 45,
        BYTE47 = 47,
        ADD_16_SET = 48,
        ADD_19_SET = 51,
        SUB_16_SET = 52,
        SUB_19_SET = 55,
        ACTOR_FUNC = 56,
        GROUP_ACTOR_FUNC = 57,
        TEAM_ACTOR_FUNC = 58,
        NAV_MESH_STATE_CHANGE = 59,
        SPAWNING_FUNC = 60,
        GROUP_SPAWNING_FUNC = 61,
        STATIC_PROP_FUNC = 62,
    }

    public enum ScriptDataType {
        Void,
        Any,
        Int,
        Bool,
        GlobalVar,
        SystemVar,
        TimerVar,
        UserVar,
    }

    public enum ScriptVariableType {
        Global = 16,
        System = 17,
        Timer = 18,
        User = 19
    }

    public struct ScriptDataKey {

        public ByteCode byteCode;
        public int parameterCount;

        public ScriptDataKey(ByteCode byteCode, int parameterCount) {
            this.byteCode = byteCode;
            this.parameterCount = parameterCount;
        }

    }

    public struct ScriptParameter {

        public string name;
        public ScriptDataType dataType;

        public ScriptParameter(string name, ScriptDataType dataType) {
            this.name = name;
            this.dataType = dataType;
        }

    }

    public struct ScriptOperationData {

        public string name;
        public ScriptDataType defaultReturnType;
        public List<ScriptParameter> parameterData;

        public ScriptOperationData(string name, ScriptDataType defaultReturnType, List<ScriptParameter> parameterData) {
            this.name = name;
            this.defaultReturnType = defaultReturnType;
            this.parameterData = parameterData;
        }

    }

    public class ScriptNode {

        public ScriptNestingNode parent = null;

        public ByteCode byteCode;
        public string name;
        public ScriptDataType defaultReturnType;
        public List<ScriptParameter> parameterData;

        public List<ScriptNode> parameters = new();

        internal ScriptNode() {

        }

        ScriptNode(ByteCode byteCode, string name, ScriptDataType defaultReturnType, List<ScriptParameter> parameterData) {
            this.byteCode = byteCode;
            this.name = name;
            this.defaultReturnType = defaultReturnType;
            this.parameterData = parameterData;
        }

        public ScriptNode(ByteCode byteCode, string name, ScriptDataType defaultReturnType, List<ScriptParameter> parameterData, List<ScriptNode> parameters) : this(byteCode, name, defaultReturnType, parameterData) {
            this.parameters = parameters;
            CastParametersToTypes();
        }

        // The idea of these casts were to take literal nodes and cast them to a specifc type of ease of use.
        // The only issue is bitfields, because casting a literal node would apply it to the entire value.
        // This idea is ultimately scrapped. The only reason this still applies is variables.
        internal void CastParametersToTypes() {

            foreach (var i in Enumerable.Range(0, parameters.Count)) {
                var parData = parameterData[i];
                var parNode = parameters[i];

                if (parNode is LiteralNode literalNode && FCopScript.varDataTypes.Contains(parData.dataType)) {

                    parameters[i] = CastLiteralsToType(literalNode, parData.dataType);

                }

            }

        }

        public LiteralNode CastLiteralsToType(LiteralNode literalNode, ScriptDataType dataType) {

            switch (dataType) {
                case ScriptDataType.GlobalVar:
                    return new VariableNode(false, ScriptVariableType.Global, literalNode.value);
                case ScriptDataType.SystemVar:
                    return new VariableNode(false, ScriptVariableType.System, literalNode.value);
                case ScriptDataType.TimerVar:
                    return new VariableNode(false, ScriptVariableType.Timer, literalNode.value);
                case ScriptDataType.UserVar:
                    return new VariableNode(false, ScriptVariableType.User, literalNode.value);
            }

            return new LiteralNode(literalNode.value);

        }

        public virtual ScriptDataType ReturnType() {
            return defaultReturnType;
        }

        public virtual List<ParameterNode> GetParameters() {

            var parameters = new List<ParameterNode>();

            foreach (var i in Enumerable.Range(0, this.parameters.Count)) {
                var parData = parameterData[i];
                var parNode = this.parameters[i];

                parameters.Add(new ParameterNode(parNode, this, parData.name, i, parData.dataType));
            }

            return parameters;
        }

        public virtual List<byte> Compile() {

            if (byteCode == ByteCode.NONE) {
                return new();
            }

            return new() { (byte)byteCode };
        }

    }

    public class ScriptNestingNode : ScriptNode {

        public List<ScriptNode> nestedNodes = new();

        public ScriptNestingNode(ByteCode byteCode, string name, ScriptDataType defaultReturnType, List<ScriptParameter> parameterData, List<ScriptNode> parameters) : base(byteCode, name, defaultReturnType, parameterData, parameters) {
        }

    }

    public class ParameterNode {

        public ScriptNode parent;
        public ScriptNode scriptNode;
        public string parameterName;
        public int parameterIndex;
        public ScriptDataType dataType;

        public ParameterNode(ScriptNode scriptNode, ScriptNode parent, string parameterName, int parameterIndex, ScriptDataType dataType) {
            this.parent = parent;
            this.scriptNode = scriptNode;
            this.parameterName = parameterName;
            this.parameterIndex = parameterIndex;
            this.dataType = dataType;
        }

    }

    public class OperatorNode : ScriptNode {

        public bool reversed;

        public OperatorNode(ByteCode byteCode, string name, ScriptDataType defaultReturnType, List<ScriptParameter> parameterData, List<ScriptNode> parameters, bool reversed) : base(byteCode, name, defaultReturnType, parameterData, parameters) {
            this.reversed = reversed;
        }

        public override List<ParameterNode> GetParameters() {
            var parameters = new List<ParameterNode>();

            foreach (var i in Enumerable.Range(0, this.parameters.Count)) {
                var parData = parameterData[i];
                var parNode = this.parameters[i];
                parameters.Add(new ParameterNode(parNode, this, "", i, parData.dataType));
            }

            if (reversed) {
                parameters.Reverse();
            }

            return parameters;
        }

    }

    public class VariableAssignmentNode : OperatorNode {

        const int incrementByteCodeOffset = 5;
        const int decrementByteCodeOffset = 9;
        const int assignByteCodeOffset = 13;
        const int addByteCodeOffset = 32;
        const int subByteCodeOffset = 36;

        public enum AssignmentType {
            Increment,
            Decrement,
            Assign,
            Add,
            Sub
        }

        VariableNode varNode;
        AssignmentType assignmentType;

        public VariableAssignmentNode(ByteCode byteCode, string name, ScriptDataType defaultReturnType, List<ScriptParameter> parameterData, List<ScriptNode> parameters, bool reversed) : base(byteCode, name, defaultReturnType, parameterData, parameters, reversed) {

            foreach (var parameter in this.parameters) {
                if (parameter is VariableNode varNode) {

                    if (!varNode.isGet) {
                        this.varNode = varNode;
                        break;
                    }

                }
            }
            if (byteCode == ByteCode.INCREMENT_16 || byteCode == ByteCode.INCREMENT_19) {
                varNode.allowedVarTypeConverstion = new() { ScriptVariableType.Global, ScriptVariableType.User };
                assignmentType = AssignmentType.Increment;
            }
            else if (byteCode == ByteCode.DECREMENT_16 || byteCode == ByteCode.DECREMENT_19) {
                varNode.allowedVarTypeConverstion = new() { ScriptVariableType.Global, ScriptVariableType.User };
                assignmentType = AssignmentType.Decrement;
            }
            else if (byteCode == ByteCode.SET_16 || byteCode == ByteCode.SET_17 || byteCode == ByteCode.SET_18 || byteCode == ByteCode.SET_19) {
                assignmentType = AssignmentType.Assign;
            }
            else if (byteCode == ByteCode.ADD_16_SET || byteCode == ByteCode.ADD_19_SET) {
                varNode.allowedVarTypeConverstion = new() { ScriptVariableType.Global, ScriptVariableType.User };
                assignmentType = AssignmentType.Add;
            }
            else if (byteCode == ByteCode.SUB_16_SET || byteCode == ByteCode.SUB_19_SET) {
                varNode.allowedVarTypeConverstion = new() { ScriptVariableType.Global, ScriptVariableType.User };
                assignmentType = AssignmentType.Sub;
            }

        }

        public override List<byte> Compile() {

            var varTypeValue = (int)varNode.varibleType;

            // Alright this is kind of a little sneaky.
            // The values on the enum variableType are the byte codes for the gets (16, 17, 18, 19).
            // These assignment byte codes are still sorted in the same way even if one var type is unused.
            // So to get the correct byte code from the var type, we can just offset the number.
            // For exampled the var type global which is 16, as a set code of 29. The difference is 13.
            // Well the set for user vars (19), is 32, which is still 13 away.
            // Because these numbers will never change this simple math way of getting the byte code works.
            switch (assignmentType) {
                case AssignmentType.Increment:
                    return new() { (byte)(varTypeValue + incrementByteCodeOffset) };
                case AssignmentType.Decrement:
                    return new() { (byte)(varTypeValue + decrementByteCodeOffset) };
                case AssignmentType.Assign:
                    return new() { (byte)(varTypeValue + assignByteCodeOffset) };
                case AssignmentType.Add:
                    return new() { (byte)(varTypeValue + addByteCodeOffset) };
                case AssignmentType.Sub:
                    return new() { (byte)(varTypeValue + subByteCodeOffset) };
                default:
                    return base.Compile();
            }
        }

        public override List<ParameterNode> GetParameters() {

            varNode.RefreshData();
            
            if (parameters.Count == 2 && parameters[0] is LiteralNode litNode) {

                var parameters = new List<ParameterNode>();

                foreach (var i in Enumerable.Range(0, this.parameters.Count)) {
                    var parData = parameterData[i];
                    var parNode = this.parameters[i];

                    parameters.Add(new ParameterNode(parNode, this, parData.name, i, parData.dataType));
                }

                return new() {
                    new ParameterNode(varNode, this, "", 1, VariableNode.VarTypeToDataType(varNode.varibleType)),
                    new ParameterNode(litNode, this, "", 0, varNode.defaultReturnType),

                };

            }
            else {
                return base.GetParameters();
            }

        }

    }

    public class LiteralNode : ScriptNode {

        public int value;

        public LiteralNode() : base() {

        }

        public LiteralNode(int value) : base(ByteCode.LITERAL, "", ScriptDataType.Int, new(), new()) {
            this.value = value;
        }

        public override List<byte> Compile() {
            
            if (value < 0) {
                var flippedValue = value ^= -1;
                return new() { (byte)ByteCode.BIT_FLIP, (byte)flippedValue };
            }

            if (value > 127 && value < 384) {
                return new() { (byte)ByteCode.BIT_SHIFT_RIGHT, (byte)(value - 128) };
            }

            if (value > 127) {
                var compiled16bit = BitConverter.GetBytes((short)value);
                var total = new List<byte>() { (byte)ByteCode.LITERAL_16 };
                // Remember, it's big-endian
                total.AddRange(compiled16bit.Reverse());
                return total;
            }

            return new() { (byte)(value + 128) };

        }

    }

    // Having this as a node is a little bit of a stretch. (as opposed to parameter types passed into the view)
    // The "allowedVarTypeConverstion" is the only thing keeping this here.
    public class VariableNode : LiteralNode {

        public static ScriptDataType VarTypeToDataType(ScriptVariableType varType) {
            return varType switch {
                ScriptVariableType.Global => ScriptDataType.GlobalVar,
                ScriptVariableType.System => ScriptDataType.SystemVar,
                ScriptVariableType.Timer => ScriptDataType.TimerVar,
                ScriptVariableType.User => ScriptDataType.UserVar,
                _ => ScriptDataType.GlobalVar
            };
        }

        public bool isGet;
        public ScriptVariableType varibleType;
        public List<ScriptVariableType> allowedVarTypeConverstion = new() { ScriptVariableType.Global, ScriptVariableType.System, ScriptVariableType.Timer, ScriptVariableType.User };

        public void SetData(ScriptVariableType varibleType, int id) {
            this.value = id;
            this.varibleType = varibleType;

            RefreshData();
        }

        public void RefreshData() {

            ScriptVariable varData = new ScriptVariable("Var" + value, value, varibleType, ScriptDataType.Int, "Variable " + value);

            switch (varibleType) {
                case ScriptVariableType.Global:
                    varData = FCopScriptingProject.globalVariables[value];
                    break;
                case ScriptVariableType.System:
                    varData = FCopScriptingProject.systemVariables[value];

                    break;
                case ScriptVariableType.Timer:
                    varData = FCopScriptingProject.timerVariables[value];

                    break;
                case ScriptVariableType.User:

                    if (!FCopScriptingProject.userVariables.TryGetValue(value, out varData)) {
                        varData = new ScriptVariable("Var" + value, value, varibleType, ScriptDataType.Int, "User Variable " + value);
                    }

                    break;
            }

            this.name = varData.name;
            this.defaultReturnType = varData.dataType;

        }

        public VariableNode(bool isGet, ScriptVariableType varibleType, int id) : base() {
            this.isGet = isGet;
            SetData(varibleType, id);
        }

        public override List<byte> Compile() {

            if (!isGet) {
                return new() { (byte)(value + 128) };
            }
            else {
                return new() { (byte)(value + 128), (byte)varibleType };
            }

        }

    }

    public struct ScriptVariable {

        public int id;
        public string name;
        public string description;
        public ScriptVariableType varibleType;
        public ScriptDataType dataType;

        public ScriptVariable(string name, int id, ScriptVariableType varibleType, ScriptDataType dataType, string descrition) {
            this.name = name;
            this.id = id;
            this.description = descrition;
            this.varibleType = varibleType;
            this.dataType = dataType;
        }

    }

}