
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using System.Text;

namespace FCopParser {


    public class FCopLevelSectionParser {

        const int colorCountOffset = 8;
        const int textureCordCountOffset = 10;

        const int heightMapOffset = 12;
        const int heightMapLength = 867;

        const int specialTileTypeOffset = 880;

        const int renderDistanceOffset = 884;
        const int rednerDistanceLength = 84;

        const int animationVectorOffset = 968;
        const int tileCountOffset = 970;

        const int thirdSectionOffset = 972;
        const int thirdSectionLength = 512;

        const int tileArrayOffset = 1488;

        static List<byte> fourCC = new List<byte>() { 116, 99, 101, 83 };
        static List<byte> fourCCScTA = new List<byte>() { 65, 84, 99, 83 };
        static List<byte> fourCCSLFX = new List<byte>() { 88, 70, 76, 83 };



        public IFFDataFile rawFile;

        public short textureCordCount;
        public short tileCount;
        public short colorCount;

        // Sect
        public List<HeightPoint3> heightPoints = new();
        public LevelCulling culling;
        public List<byte> tileEffects;
        public List<ThirdSectionBitfield> thirdSectionBitfields = new();
        public List<byte> animationVector = new();
        public List<TileBitfield> tiles = new();
        public List<int> textureCoordinates = new();
        public List<XRGB555> colors = new();
        public List<TileGraphicsItem> tileGraphics = new();

        // ScTA
        public List<TileUVAnimationMetaData> tileUVAnimationMetaData = new();
        public List<int> animatedTextureCoordinates = new();

        // SLFX
        public List<byte> slfxData;

        public List<ChunkHeader> offsets = new List<ChunkHeader>();

        int offset = 0;

        public FCopLevelSectionParser(IFFDataFile rawFile) {
            this.rawFile = rawFile;

            if (rawFile == null) {
                return;
            }

            FindStartChunkOffset();

            colorCount = Utils.BytesToShort(rawFile.data.ToArray(), colorCountOffset);
            textureCordCount = Utils.BytesToShort(rawFile.data.ToArray(), textureCordCountOffset);

            ParseHeightPoints();

            culling = new LevelCulling(rawFile.data.GetRange(renderDistanceOffset, rednerDistanceLength));

            tileEffects = rawFile.data.GetRange(specialTileTypeOffset, 4);

            tileCount = Utils.BytesToShort(rawFile.data.ToArray(), tileCountOffset);

            ParseThirdSection();

            animationVector = rawFile.data.GetRange(animationVectorOffset, 2);

            ParseTiles();
            ParseTextures();
            ParseColors();
            ParseTileGraphics();

            ParseUVAnimations();
            ParseShaderAnimations();

        }

