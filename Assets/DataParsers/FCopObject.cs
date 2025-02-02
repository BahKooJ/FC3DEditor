

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

        public class ObjectElementGroup {

            public List<Vertex> vertices = new();
            public List<Vertex> normals = new();
            public List<int> lengths = new();
            // For whatever reason there can be more lengths even if the count is differet
            public List<int> overflowLengths = new();

        }

        public Dictionary<int, ObjectElementGroup> objectElementGroups = new();
        public ObjectElementGroup firstElementGroup {
            get { return objectElementGroups.First().Value; }
        }

        public List<ObjectElementReference> elementReferences = new();
        public List<Primitive> primitives = new();

        public List<Surface> surfaces = new();
        public Dictionary<int, Surface> surfaceByCompiledOffset = new();
        public List<BoundingBox> boundingBoxes = new();
        public int boundingBoxesPerFrame = 1;
        public List<byte> threeDTAData;
        public List<byte> threeDALData;
        public List<byte> threeDHYData;
        public List<byte> threeDMIData;
        public List<byte> threeDHSData;
        public List<byte> AnmDData;

        public int frameCount;
        public byte metaDataBitfield;
        public List<byte> positions = new();


        public List<Triangle> triangles = new();
        public List<Billboard> billboards = new();
        public List<Line> lines = new();
        public List<Star> stars = new();
        public FCopObject(IFFDataFile rawFile) : base(rawFile) {

            name = "Object " + DataID.ToString();

            Init();

        }

        public class VertexLimitExceededException : Exception { }
        public class InvalidPrimitiveException : Exception { }
        public FCopObject(WavefrontParser wavefront, IFFDataFile rawFile) : base(rawFile) {

            if (wavefront.vertices.Count > 255) {
                throw new VertexLimitExceededException();
            }

            var compiledData = new List<byte>();

            var convertedVerts = new List<Vertex>();
            var convertedNormals = new List<Vertex>();
            var convertedPrimitives = new List<Primitive>();
            var convertedSurfaces = new List<Surface>();
            var surfacesHashes = new List<string>();

            foreach (var v in wavefront.vertices) {
                convertedVerts.Add(new Vertex((int)(v.x * 512), (int)(v.y * 512), (int)(v.z * 512), 0));
            }

            foreach (var v in wavefront.normals) {
                convertedNormals.Add(new Vertex((int)(v.x * 4096), (int)(v.y * 4096), (int)(v.z * 4096), 0));
            }

            foreach (var face in wavefront.faces) {

                if (face.Count > 4) {
                    throw new InvalidPrimitiveException();
                }

                // The unknown1 bitfield does seem to do something, it caused the triangles to go crazy.
                var bitfield = face.Count == 3 ? new List<byte>() { 195, 3 } : new List<byte>() { 196, 4 };
                var vertIndexs = new List<byte>();
                var normalIndexs = new List<byte>();

                var grabedUV = new List<UV>();

                foreach (var element in face) {

                    vertIndexs.Add((byte)(element.vertIndex - 1));
                    normalIndexs.Add((byte)(element.normalIndex - 1));

                    var waveFrontUV = wavefront.uvs[element.textureIndex - 1];

                    grabedUV.Add(new UV((int)(waveFrontUV.u * 256), (int)(waveFrontUV.v * 256)));

                }

                if (vertIndexs.Count == 3) {
                    vertIndexs.Add(0);
                }

                if (normalIndexs.Count == 3) {
                    normalIndexs.Add(0);
                }

                if (grabedUV.Count == 3) {
                    grabedUV.Add(new UV(0, 0));
                }

                vertIndexs.AddRange(normalIndexs);

                var createSurface = new Surface(SurfaceType.Texture, 128, 128, 128, new UVMap(3, grabedUV.ToArray()));
                var hash = createSurface.CreateHash();

                var index = surfacesHashes.IndexOf(hash);

                if (index == -1) {
                    convertedSurfaces.Add(createSurface);
                    surfacesHashes.Add(hash);

                    index = surfacesHashes.Count - 1;

                }

                convertedPrimitives.Add(new Primitive(bitfield, index * 16, vertIndexs.ToArray()));

            }

            compiledData.AddRange(CreateFourDGI());
            compiledData.AddRange(CreateThreeDTL(convertedSurfaces));
            compiledData.AddRange(CreateThreeDQL(convertedPrimitives));
            compiledData.AddRange(CreateThreeDRF(new ObjectElementReference(1, FourCC.fourDVL, new() { 1 })));
            compiledData.AddRange(CreateThreeDRF(new ObjectElementReference(2, FourCC.fourDNL, new() { 1 })));
            compiledData.AddRange(CreateThreeDRF(new ObjectElementReference(3, FourCC.threeDRL, new() { 1 })));
            compiledData.AddRange(CreateFourDVL(convertedVerts, 1));
            compiledData.AddRange(CreateFourDNL(convertedNormals, 1));
            compiledData.AddRange(CreateThreeDRL(new(), 1, new()));
            compiledData.AddRange(CreateThreeDBB(new() { CalculateBoundingBox(convertedVerts) }, 1));

            rawFile.data = compiledData;

            name = "Object " + DataID.ToString();

            Init();

        }

        void Init() {

            FindStartChunkOffset();

            var header4DGI = offsets.FirstOrDefault(header => {
                return header.fourCCDeclaration == FourCC.fourDGI;
            });

            var fourDGI = rawFile.data.GetRange(header4DGI.index, header4DGI.chunkSize);

            frameCount = BitConverter.ToInt16(fourDGI.ToArray(), 12);
            metaDataBitfield = fourDGI[15];

            positions.Add(fourDGI[48]);
            positions.Add(fourDGI[49]);
            positions.Add(fourDGI[50]);
            positions.Add(fourDGI[51]);

            ParseElementReferences();
            ParsePrimitives();
            ParseSurfaces();
            ParseVertices();
            ParseNormals();
            ParseLengths();
            ParseBoundingBoxes();

            var header3DTA = offsets.FirstOrDefault(header => {
                return header.fourCCDeclaration == FourCC.threeDTA;
            });

            if (header3DTA != null) {
                threeDTAData = rawFile.data.GetRange(header3DTA.index, header3DTA.chunkSize);
            }

            var header3DAL = offsets.FirstOrDefault(header => {
                return header.fourCCDeclaration == FourCC.threeDAL;
            });

            if (header3DAL != null) {
                threeDALData = rawFile.data.GetRange(header3DAL.index, header3DAL.chunkSize);
            }

            var header3DHY = offsets.FirstOrDefault(header => {
                return header.fourCCDeclaration == FourCC.threeDHY;
            });

            if (header3DHY != null) {
                threeDHYData = rawFile.data.GetRange(header3DHY.index, header3DHY.chunkSize);
            }

            var header3DMI = offsets.FirstOrDefault(header => {
                return header.fourCCDeclaration == FourCC.threeDMI;
            });

            if (header3DMI != null) {
                threeDMIData = rawFile.data.GetRange(header3DMI.index, header3DMI.chunkSize);
            }

            var header3DHS = offsets.FirstOrDefault(header => {
                return header.fourCCDeclaration == FourCC.threeDHS;
            });

            if (header3DHS != null) {
                threeDHSData = rawFile.data.GetRange(header3DHS.index, header3DHS.chunkSize);
            }

            var headerAnmD = offsets.FirstOrDefault(header => {
                return header.fourCCDeclaration == FourCC.AnmD;
            });

            if (headerAnmD != null) {
                AnmDData = rawFile.data.GetRange(headerAnmD.index, headerAnmD.chunkSize);
            }

            CreateTriangles();
            CreateBillboards();
            CreateLines();
            CreateStarts();

        }

        public IFFDataFile Compile() {

            var compiledData = new List<byte>();

            compiledData.AddRange(CreateFourDGI());
            compiledData.AddRange(CreateThreeDTL(surfaces));

            if (threeDTAData != null) {
                compiledData.AddRange(threeDTAData);
            }

            compiledData.AddRange(CreateThreeDQL(primitives));

            if (threeDALData != null) {
                compiledData.AddRange(threeDALData);
            }

            foreach (var elementRef in elementReferences) {
                compiledData.AddRange(CreateThreeDRF(elementRef));
            }

            foreach (var group in objectElementGroups) {
                compiledData.AddRange(CreateFourDVL(group.Value.vertices, group.Key));
                compiledData.AddRange(CreateFourDNL(group.Value.normals, group.Key));
                compiledData.AddRange(CreateThreeDRL(group.Value.lengths, group.Key, group.Value.overflowLengths));
            }


            compiledData.AddRange(CreateThreeDBB(boundingBoxes, boundingBoxesPerFrame));

            if (threeDHYData != null) {
                compiledData.AddRange(threeDHYData);
            }

            if (threeDMIData != null) {
                compiledData.AddRange(threeDMIData);
            }

            if (threeDHSData != null) {
                compiledData.AddRange(threeDHSData);
            }

            if (AnmDData != null) {
                compiledData.AddRange(AnmDData);
            }

            rawFile.data = compiledData;

            return rawFile;

        }

        public int GetTexturePalette() {

            var value = -2;

            foreach (var surface in surfaces) {

                if (surface.uvMap != null) {

                    if (value == -2) {
                        value = surface.uvMap.Value.texturePaletteIndex;
                    }

                    if (value != surface.uvMap.Value.texturePaletteIndex) {
                        return -1;
                    }

                }

            }

            return value;

        }

        public void SetTexturePalette(int index) {

            foreach (var i in Enumerable.Range(0, surfaces.Count)) {

                var surface = surfaces[i];

                if (surface.uvMap != null) {
                    
                    var uvmap = surface.uvMap.Value;

                    uvmap.texturePaletteIndex = index;

                    surface.uvMap = uvmap;

                }

                surfaces[i] = surface;
                surfaceByCompiledOffset[surfaceByCompiledOffset.Keys.ToList()[i]] = surface;

            }

            triangles.Clear();

            CreateTriangles();

        }

        public void Import(byte[] newData) {

            rawFile.data = newData.ToList();

            offsets = new();
            elementReferences = new();
            primitives = new();
            objectElementGroups = new();
            surfaces = new();
            surfaceByCompiledOffset = new();
            boundingBoxes = new();
            triangles = new();
            billboards = new();
            lines = new();
            stars = new();

            positions = new();

            threeDTAData = null;
            threeDALData = null;
            threeDHYData = null;
            threeDMIData = null;
            threeDHSData = null;
            AnmDData = null;

            Init();

        }

        public void RefreshTriangles() {

            triangles.Clear();

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

        void ParseElementReferences() {

            var headers = offsets.Where(header => {
                return header.fourCCDeclaration == FourCC.threeDRF;
            });

            foreach (var header in headers) {

                var bytes = rawFile.data.GetRange(header.index, header.chunkSize);

                var id = BitConverter.ToInt32(bytes.ToArray(), 8);
                var fourCCRef = Encoding.Default.GetString(bytes.ToArray(), 12, 4);
                var refCount = BitConverter.ToInt32(bytes.ToArray(), 16);

                var offset = 20;

                var refIds = new List<int>();
                foreach (var i in Enumerable.Range(0, refCount)) {
                    refIds.Add(BitConverter.ToInt32(bytes.ToArray(), offset));
                    offset += 4;
                }

                elementReferences.Add(new ObjectElementReference(id, IFFParser.Reverse(fourCCRef), refIds));

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

            var headers = offsets.Where(header => {
                return header.fourCCDeclaration == FourCC.fourDVL;
            });

            foreach (var header in headers) {

                var vertices = new List<Vertex>();

                var bytes = rawFile.data.GetRange(header.index, header.chunkSize);
                var id = Utils.BytesToInt(bytes.ToArray(), 8);
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

                if (objectElementGroups.ContainsKey(id)) {

                    objectElementGroups[id].vertices = vertices;

                }
                else {

                    objectElementGroups[id] = new ObjectElementGroup();
                    objectElementGroups[id].vertices = vertices;

                }

            }

        }

        void ParseNormals() {

            var headers = offsets.Where(header => {
                return header.fourCCDeclaration == FourCC.fourDNL;
            });

            foreach (var header in headers) {

                var normals = new List<Vertex>();

                var bytes = rawFile.data.GetRange(header.index, header.chunkSize);
                var id = Utils.BytesToInt(bytes.ToArray(), 8);
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

                if (objectElementGroups.ContainsKey(id)) {

                    objectElementGroups[id].normals = normals;

                }
                else {

                    objectElementGroups[id] = new ObjectElementGroup();
                    objectElementGroups[id].normals = normals;

                }

            }

        }

        void ParseLengths() {

            var headers = offsets.Where(header => {
                return header.fourCCDeclaration == FourCC.threeDRL;
            });

            foreach (var header in headers) {

                var lengths = new List<int>();

                var bytes = rawFile.data.GetRange(header.index, header.chunkSize);
                var id = Utils.BytesToInt(bytes.ToArray(), 8);
                var lengthCount = Utils.BytesToInt(bytes.ToArray(), 12);

                var offset = 16;

                foreach (var i in Enumerable.Range(0, lengthCount)) {

                    lengths.Add(BitConverter.ToInt16(bytes.ToArray(), offset));

                    offset += 2;

                }

                var overflowLengths = new List<int>();

                if (offset != bytes.Count) {

                    while (offset < bytes.Count) {

                        overflowLengths.Add(BitConverter.ToInt16(bytes.ToArray(), offset));

                        offset += 2;

                    }

                }

                if (objectElementGroups.ContainsKey(id)) {

                    objectElementGroups[id].lengths = lengths;
                    objectElementGroups[id].overflowLengths = overflowLengths;

                }
                else {

                    objectElementGroups[id] = new ObjectElementGroup();
                    objectElementGroups[id].lengths = lengths;
                    objectElementGroups[id].overflowLengths = overflowLengths;

                }

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

            boundingBoxesPerFrame = BitConverter.ToInt32(bytes.ToArray(), 8);
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

                    verts.Add(firstElementGroup.vertices[primitive.associatedData[vertOrder[0]]]);
                    verts.Add(firstElementGroup.vertices[primitive.associatedData[vertOrder[1]]]);
                    verts.Add(firstElementGroup.vertices[primitive.associatedData[vertOrder[2]]]);

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

                    switch (primitive.Material.colorMode) {
                        case FCopObjectMaterial.VertexColorMode.Monochrome:
                            colors.Add(new float[] { surface.red / 128f, surface.red / 128f, surface.red / 128f });
                            colors.Add(new float[] { surface.red / 128f, surface.red / 128f, surface.red / 128f });
                            colors.Add(new float[] { surface.red / 128f, surface.red / 128f, surface.red / 128f });
                            break;
                        case FCopObjectMaterial.VertexColorMode.MonochromeLighting:
                            colors.Add(new float[] { (surface.red + 127) / 255f, (surface.red + 127) / 255f, (surface.red + 127) / 255f });
                            colors.Add(new float[] { (surface.red + 127) / 255f, (surface.red + 127) / 255f, (surface.red + 127) / 255f });
                            colors.Add(new float[] { (surface.red + 127) / 255f, (surface.red + 127) / 255f, (surface.red + 127) / 255f });
                            break;
                        case FCopObjectMaterial.VertexColorMode.Full:
                            colors.Add(new float[] { surface.red / 255f, surface.green / 255f, surface.blue / 255f });
                            colors.Add(new float[] { surface.red / 255f, surface.green / 255f, surface.blue / 255f });
                            colors.Add(new float[] { surface.red / 255f, surface.green / 255f, surface.blue / 255f });
                            break;
                        case FCopObjectMaterial.VertexColorMode.WarmShading:
                            colors.Add(new float[] { (surface.red + 127) / 255f, (surface.green + 127) / 255f, 0f });
                            colors.Add(new float[] { (surface.red + 127) / 255f, (surface.green + 127) / 255f, 0f });
                            colors.Add(new float[] { (surface.red + 127) / 255f, (surface.green + 127) / 255f, 0f });
                            break;
                        case FCopObjectMaterial.VertexColorMode.Black:
                            colors.Add(new float[] { 0f, 0f, 0f });
                            colors.Add(new float[] { 0f, 0f, 0f });
                            colors.Add(new float[] { 0f, 0f, 0f });
                            break;
                    }

                    triangles.Add(new Triangle(verts.ToArray(), uvs.ToArray(), colors.ToArray(), texturePalette, primitive));

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

        void CreateBillboards() {

            foreach (var primitive in primitives) {

                if (primitive.type == PrimitiveType.Billboard) {

                    var surface = surfaceByCompiledOffset[primitive.surfaceIndex];
                    var position = firstElementGroup.vertices[primitive.associatedData[0]];
                    var length = firstElementGroup.lengths[primitive.associatedData[2]];

                    billboards.Add(new Billboard(position, length, surface, primitive));

                }

            }

        }

        void CreateLines() {

            foreach (var primitive in primitives) {

                if (primitive.type == PrimitiveType.Line) {

                    var surface = surfaceByCompiledOffset[primitive.surfaceIndex];
                    var startPos = firstElementGroup.vertices[primitive.associatedData[0]];
                    var endPos = firstElementGroup.vertices[primitive.associatedData[1]];
                    var startLength = firstElementGroup.lengths[primitive.associatedData[2]];
                    var endLength = firstElementGroup.lengths[primitive.associatedData[3]];


                    lines.Add(new Line(startPos, endPos, startLength, endLength, surface, primitive));

                }

            }

        }

        void CreateStarts() {

            foreach (var primitive in primitives) {

                if (primitive.type == PrimitiveType.Star) {

                    var position = firstElementGroup.vertices[primitive.associatedData[0]];
                    var red = primitive.associatedData[1];
                    var green = primitive.associatedData[2];
                    var blue = primitive.associatedData[3];
                    var length = firstElementGroup.lengths[primitive.associatedData[4]];
                    var triCount = primitive.surfaceIndex;

                    stars.Add(new Star(position, length, red, green, blue, triCount));

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
            compiledData.AddRange(BitConverter.GetBytes((short)frameCount)); // frame count
            compiledData.Add(1); // const
            compiledData.Add(metaDataBitfield); // bitfield?
            compiledData.AddRange(BitConverter.GetBytes(0)); // const
            compiledData.AddRange(BitConverter.GetBytes(0)); // const
            compiledData.AddRange(BitConverter.GetBytes(0)); // const
            compiledData.AddRange(BitConverter.GetBytes(1)); // const
            compiledData.AddRange(BitConverter.GetBytes(2)); // const
            compiledData.AddRange(BitConverter.GetBytes(1)); // const
            compiledData.AddRange(BitConverter.GetBytes(1)); // const
            compiledData.AddRange(BitConverter.GetBytes(3)); // const

            if (positions.Count == 0) {
                positions.Add(255);
                positions.Add(255);
                positions.Add(255);
                positions.Add(255);
            }

            compiledData.Add(positions[0]); // pos0
            compiledData.Add(positions[1]); // pos1
            compiledData.Add(positions[2]); // pos2
            compiledData.Add(positions[3]); // pos3
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

        List<byte> CreateFourDVL(List<Vertex> vertices, int id) {

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
            compiledWithHeader.AddRange(BitConverter.GetBytes(id));
            compiledWithHeader.AddRange(BitConverter.GetBytes(vertices.Count));
            compiledWithHeader.AddRange(compiledData);

            return compiledWithHeader;

        }

        List<byte> CreateFourDNL(List<Vertex> vertices, int id) {

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
            compiledWithHeader.AddRange(BitConverter.GetBytes(id));
            compiledWithHeader.AddRange(BitConverter.GetBytes(vertices.Count));
            compiledWithHeader.AddRange(compiledData);

            return compiledWithHeader;

        }

        List<byte> CreateThreeDRL(List<int> lengths, int id, List<int> overflowLengths) {

            var compiledData = new List<byte>();

            foreach (var length in lengths) {

                compiledData.AddRange(BitConverter.GetBytes((short)length));

            }

            foreach (var length in overflowLengths) {

                compiledData.AddRange(BitConverter.GetBytes((short)length));

            }

            var compiledWithHeader = new List<byte>();

            compiledWithHeader.AddRange(FourCC.threeDRLbytes);
            compiledWithHeader.AddRange(BitConverter.GetBytes(compiledData.Count + 16));
            compiledWithHeader.AddRange(BitConverter.GetBytes(id));
            compiledWithHeader.AddRange(BitConverter.GetBytes(lengths.Count));
            compiledWithHeader.AddRange(compiledData);

            return compiledWithHeader;

        }

        List<byte> CreateThreeDRF(ObjectElementReference elementReference) {

            var compiledData = new List<byte>();

            compiledData.AddRange(Encoding.ASCII.GetBytes(IFFParser.Reverse(elementReference.refFourCC)));
            compiledData.AddRange(BitConverter.GetBytes(elementReference.refIds.Count));

            foreach (var idRef in elementReference.refIds) {
                compiledData.AddRange(BitConverter.GetBytes(idRef));

            }

            var compiledWithHeader = new List<byte>();

            compiledWithHeader.AddRange(FourCC.threeDRFbytes);
            compiledWithHeader.AddRange(BitConverter.GetBytes(compiledData.Count + 12));
            compiledWithHeader.AddRange(BitConverter.GetBytes(elementReference.id));
            compiledWithHeader.AddRange(compiledData);

            return compiledWithHeader;

        }

        List<byte> CreateThreeDQL(List<Primitive> primitives) {

            var compiledData = new List<byte>();

            foreach (var primitive in primitives) {

                primitive.Compile();

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

        List<byte> CreateThreeDBB(List<BoundingBox> boundingBoxes, int boxesPerFrame) {

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

        public struct ObjectElementReference {

            public int id;
            public string refFourCC;
            public List<int> refIds;

            public ObjectElementReference(int id, string refFourCC, List<int> refIds) {
                this.id = id;
                this.refFourCC = refFourCC;
                this.refIds = refIds;
            }

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

        public class Primitive {

            public int surfaceIndex;
            public byte[] associatedData;

            public List<byte> metaDatabitfeild;
            public int unknown1;
            public FCopObjectMaterial.Material Material {
                get => FCopObjectMaterial.materialByID [materialID];
            }
            public bool textureEnabled;
            public PrimitiveType type;
            public int unknown2;
            public bool isReflective;
            public int materialID;

            public Primitive(List<byte> metaDatabitfeild, int surfaceIndex, byte[] associatedData) {

                this.metaDatabitfeild = metaDatabitfeild;
                var bitField = new BitArray(metaDatabitfeild.ToArray());

                unknown1 = Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 0, 3));
                materialID = Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 3, 7));
                textureEnabled = Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 7, 8)) == 1;
                var primitiveType = Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 8, 11));
                unknown2 = Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 11, 15));
                isReflective = Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 15, 16)) == 1;

                type = (PrimitiveType)primitiveType;

                this.surfaceIndex = surfaceIndex;
                this.associatedData = associatedData;

            }

            public void Compile() {

                var bitFeild = new BitField(16, new List<BitNumber> {
                        new BitNumber(3, unknown1), 
                        new BitNumber(4, materialID),
                        new BitNumber(1, textureEnabled ? 1 : 0),
                        new BitNumber(3, (int)type),
                        new BitNumber(4, unknown2),
                        new BitNumber(1, isReflective ? 1 : 0)
                });

                metaDatabitfeild = Utils.BitArrayToByteArray(bitFeild.Compile()).ToList();

            }

        }

        public class Surface {

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

            public string CreateHash() {

                var total = type.ToString() + red.ToString() + green.ToString() + blue.ToString();

                if (uvMap != null) {

                    total += uvMap.Value.texturePaletteIndex.ToString();

                    foreach (var uv in uvMap.Value.uvs) {
                        total += uv.x.ToString();
                        total += uv.y.ToString();
                    }

                }

                return total;

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
            public Primitive primitive;

            public Triangle(Vertex[] vertices, UV[] uvs, float[][] colors, int texturePaletteIndex, Primitive primitive) {
                this.vertices = vertices;
                this.uvs = uvs;
                this.colors = colors;
                this.texturePaletteIndex = texturePaletteIndex;
                this.primitive = primitive;
            }

        }

        public struct Billboard {

            public Vertex position;
            public int length;
            public Surface surface;
            public Primitive primitive;

            public Billboard(Vertex position, int length, Surface surface, Primitive primitive) {
                this.position = position;
                this.length = length;
                this.surface = surface;
                this.primitive = primitive;
            }

        }

        public struct Line {

            public Vertex startPosition;
            public Vertex endPosition;
            public int startWidth;
            public int endWidth;
            public Surface surface;
            public Primitive primitive;

            public Line(Vertex startPosition, Vertex endPosition, int startWidth, int endWidth, Surface surface, Primitive primitive) {
                this.startPosition = startPosition;
                this.endPosition = endPosition;
                this.startWidth = startWidth;
                this.endWidth = endWidth;
                this.surface = surface;
                this.primitive = primitive;
            }

        }

        public struct Star {

            public Vertex position;
            public int length;
            public int red;
            public int green;
            public int blue;
            public int triCount;

            public Star(Vertex position, int length, int red, int green, int blue, int triCount) {
                this.position = position;
                this.length = length;
                this.red = red;
                this.green = green;
                this.blue = blue;
                this.triCount = triCount;
            }

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

    public abstract class FCopObjectMaterial {

        public static Dictionary<int, Material> materialByID = new Dictionary<int, Material>() {
            { 0, new Material(false, VertexColorMode.Monochrome, VisabilityMode.Opaque, false) },
            { 1, new Material(false, VertexColorMode.Monochrome, VisabilityMode.Transparent, false) },
            { 2, new Material(false, VertexColorMode.Full, VisabilityMode.Opaque, false) },
            { 3, new Material(false, VertexColorMode.Full, VisabilityMode.Transparent, true) },
            { 4, new Material(true, VertexColorMode.MonochromeLighting, VisabilityMode.Opaque, false) },
            { 5, new Material(true, VertexColorMode.MonochromeLighting, VisabilityMode.Transparent, false) },
            { 6, new Material(true, VertexColorMode.WarmShading, VisabilityMode.Opaque, false) },
            { 7, new Material(true, VertexColorMode.WarmShading, VisabilityMode.Transparent, true) },
            { 8, new Material(true, VertexColorMode.MonochromeLighting, VisabilityMode.Opaque, false) },
            { 9, new Material(true, VertexColorMode.MonochromeLighting, VisabilityMode.Transparent, false) },
            { 10, new Material(true, VertexColorMode.WarmShading, VisabilityMode.Opaque, false) },
            { 11, new Material(true, VertexColorMode.WarmShading, VisabilityMode.Opaque, true) },
            { 12, new Material(false, VertexColorMode.Full, VisabilityMode.Addition, true) },
            { 13, new Material(false, VertexColorMode.Full, VisabilityMode.Addition, true) },
            { 14, new Material(true, VertexColorMode.Black, VisabilityMode.Transparent, false) },
            { 15, new Material(true, VertexColorMode.Black, VisabilityMode.Transparent, false) }

        };

        public enum MaterialEnum {

            Unlit = 0,
            UnlitTransparent = 1,
            UnlitColor = 2,
            UnlitColorTransparnet = 3,
            LitFlat = 4,
            LitFlatTransparnet = 5,
            LitWarm = 6,
            LitWarmTransparent = 7,
            Lit = 8,
            LitTransparent = 9,
            LitWarm2 = 10,
            LitWarm3 = 11,
            AdditiveColor = 12,
            AdditiveColor2 = 13,
            Black = 14,
            Black2 = 15

        }

        public static int? IdByMaterial(Material mat) {

            foreach (var entry in materialByID) {

                if (entry.Value.Equals(mat)) return entry.Key;

            }

            return null;

        }

        public struct Material {

            public bool shading;
            public VertexColorMode colorMode;
            public VisabilityMode visabilityMode;
            public bool vertexColorSemiTransparent;

            public Material(bool shading, VertexColorMode colorMode, VisabilityMode visabilityMode, bool vertexColorSemiTransparent) {
                this.shading = shading;
                this.colorMode = colorMode;
                this.visabilityMode = visabilityMode;
                this.vertexColorSemiTransparent = vertexColorSemiTransparent;
            }

        }

        public enum VertexColorMode {
            Monochrome = 0,
            MonochromeLighting = 1,
            Full = 2,
            WarmShading = 3,
            Black = 4
        }

        public enum VisabilityMode {

            Opaque = 0,
            Transparent = 1,
            Addition = 2

        }

    }

}