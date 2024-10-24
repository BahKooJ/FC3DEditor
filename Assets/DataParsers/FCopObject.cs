

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static FCopParser.FCopObject;

namespace FCopParser {

    public class FCopObject : FCopAsset {

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

        public List<Primitive> primitives = new();
        public List<Vertex> vertices = new();
        public List<Surface> surfaces = new();
        public Dictionary<int, Surface> surfaceByCompiledOffset = new();

        public List<Triangle> triangles = new();

        public FCopObject(IFFDataFile rawFile) : base(rawFile) {

            name = "Object " + DataID.ToString();
            
            FindStartChunkOffset();
            ParseVertices();
            ParsePrimitives();
            ParseSurfaces();

            CreateTriangles();

        }

        public IFFDataFile Compile() {

            return rawFile;

        }

        void FindStartChunkOffset() {

            offsets.Clear();

            int offset = 0;

            while (offset < rawFile.data.Count) {

                var fourCC = BytesToStringReversed(offset, 4);
                var size = BitConverter.ToInt32(rawFile.data.ToArray(), offset + 4);

                offsets.Add(new ChunkHeader(offset, fourCC, size));

                offset += size;

            }

        }

        void ParsePrimitives() {

            var header = offsets.First(header => {
                return header.fourCCDeclaration == FourCC.threeDQL;
            });

            var bytes = rawFile.data.GetRange(header.index, header.chunkSize);

            var polyCount = BitConverter.ToInt32(bytes.ToArray(), 12);

            var offset = 16;

            foreach (var i in Enumerable.Range(0, polyCount)) {

                var metaDataBitfield = bytes.GetRange(offset, 2);
                offset += 2;
                var surfaceIndex = BitConverter.ToInt16(bytes.ToArray(), offset);
                offset += 2;
                var iData = bytes.GetRange(offset, 8);
                offset += 8;

                primitives.Add(new Primitive(metaDataBitfield, surfaceIndex, iData.ToArray()));

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

                vertices.Add(new Vertex(
                    BitConverter.ToInt16(bytes.ToArray(), offset),
                    BitConverter.ToInt16(bytes.ToArray(), offset + 2),
                    BitConverter.ToInt16(bytes.ToArray(), offset + 4),
                    BitConverter.ToInt16(bytes.ToArray(), offset + 6)
                    ));

                offset += 8;

            }

        }

        void ParseSurfaces() {

            int headerSize = 12;
            int colorSize = 4;
            int colorTextureSize = 16;

            var header = offsets.First(header => {
                return header.fourCCDeclaration == FourCC.threeDTL;
            });

            var bytes = rawFile.data.GetRange(header.index, header.chunkSize);

            var offset = 12;

            while (offset < bytes.Count) {

                var surfaceType = (SurfaceType)bytes[offset];
                offset++;
                var red = bytes[offset];
                offset++;
                var green = bytes[offset];
                offset++;
                var blue = bytes[offset];
                offset++;

                if (surfaceType != SurfaceType.Color) {

                    var uvs = new List<UV>();

                    uvs.Add(new UV(bytes[offset], bytes[offset + 1]));
                    offset += 2;
                    uvs.Add(new UV(bytes[offset], bytes[offset + 1]));
                    offset += 2;
                    uvs.Add(new UV(bytes[offset], bytes[offset + 1]));
                    offset += 2;
                    uvs.Add(new UV(bytes[offset], bytes[offset + 1]));
                    offset += 2;

                    var texturePalette = BitConverter.ToInt32(bytes.ToArray(), offset);
                    offset += 4;

                    var surface = new Surface(surfaceType, red, green, blue, new UVMap(texturePalette, uvs.ToArray()));

                    surfaces.Add(surface);
                    surfaceByCompiledOffset[offset - headerSize - colorTextureSize] = surface;
                }
                else {

                    var surface = new Surface(surfaceType, red, green, blue, null);

                    surfaces.Add(surface);
                    surfaceByCompiledOffset[offset - headerSize - colorSize] = surface;

                }

            }


        }