        public List<byte> Compile() {

            List<byte> compiledFile = new List<byte>();

            void CompileHeights() {

                foreach (HeightPoint3 heightPoint3 in heightPoints) {

                    compiledFile.Add((byte)heightPoint3.height1);
                    compiledFile.Add((byte)heightPoint3.height2);
                    compiledFile.Add((byte)heightPoint3.height3);

                }

                compiledFile.Add(0);

            }

            void CompileThirdSection() {

                foreach (ThirdSectionBitfield thirdSectionItem in thirdSectionBitfields) {

                    // Well this sucks
                    // Because the offset to tiles is only 10 bits long the max number of tiles is 1024.
                    var bitFeild = new BitField(16, new List<BitNumber> {
                        new BitNumber(6,thirdSectionItem.number1), new BitNumber(10,thirdSectionItem.number2)
                    });

                    compiledFile.AddRange(Utils.BitArrayToByteArray(bitFeild.Compile()));

                }

            }

            void CompileTiles() {

                foreach (TileBitfield tile in tiles) {

                    var bitFeild = new BitField(32, new List<BitNumber> {
                        new BitNumber(1,tile.isEndInColumnArray), new BitNumber(10,tile.textureIndex),
                        new BitNumber(2,tile.culling), new BitNumber(2,tile.number4),
                        new BitNumber(7,tile.meshID), new BitNumber(10,tile.graphicIndex)
                    });

                    compiledFile.AddRange(Utils.BitArrayToByteArray(bitFeild.Compile()));

                }

            }

            void CompileGraphics() {

                foreach (TileGraphicsItem graphicsItem in tileGraphics) {

                    if (graphicsItem is TileGraphics) {

                        var graphic = (TileGraphics)graphicsItem;

                        var bitFeild = new BitField(16, new List<BitNumber> {
                            new BitNumber(8,graphic.lightingInfo), new BitNumber(3,graphic.cbmpID),
                            new BitNumber(1,graphic.isAnimated), new BitNumber(1,graphic.isSemiTransparent),
                            new BitNumber(1,graphic.isRect), new BitNumber(2,graphic.graphicsType)
                        });

                        compiledFile.AddRange(Utils.BitArrayToByteArray(bitFeild.Compile()));

                    }
                    else {

                        var graphics = (TileGraphicsMetaData)graphicsItem;

                        compiledFile.AddRange(graphics.data);

                    }

                }

            }

            void CompileAnimatedUVs() {

                if (tileUVAnimationMetaData.Count == 0) {
                    return;
                }

                var sctaCompiledFile = new List<byte>();

                foreach (var metaData in tileUVAnimationMetaData) {

                    sctaCompiledFile.Add((byte)metaData.frames);
                    sctaCompiledFile.Add(0);
                    sctaCompiledFile.Add(1);
                    sctaCompiledFile.Add((byte)metaData.number1);
                    sctaCompiledFile.AddRange(BitConverter.GetBytes((short)metaData.frameDuration));
                    sctaCompiledFile.AddRange(BitConverter.GetBytes((short)0));
                    sctaCompiledFile.AddRange(BitConverter.GetBytes(metaData.animationOffset));
                    sctaCompiledFile.AddRange(BitConverter.GetBytes(metaData.textureReplaceOffset));

                }

                foreach (int texture in animatedTextureCoordinates) {
                    sctaCompiledFile.AddRange(BitConverter.GetBytes((ushort)texture));
                }

                var header = new List<byte>();

                header.AddRange(fourCCScTA);
                header.AddRange(BitConverter.GetBytes(12 + sctaCompiledFile.Count()));
                header.AddRange(BitConverter.GetBytes(tileUVAnimationMetaData.Count));
                header.AddRange(sctaCompiledFile);

                compiledFile.AddRange(header);

            }

            void CompileSLFX() {

                if (slfxData == null) {
                    return;
                }

                var header = new List<byte>();
                header.AddRange(fourCCSLFX);
                header.AddRange(BitConverter.GetBytes(slfxData.Count + 8));
                header.AddRange(slfxData);
                compiledFile.AddRange(header);

            }

            CompileHeights();

            compiledFile.AddRange(tileEffects);

            compiledFile.AddRange(culling.Compile());

            compiledFile.AddRange(animationVector);

            compiledFile.AddRange(BitConverter.GetBytes((short)tiles.Count));

            CompileThirdSection();

            // I'm not sure what this value is, it's like the tile count?
            // In anycase changing the value doesn't give a reaction to future cop
            // so I guess I can ignore it.
            compiledFile.AddRange(BitConverter.GetBytes(16384));

            CompileTiles();

            foreach (int texture in textureCoordinates) {
                compiledFile.AddRange(BitConverter.GetBytes((ushort)texture));
            }

            foreach (XRGB555 color in colors) {
                compiledFile.AddRange(color.Compile(true));
            }

            CompileGraphics();

            compiledFile.Add(0);
            compiledFile.Add(0);

            var header = new List<byte>();

            header.AddRange(fourCC);

            header.AddRange(BitConverter.GetBytes(12 + compiledFile.Count()));

            header.AddRange(BitConverter.GetBytes((short)colors.Count));

            header.AddRange(BitConverter.GetBytes((short)textureCoordinates.Count));

            header.AddRange(compiledFile);

            compiledFile = new(header);

            CompileAnimatedUVs();
            CompileSLFX();

            return compiledFile;

        }

        public FCopLevelSection Parse(FCopLevel parent) {
            return new FCopLevelSection(this, parent);
        }

