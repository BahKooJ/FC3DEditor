
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace FCopParser {

    // TODO: Turns out the starting node is not just a bool but has more going on with it.
    public class FCopNavMesh {

        const int nodeCountOffset = 14;
        const int nodesOffset = 16;

        static List<byte> fourCC = new List<byte>() { 78, 116, 68, 79 };

        public IFFDataFile rawFile;

        public List<NavNode> nodes = new();

        public FCopNavMesh(IFFDataFile rawFile) {
            this.rawFile = rawFile;

            int nodeCount = Utils.BytesToShort(rawFile.data.ToArray(),nodeCountOffset);

            int index = 0;
            int offset = nodesOffset;
            foreach (var i in Enumerable.Range(0,nodeCount)) {

                var byteFiled = rawFile.data.GetRange(offset, 12).ToArray();

                var bitField = new BitArray(byteFiled);

                nodes.Add(new NavNode(
                    index,
                    Utils.BytesToShort(rawFile.data.ToArray(), offset + 4),
                    Utils.BytesToShort(rawFile.data.ToArray(), offset + 10),
                    Utils.BytesToShort(rawFile.data.ToArray(), offset + 6),
                    Utils.BytesToShort(rawFile.data.ToArray(), offset + 8),
                    Utils.BytesToShort(rawFile.data.ToArray(), offset + 10) == 1,
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 22, 32)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 12, 22)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 2, 12))
                    ));

                offset += 12;
                index++;
            }

        }

        public void Compile() {

            var total = new List<byte>();

            foreach (var node in nodes) {

                var bitfield = new BitField(32, new List<BitNumber> {
                    new BitNumber(2,0), new BitNumber(10,0), new BitNumber(10,0), new BitNumber(10,0)
                });

                var nextNodeIndex = 3;
                foreach (var index in node.nextNode) {

                    bitfield.bitNumbers[nextNodeIndex].number = index;

                    nextNodeIndex--;
                }

                total.AddRange(Utils.BitArrayToByteArray(bitfield.Compile()));

                // There's a 16 bit number after the paths, the use is unknown but it seems to mostly be -64, temp -64 is placed.
                total.AddRange(BitConverter.GetBytes((short)node.unknown));

                total.AddRange(BitConverter.GetBytes((short)node.x));
                total.AddRange(BitConverter.GetBytes((short)node.y));

                if (node.unknown2 != 0 && node.unknown2 != 1) {
                    total.AddRange(BitConverter.GetBytes((short)node.unknown2));
                } else {
                    total.AddRange(BitConverter.GetBytes((short)(node.isStartingPoint ? 1 : 0)));
                }

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

            rawFile.data = header;
            rawFile.modified = true;

        }

    }

    public class NavNode {

        public static int invalid = 1023;

        public int index;
        public int[] nextNode = new int[3];
        public int x;
        public int y;
        public bool isStartingPoint;

        public int unknown;
        public int unknown2;

        public NavNode(int index, int unknown, int unknown2, int x, int y, bool isStartingPoint, int nextNodeA = 1023, int nextNodeB = 1023, int nextNodeC = 1023) {
            this.index = index;
            this.unknown = unknown;
            this.unknown2 = unknown2;
            this.nextNode[0] = nextNodeA;
            this.nextNode[1] = nextNodeB;
            this.nextNode[2] = nextNodeC;
            this.x = x;
            this.y = y;
            this.isStartingPoint = isStartingPoint;
        }

    }

}