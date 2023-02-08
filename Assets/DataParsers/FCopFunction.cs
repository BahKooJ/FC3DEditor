using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCopParser {

    public class FCopFunction {

        public static List<byte> tFUNFourCC = new List<byte>() { 78, 85, 70, 116 };
        public static List<byte> tEXTFourCC = new List<byte>() { 84, 88, 69, 116 };


        public List<CFuntFUNData> tFUNData = new();
        public List<CFuntEXTData> tEXTData = new();

        public IFFDataFile rawFile;

        public FCopFunction(IFFDataFile rawFile) {

            this.rawFile = rawFile;

            var chunks = FindChunks(rawFile.data.ToArray());

            var tFUNDataCount = (chunks[0].chunkSize / 4) / 5;

            // Offset starts at 12 to move past the header data
            var offset = 12;

            var data = rawFile.data.ToArray();

            foreach (var i in Enumerable.Range(0, tFUNDataCount)) {

                tFUNData.Add(
                    new CFuntFUNData(
                            Utils.BytesToInt(data, offset),
                            Utils.BytesToInt(data, offset + 4),
                            Utils.BytesToInt(data, offset + 8),
                            Utils.BytesToInt(data, offset + 12),
                            Utils.BytesToInt(data, offset + 16)

                        )
                    );

                offset += 20;

            }

            // Add 12 to move past the tEXT header
            offset += 12;

            var nextIndex = 1;
            foreach (var item in tFUNData) {

                var tEXTItem = new CFuntEXTData();

                tEXTItem.line1 = rawFile.data.GetRange(offset + item.startingOffset, item.endingOffset - item.startingOffset);

                if (nextIndex < tFUNData.Count) {

                    var nextItem = tFUNData[nextIndex];

                    tEXTItem.line2 = rawFile.data.GetRange(offset + item.endingOffset, nextItem.startingOffset - item.endingOffset);

                } else {

                    tEXTItem.line2 = rawFile.data.GetRange(offset + item.endingOffset, rawFile.data.Count - (offset + item.endingOffset));

                }

                tEXTData.Add(tEXTItem);

                nextIndex++;

            }

            //foreach (var item in tFUNData) {
            //    Console.WriteLine(
            //        item.number1.ToString() + " " +
            //        item.number2.ToString() + " " +
            //        item.number3.ToString() + " " +
            //        item.startingOffset.ToString() + " " +
            //        item.endingOffset.ToString()
            //    );
            //}

            //foreach (var item in tEXTData) {

            //    foreach (var b in item.line1) {
            //        Console.Write(b.ToString() + " ");
            //    }

            //    Console.WriteLine();

            //    foreach (var b in item.line2) {
            //        Console.Write(b.ToString() + " ");
            //    }

            //    Console.WriteLine();

            //}

        }

        public void Compile() {

            var total = new List<byte>();

            var tFUNSize = (tFUNData.Count * 5 * 4) + 12;

            total.AddRange(tFUNFourCC);
            total.AddRange(BitConverter.GetBytes(tFUNSize));
            total.AddRange(BitConverter.GetBytes(1));

            foreach (var item in tFUNData) {

                total.AddRange(BitConverter.GetBytes(item.number1));
                total.AddRange(BitConverter.GetBytes(item.number2));
                total.AddRange(BitConverter.GetBytes(item.number3));
                total.AddRange(BitConverter.GetBytes(item.startingOffset));
                total.AddRange(BitConverter.GetBytes(item.endingOffset));

            }

            var tEXTTotal = new List<byte>();

            foreach (var item in tEXTData) {

                tEXTTotal.AddRange(item.line1);
                tEXTTotal.AddRange(item.line2);

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

    public class CFuntFUNData {

        public int number1, number2, number3, startingOffset, endingOffset;

        public CFuntFUNData(int number1, int number2, int number3, int startingOffset, int endingOffset) {
            this.number1 = number1;
            this.number2 = number2;
            this.number3 = number3;
            this.startingOffset = startingOffset;
            this.endingOffset = endingOffset;
        }
    }

    public class CFuntEXTData {

        public List<byte> line1 = new();
        public List<byte> line2 = new();

        public CFuntEXTData() {

        }

    }

}