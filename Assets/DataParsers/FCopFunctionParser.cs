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

                functions.Add(
                    new FCopFunction(
                            Utils.BytesToInt(data, offset),
                            Utils.BytesToInt(data, offset + 4),
                            Utils.BytesToInt(data, offset + 12),
                            Utils.BytesToInt(data, offset + 16)

                        )
                    );

                offset += 20;

            }

            // Add 12 to move past the tEXT header
            offset += 12;

            var codeBytes = rawFile.data.GetRange(offset, rawFile.data.Count - offset);

            foreach (var item in functions) {

                item.runCondition = new FCopScript(item.line1Offset, codeBytes);
                item.code = new FCopScript(item.line2Offset, codeBytes);

            }

        }

        public IFFDataFile Compile() {

            // TODO: Actually compile
            return rawFile;

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
                tEXTTotal.AddRange(item.runCondition.compiledBytes);

                total.AddRange(BitConverter.GetBytes(tEXTTotal.Count));
                tEXTTotal.AddRange(item.code.compiledBytes);


            }


            total.AddRange(tEXTFourCC);
            total.AddRange(BitConverter.GetBytes(tEXTTotal.Count + 12));
            total.AddRange(BitConverter.GetBytes(1));

            total.AddRange(tEXTTotal);

            rawFile.data = total;
            rawFile.modified = true;

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
        public FCopScript runCondition;
        public FCopScript code;

        public FCopFunction(int repeatCount, int repeatTimer, int line1Offset, int line2Offset) {
            this.repeatCount = repeatCount;
            this.repeatTimer = repeatTimer;
            this.line1Offset = line1Offset;
            this.line2Offset = line2Offset;
        }
    }

}