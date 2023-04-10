

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCopParser {

    public class FCopObject {

        static class FourCC {

            public const string fourDGI = "4DGI";
            public const string threeDTL = "3DTL";
            public const string threeDQL = "3DQL";
            public const string threeDRF = "3DRF";
            public const string threeDRL = "3DRL";
            public const string threeDHY = "3DHY";
            public const string threeDHS = "3DHS";
            public const string threeDMI = "3DMI";
            public const string threeDTA = "3DTA";
            public const string threeDAL = "3DAL";
            public const string fourDVL = "4DVL";
            public const string fourDNL = "4DNL";
            public const string AnmD = "AnmD";
            public const string threeDBB = "3DBB";

            public readonly static List<byte> fourDGIbytes = new() { 73, 71, 68, 52 };
            public readonly static List<byte> threeDTLbytes = new() { 76, 84, 68, 51 };
            public readonly static List<byte> threeDQLbytes = new() { 76, 81, 68, 51 };
            public readonly static List<byte> threeDRFbytes = new() { 70, 82, 68, 51 };
            public readonly static List<byte> threeDRLbytes = new() { 76, 82, 68, 51 };
            public readonly static List<byte> threeDHYbytes = new() { 89, 72, 68, 51 };
            public readonly static List<byte> threeDHSbytes = new() { 82, 72, 68, 51 };
            public readonly static List<byte> threeDMIbytes = new() { 73, 77, 68, 51 };
            public readonly static List<byte> threeDTAbytes = new() { 65, 84, 68, 51 };
            public readonly static List<byte> threeDALbytes = new() { 76, 65, 68, 51 };
            public readonly static List<byte> fourDVLbytes = new() { 76, 86, 68, 52 };
            public readonly static List<byte> fourDNLbytes = new() { 76, 78, 68, 52 };
            public readonly static List<byte> AmnDbytes = new() { 68, 109, 110, 65 };
            public readonly static List<byte> threeDBBbytes = new() { 66, 66, 68, 51 };

        }

        public List<ChunkHeader> offsets = new();

        public IFFDataFile rawFile;

        public List<FCopPolygon> polygons = new();
        public List<FCopVertex> vertices = new();
        public List<FCopUVMap> uvMaps = new();

        public FCopObject(IFFDataFile rawFile) {
            this.rawFile = rawFile;
            FindStartChunkOffset();
            ParseVertices();
            ParsePolygons();
            ParseUVMaps();
            var bonk = 3;
        }

        void FindStartChunkOffset() {

            offsets.Clear();

            int offset = 0;

            while (offset < rawFile.data.Count) {

                var fourCC = BytesToStringReversed(offset, 4);
                var size = BytesToInt(offset + 4);

                offsets.Add(new ChunkHeader(offset, fourCC, size));

                offset += size;

            }

        }

        void ParsePolygons() {

            var header = offsets.First(header => {
                return header.fourCCDeclaration == FourCC.threeDQL;
            });

            var bytes = rawFile.data.GetRange(header.index, header.chunkSize);

            var polyCount = Utils.BytesToInt(bytes.ToArray(), 12);

            var offset = 16;

            foreach (var i in Enumerable.Range(0, polyCount)) {

                polygons.Add(new FCopPolygon(
                    bytes[offset],
                    bytes[offset + 1],
                    Utils.BytesToShort(bytes.ToArray(), offset + 2),
                    new List<int>() { bytes[offset + 4], bytes[offset + 5], bytes[offset + 6], bytes[offset + 7] },
                    new List<int>() { bytes[offset + 8], bytes[offset + 9], bytes[offset + 10], bytes[offset + 11] }
                    ));;

                offset += 12;

            }

        }

        void ParseVertices() {

            var header = offsets.First(header => {
                return header.fourCCDeclaration == FourCC.fourDVL;
            });

            var bytes = rawFile.data.GetRange(header.index, header.chunkSize);

            var vertexCount = Utils.BytesToInt(bytes.ToArray(), 12);

            var offset = 16;

            foreach (var i in Enumerable.Range(0, vertexCount)) {

                vertices.Add(new FCopVertex(
                    Utils.BytesToShort(bytes.ToArray(), offset),
                    Utils.BytesToShort(bytes.ToArray(), offset + 2),
                    Utils.BytesToShort(bytes.ToArray(), offset + 4),
                    Utils.BytesToShort(bytes.ToArray(), offset + 6)
                    ));

                offset += 8;

            }

        }

        void ParseUVMaps() {

            var header = offsets.First(header => {
                return header.fourCCDeclaration == FourCC.threeDTL;
            });

            var bytes = rawFile.data.GetRange(header.index, header.chunkSize);

            var uvMapCount = (header.chunkSize - 12) / 16;

            var offset = 12;

            foreach (var i in Enumerable.Range(0, uvMapCount)) {

                uvMaps.Add(new FCopUVMap(
                    Utils.BytesToInt(bytes.ToArray(), offset + 12),
                    new List<int> { bytes[offset + 4], bytes[offset + 6], bytes[offset + 8], bytes[offset + 10] },
                    new List<int> { bytes[offset + 5], bytes[offset + 7], bytes[offset + 9], bytes[offset + 11] }
                    ));

                offset += 16;

            }


        }

        string Reverse(string s) {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        int BytesToInt(int offset) {
            return BitConverter.ToInt32(rawFile.data.ToArray(), offset);
        }

        string BytesToStringReversed(int offset, int length) {
            return Reverse(Encoding.Default.GetString(rawFile.data.ToArray(), offset, length));
        }

        public class FCopVertex {

            public int x;
            public int y;
            public int z;
            public int w;

            public FCopVertex(int x, int y, int z, int w) {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }

        }

        public class FCopPolygon {

            public int num1; // 8 bit
            public int num2; // 8 bit

            public int textureIndex; // 16 bit

            public List<int> vertices;
            public List<int> normals;

            public FCopPolygon(int num1, int num2, int textureIndex, List<int> vertices, List<int> normals) {
                this.num1 = num1;
                this.num2 = num2;
                this.textureIndex = textureIndex;
                this.vertices = vertices;
                this.normals = normals;
            }

        }

        public class FCopUVMap {

            public int textureResourceIndex;

            public List<int> x;
            public List<int> y;

            public FCopUVMap(int textureResourceIndex, List<int> x, List<int> y) {
                this.textureResourceIndex = textureResourceIndex;
                this.x = x;
                this.y = y;
            }

        }

    }

}