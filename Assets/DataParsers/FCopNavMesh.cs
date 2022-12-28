
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FCopParser {

    public class FCopNavMesh {

        const int nodeCountOffset = 14;
        const int nodesOffset = 16;

        IFFDataFile rawFile;

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
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 22, 32)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 12, 22)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 2, 12)),
                    Utils.BytesToShort(rawFile.data.ToArray(), offset + 6),
                    Utils.BytesToShort(rawFile.data.ToArray(), offset + 8),
                    Utils.BytesToShort(rawFile.data.ToArray(), offset + 10) == 1
                    ));

                offset += 12;
                index++;
            }

        }

    }

    public class NavNode {

        public int index;
        public int nextNodeA;
        public int nextNodeB;
        public int nextNodeC;
        public int x;
        public int y;
        public bool isStartingPoint;

        public NavNode(int index, int nextNodeA, int nextNodeB, int nextNodeC, int x, int y, bool isStartingPoint) {
            this.index = index;
            this.nextNodeA = nextNodeA;
            this.nextNodeB = nextNodeB;
            this.nextNodeC = nextNodeC;
            this.x = x;
            this.y = y;
            this.isStartingPoint = isStartingPoint;
        }

    }

}