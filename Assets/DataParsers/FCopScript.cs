
using System;
using System.Collections.Generic;
using System.Linq;

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

        public class ScriptNode {

            // The difference between the two is expressive is used for parameters or other values
            // nested nodes is for like code blocks
            public List<ScriptNode> nestedExpressiveNodes = new();
            public List<ScriptNode> nestedNodes = new();
            public ScriptNode parent = null;

            public int expectedNestedNodeCount;

            public string name;

            public ScriptNode(int expectedNestedNodeCount) {
                this.expectedNestedNodeCount = expectedNestedNodeCount;
            }

        }

        public class ExpressionNode : ScriptNode {

            public Operator operationType;

            public ExpressionNode(Operator operationType, int expectedNestedNodeCount) : base(expectedNestedNodeCount) {
                this.operationType = operationType;
            }

        }

        public class StatementNode : ScriptNode {

            public Instruction instruction;

            public StatementNode(Instruction instruction, int expectedNestedNodeCount) : base(expectedNestedNodeCount) {
                this.instruction = instruction;
            }
        }

        public class VariableNode : ScriptNode {

            public int varID;

            public VariableNode(int varID) : base(1) {
                this.varID = varID;
            }

        }

        public class ActorRefNode : ScriptNode {

            public int actorID;

            public ActorRefNode(int actorID) : base(0) {
                this.actorID = actorID;
            }

        }

        public class LiteralNode : ScriptNode {

            public int value;

            public LiteralNode(int value) : base(0) {
                this.value = value;
            }

        }

        public class Expression {

            public Operator operationType;
            public List<Expression> nestedExpressions = new();
            public object value = null;

            public int byteCount;
            public ScriptPrimitiveType returnType;

            public Expression(List<Expression> nestedExpressions, Operator operationType, int byteCount, ScriptPrimitiveType returnType) {
                this.nestedExpressions = nestedExpressions;
                this.operationType = operationType;
                this.byteCount = byteCount;
                this.returnType = returnType;
            }

            public Expression(object value, Operator operationType, int byteCount, ScriptPrimitiveType returnType) {
                this.value = value;
                this.operationType = operationType;
                this.byteCount = byteCount;
                this.returnType = returnType;

            }

        }

        public class Statement {

            public Instruction instruction;
            public List<Expression> parametes = new();

            public int byteCount;

            public Statement(Instruction instruction, int byteCount) {
                this.instruction = instruction;
                this.byteCount = byteCount;
            }

            public int GetTotalStatementByteCount() {

                int GetTotalExpressionByteCount(Expression expression) {

                    int total = expression.byteCount;

                    foreach (var exp in expression.nestedExpressions) {
                        total += GetTotalExpressionByteCount(exp);
                    }

                    return total;

                }

                var total = byteCount;

                foreach (var par in parametes) {
                    total += GetTotalExpressionByteCount(par);
                }

                return total;

            }

        }

        public struct ScriptOperationMetaData {

            public bool leftRightOperation;
            public bool topLevelReverseleftRightOperation;
            public string topLevelOperatorString;
            public string topLevelName;

            public List<ParameterMetaData> parameters;
            public int parameterCount;
            public ScriptPrimitiveType returnType;

            public bool overloaded;
            public List<Operator> overloadedOperators;
            public List<Instruction> overloadedInstructions;

            public ScriptOperationMetaData(bool leftRightOperation, bool topLevelReverseleftRightOperation, string topLevelOperatorString, string topLevelName, List<ParameterMetaData> parameters, ScriptPrimitiveType returnType) {
                this.leftRightOperation = leftRightOperation;
                this.topLevelReverseleftRightOperation = topLevelReverseleftRightOperation;
                this.topLevelOperatorString = topLevelOperatorString;
                this.topLevelName = topLevelName;
                this.parameters = parameters;
                this.parameterCount = parameters.Count;
                this.returnType = returnType;
                this.overloaded = false;
                overloadedOperators = null;
                overloadedInstructions = null;
            }

            public ScriptOperationMetaData(bool leftRightOperation, string topLevelOperatorString, string topLevelName, List<ParameterMetaData> parameters, ScriptPrimitiveType returnType) {
                this.leftRightOperation = leftRightOperation;
                this.topLevelReverseleftRightOperation = false;
                this.topLevelOperatorString = topLevelOperatorString;
                this.topLevelName = topLevelName;
                this.parameters = parameters;
                this.parameterCount = parameters.Count;
                this.returnType = returnType;
                this.overloaded = false;
                overloadedOperators = null;
                overloadedInstructions = null;
            }

            public ScriptOperationMetaData(string topLevelName, List<ParameterMetaData> parameters) : this() {
                this.leftRightOperation = false;
                this.topLevelReverseleftRightOperation = false;
                this.topLevelOperatorString = "";
                this.topLevelName = topLevelName;
                this.parameters = parameters;
                this.parameterCount = parameters.Count;
                this.returnType = ScriptPrimitiveType.Void;
                this.overloaded = false;
                overloadedOperators = null;
                overloadedInstructions = null;
            }

            public ScriptOperationMetaData(string topLevelName, List<ParameterMetaData> parameters, ScriptPrimitiveType returnType) {
                this.leftRightOperation = false;
                this.topLevelReverseleftRightOperation = false;
                this.topLevelOperatorString = "";
                this.topLevelName = topLevelName;
                this.parameters = parameters;
                this.parameterCount = parameters.Count;
                this.returnType = returnType;
                this.overloaded = false;
                overloadedOperators = null;
                overloadedInstructions = null;
            }

            public ScriptOperationMetaData(string topLevelOperatorString, string topLevelName, List<ParameterMetaData> parameters) : this() {
                this.leftRightOperation = false;
                this.topLevelReverseleftRightOperation = false;
                this.topLevelOperatorString = topLevelOperatorString;
                this.topLevelName = topLevelName;
                this.parameters = parameters;
                this.parameterCount = parameters.Count;
                this.returnType = ScriptPrimitiveType.Void;
                this.overloaded = false;
                overloadedOperators = null;
                overloadedInstructions = null;
            }

            public ScriptOperationMetaData(bool leftRightOperation, string topLevelOperatorString, string topLevelName, List<ParameterMetaData> parameters, ScriptPrimitiveType returnType, List<Operator> overloadedOperators) : this() {
                this.leftRightOperation = leftRightOperation;
                this.topLevelReverseleftRightOperation = false;
                this.topLevelOperatorString = topLevelOperatorString;
                this.topLevelName = topLevelName;
                this.parameters = parameters;
                this.parameterCount = parameters.Count;
                this.returnType = returnType;
                this.overloaded = true;
                this.overloadedOperators = overloadedOperators;
                overloadedInstructions = null;
            }

            public ScriptOperationMetaData(bool leftRightOperation, string topLevelOperatorString, string topLevelName, List<ParameterMetaData> parameters, ScriptPrimitiveType returnType, List<Instruction> overloadedInstructions) : this() {
                this.leftRightOperation = leftRightOperation;
                this.topLevelReverseleftRightOperation = false;
                this.topLevelOperatorString = topLevelOperatorString;
                this.topLevelName = topLevelName;
                this.parameters = parameters;
                this.parameterCount = parameters.Count;
                this.returnType = returnType;
                this.overloaded = true;
                this.overloadedOperators = null;
                this.overloadedInstructions = overloadedInstructions;
            }

        }

        public struct ParameterMetaData {

            public string name;
            public ScriptPrimitiveType type;

            public ParameterMetaData(string name, ScriptPrimitiveType type) {
                this.name = name;
                this.type = type;
            }

        }

        public enum ScriptPrimitiveType {
            Void = 0,
            Int = 1,
            Bool = 2,
            VarRef = 3,
            ActorRef = 4
        }

        public enum Operator {

            Literal = 256,
            Unknown11 = 11,
            GET_16 = 16,
            GET_18 = 18,
            GET_19 = 19,
            Equal = 33,
            NotEqual = 34,
            GreaterThan = 35,
            GreaterThanOrEqual = 36,
            LessThan = 37,
            LessThanOrEqual = 38,
            Add = 39,
            Subtract = 40,
            And = 44,
            Unknown47 = 47,
            // Overloads
            Is = 289

        }

        public enum Instruction {

            None = 256,
            End = 0,
            Jump = 8,
            Unknown12 = 12,
            Unknown13 = 13,
            ConditionalJump = 20,
            Increment = 21,
            INCREMENT_19 = 24,
            Decrement = 25,
            DECREMENT_19 = 28,
            Set = 29,
            Sound = 30,
            Unknown31 = 31,
            SET_19 = 32,
            Add = 48,
            Unkown51 = 51,
            Subtract = 52,
            Destroy = 56,
            Unknown57 = 57,
            Unknown59 = 59,
            Spawn = 60,
            SpawnAll = 61

        }

        public static Dictionary<Operator, ScriptOperationMetaData> operatorMetaData = new Dictionary<Operator, ScriptOperationMetaData>() {
            { Operator.Literal, new ScriptOperationMetaData("Literal", new(), ScriptPrimitiveType.Int) },
            { Operator.Unknown11, new ScriptOperationMetaData("Unknown11", new() { new ParameterMetaData("par1", ScriptPrimitiveType.Int) }, ScriptPrimitiveType.Int) },
            { Operator.GET_16, new ScriptOperationMetaData("Get 16", new() { new ParameterMetaData("VariableID", ScriptPrimitiveType.VarRef) }, ScriptPrimitiveType.Int) },
            { Operator.GET_18, new ScriptOperationMetaData("Get 18", new() { new ParameterMetaData("VariableID", ScriptPrimitiveType.VarRef) }, ScriptPrimitiveType.Int) },
            { Operator.GET_19, new ScriptOperationMetaData("Get 19", new() { new ParameterMetaData("VariableID", ScriptPrimitiveType.VarRef) }, ScriptPrimitiveType.Int) },
            { Operator.Equal, new ScriptOperationMetaData(true, "==", "Is Equal", new() { new ParameterMetaData("Left", ScriptPrimitiveType.Int), new ParameterMetaData("Right", ScriptPrimitiveType.Int) }, ScriptPrimitiveType.Bool, new List<Operator>() { Operator.Is }) },
            { Operator.NotEqual, new ScriptOperationMetaData(true, "!=", "Is Not Equal", new() { new ParameterMetaData("Left", ScriptPrimitiveType.Int), new ParameterMetaData("Right", ScriptPrimitiveType.Int) }, ScriptPrimitiveType.Bool) },
            { Operator.GreaterThan, new ScriptOperationMetaData(true, ">", "Is Greater Than", new() { new ParameterMetaData("Left", ScriptPrimitiveType.Int), new ParameterMetaData("Right", ScriptPrimitiveType.Int) }, ScriptPrimitiveType.Bool) },
            { Operator.GreaterThanOrEqual, new ScriptOperationMetaData(true, ">=", "Is Greater Than Or Equal", new() { new ParameterMetaData("Left", ScriptPrimitiveType.Int), new ParameterMetaData("Right", ScriptPrimitiveType.Int) }, ScriptPrimitiveType.Bool) },
            { Operator.LessThan, new ScriptOperationMetaData(true, "<", "Is Less Than", new() { new ParameterMetaData("Left", ScriptPrimitiveType.Int), new ParameterMetaData("Right", ScriptPrimitiveType.Int) }, ScriptPrimitiveType.Bool) },
            { Operator.LessThanOrEqual, new ScriptOperationMetaData(true, "<=", "Is Less Than Or Equal", new() { new ParameterMetaData("Left", ScriptPrimitiveType.Int), new ParameterMetaData("Right", ScriptPrimitiveType.Int) }, ScriptPrimitiveType.Bool) },
            { Operator.Add, new ScriptOperationMetaData(true, "+", "Add", new() { new ParameterMetaData("Left", ScriptPrimitiveType.Int), new ParameterMetaData("Right", ScriptPrimitiveType.Int) }, ScriptPrimitiveType.Int) },
            { Operator.Subtract, new ScriptOperationMetaData(true, "-", "Subtrack", new() { new ParameterMetaData("Left", ScriptPrimitiveType.Int), new ParameterMetaData("Right", ScriptPrimitiveType.Int) }, ScriptPrimitiveType.Int) },
            { Operator.And, new ScriptOperationMetaData(true, "&&", "And", new() { new ParameterMetaData("Left", ScriptPrimitiveType.Bool), new ParameterMetaData("Right", ScriptPrimitiveType.Bool) }, ScriptPrimitiveType.Bool) },
            { Operator.Unknown47, new ScriptOperationMetaData("Unknown47", new() { new ParameterMetaData("Unknown", ScriptPrimitiveType.Int), new ParameterMetaData("Unknown", ScriptPrimitiveType.Int) }, ScriptPrimitiveType.Int) },
            
            // Overloads
            { Operator.Is, new ScriptOperationMetaData(false, "", "Is", new() { new ParameterMetaData("Actor", ScriptPrimitiveType.ActorRef) }, ScriptPrimitiveType.Bool) },

        };

        public static Dictionary<Instruction, ScriptOperationMetaData> instructionMetaData = new Dictionary<Instruction, ScriptOperationMetaData>() {
            { Instruction.None, new ScriptOperationMetaData("None", new() { new ParameterMetaData("Expression", ScriptPrimitiveType.Int) }) },
            { Instruction.End, new ScriptOperationMetaData("End", new()) },
            { Instruction.Jump, new ScriptOperationMetaData("Else", new() { new ParameterMetaData("JumpCount", ScriptPrimitiveType.Int) }) },
            { Instruction.Unknown12, new ScriptOperationMetaData("Unknown12", new() { new ParameterMetaData("Unknown", ScriptPrimitiveType.Int) }) },
            { Instruction.Unknown13, new ScriptOperationMetaData("Unknown13", new() { new ParameterMetaData("Unknown", ScriptPrimitiveType.Int) }) },
            { Instruction.ConditionalJump, new ScriptOperationMetaData("If", new() { new ParameterMetaData("Condition", ScriptPrimitiveType.Bool), new ParameterMetaData("JumpCount", ScriptPrimitiveType.Int) }) },
            { Instruction.Increment, new ScriptOperationMetaData("++", "Increment By 1", new() { new ParameterMetaData("VariableID", ScriptPrimitiveType.VarRef) }) },
            { Instruction.INCREMENT_19, new ScriptOperationMetaData("++(19)", "Increment By 1", new() { new ParameterMetaData("VariableID", ScriptPrimitiveType.VarRef) }) },
            { Instruction.Decrement, new ScriptOperationMetaData("--", "Decrement By 1", new() { new ParameterMetaData("VariableID", ScriptPrimitiveType.VarRef) }) },
            { Instruction.DECREMENT_19, new ScriptOperationMetaData("--(19)", "Decrement By 1", new() { new ParameterMetaData("VariableID", ScriptPrimitiveType.VarRef) }) },
            { Instruction.Set, new ScriptOperationMetaData(true, true, "=", "Set", new() { new ParameterMetaData("Value", ScriptPrimitiveType.Int), new ParameterMetaData("VariableID", ScriptPrimitiveType.VarRef) }, ScriptPrimitiveType.Void) },
            { Instruction.Sound, new ScriptOperationMetaData("Play Sound", new() { new ParameterMetaData("Unkown", ScriptPrimitiveType.Int), new ParameterMetaData("Unkown", ScriptPrimitiveType.Int) }) },
            { Instruction.Unknown31, new ScriptOperationMetaData("Unknown31", new() { new ParameterMetaData("Unkown", ScriptPrimitiveType.Int), new ParameterMetaData("Unkown", ScriptPrimitiveType.Int) }) },
            { Instruction.SET_19, new ScriptOperationMetaData(true, true, "=(19)", "Set", new() { new ParameterMetaData("Value", ScriptPrimitiveType.Int), new ParameterMetaData("VariableID", ScriptPrimitiveType.VarRef) }, ScriptPrimitiveType.Void) },
            { Instruction.Add, new ScriptOperationMetaData(true, true, "+=", "Add And Set", new() { new ParameterMetaData("Value", ScriptPrimitiveType.Int), new ParameterMetaData("VariableID", ScriptPrimitiveType.VarRef) }, ScriptPrimitiveType.Void) },
            { Instruction.Unkown51, new ScriptOperationMetaData("Unkown51", new() { new ParameterMetaData("Unknown", ScriptPrimitiveType.Int), new ParameterMetaData("Unknown", ScriptPrimitiveType.Int) }) },
            { Instruction.Subtract, new ScriptOperationMetaData(true, true, "-=", "Subtract And Set", new() { new ParameterMetaData("Value", ScriptPrimitiveType.Int), new ParameterMetaData("VariableID", ScriptPrimitiveType.VarRef) }, ScriptPrimitiveType.Void) },
            { Instruction.Destroy, new ScriptOperationMetaData("Destroy Actors", new() { new ParameterMetaData("Unkown", ScriptPrimitiveType.Int), new ParameterMetaData("Unkown", ScriptPrimitiveType.Int), new ParameterMetaData("Unkown", ScriptPrimitiveType.Int) }) },
            { Instruction.Unknown57, new ScriptOperationMetaData("Unkown57", new() { new ParameterMetaData("Unkown", ScriptPrimitiveType.Int), new ParameterMetaData("Unkown", ScriptPrimitiveType.Int), new ParameterMetaData("Unkown", ScriptPrimitiveType.Int) }) },
            { Instruction.Unknown59, new ScriptOperationMetaData("Unkown59", new() { new ParameterMetaData("Unkown", ScriptPrimitiveType.Int), new ParameterMetaData("Unkown", ScriptPrimitiveType.Int), new ParameterMetaData("Unkown", ScriptPrimitiveType.Int) }) },
            { Instruction.Spawn, new ScriptOperationMetaData("Spawn", new() { new ParameterMetaData("ActorID", ScriptPrimitiveType.Int), new ParameterMetaData("Unkown", ScriptPrimitiveType.Int), new ParameterMetaData("Unkown", ScriptPrimitiveType.Int) }) },
            { Instruction.SpawnAll, new ScriptOperationMetaData("Spawn Group", new() { new ParameterMetaData("GroupID", ScriptPrimitiveType.Int), new ParameterMetaData("Unkown", ScriptPrimitiveType.Int), new ParameterMetaData("Unkown", ScriptPrimitiveType.Int) }) },

        };

        public List<ScriptNode> scriptNodes = new List<ScriptNode>();
        List<Statement> statements = new List<Statement>();

        public bool failed = false;

        public string name = "";
        public int id;

        public int offset;
        public List<byte> compiledBytes = new();

        public FCopScript(int offset, List<byte> compiledBytes) {
            this.id = offset;
            this.offset = offset;
            this.compiledBytes.AddRange(compiledBytes);

            try {
                this.Disassemble(compiledBytes);
                this.DeCompile();
            } catch (Exception e) { 
                this.failed = true;
            }

        }

        class VarHasNoID : Exception { }
        class MissingArguments : Exception {
            public MissingArguments(string message) : base(message) {
            }

        }
        class TypeMisMatch : Exception {
            public TypeMisMatch(string message) : base(message) {
            }

        }

        ScriptNode UnwrapExpression(Expression expression, ScriptPrimitiveType expectedType) {

            var nestedExp = new List<ScriptNode>();

            var parameterData = operatorMetaData[expression.operationType].parameters;

            var i = 0;
            foreach (var data in parameterData) {

                nestedExp.Add(UnwrapExpression(expression.nestedExpressions[i], data.type));

                i++;
            }

            if (expression.operationType == Operator.Literal && expectedType == ScriptPrimitiveType.Int) {
                return new LiteralNode((int)expression.value);
            }
            else if (expression.operationType == Operator.Literal && expectedType == ScriptPrimitiveType.VarRef) {
                return new VariableNode((int)expression.value);
            }
            else if (expression.operationType == Operator.Literal && expectedType == ScriptPrimitiveType.ActorRef) {
                return new ActorRefNode((int)expression.value);
            }
            //else if (expectedType == ScriptPrimitiveType.Int && parameterData.Count == 1) {

            //    // This unnests those "Get Var" instuctions for readability.
            //    if (parameterData[0].type == ScriptPrimitiveType.VarRef) {
            //        return nestedExp[0];
            //    }
            //    else {
            //        var node = new ExpressionNode(expression.operationType, operatorMetaData[expression.operationType].parameterCount);
            //        node.nestedExpressiveNodes = nestedExp;
            //        return node;
            //    }

            //}
            else {
                var node = new ExpressionNode(expression.operationType, operatorMetaData[expression.operationType].parameterCount);
                node.nestedExpressiveNodes = nestedExp;
                return node;
            }

        }

        // This method also nest nodes and it's a little confusing how it works.
        // The byte code uses a byte count for jump statements.
        // In order to know which stuff is nested it has to iterate through.
        (int statementsUnwrapped, int compiledBytesUnwrapped, ScriptNode node) UnwrapStatement(int it) {

            int i = it;

            int NestNodes(int byteCount, StatementNode node) {

                var byteIt = 0;

                while (byteIt < byteCount) {

                    i++;

                    var unwrappedNextStatement = UnwrapStatement(i);

                    // Iterator already moves over one at the top to get the next statement.
                    // Iterator only needs to move over if more than one statement was unwrapped.
                    i += unwrappedNextStatement.statementsUnwrapped;
                    byteIt += unwrappedNextStatement.compiledBytesUnwrapped;

                    unwrappedNextStatement.node.parent = node;

                    node.nestedNodes.Add(unwrappedNextStatement.node);

                    if (byteIt >= byteCount) {
                        break;
                    }

                }

                return byteIt;

            }

            var statement = statements[i];

            if (statement.instruction == Instruction.ConditionalJump) {

                var node = new StatementNode(statement.instruction, 1);

                node.nestedExpressiveNodes.Add(UnwrapExpression(statement.parametes[0], ScriptPrimitiveType.Bool));

                var byteCount = (int)statement.parametes[1].value - 1;

                var byteIt = NestNodes(byteCount, node);

                return (i - it, byteIt + statement.GetTotalStatementByteCount(), node);

            }
            else if (statement.instruction == Instruction.Jump) {

                var node = new StatementNode(statement.instruction, 0);

                var byteCount = (int)statement.parametes[0].value - 1;

                var byteIt = NestNodes(byteCount, node);

                return (i - it, byteIt + statement.GetTotalStatementByteCount(), node);

            }
            else {

                var node = new StatementNode(statement.instruction, instructionMetaData[statement.instruction].parameterCount);

                var parameterData = instructionMetaData[statement.instruction].parameters;
                var pi = 0;
                foreach (var data in parameterData) {

                    node.nestedExpressiveNodes.Add(UnwrapExpression(statement.parametes[pi], data.type));

                    pi++;
                }

                return (i - it, statement.GetTotalStatementByteCount(), node);
            }

        }

        public void DeCompile() {

            scriptNodes.Clear();

            var i = 0;
            while (i < statements.Count) {

                var (statementsUnwrapped, compiledBytesUnwrapped, node) = UnwrapStatement(i);

                i += statementsUnwrapped;

                scriptNodes.Add(node);

                i++;

            }

        }

        public void Disassemble(List<byte> code) {

            statements.Clear();

            List<Expression> floatingExpressions = new();

            var i = 0;
            while (i < code.Count) {

                var b = code[i];

                if (Enum.IsDefined(typeof(Operator), (Int32)b)) {



                    // As if things weren't compilcated enough, instructions/operators can be overloaded.

                    bool CheckParameterTypes(ScriptOperationMetaData localMetaData) {

                        var typeMismatch = false;

                        var expressions = floatingExpressions.GetRange(floatingExpressions.Count - localMetaData.parameterCount, localMetaData.parameterCount);

                        var localI = 0;
                        foreach (var par in localMetaData.parameters) {

                            if (par.type != expressions[localI].returnType) {

                                if (!((par.type == ScriptPrimitiveType.VarRef || par.type == ScriptPrimitiveType.ActorRef) && 
                                    expressions[localI].returnType == ScriptPrimitiveType.Int)) {
                                    typeMismatch = true;
                                    break;
                                }

                            }

                            localI++;
                        }

                        return typeMismatch;

                    }

                    void CreateFloatingExpression(Operator operatorCase, ScriptPrimitiveType returnType, int parameterCount) {

                        var expressions = floatingExpressions.GetRange(floatingExpressions.Count - parameterCount, parameterCount);

                        floatingExpressions.RemoveRange(floatingExpressions.Count - parameterCount, parameterCount);

                        floatingExpressions.Add(new Expression(expressions, operatorCase, 1, returnType));

                    }

                    void ThrowTypeMismatchError(List<Expression> expressions, ScriptOperationMetaData metaData, Operator opCase) {

                        var message = "";

                        var localI = 0;

                        foreach (var par in metaData.parameters) {

                            message += "Expected Type: " + par.type.ToString() + " Type: " + expressions[localI].returnType.ToString() + ", ";

                            localI++;
                        }

                        throw new TypeMisMatch("Type mismatch from provided arguments for " + opCase.ToString() + ". " + message);


                    }

                    var opCase = (Operator)b;

                    var metaData = operatorMetaData[opCase];

                    var checkForOverloads = false;

                    checkForOverloads = floatingExpressions.Count < metaData.parameterCount;

                    if (!checkForOverloads) { // Enough arguments are provided

                        var typeMismatch = CheckParameterTypes(metaData);

                        if (!typeMismatch) {
                            CreateFloatingExpression(opCase, metaData.returnType, metaData.parameterCount);
                        }
                        else {

                            if (!metaData.overloaded) { // Type mismatch and no overloads
                                ThrowTypeMismatchError(floatingExpressions.GetRange(floatingExpressions.Count - metaData.parameterCount, metaData.parameterCount), metaData, opCase);
                            }

                            checkForOverloads = true;
                        }

                    }
                    else if (!metaData.overloaded) {

                        throw new MissingArguments("Not enough arguments for Operator " + opCase.ToString());

                    }

                    #region Check Overloads
                    if (checkForOverloads && metaData.overloaded) {

                        Operator? foundValidOverload = null;

                        foreach (var overloadOpCase in metaData.overloadedOperators) {

                            var overloadData = operatorMetaData[overloadOpCase];

                            if (floatingExpressions.Count < overloadData.parameterCount) {
                                continue;
                            }

                            var typeMismatch = CheckParameterTypes(overloadData);

                            if (!typeMismatch) {
                                foundValidOverload = overloadOpCase;
                                break;
                            }

                        }

                        if (foundValidOverload != null) {
                            var overloadOpCase = (Operator)foundValidOverload;

                            var overloadData = operatorMetaData[overloadOpCase];

                            CreateFloatingExpression(overloadOpCase, overloadData.returnType, overloadData.parameterCount);

                        }
                        else {

                            throw new MissingArguments("No overloads with the provided arguments");

                        }

                    }
                    #endregion


                }
                else if (Enum.IsDefined(typeof(Instruction), (Int32)b)) {

                    var instuctionCase = (Instruction)b;

                    if (instuctionCase == Instruction.ConditionalJump) {

                        var state = new Statement(instuctionCase, 1);

                        state.parametes.Add(floatingExpressions.Last());

                        floatingExpressions.RemoveAt(floatingExpressions.Count - 1);

                        state.parametes.Add(new Expression((int)code[i + 1], Operator.Literal, 1, ScriptPrimitiveType.Int));

                        statements.Add(state);

                        i += 2;
                        continue;

                    }
                    else if (instuctionCase == Instruction.Jump) {

                        var state = new Statement(instuctionCase, 1);

                        state.parametes.Add(new Expression((int)code[i + 1], Operator.Literal, 1, ScriptPrimitiveType.Int));

                        statements.Add(state);

                        i += 2;
                        continue;

                    }
                    else {

                        var state = new Statement(instuctionCase, 1);

                        var parCount = instructionMetaData[instuctionCase].parameterCount;

                        if (parCount != 0) {

                            var expressions = floatingExpressions.GetRange(floatingExpressions.Count - parCount, parCount);

                            state.parametes = expressions;

                            floatingExpressions.RemoveRange(floatingExpressions.Count - parCount, parCount);

                        }

                        statements.Add(state);

                    }

                }
                else {

                    if (b > 127) {
                        floatingExpressions.Add(new Expression(b - 128, Operator.Literal, 1, ScriptPrimitiveType.Int));
                    }
                    else if (b == 2) {
                        floatingExpressions.Add(new Expression(128 + (code[i + 1]), Operator.Literal, 2, ScriptPrimitiveType.Int));
                        i += 2;
                        continue;
                    }
                    else if (b == 3) {

                        var value = 256;
                        value += code[i + 1] * 128;
                        if (code[i + 1] == 1) {
                            value += code[i + 2] - 128;
                        }
                        else {
                            value += code[i + 2];
                        }

                        floatingExpressions.Add(new Expression(value, Operator.Literal, 3, ScriptPrimitiveType.Int));
                        i += 3;
                        continue;
                    }
                    else {
                        throw new Exception("Unknown byte: " + b.ToString());
                    }

                }

                i++;
            }

            if (floatingExpressions.Count > 0) {
                Console.WriteLine("floatingExpressions still has count");

                var state = new Statement(Instruction.None, 0);

                state.parametes.Add(floatingExpressions[^1]);
                floatingExpressions.RemoveAt(floatingExpressions.Count - 1);

                statements.Add(state);

            }

        }

        public List<byte> Compile(int newOffset) {

            offset = newOffset;

            return compiledBytes;

        }

    }


}