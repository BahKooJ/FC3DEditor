
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FCopParser {

    public class FCopNavMesh : FCopAsset {

        const int nodeCountOffset = 14;
        const int nodesOffset = 16;

        static List<byte> fourCC = new List<byte>() { 78, 116, 68, 79 };

        public List<NavNode> nodes = new();

        public FCopNavMesh(IFFDataFile rawFile) : base(rawFile) {

            name = "NavMesh " + DataID;

            if (rawFile.data.Count == 0) {
                return;
            }

            int nodeCount = Utils.BytesToShort(rawFile.data.ToArray(), nodeCountOffset);

            int index = 0;
            int offset = nodesOffset;
            foreach (var i in Enumerable.Range(0, nodeCount)) {

                var nodeData = rawFile.data.GetRange(offset, 12).ToArray();

                var bitField = new BitArray(nodeData);

                var groundCast = Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 80, 82));
                var readHeightOffset = Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 82, 84));
                var heightOffset = Utils.BitsToSignedInt(Utils.CopyBitsOfRange(bitField, 84, 96), 12);

                nodes.Add(new NavNode(
                    index,
                    (NavNodeState)Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 0, 2)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 22, 32)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 12, 22)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 2, 12)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 38, 48)),
                    BitConverter.ToInt16(nodeData, 6),
                    BitConverter.ToInt16(nodeData, 8),
                    (NavNodeGroundCast)groundCast,
                    readHeightOffset == 1,
                    heightOffset
                    ));

                offset += 12;
                index++;
            }

        }

        public IFFDataFile Compile() {

            var total = new List<byte>();

            foreach (var node in nodes) {

                var nextNodesBitfield = new BitField(48, new List<BitNumber> {
                    new BitNumber(2, (int)node.state), 
                    new BitNumber(10, node.nextNodeIndexes[2]), 
                    new BitNumber(10, node.nextNodeIndexes[1]), 
                    new BitNumber(10, node.nextNodeIndexes[0]),
                    new BitNumber(6, 0),
                    new BitNumber(10, node.nextNodeIndexes[3]),
                });

                total.AddRange(Utils.BitArrayToByteArray(nextNodesBitfield.Compile()));

                total.AddRange(BitConverter.GetBytes((short)node.x));
                total.AddRange(BitConverter.GetBytes((short)node.y));

                var heightOffsetingBitfield = new BitField(16, new List<BitNumber> {
                    new BitNumber(2, (int)node.groundCast),
                    new BitNumber(2, node.readHeightOffset ? 1 : 0),
                    new BitNumber(12, node.heightOffset)
                });

                total.AddRange(Utils.BitArrayToByteArray(heightOffsetingBitfield.Compile()));

            }

            var header = new List<byte>();

            header.AddRange(fourCC);
            // 16 is the header size
            header.AddRange(BitConverter.GetBytes(total.Count + 16));
            header.AddRange(BitConverter.GetBytes((short)0));

            // Another unknown number but always seems to be one
            header.AddRange(BitConverter.GetBytes(1));
            header.AddRange(BitConverter.GetBytes((short)nodes.Count));

            header.AddRange(total);

            //if (!header.SequenceEqual(rawFile.data)) {
            //    //File.WriteAllBytes("CompiledCnet" + DataID, header.ToArray());
            //    //File.WriteAllBytes("Cnet" + DataID, rawFile.data.ToArray());
            //    throw new Exception("different data");
            //}

            rawFile.data = header;

            return rawFile;

        }

    }

    public class NavNode {

        public static int invalid = 1023;

        public int index;
        public NavNodeState state;
        public int[] nextNodeIndexes = new int[4];
        public int x;
        public int y;
        public NavNodeGroundCast groundCast;
        public bool readHeightOffset;
        public int heightOffset; // 12-Bit

        public NavNode(int index, NavNodeState state, int nextNodeA, int nextNodeB, int nextNodeC, int nextNodeD, int x, int y, NavNodeGroundCast groundCast, bool readHeightOffset, int heightOffset) {
            this.index = index;
            this.state = state;
            this.nextNodeIndexes[0] = nextNodeA;
            this.nextNodeIndexes[1] = nextNodeB;
            this.nextNodeIndexes[2] = nextNodeC;
            this.nextNodeIndexes[3] = nextNodeD;
            this.x = x;
            this.y = y;
            this.groundCast = groundCast;
            this.readHeightOffset = readHeightOffset;
            this.heightOffset = heightOffset;
        }

        public NavNode(int index, int x, int y) {
            this.index = index;
            this.state = NavNodeState.Enabled;
            this.nextNodeIndexes[0] = invalid;
            this.nextNodeIndexes[1] = invalid;
            this.nextNodeIndexes[2] = invalid;
            this.nextNodeIndexes[3] = invalid;
            this.x = x;
            this.y = y;
            this.groundCast = NavNodeGroundCast.Highest;
            this.readHeightOffset = false;
            this.heightOffset = 0;

        }

        public NavNode Clone() {

            return new NavNode(index, state, nextNodeIndexes[0], nextNodeIndexes[1], nextNodeIndexes[2], nextNodeIndexes[3], x, y, groundCast, readHeightOffset, heightOffset);

        }

        public void ReciveData(NavNode node) {

            this.index = node.index;
            this.state = node.state;
            this.nextNodeIndexes[0] = node.nextNodeIndexes[0];
            this.nextNodeIndexes[1] = node.nextNodeIndexes[1];
            this.nextNodeIndexes[2] = node.nextNodeIndexes[2];
            this.nextNodeIndexes[3] = node.nextNodeIndexes[3];
            this.x = node.x;
            this.y = node.y;
            this.groundCast = node.groundCast;
            this.readHeightOffset = node.readHeightOffset;
            this.heightOffset = node.heightOffset;

        }

        public void SafeSetHeight(int newValue) {

            var maxValue = (int)((Math.Pow(2, 12) - 1) / 2);
            var minValue = -(int)(Math.Pow(2, 12) / 2);

            heightOffset = newValue;

            if (heightOffset > maxValue) {
                heightOffset = maxValue;
            }
            if (heightOffset < minValue) {
                heightOffset = minValue;
            }

        }

    }

    public enum NavNodeGroundCast {
        Highest = 0,
        Lowest = 1,
        LowestDisableHeight = 2,
        Middle = 3
    }

    public enum NavNodeState {
        Enabled = 0,
        Unknown = 1,
        Disabled = 2
    }

}