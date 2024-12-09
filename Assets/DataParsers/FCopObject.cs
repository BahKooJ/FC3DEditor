

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public List<Vertex> normals = new();
        public List<Surface> surfaces = new();
        public Dictionary<int, Surface> surfaceByCompiledOffset = new();
        public List<BoundingBox> boundingBoxes = new();

        public List<Triangle> triangles = new();


        public FCopObject(IFFDataFile rawFile) : base(rawFile) {

            name = "Object " + DataID.ToString();
            
            FindStartChunkOffset();
            ParseVertices();
            ParseNormals();
            ParsePrimitives();
            ParseSurfaces();
            ParseBoundingBoxes();

            CreateTriangles();

        }

        public FCopObject(WavefrontParser wavefront, IFFDataFile rawFile) : base(rawFile) {

            var compiledData = new List<byte>();

            var convertedVerts = new List<Vertex>();
            var convertedNormals = new List<Vertex>();
            var convertedPrimitives = new List<Primitive>();

            foreach (var v in wavefront.vertices) {
                convertedVerts.Add(new Vertex((int)(v.x * 512), (int)(v.y * 512), (int)(v.z * 512), 0));
            }

            foreach (var v in wavefront.normals) {
                convertedNormals.Add(new Vertex((int)(v.x * 512), (int)(v.y * 512), (int)(v.z * 512), 0));
            }

            foreach (var face in wavefront.faces) {

                // The unknown1 bitfield does seem to do something, it caused the triangles to go crazy.
                var bitfield = face.Count == 3 ? new List<byte>() { 195, 3 } : new List<byte>() { 196, 4 };
                var vertIndexs = new List<byte>();
                var normalIndexs = new List<byte>();

                foreach (var element in face) {

                    vertIndexs.Add((byte)(element.vertIndex - 1));
                    normalIndexs.Add((byte)(element.normalIndex - 1));

                }

                if (vertIndexs.Count == 3) {
                    vertIndexs.Add(0);
                }

                if (normalIndexs.Count == 3) {
                    normalIndexs.Add(0);
                }

                vertIndexs.AddRange(normalIndexs);

                convertedPrimitives.Add(new Primitive(bitfield, 0, vertIndexs.ToArray()));

            }

            compiledData.AddRange(CreateFourDGI());
            compiledData.AddRange(CreateThreeDTL(new List<Surface>() { new Surface(SurfaceType.Texture, 0, 0, 0, new UVMap(3, new UV[] { new UV(0,0), new UV(0, 0), new UV(0, 0), new UV(0, 0) })) }));
            compiledData.AddRange(CreateThreeDQL(convertedPrimitives));
            compiledData.AddRange(CreateThreeDRF(1, FourCC.fourDVLbytes, new() { 1 }));
            compiledData.AddRange(CreateThreeDRF(2, FourCC.fourDNLbytes, new() { 1 }));
            compiledData.AddRange(CreateThreeDRF(3, FourCC.threeDRLbytes, new() { 1 }));
            compiledData.AddRange(CreateFourDVL(convertedVerts));
            compiledData.AddRange(CreateFourDNL(convertedNormals));
            compiledData.AddRange(CreateThreeDRL(new()));
            compiledData.AddRange(CreateThreeDBB(new() { CalculateBoundingBox(convertedVerts) }));

            rawFile.data = compiledData;

            name = "Object " + DataID.ToString();

            FindStartChunkOffset();
            ParseVertices();
            ParseNormals();
            ParsePrimitives();
            ParseSurfaces();
            ParseBoundingBoxes();

            CreateTriangles();

        }

        public IFFDataFile Compile() {

            return rawFile;

        }

        public void TestCompile() {

            var compiledData = new List<byte>();

            compiledData.AddRange(CreateFourDGI());
            compiledData.AddRange(CreateThreeDTL(surfaces));
            compiledData.AddRange(CreateThreeDQL(primitives));
            compiledData.AddRange(CreateThreeDRF(1, FourCC.fourDVLbytes, new() { 1 }));
            compiledData.AddRange(CreateThreeDRF(2, FourCC.fourDNLbytes, new() { 1 }));
            compiledData.AddRange(CreateThreeDRF(3, FourCC.threeDRLbytes, new() { 1 }));
            compiledData.AddRange(CreateFourDVL(vertices));
            compiledData.AddRange(CreateFourDNL(normals));
            compiledData.AddRange(CreateThreeDRL(new()));
            compiledData.AddRange(CreateThreeDBB(new() { CalculateBoundingBox(vertices) }));

            rawFile.data = compiledData;

        }

        public void Import(byte[] newData) {

            rawFile.data = newData.ToList();

            offsets = new();
            primitives = new();
            vertices = new();
            surfaces = new();
            surfaceByCompiledOffset = new();
            triangles = new();

            FindStartChunkOffset();
            ParseVertices();
            ParsePrimitives();
            ParseSurfaces();

            CreateTriangles();

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

        void ParseNormals() {

            var header = offsets.First(header => {
                return header.fourCCDeclaration == FourCC.fourDNL;
            });

            var bytes = rawFile.data.GetRange(header.index, header.chunkSize);

            var vertexCount = Utils.BytesToInt(bytes.ToArray(), 12);

            var offset = 16;

            foreach (var i in Enumerable.Range(0, vertexCount)) {

                normals.Add(new Vertex(
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

        void ParseBoundingBoxes() {

            var header = offsets.First(header => {
                return header.fourCCDeclaration == FourCC.threeDBB;
            });

            var bytes = rawFile.data.GetRange(header.index, header.chunkSize);

            var boxesPerFrame = BitConverter.ToInt32(bytes.ToArray(), 8);
            var boxesCount = BitConverter.ToInt32(bytes.ToArray(), 12);

            var offset = 16;

            foreach (var i in Enumerable.Range(0, boxesCount)) {

                boundingBoxes.Add(new BoundingBox(
                        BitConverter.ToInt16(bytes.ToArray(), offset),
                        BitConverter.ToInt16(bytes.ToArray(), offset + 2),
                        BitConverter.ToInt16(bytes.ToArray(), offset + 4),
                        BitConverter.ToInt16(bytes.ToArray(), offset + 6),
                        BitConverter.ToInt16(bytes.ToArray(), offset + 8),
                        BitConverter.ToInt16(bytes.ToArray(), offset + 10),
                        BitConverter.ToInt16(bytes.ToArray(), offset + 12),
                        BitConverter.ToInt16(bytes.ToArray(), offset + 14)
                    ));

                offset += 16;

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

        BoundingBox CalculateBoundingBox(List<Vertex> vertices) {

            var maxX = vertices.Max(v => v.x);
            var maxY = vertices.Max(v => v.y);
            var maxZ = vertices.Max(v => v.z);

            var minX = vertices.Min(v => v.x);
            var minY = vertices.Min(v => v.y);
            var minZ = vertices.Min(v => v.z);

            var centerX = (minX + maxX) / 2;
            var centerY = (minY + maxY) / 2;
            var centerZ = (minZ + maxZ) / 2;

            var lengthX = (maxX - minX) / 2;
            var lengthY = (maxY - minY) / 2;
            var lengthZ = (maxZ - minZ) / 2;

            var pythXYZ = MathF.Pow(lengthX, 2) + MathF.Pow(lengthY, 2) + MathF.Pow(lengthZ, 2);
            pythXYZ = MathF.Sqrt(pythXYZ);
            var pythXZ = MathF.Pow(lengthX, 2) + MathF.Pow(lengthZ, 2);
            pythXZ = MathF.Sqrt(pythXZ);

            return new BoundingBox(centerX, centerY, centerZ, lengthX, lengthY, lengthZ, (int)MathF.Ceiling(pythXYZ), (int)MathF.Ceiling(pythXZ));

        }

        List<byte> CreateFourDGI() {

            var compiledData = new List<byte>();

            compiledData.AddRange(FourCC.fourDGIbytes);
            compiledData.AddRange(BitConverter.GetBytes(60)); // size
            compiledData.AddRange(BitConverter.GetBytes(1)); // id
            compiledData.AddRange(BitConverter.GetBytes((short)1)); // frame count
            compiledData.Add(1); // const
            compiledData.Add(8); // bitfield?
            compiledData.AddRange(BitConverter.GetBytes(0)); // const
            compiledData.AddRange(BitConverter.GetBytes(0)); // const
            compiledData.AddRange(BitConverter.GetBytes(0)); // const
            compiledData.AddRange(BitConverter.GetBytes(1)); // const
            compiledData.AddRange(BitConverter.GetBytes(2)); // const
            compiledData.AddRange(BitConverter.GetBytes(1)); // const
            compiledData.AddRange(BitConverter.GetBytes(1)); // const
            compiledData.AddRange(BitConverter.GetBytes(3)); // const
            compiledData.Add(255); // pos0
            compiledData.Add(255); // pos1
            compiledData.Add(255); // pos2
            compiledData.Add(255); // pos3
            compiledData.AddRange(BitConverter.GetBytes(4)); // const
            compiledData.AddRange(BitConverter.GetBytes(5)); // const

            return compiledData;

        }

        List<byte> CreateThreeDTL(List<Surface> surfaces) {

            var compiledData = new List<byte>();

            foreach (var surface in surfaces) {

                compiledData.Add((byte)surface.type);
                compiledData.Add((byte)surface.red);
                compiledData.Add((byte)surface.green);
                compiledData.Add((byte)surface.blue);

                if (surface.uvMap != null) {

                    foreach (var uv in surface.uvMap.Value.uvs) {
                        compiledData.Add((byte)uv.x);
                        compiledData.Add((byte)uv.y);

                    }

                    compiledData.AddRange(BitConverter.GetBytes(surface.uvMap.Value.texturePaletteIndex));

                }

            }

            var compiledWithHeader = new List<byte>();

            compiledWithHeader.AddRange(FourCC.threeDTLbytes);
            compiledWithHeader.AddRange(BitConverter.GetBytes(compiledData.Count + 12));
            compiledWithHeader.AddRange(BitConverter.GetBytes(1));
            compiledWithHeader.AddRange(compiledData);

            return compiledWithHeader;

        }

        List<byte> CreateFourDVL(List<Vertex> vertices) {

            var compiledData = new List<byte>();

            foreach (var vertex in vertices) {

                compiledData.AddRange(BitConverter.GetBytes((short)vertex.x));
                compiledData.AddRange(BitConverter.GetBytes((short)vertex.y));
                compiledData.AddRange(BitConverter.GetBytes((short)vertex.z));
                compiledData.AddRange(BitConverter.GetBytes((short)vertex.w));

            }

            var compiledWithHeader = new List<byte>();

            compiledWithHeader.AddRange(FourCC.fourDVLbytes);
            compiledWithHeader.AddRange(BitConverter.GetBytes(compiledData.Count + 16));
            compiledWithHeader.AddRange(BitConverter.GetBytes(1));
            compiledWithHeader.AddRange(BitConverter.GetBytes(vertices.Count));
            compiledWithHeader.AddRange(compiledData);

            return compiledWithHeader;

        }

        List<byte> CreateFourDNL(List<Vertex> vertices) {

            var compiledData = new List<byte>();

            foreach (var vertex in vertices) {

                compiledData.AddRange(BitConverter.GetBytes((short)vertex.x));
                compiledData.AddRange(BitConverter.GetBytes((short)vertex.y));
                compiledData.AddRange(BitConverter.GetBytes((short)vertex.z));
                compiledData.AddRange(BitConverter.GetBytes((short)vertex.w));

            }

            var compiledWithHeader = new List<byte>();

            compiledWithHeader.AddRange(FourCC.fourDNLbytes);
            compiledWithHeader.AddRange(BitConverter.GetBytes(compiledData.Count + 16));
            compiledWithHeader.AddRange(BitConverter.GetBytes(1));
            compiledWithHeader.AddRange(BitConverter.GetBytes(vertices.Count));
            compiledWithHeader.AddRange(compiledData);

            return compiledWithHeader;

        }

        List<byte> CreateThreeDRL(List<int> lengths) {

            var compiledData = new List<byte>();

            foreach (var length in lengths) {

                compiledData.AddRange(BitConverter.GetBytes((short)length));

            }

            var compiledWithHeader = new List<byte>();

            compiledWithHeader.AddRange(FourCC.threeDRLbytes);
            compiledWithHeader.AddRange(BitConverter.GetBytes(compiledData.Count + 16));
            compiledWithHeader.AddRange(BitConverter.GetBytes(1));
            compiledWithHeader.AddRange(BitConverter.GetBytes(lengths.Count));
            compiledWithHeader.AddRange(compiledData);

            return compiledWithHeader;

        }

        List<byte> CreateThreeDRF(int id, List<byte> fourCC, List<int> idRefs) {

            var compiledData = new List<byte>();

            compiledData.AddRange(fourCC);
            compiledData.AddRange(BitConverter.GetBytes(idRefs.Count));

            foreach (var idRef in idRefs) {
                compiledData.AddRange(BitConverter.GetBytes(idRef));

            }

            var compiledWithHeader = new List<byte>();

            compiledWithHeader.AddRange(FourCC.threeDRFbytes);
            compiledWithHeader.AddRange(BitConverter.GetBytes(compiledData.Count + 12));
            compiledWithHeader.AddRange(BitConverter.GetBytes(id));
            compiledWithHeader.AddRange(compiledData);

            return compiledWithHeader;

        }

        List<byte> CreateThreeDQL(List<Primitive> primitives) {

            var compiledData = new List<byte>();

            foreach (var primitive in primitives) {

                compiledData.AddRange(primitive.metaDatabitfeild);
                compiledData.AddRange(BitConverter.GetBytes((short)primitive.surfaceIndex));

                foreach (var data in primitive.associatedData) {
                    compiledData.Add(data);
                }

            }

            var compiledWithHeader = new List<byte>();

            compiledWithHeader.AddRange(FourCC.threeDQLbytes);
            compiledWithHeader.AddRange(BitConverter.GetBytes(compiledData.Count + 16));
            compiledWithHeader.AddRange(BitConverter.GetBytes(1));
            compiledWithHeader.AddRange(BitConverter.GetBytes(primitives.Count));
            compiledWithHeader.AddRange(compiledData);

            return compiledWithHeader;

        }

        List<byte> CreateThreeDBB(List<BoundingBox> boundingBoxes, int boxesPerFrame = 1) {

            var compiledData = new List<byte>();

            foreach (var boxes in boundingBoxes) {

                compiledData.AddRange(BitConverter.GetBytes((short)boxes.x));
                compiledData.AddRange(BitConverter.GetBytes((short)boxes.y));
                compiledData.AddRange(BitConverter.GetBytes((short)boxes.z));
                compiledData.AddRange(BitConverter.GetBytes((short)boxes.lengthX));
                compiledData.AddRange(BitConverter.GetBytes((short)boxes.lengthY));
                compiledData.AddRange(BitConverter.GetBytes((short)boxes.lengthZ));
                compiledData.AddRange(BitConverter.GetBytes((short)boxes.pythXYZ));
                compiledData.AddRange(BitConverter.GetBytes((short)boxes.pythXZ));

            }

            var compiledWithHeader = new List<byte>();

            compiledWithHeader.AddRange(FourCC.threeDBBbytes);
            compiledWithHeader.AddRange(BitConverter.GetBytes(compiledData.Count + 16));
            compiledWithHeader.AddRange(BitConverter.GetBytes(boxesPerFrame));
            compiledWithHeader.AddRange(BitConverter.GetBytes(boundingBoxes.Count));
            compiledWithHeader.AddRange(compiledData);

            return compiledWithHeader;

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

            public List<byte> metaDatabitfeild;

            public Primitive(List<byte> metaDatabitfeild, int surfaceIndex, byte[] associatedData) {

                this.metaDatabitfeild = metaDatabitfeild;

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

        public struct Billboard {

            public Vertex position;
            public int length;

        }

        public struct BoundingBox {

            public int x;
            public int y;
            public int z;
            public int lengthX;
            public int lengthY;
            public int lengthZ;
            public int pythXYZ;
            public int pythXZ;

            public BoundingBox(int x, int y, int z, int lengthX, int height, int lengthZ, int pythXYZ, int pythXZ) {
                this.x = x;
                this.y = y;
                this.z = z;
                this.lengthX = lengthX;
                this.lengthY = height;
                this.lengthZ = lengthZ;
                this.pythXYZ = pythXYZ;
                this.pythXZ = pythXZ;
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