        void ParseHeightPoints() {

            var bytes = rawFile.data.GetRange(heightMapOffset, heightMapLength);

            var pointCount = 0;

            List<byte> heights = new List<byte>();

            foreach (byte b in bytes) {

                heights.Add(b);

                pointCount++;

                if (pointCount == 3) {
                    heightPoints.Add(new HeightPoint3(
                        (sbyte)heights[0],
                        (sbyte)heights[1],
                        (sbyte)heights[2]
                        ));

                    pointCount = 0;
                    heights.Clear();

                }

            }

        }

        void ParseThirdSection() {

            var bytes = rawFile.data.GetRange(thirdSectionOffset, thirdSectionLength);

            foreach (int i in Enumerable.Range(0, thirdSectionLength / 2)) {

                var byteField = bytes.GetRange(i * 2, 2).ToArray();

                var bitField = new BitArray(byteField);

                // Tile count
                var bitNumber6 = Utils.CopyBitsOfRange(bitField, 0, 6);
                // Index of tiles
                var bitNumber10 = Utils.CopyBitsOfRange(bitField, 6, 16);

                thirdSectionBitfields.Add(new ThirdSectionBitfield(
                    Utils.BitsToInt(bitNumber6),
                    Utils.BitsToInt(bitNumber10)
                    ));

            }

        }

