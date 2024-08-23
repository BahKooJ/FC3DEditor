

using System;
using System.Collections.Generic;
using System.Linq;

namespace FCopParser {

    public class FCopScriptingProject {

        public FCopRPNS rpns;
        public FCopFunctionParser functionParser;

        public FCopScriptingProject(FCopRPNS rpns, FCopFunctionParser functionParser) {
            this.rpns = rpns;
            this.functionParser = functionParser;
        }

        public void Compile() {

        }
    }

    public class FCopScript {

        public class ScriptNode {

            // The difference between the two is expressive is used for parameters or other values
            // nested nodes is for like code blocks
            public List<ScriptNode> nestedExpressiveNodes = new();
            public List<ScriptNode> nestedNodes = new();

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

            public Expression(List<Expression> nestedExpressions, Operator operationType, int byteCount) {
                this.nestedExpressions = nestedExpressions;
                this.operationType = operationType;
                this.byteCount = byteCount;
            }

            public Expression(object value, Operator operationType, int byteCount) {
                this.value = value;
                this.operationType = operationType;
                this.byteCount = byteCount;
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

        public enum Operator {

            Literal = 256,
            GET_16 = 16,
            GET_18 = 18,
            GET_19 = 19,
            Equal = 33,
            GreaterThan = 35,
            GreaterThanOrEqual = 36,
            LessThan = 37,
            Subtract = 40,
            And = 44

        }

        public enum Instruction {

            None = 256,
            End = 0,
            Jump = 8,
            Unknown12 = 12,
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
            Subtract = 52,
            Destroy = 56,
            Unknown57 = 57,
            Spawn = 60

        }

        public Dictionary<Operator, int> operatorParameterCount = new Dictionary<Operator, int>() {

            { Operator.Literal, 0 },
            { Operator.GET_16, 1 },
            { Operator.GET_18, 1 },
            { Operator.GET_19, 1 },
            { Operator.Equal, 2 },
            { Operator.GreaterThan, 2 },
            { Operator.GreaterThanOrEqual, 2 },
            { Operator.LessThan, 2 },
            { Operator.Subtract, 2 },
            { Operator.And, 2 },

        };

        public Dictionary<Instruction, int> instructionParameterCount = new Dictionary<Instruction, int>() {

            { Instruction.None, 0 },
            { Instruction.End, 0 },
            { Instruction.Jump, 1 },
            { Instruction.Unknown12, 1 },
            { Instruction.ConditionalJump, 2 },
            { Instruction.Increment, 1 },
            { Instruction.INCREMENT_19, 1 },
            { Instruction.Decrement, 1 },
            { Instruction.DECREMENT_19, 1 },
            { Instruction.Set, 2 },
            { Instruction.Sound, 2 },
            { Instruction.Unknown31, 2 },
            { Instruction.SET_19, 2 },
            { Instruction.Add, 2 },
            { Instruction.Subtract, 2 },
            { Instruction.Destroy, 3 },
            { Instruction.Unknown57, 3 },
            { Instruction.Spawn, 3 },



        };

        public List<ScriptNode> scriptNodes = new List<ScriptNode>();
        List<Statement> statements = new List<Statement>();

        public string name = "";
        public int id;

        public int offset;
        public List<byte> compiledBytes = new();

        public FCopScript(int offset, List<byte> compiledBytes) {
            this.id = offset;
            this.offset = offset;
            this.compiledBytes.AddRange(compiledBytes);

            this.Disassemble(compiledBytes);
            // TODO: Line by line debug DeCompile
            this.DeCompile();
        }

        class VarHasNoID : Exception { }

        ScriptNode UnwrapExpression(Expression expression) {

            var nestedExp = new List<ScriptNode>();

            foreach (var nestE in expression.nestedExpressions) {
                nestedExp.Add(UnwrapExpression(nestE));
            }

            if (expression.operationType == Operator.Literal) {
                return new LiteralNode((int)expression.value);
            }
            else if (expression.operationType == Operator.GET_16 ||
                expression.operationType == Operator.GET_18 ||
                expression.operationType == Operator.GET_19) {
                try {
                    return new VariableNode(((LiteralNode)nestedExp[0]).value);

                }
                catch {
                    throw new VarHasNoID();
                }
            }
            else {
                var node = new ExpressionNode(expression.operationType, operatorParameterCount[expression.operationType]);
                node.nestedExpressiveNodes = nestedExp;
                return node;
            }

        }

        (int, int, ScriptNode) UnwrapStatement(int it) {

            int i = it;

            var statement = statements[i];

            if (statement.instruction == Instruction.ConditionalJump) {

                var node = new StatementNode(statement.instruction, 1);

                node.nestedExpressiveNodes.Add(UnwrapExpression(statement.parametes[0]));

                var byteIt = 0;
                var byteCount = (int)statement.parametes[1].value - 1;


                while (byteCount != byteIt) {

                    i++;

                    var nextStatement = statements[i];

                    var unwrappedNextStatement = UnwrapStatement(i);

                    i += unwrappedNextStatement.Item1;
                    byteIt += unwrappedNextStatement.Item2;

                    node.nestedNodes.Add(unwrappedNextStatement.Item3);

                    if (byteCount == byteIt) {
                        break;
                    }

                    byteIt += nextStatement.GetTotalStatementByteCount();


                }

                return (i, byteCount, node);

            }
            else {

                var node = new StatementNode(statement.instruction, instructionParameterCount[statement.instruction]);

                foreach (var par in statement.parametes) {
                    node.nestedExpressiveNodes.Add(UnwrapExpression(par));

                }

                return (i, statement.GetTotalStatementByteCount(), node);
            }

        }

        public void DeCompile() {

            var i = 0;
            while (i < statements.Count) {

                var unwrappedNextStatement = UnwrapStatement(i);

                i += unwrappedNextStatement.Item1;

                scriptNodes.Add(unwrappedNextStatement.Item3);

            }

        }

        public void Disassemble(List<byte> code) {

            List<Expression> floatingExpressions = new();

            var i = 0;
            while (i < code.Count) {

                var b = code[i];

                if (Enum.IsDefined(typeof(Operator), (Int32)b)) {

                    var opCase = (Operator)b;

                    var parCount = operatorParameterCount[opCase];

                    var expressions = floatingExpressions.GetRange(floatingExpressions.Count - parCount, parCount);

                    floatingExpressions.RemoveRange(floatingExpressions.Count - parCount, parCount);

                    floatingExpressions.Add(new Expression(expressions, opCase, 1));

                }
                else if (Enum.IsDefined(typeof(Instruction), (Int32)b)) {

                    var instuctionCase = (Instruction)b;

                    if (instuctionCase == Instruction.ConditionalJump) {

                        var state = new Statement(instuctionCase, 1);

                        state.parametes.Add(floatingExpressions.Last());

                        floatingExpressions.RemoveAt(floatingExpressions.Count - 1);

                        state.parametes.Add(new Expression((int)code[i + 1], Operator.Literal, 1));

                        statements.Add(state);

                        i += 2;
                        continue;

                    }
                    else if (instuctionCase == Instruction.Jump) {

                        var state = new Statement(instuctionCase, 1);

                        state.parametes.Add(new Expression((int)code[i + 1], Operator.Literal, 1));

                        statements.Add(state);

                        i += 2;
                        continue;

                    }
                    else {

                        var state = new Statement(instuctionCase, 1);

                        var parCount = instructionParameterCount[instuctionCase];

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
                        floatingExpressions.Add(new Expression(b - 128, Operator.Literal, 1));
                    }
                    else if (b == 2) {
                        floatingExpressions.Add(new Expression(128 + (code[i + 1]), Operator.Literal, 2));
                        i += 2;
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