        void CreateTriangles() {

            var total = new List<Triangle>();

            foreach (var primitive in primitives) {

                if (primitive.type != PrimitiveType.Tri && primitive.type != PrimitiveType.Quad) {
                    continue;
                }

                var surface = surfaceByCompiledOffset[primitive.surfaceIndex];

                var verts = new List<Vertex>();
                List<UV> uvs = new();
                List<float[]> colors = new();
                var texturePalette = 0;

                void CreateTriangle(int[] vertOrder) {

                    verts.Add(vertices[primitive.associatedData[vertOrder[0]]]);
                    verts.Add(vertices[primitive.associatedData[vertOrder[1]]]);
                    verts.Add(vertices[primitive.associatedData[vertOrder[2]]]);

                    if (surface.uvMap != null) {

                        uvs.Add(surface.uvMap.Value.uvs[vertOrder[0]]);
                        uvs.Add(surface.uvMap.Value.uvs[vertOrder[1]]);
                        uvs.Add(surface.uvMap.Value.uvs[vertOrder[2]]);

                        texturePalette = surface.uvMap.Value.texturePaletteIndex;

                    }
                    else {

                        uvs.Add(new UV(0, 0));
                        uvs.Add(new UV(0, 0));
                        uvs.Add(new UV(0, 0));

                    }

                    colors.Add(new float[] { surface.red / 255f, surface.green / 255f, surface.blue / 255f });
                    colors.Add(new float[] { surface.red / 255f, surface.green / 255f, surface.blue / 255f });
                    colors.Add(new float[] { surface.red / 255f, surface.green / 255f, surface.blue / 255f });

                    triangles.Add(new Triangle(verts.ToArray(), uvs.ToArray(), colors.ToArray(), texturePalette));

                }

                if (primitive.type == PrimitiveType.Tri) {

                    CreateTriangle(new int[] { 0, 1, 2 });

                }
                else if (primitive.type == PrimitiveType.Quad) {

                    CreateTriangle(new int[] { 0, 1, 2 });

                    verts.Clear();
                    uvs.Clear();
                    colors.Clear();
                    texturePalette = 0;

                    CreateTriangle(new int[] { 2, 3, 0 });

                }

            }

        }

        string BytesToStringReversed(int offset, int length) {

            string Reverse(string s) {
                char[] charArray = s.ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
            }

            return Reverse(Encoding.Default.GetString(rawFile.data.ToArray(), offset, length));
        }

        public struct Vertex {

            public int x;
            public int y;
            public int z;
            public int w;

            public Vertex(int x, int y, int z, int w) {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }

        }

        public struct Primitive {

            public PrimitiveType type;
            public int surfaceIndex;
            public byte[] associatedData;

            public Primitive(List<byte> metaDatabitfeild, int surfaceIndex, byte[] associatedData) {

                var bitField = new BitArray(metaDatabitfeild.ToArray());

                var unknown1 = Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 0, 3));
                var material = Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 3, 7));
                var textureEnabled = Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 7, 8));
                var primitiveType = Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 8, 11));
                var unknown2 = Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 11, 15));
                var isReflective = Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 15, 16));

                type = (PrimitiveType)primitiveType;

                this.surfaceIndex = surfaceIndex;
                this.associatedData = associatedData;

            }

        }

        public struct Surface {

            public SurfaceType type;
            public int red;
            public int green;
            public int blue;
            public UVMap? uvMap;

            public Surface(SurfaceType type, int red, int green, int blue, UVMap? uvMap) {
                this.type = type;
                this.red = red;
                this.green = green;
                this.blue = blue;
                this.uvMap = uvMap;
            }

        }

        public struct UVMap {

            public int texturePaletteIndex;

            public UV[] uvs;

            public UVMap(int texturePaletteIndex, UV[] uvs) {
                this.texturePaletteIndex = texturePaletteIndex;
                this.uvs = uvs;
            }

        }

        public struct UV {

            public int x;
            public int y;

            public UV(int x, int y) {
                this.x = x;
                this.y = y;
            }

        }

        public struct Triangle {

            public Vertex[] vertices;
            public UV[] uvs;
            public float[][] colors;
            public int texturePaletteIndex;

            public Triangle(Vertex[] vertices, UV[] uvs, float[][] colors, int texturePaletteIndex) {
                this.vertices = vertices;
                this.uvs = uvs;
                this.colors = colors;
                this.texturePaletteIndex = texturePaletteIndex;
            }

        }

        public enum SurfaceType {
            Color = 1,
            Texture = 2,
            ColorTexture = 3
        }

        public enum PrimitiveType {
            Star = 0,
            Tri = 3,
            Quad = 4,
            Billboard = 5,
            Line = 7
        }

    }

}