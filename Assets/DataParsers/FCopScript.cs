
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace FCopParser {

    public class FCopScriptingProject {

        public FCopRPNS rpns;
        public FCopFunctionParser functionParser;
        public int emptyOffset;

        public FCopScriptingProject(FCopRPNS rpns, FCopFunctionParser functionParser) {
            this.rpns = rpns;
            this.functionParser = functionParser;
            emptyOffset = rpns.code.Last().Key;
        }

        public List<IFFDataFile> Compile() {

            List<IFFDataFile> total = new() {
                rpns.Compile(),
                functionParser.Compile()
            };

            emptyOffset = rpns.code.Last().Value.offset;

            return total;
        }

        public void ResetIDAndOffsets() {
            rpns.ResetKeys();
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
            { ByteCode.Destroy, 3 },
            { ByteCode.BYTE57, 3 },
            { ByteCode.BYTE58, 3 },
            { ByteCode.BYTE59, 3 },
            { ByteCode.Spawn, 3 },
            { ByteCode.SpawnAll, 3 },
            { ByteCode.BYTE62, 3 }

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
                new ScriptOperationData("++(16)", ScriptDataType.Void, new() { new ScriptParameter("Var ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.INCREMENT_19, 1),
                new ScriptOperationData("++(19)", ScriptDataType.Void, new() { new ScriptParameter("Var ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.DECREMENT_16, 1),
                new ScriptOperationData("--(16)", ScriptDataType.Void, new() { new ScriptParameter("Var ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.DECREMENT_19, 1),
                new ScriptOperationData("--(19)", ScriptDataType.Void, new() { new ScriptParameter("Var ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.SET_16, 2),
                new ScriptOperationData("=(16)", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Int), new ScriptParameter("Var ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.SET_17, 2),
                new ScriptOperationData("30", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Int), new ScriptParameter("Var ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.SET_18, 2),
                new ScriptOperationData("=(18)", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int), new ScriptParameter("Par1", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.SET_18, 1),
                new ScriptOperationData("=(18) Temp Debug 1", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int)})
            },
            { new ScriptDataKey(ByteCode.SET_19, 2),
                new ScriptOperationData("=(19)", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int), new ScriptParameter("Par1", ScriptDataType.Int) })
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
                new ScriptOperationData("+=(16)", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Int), new ScriptParameter("Var ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.ADD_19_SET, 2),
                new ScriptOperationData("+=(19)", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Int), new ScriptParameter("Var ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.SUB_16_SET, 2),
                new ScriptOperationData("-=(16)", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Int), new ScriptParameter("Var ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.SUB_19_SET, 2),
                new ScriptOperationData("-=(19)", ScriptDataType.Void, new() { new ScriptParameter("Value", ScriptDataType.Int), new ScriptParameter("Var ID", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.Destroy, 3),
                new ScriptOperationData("56", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int), new ScriptParameter("Par1", ScriptDataType.Int), new ScriptParameter("Par2", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.Destroy, 2),
                new ScriptOperationData("56(2)", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int), new ScriptParameter("Par1", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.BYTE57, 3),
                new ScriptOperationData("57", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int), new ScriptParameter("Par1", ScriptDataType.Int), new ScriptParameter("Par2", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.BYTE58, 3),
                new ScriptOperationData("58", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int), new ScriptParameter("Par1", ScriptDataType.Int), new ScriptParameter("Par2", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.BYTE59, 3),
                new ScriptOperationData("59", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int), new ScriptParameter("Par1", ScriptDataType.Int), new ScriptParameter("Par2", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.Spawn, 3),
                new ScriptOperationData("60", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int), new ScriptParameter("Par1", ScriptDataType.Int), new ScriptParameter("Par2", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.SpawnAll, 3),
                new ScriptOperationData("61", ScriptDataType.Void, new() { new ScriptParameter("Par0", ScriptDataType.Int), new ScriptParameter("Par1", ScriptDataType.Int), new ScriptParameter("Par2", ScriptDataType.Int) })
            },
            { new ScriptDataKey(ByteCode.BYTE62, 3),
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

        public static List<ByteCode> reverseDoubleOperators = new() {
            ByteCode.SET_16,
            ByteCode.SET_18,
            ByteCode.SET_19,
            ByteCode.ADD_16_SET,
            ByteCode.ADD_19_SET,
            ByteCode.SUB_16_SET,
            ByteCode.SUB_19_SET
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

                        var jumpNode = new ScriptNestingNode(ByteCode.JUMP, "Else", ScriptDataType.Void, new());

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

                    ScriptNode node;

                    if (doubleOperators.Contains(byteCode) && paraCount == 2) {
                        node = new DoubleOperator(byteCode, scriptData.name, scriptData.defaultReturnType, new(scriptData.parameterData), false);
                    }
                    else if (reverseDoubleOperators.Contains(byteCode) && paraCount == 2) {
                        node = new DoubleOperator(byteCode, scriptData.name, scriptData.defaultReturnType, new(scriptData.parameterData), true);
                    }
                    else if (byteCode == ByteCode.CONDITIONAL_JUMP) {
                        node = new ScriptNestingNode(byteCode, scriptData.name, scriptData.defaultReturnType, new(scriptData.parameterData));
                    }
                    else {
                        node = new ScriptNode(byteCode, scriptData.name, scriptData.defaultReturnType, new(scriptData.parameterData));
                    }

                    node.parameters = floatingExpressions.GetRange(floatingExpressions.Count - paraCount, paraCount);
                    floatingExpressions.RemoveRange(floatingExpressions.Count - paraCount, paraCount);

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
        Destroy = 56,
        BYTE57 = 57,
        BYTE58 = 58,
        BYTE59 = 59,
        Spawn = 60,
        SpawnAll = 61,
        BYTE62 = 62,
    }

    public enum ScriptDataType {
        Void,
        Any,
        Int,
        Bool,
        Asset
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

        public ScriptNode(ByteCode byteCode, string name, ScriptDataType defaultReturnType, List<ScriptParameter> parameterData) {
            this.byteCode = byteCode;
            this.name = name;
            this.defaultReturnType = defaultReturnType;
            this.parameterData = parameterData;
        }

        public ScriptNode(ByteCode byteCode, string name, ScriptDataType defaultReturnType, List<ScriptParameter> parameterData, List<ScriptNode> parameters) : this(byteCode, name, defaultReturnType, parameterData) {
            this.parameters = parameters;
        }

        public virtual ScriptDataType ReturnType() {
            return defaultReturnType;
        }

        public virtual List<ParameterNode> GetParameters() {

            var parameters = new List<ParameterNode>();

            foreach (var i in Enumerable.Range(0, this.parameters.Count)) {
                var parData = parameterData[i];
                var parNode = this.parameters[i];

                parameters.Add(new ParameterNode(parNode, this, parData.name, i));
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

        public ScriptNestingNode(ByteCode byteCode, string name, ScriptDataType defaultReturnType, List<ScriptParameter> parameterData) : base(byteCode, name, defaultReturnType, parameterData) {
        }

    }

    public class ParameterNode {

        public ScriptNode parent;
        public ScriptNode scriptNode;
        public string parameterName;
        public int parameterIndex;

        public ParameterNode(ScriptNode scriptNode, ScriptNode parent, string parameterName, int parameterIndex) {
            this.parent = parent;
            this.scriptNode = scriptNode;
            this.parameterName = parameterName;
            this.parameterIndex = parameterIndex;
        }

    }

    public class DoubleOperator : ScriptNode {

        public bool reversed;

        public DoubleOperator(ByteCode byteCode, string name, ScriptDataType defaultReturnType, List<ScriptParameter> parameterData, bool reversed) : base(byteCode, name, defaultReturnType, parameterData) {
            this.reversed = reversed;
        }

        public override List<ParameterNode> GetParameters() {
            var parameters = new List<ParameterNode>();

            foreach (var i in Enumerable.Range(0, this.parameters.Count)) {
                var parData = parameterData[i];
                var parNode = this.parameters[i];
                parameters.Add(new ParameterNode(parNode, this, "", i));
            }

            if (reversed) {
                parameters.Reverse();
            }

            return parameters;
        }

    }

    public class LiteralNode : ScriptNode {

        public int value;

        public LiteralNode(int value) : base(ByteCode.LITERAL, "", ScriptDataType.Int, new()) {
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


}