        void ParseTiles() {

            var bytes = rawFile.data.GetRange(tileArrayOffset, tileCount * 4);

            foreach (int i in Enumerable.Range(0, tileCount)) {

                var byteFiled = bytes.GetRange(i * 4, 4).ToArray();

                var bitField = new BitArray(byteFiled);

                tiles.Add(new TileBitfield(
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 0, 1)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 1, 11)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 11, 13)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 13, 15)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 15, 22)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 22, 32))
                        ));

            }

            offset = tileArrayOffset + tileCount * 4;

        }

        void ParseTextures() {

            var bytes = rawFile.data.GetRange(offset, textureCordCount * 2);

            foreach (int i in Enumerable.Range(0, textureCordCount)) {

                textureCoordinates.Add(Utils.BytesToUShort(bytes.ToArray(), i * 2));

            }

            offset += textureCordCount * 2;

        }

        void ParseColors() {

            var bytes = rawFile.data.GetRange(offset, colorCount * 2);

            foreach (int i in Enumerable.Range(0, colorCount)) {

                colors.Add(new XRGB555(bytes.GetRange(i * 2, 2), true));

            }

            offset += colorCount * 2;

        }

        void ParseTileGraphics() {

            var length = offsets[0].chunkSize - offset;

            var bytes = rawFile.data.GetRange(offset, length);


            var additionalDataCount = 0;

            foreach (int i in Enumerable.Range(0, length / 2)) {

                if (additionalDataCount > 0) {

                    // For whatever reason it counts tile graphics metadata as a whole tile graphics
                    tileGraphics.Add(new TileGraphicsMetaData(bytes.GetRange(i * 2, 2)));

                    additionalDataCount--;

                    continue;

                }

                var byteFiled = bytes.GetRange(i * 2, 2).ToArray();

                var bitField = new BitArray(byteFiled);

                var graphic = new TileGraphics(
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 0, 8)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 8, 11)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 11, 12)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 12, 13)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 13, 14)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 14, 16))
                    );

                tileGraphics.Add(graphic);

                if (graphic.graphicsType == 1) {
                    additionalDataCount = 1;
                }
                else if (graphic.graphicsType == 2) {

                    if (graphic.isRect == 1) {
                        additionalDataCount = 2;
                    }
                    else {
                        additionalDataCount = 1;
                    }

                }

            }

        }

        void ParseUVAnimations() {

            var sctaHeader = offsets.Find(header => {
                return header.fourCCDeclaration == "ScTA";
            });

            if (sctaHeader == null) {
                return;
            }

            var sctaData = rawFile.data.GetRange(sctaHeader.index, sctaHeader.chunkSize);

            var offset = 8;
            var animationCount = Utils.BytesToInt(sctaData.ToArray(), offset);
            offset += 4;

            foreach (var i in Enumerable.Range(0, animationCount)) {

                tileUVAnimationMetaData.Add(
                    new TileUVAnimationMetaData(
                        sctaData[offset], sctaData[offset + 3], Utils.BytesToShort(sctaData.ToArray(), offset + 4),
                        Utils.BytesToInt(sctaData.ToArray(), offset + 8), Utils.BytesToInt(sctaData.ToArray(), offset + 12)
                    ));

                offset += 16;

            }

            foreach (var i in Enumerable.Range(0, (sctaData.Count - offset) / 2)) {

                animatedTextureCoordinates.Add(Utils.BytesToUShort(sctaData.ToArray(), offset + (i * 2)));

            }

        }

        void ParseShaderAnimations() {

            var slfxHeader = offsets.Find(header => {
                return header.fourCCDeclaration == "SLFX";
            });

            if (slfxHeader == null) {
                return;
            }

            slfxData = rawFile.data.GetRange(slfxHeader.index + 8, slfxHeader.chunkSize - 8);

        }

        void FindStartChunkOffset() {

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

            offsets.Clear();

            int offset = 0;

            while (offset < rawFile.data.Count) {

                var fourCC = BytesToStringReversed(offset, 4);
                var size = BytesToInt(offset + 4);

                offsets.Add(new ChunkHeader(offset, fourCC, size));

                offset += size;

            }

        }


    }

    public abstract class FCopLevelLayoutParser {

        static List<byte> fourCC = new List<byte>() { 66, 68, 82, 71 };

        const int sectionCountOffset = 12;
        const int widthOffset = 16;
        const int heightOffset = 20;
        const int worldBoarderOffset = 26;
        const int layoutOffset = 48;

        public static List<List<int>> Parse(IFFDataFile file) {

            int width = Utils.BytesToInt(file.data.ToArray(), widthOffset);
            int height = Utils.BytesToInt(file.data.ToArray(), heightOffset);

            var layout = new List<List<int>>();

            var offset = layoutOffset;

            foreach (int _ in Enumerable.Range(0, height)) {

                layout.Add(new List<int>());


                foreach (int i in Enumerable.Range(0, width)) {
                    layout.Last().Add(Utils.BytesToInt(file.data.ToArray(), offset) / 4);
                    offset += 4;
                }

            }

            return layout;

        }

        public static void Compile(List<List<int>> parsedData, IFFDataFile rawFile) {

            var total = new List<byte>();

            var maxID = 0;

            foreach (var row in parsedData) {

                foreach (var item in row) {

                    if (item > maxID) {
                        maxID = item;
                    }

                    total.AddRange(BitConverter.GetBytes(item * 4));

                }

            }

            var header = new List<byte>();

            header.AddRange(fourCC);

            header.AddRange(BitConverter.GetBytes(total.Count + 48));

            header.AddRange(BitConverter.GetBytes(0));

            header.AddRange(BitConverter.GetBytes(maxID));

            header.AddRange(BitConverter.GetBytes(parsedData[0].Count));

            header.AddRange(BitConverter.GetBytes(parsedData.Count));

            header.AddRange(BitConverter.GetBytes(0));
            header.AddRange(BitConverter.GetBytes(0));
            header.AddRange(BitConverter.GetBytes(0));
            header.AddRange(BitConverter.GetBytes(4));
            header.AddRange(BitConverter.GetBytes(0));
            header.AddRange(BitConverter.GetBytes(0));

            header.AddRange(total);

            rawFile.data = header;
            rawFile.modified = true;

        }

    }

    public struct HeightPoint3 {
        public sbyte height1;
        public sbyte height2;
        public sbyte height3;

        public HeightPoint3(sbyte height1, sbyte height2, sbyte height3) {
            this.height1 = height1;
            this.height2 = height2;
            this.height3 = height3;
        }

    }

    public class LevelCulling {

        const int chunkCulling8Size = 4;
        const int chunkCulling4Size = 16;

        public ChunkCulling sectionCulling;
        public List<ChunkCulling> chunkCulling8 = new();
        public List<ChunkCulling> chunkCulling4 = new();

        public LevelCulling(List<byte> data) {

            var offset = 0;

            sectionCulling = new ChunkCulling(data.GetRange(offset, 4));

            offset += 4;

            foreach (var i in Enumerable.Range(0, chunkCulling8Size)) {
                chunkCulling8.Add(new ChunkCulling(data.GetRange(offset, 4)));
                offset += 4;
            }

            foreach (var i in Enumerable.Range(0, chunkCulling4Size)) {
                chunkCulling4.Add(new ChunkCulling(data.GetRange(offset, 4)));
                offset += 4;
            }

        }

        public LevelCulling() {

            sectionCulling = new ChunkCulling(0, 0);

            foreach (var i in Enumerable.Range(0, chunkCulling8Size)) {
                chunkCulling8.Add(new ChunkCulling(0, 0));
            }

            foreach (var i in Enumerable.Range(0, chunkCulling4Size)) {
                chunkCulling4.Add(new ChunkCulling(0, 0));
            }

        }

        public List<byte> Compile() {

            var total = new List<byte>();

            total.AddRange(BitConverter.GetBytes((short)sectionCulling.radius));
            total.AddRange(BitConverter.GetBytes((short)sectionCulling.height));

            foreach (var culling in chunkCulling8) {
                total.AddRange(BitConverter.GetBytes((short)culling.radius));
                total.AddRange(BitConverter.GetBytes((short)culling.height));
            }

            foreach (var culling in chunkCulling4) {
                total.AddRange(BitConverter.GetBytes((short)culling.radius));
                total.AddRange(BitConverter.GetBytes((short)culling.height));
            }

            return total;

        }

        public void CalculateCulling(FCopLevelSection section) {

            sectionCulling.CalculateCulling(section, 0, 0, 16);

            var x8 = 0;
            var y8 = 0;

            var chunkCulling4Offset = 0;

            foreach (var culling in chunkCulling8) {

                culling.CalculateCulling(section, x8 * 8, y8 * 8, 8);

                foreach (var i in Enumerable.Range(0, 4)) {

                    var chunk4 = chunkCulling4[i + chunkCulling4Offset];

                    var x4 = (x8 * 8) + ((i % 2) * 4);
                    var y4 = (y8 * 8) + ((i / 2) * 4);

                    chunk4.CalculateCulling(section, x4, y4, 4);

                }

                chunkCulling4Offset += 4;

                x8++;
                if (x8 == 2) {
                    y8++;
                    x8 = 0;
                }

            }


        }

    }

    public class ChunkCulling {

        public int radius;
        public int height;

        public ChunkCulling(List<byte> data) {

            radius = BitConverter.ToInt16(data.GetRange(0, 2).ToArray());
            height = BitConverter.ToInt16(data.GetRange(2, 2).ToArray());

        }

        public ChunkCulling(int radius, int height) {
            this.radius = radius;
            this.height = height;
        }

        public void CalculateCulling(FCopLevelSection section, int x, int y, int size) {

            const int cullingUnitPerTile = 724;

            float minHeight = 128;
            float maxHeight = -128;

            List<TileColumn> columns = new();

            if (size == 16) {
                columns = section.tileColumns;
            }
            else {

                foreach (var ty in Enumerable.Range(y, size)) {

                    foreach (var tx in Enumerable.Range(x, size)) {

                        columns.Add(section.GetTileColumn(tx, ty));

                    }

                }

            }


            foreach (var column in columns) {

                foreach (var tile in column.tiles) {

                    foreach (var vert in tile.verticies) {

                        var hx = column.x;
                        var hy = column.y;

                        if (vert.vertexPosition == VertexPosition.TopRight || vert.vertexPosition == VertexPosition.BottomRight) {
                            hx++;
                        }
                        if (vert.vertexPosition == VertexPosition.BottomRight || vert.vertexPosition == VertexPosition.BottomLeft) {
                            hy++;
                        }

                        var height = section.GetHeightPoint(hx, hy);

                        var value = height.GetTruePoint(vert.heightChannel);

                        if (value < minHeight) {

                            minHeight = value;
                        }
                        if (value > maxHeight) { 
                            maxHeight = value;
                        }

                    }

                }

            }

            var difference = maxHeight - minHeight;

            height = (int)((minHeight + maxHeight) / 2f * 16f);
            radius = (int)((cullingUnitPerTile * size / 2) + (MathF.Abs(difference) * 2));

        }

    }

    public struct ThirdSectionBitfield {

        // 6 bit - Tile count
        public int number1;
        // 10 bit - Index of tiles;
        public int number2;

        public ThirdSectionBitfield(int number1, int number2) {
            this.number1 = number1;
            this.number2 = number2;
        }

    }

    public struct TileBitfield {

        public int isEndInColumnArray;
        public int textureIndex;
        public int culling;
        // Special tiles of some sort (water, damage)
        public int number4;
        public int meshID;
        public int graphicIndex;

        public TileBitfield(int number1, int number2, int number3, int number4, int number5, int number6) {
            this.isEndInColumnArray = number1;
            this.textureIndex = number2;
            this.culling = number3;
            this.number4 = number4;
            this.meshID = number5;
            this.graphicIndex = number6;
        }
    }

    public abstract class TextureCoordinate {

        static public float GetX(int offset, float width = 256f) {
            return (offset % width) / width;
        }

        static public float GetY(int offset, float width = 256f, float height = 2580f) {
            return (float)Math.Floor(offset / width) / height;
        }

        static public int GetXPixel(int offset, int width = 256) {
            return offset % width;
        }

        static public int GetYPixel(int offset, int width = 256, int height = 256) {
            return offset / width;
        }

        static public int SetXPixel(int x, int originalValue) {

            return (GetYPixel(originalValue) * 256) + x;

        }

        static public int SetYPixel(int y, int originalValue) {

            return GetXPixel(originalValue) + (y * 256);

        }

        static public int SetPixel(int x, int y) {
            return (y * 256) + x;
        }

        static public int[] GetVector(int offset) {
            return new int[] { GetXPixel(offset), GetYPixel(offset) };
        }

    }

    public interface TileGraphicsItem { }

    public struct TileGraphics: TileGraphicsItem {

        public int lightingInfo;
        public int cbmpID;
        public int isAnimated;
        public int isSemiTransparent;
        public int isRect;
        public int graphicsType;

        public TileGraphics(int lightingInfo, int cbmpID, int isAnimated, int isSemiTransparent, int isRect, int graphicsType) {
            this.lightingInfo = lightingInfo;
            this.cbmpID = cbmpID;
            this.isAnimated = isAnimated;
            this.isSemiTransparent = isSemiTransparent;
            this.isRect = isRect;
            this.graphicsType = graphicsType;
        }

        public static bool operator ==(TileGraphics tg1, TileGraphics tg2) {
            return tg1.lightingInfo == tg2.lightingInfo &&
                tg1.cbmpID == tg2.cbmpID &&
                tg1.isAnimated == tg2.isAnimated &&
                tg1.isSemiTransparent == tg2.isSemiTransparent &&
                tg1.isRect == tg2.isRect &&
                tg1.graphicsType == tg2.graphicsType;
        }

        public static bool operator !=(TileGraphics tg1, TileGraphics tg2) {
            return !(tg1.lightingInfo == tg2.lightingInfo &&
                tg1.cbmpID == tg2.cbmpID &&
                tg1.isAnimated == tg2.isAnimated &&
                tg1.isSemiTransparent == tg2.isSemiTransparent &&
                tg1.isRect == tg2.isRect &&
                tg1.graphicsType == tg2.graphicsType);
        }

    }

    public struct TileGraphicsMetaData: TileGraphicsItem {

        public List<byte> data;

        public TileGraphicsMetaData(List<byte> data) {
            this.data = data;
        }

    }

    public struct TileUVAnimationMetaData {

        public const float secondsPerValue = 1f / 300f;

        public int frames;
        public int number1;
        public int frameDuration;
        public int animationOffset;
        public int textureReplaceOffset;

        public TileUVAnimationMetaData(int frames, int number1, int frameDuration, int animationOffset, int textureReplaceOffset) {
            this.frames = frames;
            this.number1 = number1;
            this.frameDuration = frameDuration;
            this.animationOffset = animationOffset;
            this.textureReplaceOffset = textureReplaceOffset;
        }

        public static bool operator ==(TileUVAnimationMetaData uva1, TileUVAnimationMetaData uva2) {
            return uva1.frames == uva2.frames &&
                uva1.number1 == uva2.number1 &&
                uva1.frameDuration == uva2.frameDuration;
        }

        public static bool operator !=(TileUVAnimationMetaData uva1, TileUVAnimationMetaData uva2) {
            return !(uva1.frames == uva2.frames &&
                uva1.number1 == uva2.number1 &&
                uva1.frameDuration == uva2.frameDuration);
        }

    }

}