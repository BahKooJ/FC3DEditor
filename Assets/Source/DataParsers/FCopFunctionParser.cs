using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCopParser {

    public class FCopFunctionParser {

        public static List<byte> tFUNFourCC = new List<byte>() { 78, 85, 70, 116 };
        public static List<byte> tEXTFourCC = new List<byte>() { 84, 88, 69, 116 };

        public List<FCopFunction> functions = new();

        public IFFDataFile rawFile;

        public FCopFunctionParser(IFFDataFile rawFile) {

            this.rawFile = rawFile;

            var chunks = FindChunks(rawFile.data.ToArray());

            var tFUNDataCount = (chunks[0].chunkSize / 4) / 5;

            // Offset starts at 12 to move past the header data
            var offset = 12;

            var data = rawFile.data.ToArray();

            foreach (var i in Enumerable.Range(0, tFUNDataCount)) {

                functions.Add(new FCopFunction(
                            Utils.BytesToInt(data, offset),
                            Utils.BytesToInt(data, offset + 4),
                            Utils.BytesToInt(data, offset + 12),
                            Utils.BytesToInt(data, offset + 16)
                        ));

                offset += 20;

            }

            // Add 12 to move past the tEXT header
            offset += 12;

            var codeBytes = rawFile.data.GetRange(offset, rawFile.data.Count - offset);

            var i2 = 0;
            foreach (var item in functions) {

                var runCondition = new FCopScript(item.line1Offset, codeBytes);
                var code = new FCopScript(item.line2Offset, codeBytes);

                if (runCondition.code.Count > 1) {
                    throw new Exception("Run Condition has more than one line of code");
                }

                var runConditionNestingNode = new ScriptNestingNode(ByteCode.RUN, "Run", ScriptDataType.Void, new() { new ScriptParameter("", ScriptDataType.Bool) }, new() { runCondition.code[0] });

                runConditionNestingNode.nestedNodes = code.code;

                foreach (var line in code.code) {
                    line.parent = runConditionNestingNode;
                }

                code.code = new() { runConditionNestingNode };
                code.name = "Update Script " + i2;
                item.code = code;

                i2++;
            }

        }

        public FCopFunctionParser(List<byte> ncfcBytes, int emptyOffset) {

            var arrayBytes = ncfcBytes.ToArray();

            var i = 0;
            while (i < ncfcBytes.Count) {

                var repeatCount = BitConverter.ToInt32(arrayBytes, i);
                i += 4;
                var repeatTimer = BitConverter.ToInt32(arrayBytes, i);
                i += 4;
                var nameSize = BitConverter.ToInt32(arrayBytes, i);
                i += 4;
                var name = Encoding.ASCII.GetString(ncfcBytes.GetRange(i, nameSize).ToArray());
                i += nameSize;
                var commentSize = BitConverter.ToInt32(arrayBytes, i);
                i += 4;
                var comment = Encoding.ASCII.GetString(ncfcBytes.GetRange(i, commentSize).ToArray());
                i += commentSize;

                var runCodeSize = BitConverter.ToInt32(arrayBytes, i);
                i += 4;
                var runCode = ncfcBytes.GetRange(i, runCodeSize);
                i += runCodeSize;

                var codeSize = BitConverter.ToInt32(arrayBytes, i);
                i += 4;
                var code = ncfcBytes.GetRange(i, codeSize);
                i += codeSize;

                var runScript = new FCopScript(0, runCode);
                var script = new FCopScript(0, code);

                if (runScript.code.Count > 1) {
                    throw new Exception("Run Condition has more than one line of code");
                }

                var runConditionNestingNode = new ScriptNestingNode(ByteCode.RUN, "Run", ScriptDataType.Void, new() { new ScriptParameter("", ScriptDataType.Bool) }, new() { runScript.code[0] });

                runConditionNestingNode.nestedNodes = script.code;

                foreach (var line in script.code) {
                    line.parent = runConditionNestingNode;
                }

                script.code = new() { runConditionNestingNode };
                script.name = name;
                script.comment = comment;

                functions.Add(new FCopFunction(repeatCount, repeatTimer, script));

            }

            this.rawFile = new IFFDataFile(2, new(), "Cfun", 1, emptyOffset);


        }

        public void AddFunc() {

            var code = new FCopScript(0);
            // Changed name for better user expirence.
            code.name = "Update Script";
            code.code.Add(new ScriptNestingNode(ByteCode.RUN, "Run", ScriptDataType.Void, new() { new ScriptParameter("", ScriptDataType.Bool) }, new() { new LiteralNode(0) }));

            var newFunc = new FCopFunction(-1, 55, code);
            functions.Insert(0, newFunc);

        }

        public IFFDataFile Compile() {

            var total = new List<byte>();

            var tFUNSize = (functions.Count * 5 * 4) + 12;

            total.AddRange(tFUNFourCC);
            total.AddRange(BitConverter.GetBytes(tFUNSize));
            total.AddRange(BitConverter.GetBytes(1));

            var tEXTTotal = new List<byte>();


            foreach (var item in functions) {

                total.AddRange(BitConverter.GetBytes(item.repeatCount));
                total.AddRange(BitConverter.GetBytes(item.repeatTimer));
                total.AddRange(BitConverter.GetBytes(0));

                total.AddRange(BitConverter.GetBytes(tEXTTotal.Count));

                var runCondition = new FCopScript(0, new List<ScriptNode> { item.code.code[0].parameters[0] });
                tEXTTotal.AddRange(runCondition.Compile(0, false));

                total.AddRange(BitConverter.GetBytes(tEXTTotal.Count));

                var unnestedCode = (item.code.code[0] as ScriptNestingNode).nestedNodes;
                tEXTTotal.AddRange(new FCopScript(0, unnestedCode).Compile(0, false));

            }


            total.AddRange(tEXTFourCC);
            total.AddRange(BitConverter.GetBytes(tEXTTotal.Count + 12));
            total.AddRange(BitConverter.GetBytes(1));

            total.AddRange(tEXTTotal);

            if (!rawFile.data.SequenceEqual(total)) {
                //throw new Exception("skissue");
            }

            rawFile.data = total;

            return rawFile;

        }

        public List<byte> CompileNCFC() {

            var funcData = new List<byte>();

            foreach (var func in functions) {

                funcData.AddRange(BitConverter.GetBytes(func.repeatCount));
                funcData.AddRange(BitConverter.GetBytes(func.repeatTimer));
                funcData.AddRange(BitConverter.GetBytes(func.code.name.Length));
                funcData.AddRange(Encoding.ASCII.GetBytes(func.code.name));
                funcData.AddRange(BitConverter.GetBytes(func.code.comment.Length));
                funcData.AddRange(Encoding.ASCII.GetBytes(func.code.comment));

                var runCondition = new FCopScript(0, new List<ScriptNode> { func.code.code[0].parameters[0] }).Compile(0, true);
                funcData.AddRange(BitConverter.GetBytes(runCondition.Count));
                funcData.AddRange(runCondition);

                var unnestedCode = (func.code.code[0] as ScriptNestingNode).nestedNodes;
                var compiledCode = new FCopScript(0, unnestedCode).Compile(0, false);
                funcData.AddRange(BitConverter.GetBytes(compiledCode.Count));
                funcData.AddRange(compiledCode);

            }

            var total = new List<byte>();

            total.AddRange(Encoding.ASCII.GetBytes("FUNCTION"));
            total.AddRange(BitConverter.GetBytes(funcData.Count + 16));
            total.AddRange(BitConverter.GetBytes(functions.Count));
            total.AddRange(funcData);

            return total;

        }

        List<ChunkHeader> FindChunks(byte[] bytes) {

            var offsets = new List<ChunkHeader>();

            int offset = 0;

            while (offset < bytes.Length) {

                var fourCC = BytesToStringReversed(bytes, offset, 4);
                var size = Utils.BytesToInt(bytes, offset + 4);

                offsets.Add(new ChunkHeader(offset, fourCC, size));

                offset += size;

            }

            return offsets;

        }

        string BytesToStringReversed(byte[] bytes, int offset, int length) {
            var s = Encoding.Default.GetString(bytes, offset, length);
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

    }

    public class FCopFunction {

        public int repeatCount, repeatTimer, line1Offset, line2Offset;
        public FCopScript code;

        public FCopFunction(int repeatCount, int repeatTimer, int line1Offset, int line2Offset) {
            this.repeatCount = repeatCount;
            this.repeatTimer = repeatTimer;
            this.line1Offset = line1Offset;
            this.line2Offset = line2Offset;
        }

        public FCopFunction(int repeatCount, int repeatTimer, FCopScript code) {
            this.repeatCount = repeatCount;
            this.repeatTimer = repeatTimer;
            this.line1Offset = 0;
            this.line2Offset = 0;
            this.code = code;
        }

    }

}