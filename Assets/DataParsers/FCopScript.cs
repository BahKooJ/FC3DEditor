
using System;
using System.Collections.Generic;
using System.Linq;

namespace FCopParser {

    public class FCopScriptingProject {

        public static Dictionary<int, ScriptVariable> globalVariables = new() {
            { 0, new ScriptVariable("None", 0, ScriptVariableType.Global, ScriptDataType.Any, "None") },
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

        public static int AddUserVariable() {

            var ids = userVariables.Keys.ToList();

            var varID = Utils.FindNextInt(ids);

            if (varID > 127) {
                return -1;
            }

            userVariables.Add(varID, new ScriptVariable("Var" + varID, varID, ScriptVariableType.User, ScriptDataType.Any, "User Variable " + varID));

            return varID;

        }

        public static void ClearVars() {

            userVariables = new();

            timerVariables = new() {
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

        }

        public FCopRPNS rpns;
        public FCopFunctionParser functionParser;
        public int emptyOffset;

        public FCopScriptingProject(FCopRPNS rpns, FCopFunctionParser functionParser) {
            this.rpns = rpns;
            this.functionParser = functionParser;
            rpns.code.Last().name = "None";
            emptyOffset = rpns.code.Last().offset;
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
                
                foreach (var script in code.code) {
                    SearchScript(script);
                }

            }

            foreach (var fun in functionParser.functions) {

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

        public List<byte> CompileNCFC() {

            var total = new List<byte>();

            total.AddRange(rpns.CompileNCFC());
            total.AddRange(functionParser.CompileNCFC());

            return total;

        }

        public void ResetIDAndOffsets() {
            rpns.ResetKeys();
            emptyOffset = rpns.code.Last().offset;
        }

        // Added a debug code I forgot about and now mission files are messed up
        public void DebugScriptDupeFix() {

            var first = rpns.codeByOffset.First().Value;
            var copy = new Dictionary<int, FCopScript>(rpns.codeByOffset.Skip(1));
            foreach (var code in copy) {

                if (first.compiledBytes.SequenceEqual(code.Value.compiledBytes)) {
                    rpns.codeByOffset.Remove(code.Key);
                }

            }

        }

    }

    public class FCopScript {

        public bool failed = false;

        public string name = "";
        public string comment = "";

        public int offset;
        public int terminationOffset;
        public List<byte> compiledBytes = new();
        public List<ScriptNode> code = new();

        public FCopScript(int offset, List<byte> compiledBytes) {
            this.offset = offset;
            this.code = Decompile(offset, compiledBytes, out terminationOffset);
            this.compiledBytes = compiledBytes.GetRange(offset, terminationOffset - offset);
            this.name = "Script " + offset;
        }

        public FCopScript(int offset) {
            this.offset = offset;
            this.code = new();
            this.compiledBytes = new() { 0 };
            this.name = "Script " + offset;
        }

        public FCopScript(int offset, List<ScriptNode> code) {
            this.offset = offset;
            this.code = code;
            this.compiledBytes = new() { 0 };
            this.name = "Script " + offset;
        }

        public void Refresh() {

            code.Clear();
            code = Decompile(0, compiledBytes, out var _);

        }

        public int RemoveNode(ScriptNode scriptNode) {

            int indexOfRemoved;
            if (scriptNode.parent == null) {
                indexOfRemoved = code.IndexOf(scriptNode);
                code.Remove(scriptNode);
            }
            else {
                indexOfRemoved = scriptNode.parent.nestedNodes.IndexOf(scriptNode);
                scriptNode.parent.nestedNodes.Remove(scriptNode);
            }
            return indexOfRemoved;

        }

        public static Dictionary<ByteCode, int> maxArgumentsByCode = new() {

            { ByteCode.RANDOM, 1 },
            { ByteCode.QUEUE_STREAM, 1 },
            { ByteCode.PLAY_STREAM, 1 },
            { ByteCode.PLAY_STREAM_ON_ACT, 1 },
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
                new ScriptOperationData("Queue Stream", ScriptDataType.Void, new() { new ScriptParameter("", ScriptDataType.Stream) })
            },
            { new ScriptDataKey(ByteCode.PLAY_STREAM, 1),
                new ScriptOperationData("Play Stream", ScriptDataType.Void, new() { new ScriptParameter("", ScriptDataType.Stream) })
            },
            { new ScriptDataKey(ByteCode.PLAY_STREAM_ON_ACT, 1),
                new ScriptOperationData("Play Stream Actor", ScriptDataType.Void, new() { new ScriptParameter("", ScriptDataType.Stream) })
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
                new ScriptOperationData("If", ScriptDataType.Void, new() { new ScriptParameter("", ScriptDataType.Bool) })
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
                new ScriptOperationData("=", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Any), new ScriptParameter("Var ID", ScriptDataType.GlobalVar) })
            },
            { new ScriptDataKey(ByteCode.SET_17, 2),
                new ScriptOperationData("System Func", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Int), new ScriptParameter("Var ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.SET_18, 2),
                new ScriptOperationData("=", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Any), new ScriptParameter("Var ID", ScriptDataType.TimerVar) })
            },
            { new ScriptDataKey(ByteCode.SET_18, 1),
                new ScriptOperationData("=(18) Temp Debug 1", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.TimerVar)})
            },
            { new ScriptDataKey(ByteCode.SET_19, 2),
                new ScriptOperationData("=", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Any), new ScriptParameter("Var ID", ScriptDataType.UserVar) })
            },
            { new ScriptDataKey(ByteCode.EQUAL, 2),
                new ScriptOperationData("==", ScriptDataType.Bool, new() { new ScriptParameter("Left", ScriptDataType.Any), new ScriptParameter("Right", ScriptDataType.Any) })
            },
            { new ScriptDataKey(ByteCode.EQUAL, 1),
                new ScriptOperationData("==(1)", ScriptDataType.Bool, new() { new ScriptParameter("Par0", ScriptDataType.Any) })
            },
            { new ScriptDataKey(ByteCode.NOT_EQUAL, 2),
                new ScriptOperationData("!=", ScriptDataType.Bool, new() { new ScriptParameter("Left", ScriptDataType.Any), new ScriptParameter("Right", ScriptDataType.Any) })
            },
            { new ScriptDataKey(ByteCode.GREATER_THAN, 2),
                new ScriptOperationData(">", ScriptDataType.Bool, new() { new ScriptParameter("Left", ScriptDataType.Any), new ScriptParameter("Right", ScriptDataType.Any) })
            },
            { new ScriptDataKey(ByteCode.GREATER_THAN_OR_EQUAL, 2),
                new ScriptOperationData(">=", ScriptDataType.Bool, new() { new ScriptParameter("Left", ScriptDataType.Any), new ScriptParameter("Right", ScriptDataType.Any) })
            },
            { new ScriptDataKey(ByteCode.LESS_THAN, 2),
                new ScriptOperationData("<", ScriptDataType.Bool, new() { new ScriptParameter("Left", ScriptDataType.Any), new ScriptParameter("Right", ScriptDataType.Any) })
            },
            { new ScriptDataKey(ByteCode.LESS_THAN_OR_EQUAL, 2),
                new ScriptOperationData("<=", ScriptDataType.Bool, new() { new ScriptParameter("Left", ScriptDataType.Any), new ScriptParameter("Right", ScriptDataType.Any) })
            },
            { new ScriptDataKey(ByteCode.ADD, 2),
                new ScriptOperationData("+", ScriptDataType.Int, new() { new ScriptParameter("Left", ScriptDataType.Any), new ScriptParameter("Right", ScriptDataType.Any) })
            },
            { new ScriptDataKey(ByteCode.SUBTRACT, 2),
                new ScriptOperationData("-", ScriptDataType.Int, new() { new ScriptParameter("Left", ScriptDataType.Any), new ScriptParameter("Right", ScriptDataType.Any) })
            },
            { new ScriptDataKey(ByteCode.MULTIPLY, 2),
                new ScriptOperationData("*", ScriptDataType.Int, new() { new ScriptParameter("Left", ScriptDataType.Any), new ScriptParameter("Right", ScriptDataType.Any) })
            },
            { new ScriptDataKey(ByteCode.DIVIDE, 2),
                new ScriptOperationData("/", ScriptDataType.Int, new() { new ScriptParameter("Left", ScriptDataType.Any), new ScriptParameter("Right", ScriptDataType.Any) })
            },
            { new ScriptDataKey(ByteCode.MOD, 2),
                new ScriptOperationData("%", ScriptDataType.Int, new() { new ScriptParameter("Left", ScriptDataType.Any), new ScriptParameter("Right", ScriptDataType.Any) })
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
                new ScriptOperationData("+=", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Any), new ScriptParameter("Var ID", ScriptDataType.GlobalVar) })
            },
            { new ScriptDataKey(ByteCode.ADD_19_SET, 2),
                new ScriptOperationData("+=", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Any), new ScriptParameter("Var ID", ScriptDataType.UserVar) })
            },
            { new ScriptDataKey(ByteCode.SUB_16_SET, 2),
                new ScriptOperationData("-=", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Any), new ScriptParameter("Var ID", ScriptDataType.GlobalVar) })
            },
            { new ScriptDataKey(ByteCode.SUB_19_SET, 2),
                new ScriptOperationData("-=", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Any), new ScriptParameter("Var ID", ScriptDataType.UserVar) })
            },
            { new ScriptDataKey(ByteCode.ACTOR_FUNC, 3),
                new ScriptOperationData("Actor Func", ScriptDataType.Void, new() { new ScriptParameter("Actor", ScriptDataType.Actor), new ScriptParameter("Method", ScriptDataType.Int), new ScriptParameter("Parameter", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.ACTOR_FUNC, 2),
                new ScriptOperationData("Actor Func(2)", ScriptDataType.Void, new() { new ScriptParameter("Method", ScriptDataType.Int), new ScriptParameter("Parameter", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.GROUP_ACTOR_FUNC, 3),
                new ScriptOperationData("Actor Func", ScriptDataType.Void, new() { new ScriptParameter("Actor Group", ScriptDataType.Group), new ScriptParameter("Method", ScriptDataType.Int), new ScriptParameter("Parameter", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.TEAM_ACTOR_FUNC, 3),
                new ScriptOperationData("Actor Func", ScriptDataType.Void, new() { new ScriptParameter("Team", ScriptDataType.Team), new ScriptParameter("Method", ScriptDataType.Int), new ScriptParameter("Parameter", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.NAV_MESH_STATE_CHANGE, 3),
                new ScriptOperationData("Nav Mesh State Change", ScriptDataType.Void, new() { new ScriptParameter("Nav Mesh", ScriptDataType.Cnet), new ScriptParameter("Disabled", ScriptDataType.Bool), new ScriptParameter("Nav Mesh Node Index", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.SPAWNING_FUNC, 3),
                new ScriptOperationData("Spawning Func", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Actor), new ScriptParameter("Par1", ScriptDataType.Int), new ScriptParameter("Par2", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.GROUP_SPAWNING_FUNC, 3),
                new ScriptOperationData("Spawning Func", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Group), new ScriptParameter("Par1", ScriptDataType.Int), new ScriptParameter("Par2", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.STATIC_PROP_FUNC, 3),
                new ScriptOperationData("Static Prop Func", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int), new ScriptParameter("Par1", ScriptDataType.Int), new ScriptParameter("Par2", ScriptDataType.Int) })
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

        public static List<ByteCode> actorMethods = new() {
            ByteCode.ACTOR_FUNC,
            ByteCode.GROUP_ACTOR_FUNC,
            ByteCode.TEAM_ACTOR_FUNC,
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
                    else if (byteCode == ByteCode.SET_17) {
                        node = new SystemMethodNode(byteCode, scriptData.name, scriptData.defaultReturnType, new(scriptData.parameterData), parameters);
                    }
                    else if (actorMethods.Contains(byteCode)) {
                        node = new ActorMethodNode(byteCode, scriptData.name, scriptData.defaultReturnType, new(scriptData.parameterData), parameters);
                    }
                    else if (byteCode == ByteCode.SPAWNING_FUNC || byteCode == ByteCode.GROUP_SPAWNING_FUNC) {
                        node = new SpawningActorMethod(byteCode, scriptData.name, scriptData.defaultReturnType, new(scriptData.parameterData), parameters);
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

        public List<byte> Compile(int newOffset, bool isNCFC) {

            var total = new List<byte>();

            var nestedBytesStack = new List<List<byte>>();

            void CompileNode(ScriptNode node) {

                foreach (var parameter in node.parameters) {
                    CompileNode(parameter);
                }

                if (nestedBytesStack.Count != 0) {
                    nestedBytesStack.Last().AddRange(node.Compile(isNCFC));
                }
                else {
                    total.AddRange(node.Compile(isNCFC));
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

        public List<ScriptNode> CloneScripts() {

            var clonedScripts = new List<ScriptNode>();

            foreach (var node in code) {
                clonedScripts.Add(node.Clone());
            }

            return clonedScripts;

        }

    }

    public enum ByteCode {
        RUN = -3,
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
        PLAY_STREAM_ON_ACT = 14,
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
        // Add pseudo byte codes for NCFC file here:
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
        Enum,
        Cwav,
        ActorDirect,
        Actor,
        Group,
        Team,
        Cnet,
        Stream
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
            this.enumType = null;
            this.bitCount = BitCount.NA;
            this.bitPosition = 0;
        }

        public Type enumType;

        public ScriptParameter(string name, ScriptDataType dataType, Type enumType) : this(name, dataType) {
            this.enumType = enumType;
            this.bitCount = BitCount.NA;
            this.bitPosition = 0;
        }

        public BitCount bitCount;
        public int bitPosition;

        public ScriptParameter(string name, ScriptDataType dataType, BitCount bitCount, int bitPosition) : this(name, dataType) {
            this.bitCount = bitCount;
            this.bitPosition = bitPosition;
        }

        public ScriptParameter(string name, ScriptDataType dataType, Type enumType, BitCount bitCount, int bitPosition) : this(name, dataType, enumType) {
            this.bitCount = bitCount;
            this.bitPosition = bitPosition;
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

                parameters.Add(new ParameterNode(parNode, this, parData.name, i, parData.dataType, true));
            }

            return parameters;
        }

        public virtual List<byte> Compile(bool isNCFC) {

            if (byteCode == ByteCode.NONE || byteCode == ByteCode.RUN) {
                return new();
            }

            return new() { (byte)byteCode };
        }

        public virtual ScriptNode Clone() {

            var clonedParameters = new List<ScriptNode>();

            for (int i = 0; i < parameters.Count; i++) {
                clonedParameters.Add(parameters[i].Clone());
            }

            return new ScriptNode(byteCode, name, defaultReturnType, new(parameterData), clonedParameters);

        }

    }

    public class ScriptNestingNode : ScriptNode {

        public List<ScriptNode> nestedNodes = new();

        public ScriptNestingNode(ByteCode byteCode, string name, ScriptDataType defaultReturnType, List<ScriptParameter> parameterData, List<ScriptNode> parameters) : base(byteCode, name, defaultReturnType, parameterData, parameters) {
        }

        public bool NestNode(ScriptNode node, int requestedIndex) {

            var presentJumpNode = nestedNodes.FirstOrDefault(n => n.byteCode == ByteCode.JUMP);

            if (node is ScriptNestingNode && node.byteCode == ByteCode.JUMP) {

                // Cannot add two jumps
                if (presentJumpNode != null) {
                    return false;
                }

                // Cannot add a junp anywhere but a conditional jump
                if (byteCode != ByteCode.CONDITIONAL_JUMP) {
                    return false;
                }

                node.parent = this;
                nestedNodes.Add(node);
                return true;

            }

            node.parent = this;
            nestedNodes.Insert(requestedIndex, node);
            
            // Ensures the jump is last
            if (presentJumpNode != null) {
                nestedNodes.Remove(presentJumpNode);
                nestedNodes.Add(presentJumpNode);
            }

            return true;

        }

        public override ScriptNode Clone() {
            var baseClone = base.Clone();

            var cloneNode = new ScriptNestingNode(byteCode, name, defaultReturnType, new(parameterData), baseClone.parameters);

            foreach (var node in nestedNodes) {
                var clonedNestNode = node.Clone();
                cloneNode.nestedNodes.Add(clonedNestNode);
                clonedNestNode.parent = cloneNode;
            }

            return cloneNode;

        }

    }

    public class ParameterNode {

        public ScriptNode parent;
        public ScriptNode scriptNode;
        public string parameterName;
        public int parameterIndex;
        public ScriptDataType dataType;
        public bool acceptsExpression;

        public ParameterNode(ScriptNode scriptNode, ScriptNode parent, string parameterName, int parameterIndex, ScriptDataType dataType, bool acceptsExpression) {
            this.parent = parent;
            this.scriptNode = scriptNode;
            this.parameterName = parameterName;
            this.parameterIndex = parameterIndex;
            this.dataType = dataType;
            this.acceptsExpression = acceptsExpression;
        }

        public BitCount bitCount = BitCount.NA;
        public int bitPosition = 0;

        public ParameterNode(ScriptNode parent, ScriptNode scriptNode, string parameterName, int parameterIndex, ScriptDataType dataType, BitCount bitCount, int bitPosition) : this(parent, scriptNode, parameterName, parameterIndex, dataType, false) {
            this.bitCount = bitCount;
            this.bitPosition = bitPosition;
        }
    }

    public class EnumParameterNode : ParameterNode {

        public Type enumType;
        public bool affectsParameters;

        public EnumParameterNode(ScriptNode scriptNode, ScriptNode parent, string parameterName, int parameterIndex, Type enumType, bool affectsParameters) : base(scriptNode, parent, parameterName, parameterIndex, ScriptDataType.Enum, false) {
            this.enumType = enumType;
            this.affectsParameters = affectsParameters;
        }

        public EnumParameterNode(ScriptNode parent, ScriptNode scriptNode, string parameterName, int parameterIndex, BitCount bitCount, int bitPosition, Type enumType) : base(parent, scriptNode, parameterName, parameterIndex, ScriptDataType.Enum, bitCount, bitPosition) {
            this.enumType = enumType;
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
                parameters.Add(new ParameterNode(parNode, this, "", i, parData.dataType, true));
            }

            if (reversed) {
                parameters.Reverse();
            }

            return parameters;
        }

        public override ScriptNode Clone() {
            var baseClone = base.Clone();

            var cloneNode = new OperatorNode(byteCode, name, defaultReturnType, new(parameterData), baseClone.parameters, reversed);

            return cloneNode;
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

        public override List<byte> Compile(bool isNCFC) {

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
                    return base.Compile(isNCFC);
            }
        }

        public override List<ParameterNode> GetParameters() {

            varNode.RefreshData();
            
            if (parameters.Count == 2) {

                return new() {
                    new ParameterNode(varNode, this, "", 1, VariableNode.VarTypeToDataType(varNode.varibleType), false),
                    new ParameterNode(parameters[0], this, "", 0, varNode.defaultReturnType, true),

                };

            }
            else if (parameters.Count == 1) {
                return new() {
                    new ParameterNode(varNode, this, "", 0, VariableNode.VarTypeToDataType(varNode.varibleType), false),
                };
            }
            else {
                return base.GetParameters();
            }

        }

        public override ScriptNode Clone() {

            for (var i = 0; i < parameterData.Count; i++) {

                if (FCopScript.varDataTypes.Contains(parameterData[i].dataType)) {
                    var data = parameterData[i];
                    data.dataType = VariableNode.VarTypeToDataType(varNode.varibleType);
                    parameterData[i] = data;
                }

            }
            
            var baseClone = base.Clone();

            var cloneNode = new VariableAssignmentNode(byteCode, name, defaultReturnType, new(parameterData), baseClone.parameters, reversed);
            cloneNode.assignmentType = assignmentType;

            return cloneNode;
        }

    }

    public class SystemMethodNode : ScriptNode {

        public enum SystemMethod {
            None = 0,
            EndGame = 2,
            PlaySound = 3
        }

        static Dictionary<int, ScriptParameter> methodParamters = new() {
            { 0, new ScriptParameter("None", ScriptDataType.Any) },
            { 2, new ScriptParameter("Timer", ScriptDataType.Int) },
            { 3, new ScriptParameter("Sound", ScriptDataType.Cwav) },
        };

        public LiteralNode methodID;

        public SystemMethodNode(ByteCode byteCode, string name, ScriptDataType defaultReturnType, List<ScriptParameter> parameterData, List<ScriptNode> parameters) : base(byteCode, name, defaultReturnType, parameterData, parameters) {

            if (parameters.Count > 1 && parameters[1].GetType() == typeof(LiteralNode)) {
                methodID = (LiteralNode)parameters[1];
            }

        }

        public override List<ParameterNode> GetParameters() {

            if (methodID == null) {
                return base.GetParameters();
            }

            var methodParameter = methodParamters[methodID.value];

            return new() {
                new EnumParameterNode(methodID, this, "", 1, typeof(SystemMethod), true),
                new ParameterNode(parameters[0], this, methodParameter.name, 0, methodParameter.dataType, true),
            };

        }

        public override ScriptNode Clone() {
            var baseClone = base.Clone();

            var cloneNode = new SystemMethodNode(byteCode, name, defaultReturnType, new(parameterData), baseClone.parameters);

            return cloneNode;
        }

    }

    public class ActorMethodNode : ScriptNode {

        public enum ActorMethod {
            None = 0,
            Unknown2 = 2,
            Unknown7 = 7,
            CanPath = 19,
            Unknown22 = 22,
            EnableShooter = 30,
            Unknown32 = 32,
            Unknown38 = 38,
            EnableSpinning = 45,
            ChangeObject = 46,
            PlayAnimation = 50,
            Unknown51 = 51,
            Unknown52 = 52,
            PlaySound = 57,
            SetCollideDamage = 58,
            SetColliding = 59,
            Hurt = 60,
            Despawn = 61,
            SetMapColor = 62,
            SetInvincibility = 63,
            SetPlayerTargeting = 64,
            SetHealthDoNotUse = 65,
            SetTeam = 67,
            SetUVOffsetY = 68,
            SetRendering = 69,
            ChangeNodeVisibility = 75,
            ChangeNodeColor = 76,
            MoveProp = 80,
            MoveElevator = 82,
            SetMovingElevator = 83,
            Unknown85 = 85,
            Unknown86 = 86,
            Unknown91 = 91,
            ChangeCamera = 97,
            PlayEffect = 100,
            Unknown101 = 101,
            Unknown103 = 103,
            Unknown110 = 110,
            ScriptEnable = 122,
            EnableTrigger = 124,

        }

        public static Dictionary<Type, List<ActorMethod>> methods = new() {
            { typeof(FCopActorBehavior), new() { 
                ActorMethod.None,
                ActorMethod.Hurt,
                ActorMethod.Despawn,
                ActorMethod.Unknown101,
            } },
            { typeof(FCopEntity), new() {
                ActorMethod.PlayAnimation,
                ActorMethod.PlaySound,
                ActorMethod.SetCollideDamage,
                ActorMethod.SetColliding,
                ActorMethod.SetMapColor,
                ActorMethod.SetInvincibility,
                ActorMethod.SetPlayerTargeting,
                ActorMethod.SetHealthDoNotUse,
                ActorMethod.SetTeam,
                ActorMethod.SetUVOffsetY,
                ActorMethod.SetRendering,
                ActorMethod.PlayEffect
            } },
            { typeof(FCopShooter), new() {
                ActorMethod.EnableShooter
            } },
            { typeof(FCopPathedEntity), new() {
                ActorMethod.CanPath
            } },
            { typeof(FCopTurret), new() {
                ActorMethod.EnableSpinning
            } },
            { typeof(FCopBehavior1), new() {
                ActorMethod.ChangeCamera
            } },
            { typeof(FCopBehavior10), new() {
                ActorMethod.MoveElevator,
                ActorMethod.SetMovingElevator
            } },
            { typeof(FCopBehavior14), new() {
                ActorMethod.ScriptEnable
            } },
            { typeof(FCopBehavior25), new() {
                ActorMethod.MoveProp
            } },
            { typeof(FCopBehavior27), new() {
                ActorMethod.Unknown85
            } },
            { typeof(FCopBehavior30), new() {
                ActorMethod.ChangeObject
            } },
            { typeof(FCopBehavior31), new() {
                ActorMethod.Unknown110
            } },
            { typeof(FCopBehavior35), new() {
                ActorMethod.ChangeNodeVisibility,
                ActorMethod.ChangeNodeColor
            } },
            { typeof(FCopBehavior95), new() {
                ActorMethod.EnableTrigger
            } },
        };

        static Dictionary<int, List<ScriptParameter>> methodParamters = new() {
            { 0, new() { new ScriptParameter("Par", ScriptDataType.Any) } },
            { 2, new() { new ScriptParameter("Par", ScriptDataType.Any) } },
            { 7, new() { new ScriptParameter("Par", ScriptDataType.Any) } },
            { 19, new() { new ScriptParameter("", ScriptDataType.Bool) } },
            { 22, new() { new ScriptParameter("Par", ScriptDataType.Any) } },
            { 30, new() { new ScriptParameter("Enable", ScriptDataType.Bool) } },
            { 32, new() { new ScriptParameter("Par", ScriptDataType.Any) } },
            { 38, new() { new ScriptParameter("Par", ScriptDataType.Any) } },
            { 45, new() { new ScriptParameter("Enable", ScriptDataType.Bool) } },
            { 46, new() { new ScriptParameter("Change Object", ScriptDataType.Int) } },
            { 50, new() { new ScriptParameter("Animation", ScriptDataType.Int) } },
            { 51, new() { new ScriptParameter("Unknown", ScriptDataType.Any) } },
            { 52, new() { new ScriptParameter("Unknown", ScriptDataType.Any) } },
            { 57, new() { new ScriptParameter("Sound", ScriptDataType.Cwav) } },
            { 58, new() { new ScriptParameter("Value", ScriptDataType.Int) } },
            { 59, new() { new ScriptParameter("Can Collide", ScriptDataType.Bool) } },
            { 60, new() { new ScriptParameter("Hurt Value", ScriptDataType.Int) } },
            { 61, new() { new ScriptParameter("", ScriptDataType.Bool) } },
            { 62, new() { new ScriptParameter("Map Color", ScriptDataType.Enum, typeof(MapIconColor)) } },
            { 63, new() { new ScriptParameter("Is Invincible", ScriptDataType.Bool) } },
            { 64, new() { new ScriptParameter("Disable Targeting", ScriptDataType.Bool) } },
            { 65, new() { new ScriptParameter("Value (0 - 255)", ScriptDataType.Int) } },
            { 67, new() { new ScriptParameter("Team", ScriptDataType.Team) } },
            { 68, new() { new ScriptParameter("Offset", ScriptDataType.Int) } },
            { 69, new() { new ScriptParameter("Does Render", ScriptDataType.Bool) } },
            { 75, new() { 
                new ScriptParameter("Show Arrow", ScriptDataType.Bool, BitCount.Bit1, 0),
                new ScriptParameter("Show Satellite", ScriptDataType.Bool, BitCount.Bit1, 1),
                new ScriptParameter("Show Mini Map", ScriptDataType.Bool, BitCount.Bit1, 2),
                new ScriptParameter("Node", ScriptDataType.Int, BitCount.Bit5, 3),
            } },
            { 76, new() {
                new ScriptParameter("Color", ScriptDataType.Enum, typeof(MapIconColorObjective), BitCount.Bit5, 0),
                new ScriptParameter("Node", ScriptDataType.Int, BitCount.Bit3, 5),
            } },
            { 80, new() { new ScriptParameter("To Start", ScriptDataType.Bool) } },
            { 82, new() { new ScriptParameter("Move Type", ScriptDataType.Enum, typeof(ElevatorMoveType)) } },
            { 83, new() { new ScriptParameter("Move", ScriptDataType.Bool) } },
            { 85, new() { new ScriptParameter("Unknown", ScriptDataType.Int) } },
            { 86, new() { new ScriptParameter("Unknown", ScriptDataType.Int) } },
            { 91, new() { new ScriptParameter("Unknown", ScriptDataType.Any) } },
            { 97, new() { new ScriptParameter("Camera Type", ScriptDataType.Enum, typeof(PlayerCameraType)) } },
            { 100, new() { new ScriptParameter("Unknown", ScriptDataType.Int) } },
            { 101, new() { new ScriptParameter("Unknown", ScriptDataType.Int) } },
            { 103, new() { new ScriptParameter("Unknown", ScriptDataType.Int) } },
            { 110, new() { new ScriptParameter("Unknown", ScriptDataType.Int) } },
            { 122, new() { new ScriptParameter("Enable Script", ScriptDataType.Bool) } },
            { 124, new() { new ScriptParameter("Enable", ScriptDataType.Bool) } },

        };

        public static List<ScriptDataType> allowedActorRefs = new() {
            ScriptDataType.Actor,
            ScriptDataType.Group,
            ScriptDataType.Team
        };

        public LiteralNode methodID;

        public ActorMethodNode(ByteCode byteCode, string name, ScriptDataType defaultReturnType, List<ScriptParameter> parameterData, List<ScriptNode> parameters) : base(byteCode, name, defaultReturnType, parameterData, parameters) {

            if (parameters.Count > 1 && parameters[1].GetType() == typeof(LiteralNode)) {
                methodID = (LiteralNode)parameters[1];
            }

        }

        public ParameterNode GetActorRefProperty() {

            if (parameters[0] is LiteralNode litNode) {

                return new ParameterNode(litNode, this, "", 0, parameterData[0].dataType, false);

            }
            else {
                return null;
            }

        }

        public void SetActorRef(ScriptDataType dataType, int id) {

            if (parameters[0] is LiteralNode litNode) {

                litNode.value = id;
                parameterData[0] = new ScriptParameter(parameterData[0].name, dataType);

            }

        }

        public override List<ParameterNode> GetParameters() {

            if (methodID == null) {
                return base.GetParameters();
            }

            if (parameters.Count != 3) {
                return base.GetParameters();
            }

            List<ScriptParameter> methodParameters;

            try {
                methodParameters = methodParamters[methodID.value];
            }
            catch {
                return base.GetParameters();
            }

            var totalParameters = new List<ParameterNode> {
                new ParameterNode(parameters[0], this, "", 0, parameterData[0].dataType, false),
                new EnumParameterNode(methodID, this, "", 1, typeof(ActorMethod), true),
            };

            foreach (var method in methodParameters) {

                if (method.dataType == ScriptDataType.Enum) {
                    totalParameters.Add(new EnumParameterNode(parameters[2], this, method.name, 2, method.bitCount, method.bitPosition, method.enumType));
                }
                else {
                    totalParameters.Add(new ParameterNode(parameters[2], this, method.name, 2, method.dataType, method.bitCount, method.bitPosition));
                }

            }

            return totalParameters;

        }

        public override List<byte> Compile(bool isNCFC) {

            if (parameters.Count == 3 && parameters[0] is LiteralNode) {

                if (parameterData[0].dataType == ScriptDataType.Actor) {
                    return new() { (byte)ByteCode.ACTOR_FUNC };
                }
                if (parameterData[0].dataType == ScriptDataType.Group) {
                    return new() { (byte)ByteCode.GROUP_ACTOR_FUNC };
                }
                if (parameterData[0].dataType == ScriptDataType.Team) {
                    return new() { (byte)ByteCode.TEAM_ACTOR_FUNC };
                }

                return base.Compile(isNCFC);

            }

            return base.Compile(isNCFC);
        }

        public override ScriptNode Clone() {
            var baseClone = base.Clone();

            var cloneNode = new ActorMethodNode(byteCode, name, defaultReturnType, new(parameterData), baseClone.parameters);

            return cloneNode;
        }

    }

    public class SpawningActorMethod : ScriptNode {

        public enum SpawningMethod {
            None = 0,
            Unknown = 60,
            SetRespawning = 70,
            Spawn = 71
        }

        static Dictionary<int, ScriptParameter> methodParameters = new() {
            { 0, new ScriptParameter("None", ScriptDataType.Bool) },
            { 60, new ScriptParameter("Unknown", ScriptDataType.Bool) },
            { 70, new ScriptParameter("Can Respawn", ScriptDataType.Bool) },
            { 71, new ScriptParameter("", ScriptDataType.Bool) }
        };

        public static List<ScriptDataType> allowedActorRefs = new() {
            ScriptDataType.Actor,
            ScriptDataType.Group
        };

        public LiteralNode methodID;

        public SpawningActorMethod(ByteCode byteCode, string name, ScriptDataType defaultReturnType, List<ScriptParameter> parameterData, List<ScriptNode> parameters) : base(byteCode, name, defaultReturnType, parameterData, parameters) {

            if (parameters.Count > 1 && parameters[1].GetType() == typeof(LiteralNode)) {
                methodID = (LiteralNode)parameters[1];
            }

        }

        public ParameterNode GetActorRefProperty() {

            if (parameters[0] is LiteralNode litNode) {

                return new ParameterNode(litNode, this, "", 0, parameterData[0].dataType, false);

            }
            else {
                return null;
            }

        }

        public void SetActorRef(ScriptDataType dataType, int id) {

            if (parameters[0] is LiteralNode litNode) {

                litNode.value = id;
                parameterData[0] = new ScriptParameter(parameterData[0].name, dataType);

            }

        }

        public override List<ParameterNode> GetParameters() {

            if (methodID == null) {
                return base.GetParameters();
            }

            if (parameters.Count != 3) {
                return base.GetParameters();
            }

            ScriptParameter methodParameter;

            try {
                methodParameter = methodParameters[methodID.value];
            }
            catch {
                return base.GetParameters();
            }

            var totalParameters = new List<ParameterNode> {
                new ParameterNode(parameters[0], this, "", 0, parameterData[0].dataType, false),
                new EnumParameterNode(methodID, this, "", 1, typeof(SpawningMethod), true),
                new ParameterNode(parameters[2], this, methodParameter.name, 2, methodParameter.dataType, true)
            };

            return totalParameters;

        }

        public override List<byte> Compile(bool isNCFC) {

            if (parameters.Count == 3 && parameters[0] is LiteralNode) {

                if (parameterData[0].dataType == ScriptDataType.Actor) {
                    return new() { (byte)ByteCode.SPAWNING_FUNC };
                }
                if (parameterData[0].dataType == ScriptDataType.Group) {
                    return new() { (byte)ByteCode.GROUP_SPAWNING_FUNC };
                }

                return base.Compile(isNCFC);

            }

            return base.Compile(isNCFC);
        }

        public override ScriptNode Clone() {
            var baseClone = base.Clone();

            var cloneNode = new SpawningActorMethod(byteCode, name, defaultReturnType, new(parameterData), baseClone.parameters);

            return cloneNode;
        }

    }

    public class LiteralNode : ScriptNode {

        public int value;

        public LiteralNode() : base() {

        }

        public LiteralNode(int value) : base(ByteCode.LITERAL, "", ScriptDataType.Int, new(), new()) {
            this.value = value;
        }

        public override List<byte> Compile(bool isNCFC) {
            
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

        public override ScriptNode Clone() {
            return new LiteralNode(value);
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

        public override List<byte> Compile(bool isNCFC) {

            if (!isGet) {
                return new() { (byte)(value + 128) };
            }
            else {
                return new() { (byte)(value + 128), (byte)varibleType };
            }

        }

        public override ScriptNode Clone() {
            var clone = new VariableNode(isGet, varibleType, value);
            clone.allowedVarTypeConverstion = new(allowedVarTypeConverstion);
            return clone;
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