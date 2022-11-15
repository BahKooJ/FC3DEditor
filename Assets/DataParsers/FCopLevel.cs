
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FCopParser {

    // =WIP=
    public class FCopLevel {

        public List<List<int>> layout;

        public List<FCopLevelSection> sections = new List<FCopLevelSection>();

        public List<FCopTexture> textures = new List<FCopTexture>();

        public FCopLevel(IFFFileManager fileManager) {

            var rawCtilFiles = fileManager.files.Where(file => {

                return file.dataFourCC == "Ctil";

            }).ToList();

            var rawBitmapFiles = fileManager.files.Where(file => {

                return file.dataFourCC == "Cbmp";

            }).ToList();

            foreach (var rawFile in rawCtilFiles) {
                sections.Add(new FCopLevelSectionParser(rawFile).Parse(this));
            }

            foreach (var rawFile in rawBitmapFiles) {
                textures.Add(new FCopTexture(rawFile));
            }

            layout = FCopLevelLayoutParser.Parse(fileManager.files.First(file => {

                return file.dataFourCC == "Cptc";

            }));

        }

        public FCopLevel(int width, int height, IFFFileManager fileManager) {

            layout = new List<List<int>>();

            foreach (int _ in Enumerable.Range(0, 4)) {

                layout.Add(new List<int>());

                layout.Last().AddRange(new List<int>() { 1, 1, 1, 1 });

                foreach (int __ in Enumerable.Range(0, width)) {
                    layout.Last().Add(1);
                }

                layout.Last().AddRange(new List<int>() { 1, 1, 1, 1, 0 });

            }

            var id = 1;

            foreach (int _ in Enumerable.Range(0, height)) {

                layout.Add(new List<int>());

                layout.Last().AddRange(new List<int>() { 1,1,1,1 });

                foreach (int i in Enumerable.Range(0, width)) {
                    layout.Last().Add(id);
                    id++;
                }

                layout.Last().AddRange(new List<int>() { 1, 1, 1, 1, 0 });

            }

            foreach (int _ in Enumerable.Range(0, 4)) {

                layout.Add(new List<int>());

                layout.Last().AddRange(new List<int>() { 1, 1, 1, 1 });

                foreach (int __ in Enumerable.Range(0, width)) {
                    layout.Last().Add(1);
                }

                layout.Last().AddRange(new List<int>() { 1, 1, 1, 1, 0 });

            }

            layout.Add(new List<int>());

            layout.Last().AddRange(new List<int>() { 0, 0, 0, 0 });

            foreach (int __ in Enumerable.Range(0, width)) {
                layout.Last().Add(0);
            }

            layout.Last().AddRange(new List<int>() { 0, 0, 0, 0, 0 });

            var rawCtilFiles = fileManager.files.Where(file => {

                return file.dataFourCC == "Ctil";

            }).ToList();

            var rawBitmapFiles = fileManager.files.Where(file => {

                return file.dataFourCC == "Cbmp";

            }).ToList();

            foreach (int ___ in Enumerable.Range(1, id)) {
                sections.Add(new FCopLevelSectionParser(rawCtilFiles[0]).Parse(this));
            }

            foreach (var rawFile in rawBitmapFiles) {
                textures.Add(new FCopTexture(rawFile));
            }

        }

        public void Compile() {

            foreach (var section in sections) {
                section.Compile();
            }

        }

    }

    public class FCopLevelSection {

        public FCopLevel parent;

        public const int heightMapWdith = 17;

        public List<HeightPoint> heightMap = new List<HeightPoint>();

        public List<TileColumn> tileColumns = new List<TileColumn>();

        public List<int> textureCoordinates = new List<int>();

        List<XRGB555> colors = new List<XRGB555>();

        public List<TileGraphics> tileGraphics = new List<TileGraphics>();

        // Until the file can be fully parsed, we need to have the parser on hand
        FCopLevelSectionParser parser;

        public FCopLevelSection(FCopLevelSectionParser parser, FCopLevel parent) {

            this.parser = parser;

            this.textureCoordinates = parser.textureCoordinates;
            this.colors = parser.colors;
            this.tileGraphics = parser.tileGraphics;

            foreach (var parsePoint in parser.heightPoints) {
                heightMap.Add(new HeightPoint(parsePoint));
            }

            var count = 0;
            var x = 0;
            var y = 0;
            foreach (var parseColumn in parser.thirdSectionBitfields) {

                var parsedTiles = parser.tiles.GetRange(parseColumn.number2, parseColumn.number1);

                var tiles = new List<Tile>();

                foreach (var parsedTile in parsedTiles) {
                    tiles.Add(new Tile(parsedTile));
                }

                var heights = new List<HeightPoint>();

                heights.Add(GetHeightPoint(x, y));
                heights.Add(GetHeightPoint(x + 1, y));
                heights.Add(GetHeightPoint(x, y + 1));
                heights.Add(GetHeightPoint(x + 1, y + 1));

                tileColumns.Add(new TileColumn(x, y, tiles, heights));

                x++;
                if (x == 16) {
                    y++;
                    x = 0;
                }

                count++;

            }

            this.parent = parent;

        }

        public HeightPoint GetHeightPoint(int x, int y) {
            return heightMap[(y * heightMapWdith) + x];
        }

        class Chunk {

            public int x;
            public int y;

            public List<TileColumn> tileColumns = new List<TileColumn>();

            public Chunk(int x, int y) {
                this.x = x;
                this.y = y;
            }

            public int Count() {

                var total = 0;

                foreach (var column in tileColumns) {

                    total += column.tiles.Count;

                }

                return total;

            }

        }

        public void Compile() {

            List<HeightPoint3> heightPoints = new List<HeightPoint3>();
            List<ThirdSectionBitfield> thirdSectionBitfields = new List<ThirdSectionBitfield>();
            List<TileBitfield> tiles = new List<TileBitfield>();

            List<Chunk> chunks = new List<Chunk>() { new Chunk(0,0) };

            foreach (var point in heightMap) {
                heightPoints.Add(point.Compile());
            }

            var x = 0;
            var y = 0;
            var chunkX = 0;
            var chunkY = 0;
            foreach (var i in Enumerable.Range(0,256)) {

                var offsetX = ((chunks.Count - 1) % 4) * 4;
                var offsetY = ((chunks.Count - 1) / 4) * 4;

                var index = ((y + offsetY) * 16) + (x + offsetX);

                chunks.Last().tileColumns.Add(tileColumns[index]);

                x++;

                if (x == 4) {
                    y++;
                    x = 0;
                    if (y == 4) {
                        y = 0;
                        chunkX++;

                        if (chunkX == 4) {
                            chunkY++;

                            if (chunkY == 4) {
                                break;
                            }

                            chunkX = 0;
                        }

                        chunks.Add(new Chunk(chunkX, chunkY));

                    }

                }

            }

            foreach (var chunk in chunks) {

                foreach (var column in chunk.tileColumns) {

                    foreach (var tile in column.tiles) {
                        tiles.Add(tile.Compile());
                    }

                }

            }

            var previousOffsetFromChunk = new Dictionary<int, int>() { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 } };
            var previousChunkY = 0;
            foreach (var column in tileColumns) {

                var tileOffset = 0;

                var offsetChunkX = column.x / 4;
                var offsetChunkY = column.y / 4;

                if (previousChunkY != offsetChunkY) {

                    foreach (var i in Enumerable.Range(0, 4)) {
                        previousOffsetFromChunk[i] = 0;
                    }

                    previousChunkY = offsetChunkY;

                }

                if (!(offsetChunkX == 0 && offsetChunkY == 0)) {
                    var previousChunks = chunks.GetRange(0, (offsetChunkY * 4) + offsetChunkX);

                    foreach (var chunk in previousChunks) {
                        tileOffset += chunk.Count();
                    }

                }

                var offsetTotal = previousOffsetFromChunk[offsetChunkX] + tileOffset;

                var bitField = new ThirdSectionBitfield(column.tiles.Count, offsetTotal);
                thirdSectionBitfields.Add(bitField);

                previousOffsetFromChunk[offsetChunkX] += column.tiles.Count;

            }

            parser.heightPoints = heightPoints;
            parser.thirdSectionBitfields = thirdSectionBitfields;
            parser.tiles = tiles;

            parser.Compile();

        }

    }

    public class HeightPoint {

        public const float multiplyer = 40f;
        public const float maxValue = SByte.MaxValue / multiplyer;
        public const float minValue = SByte.MinValue / multiplyer;

        public float height1;
        public float height2;
        public float height3;

        public HeightPoint(float height1, float height2, float height3) {
            this.height1 = height1;
            this.height2 = height2;
            this.height3 = height3;
        }

        public HeightPoint(HeightPoint3 parsedHeightPoint3) {
            this.height1 = parsedHeightPoint3.height1 / multiplyer;
            this.height2 = parsedHeightPoint3.height2 / multiplyer;
            this.height3 = parsedHeightPoint3.height3 / multiplyer;
        }

        public float GetPoint(int index) {

            switch(index) {
                case 1: return height1;
                case 2: return height2;
                case 3: return height3;
                default: return 0;
            }

        }

        public void AddToPoint(float amount, int channel) {

            switch (channel) {
                case 1:
                    height1 += amount;

                    if (height1 > maxValue) {
                        height1 = maxValue;
                    } else if (height1 < minValue) {
                        height1 = minValue;
                    }

                    height1 = (float)Math.Round(height1 * multiplyer) / multiplyer;

                    break;
                case 2:
                    height2 += amount;

                    if (height2 > maxValue) {
                        height2 = maxValue;
                    } else if (height2 < minValue) {
                        height2 = minValue;
                    }

                    height2 = (float)Math.Round(height2 * multiplyer) / multiplyer;

                    break;
                case 3:
                    height3 += amount;

                    if (height3 > maxValue) {
                        height3 = maxValue;
                    } else if (height3 < minValue) {
                        height3 = minValue;
                    }

                    height3 = (float)Math.Round(height3 * multiplyer) / multiplyer;

                    break;
                default: break; 
            }

        }

        public HeightPoint3 Compile() {
            return new HeightPoint3(
                (sbyte)Math.Round(height1 * multiplyer),
                (sbyte)Math.Round(height2 * multiplyer),
                (sbyte)Math.Round(height3 * multiplyer));

        }

    }

    // Columns form form left to right
    public class TileColumn {

        public int x;
        public int y;

        public List<Tile> tiles;

        public List<HeightPoint> heights;

        public TileColumn(int x, int y, List<Tile> tiles, List<HeightPoint> heights) {
            this.x = x;
            this.y = y;
            this.tiles = tiles;
            this.heights = heights;
        }

    }

    // Tiles are sorted into 4x4 chunks
    public class Tile {

        public List<TileVertex> verticies;
        public int textureIndex;
        public int graphicsIndex;

        public Rotation rotation;

        public TileBitfield parsedTile;

        public Tile(TileBitfield parsedTile) {

            verticies = MeshType.VerticiesFromID(parsedTile.number5);
            textureIndex = parsedTile.number2;

            graphicsIndex = parsedTile.number6;

            this.parsedTile = parsedTile;
        }

        public TileBitfield Compile() {

            var id = MeshType.IDFromVerticies(verticies);

            if (id == null) {
                throw new Exception("No ID exists for given mesh");
            }

            parsedTile.number5 = (int)id;
            parsedTile.number2 = textureIndex;
            parsedTile.number6 = graphicsIndex;

            return parsedTile;

        }

    }

    public struct TileVertex {

        public int heightChannel;

        public VertexPosition vertexPosition;

        public TileVertex(int heightChannel, VertexPosition vertexPosition) {
            this.heightChannel = heightChannel;
            this.vertexPosition = vertexPosition;
        }

    }

    public enum VertexPosition {
        TopLeft = 1,
        TopRight = 2,
        BottomLeft = 3,
        BottomRight = 4
    }

    public class FCopLevelSectionParser {

        const int colorCountOffset = 8;
        const int textureCordCountOffset = 10;

        const int heightMapOffset = 12;
        const int heightMapLength = 867;

        const int renderDistanceOffset = 880;
        const int rednerDistanceLength = 90;

        const int tileCountOffset = 970;

        const int thirdSectionOffset = 972;
        const int thirdSectionLength = 512;

        const int tileArrayOffset = 1488;

        static List<byte> fourCC = new List<byte>() { 116, 99, 101, 83 };


        IFFDataFile rawFile;

        public short textureCordCount;
        public short tileCount;
        public short colorCount;


        public List<HeightPoint3> heightPoints = new List<HeightPoint3>();
        public List<ThirdSectionBitfield> thirdSectionBitfields = new List<ThirdSectionBitfield>();
        public List<TileBitfield> tiles = new List<TileBitfield>();
        public List<int> textureCoordinates = new List<int>();
        public List<XRGB555> colors = new List<XRGB555>();
        public List<TileGraphics> tileGraphics = new List<TileGraphics>();


        int offset = 0;

        public FCopLevelSectionParser(IFFDataFile rawFile) {
            this.rawFile = rawFile;

            colorCount = Utils.BytesToShort(rawFile.data.ToArray(), colorCountOffset);
            textureCordCount = Utils.BytesToShort(rawFile.data.ToArray(), textureCordCountOffset);

            ParseHeightPoints();

            tileCount = Utils.BytesToShort(rawFile.data.ToArray(), tileCountOffset);

            ParseThirdSection();
            ParseTiles();
            ParseTextures();
            ParseColors();
            ParseTileGraphics();

        }

        public void Compile() {

            List<byte> compiledFile = new List<byte>();

            foreach (HeightPoint3 heightPoint3 in heightPoints) {

                compiledFile.Add((byte)heightPoint3.height1);
                compiledFile.Add((byte)heightPoint3.height2);
                compiledFile.Add((byte)heightPoint3.height3);

            }

            compiledFile.Add(0);

            compiledFile.AddRange(
                rawFile.data.GetRange(renderDistanceOffset, rednerDistanceLength)
                );

            compiledFile.AddRange(BitConverter.GetBytes((short)tiles.Count));

            foreach (ThirdSectionBitfield thirdSectionItem in thirdSectionBitfields) {

                var bitFeild = new BitField(16, new List<BitNumber> {
                new BitNumber(6,thirdSectionItem.number1), new BitNumber(10,thirdSectionItem.number2)
            });

                compiledFile.AddRange(Utils.BitArrayToByteArray(bitFeild.Compile()));

            }

            compiledFile.AddRange(rawFile.data.GetRange(1484, 4));

            foreach (TileBitfield tile in tiles) {

                var bitFeild = new BitField(32, new List<BitNumber> {
                    new BitNumber(1,tile.number1), new BitNumber(10,tile.number2),
                    new BitNumber(2,tile.number3), new BitNumber(2,tile.number4),
                    new BitNumber(7,tile.number5), new BitNumber(10,tile.number6)
                });

                compiledFile.AddRange(Utils.BitArrayToByteArray(bitFeild.Compile()));

            }

            foreach (int texture in textureCoordinates) {
                compiledFile.AddRange(BitConverter.GetBytes((ushort)texture));
            }

            foreach (XRGB555 color in colors) {
                compiledFile.AddRange(color.Compile());
            }

            foreach (TileGraphics graphic in tileGraphics) {

                var bitFeild = new BitField(16, new List<BitNumber> {
                    new BitNumber(8,graphic.number1), new BitNumber(3,graphic.number2),
                    new BitNumber(2,graphic.number3), new BitNumber(1,graphic.number4), new BitNumber(2,graphic.number5)
                });

                compiledFile.AddRange(Utils.BitArrayToByteArray(bitFeild.Compile()));

            }

            var header = new List<byte>();

            header.AddRange(fourCC);

            header.AddRange(BitConverter.GetBytes(12 + compiledFile.Count()));

            header.AddRange(BitConverter.GetBytes((short)colors.Count));

            header.AddRange(BitConverter.GetBytes((short)textureCoordinates.Count));

            header.AddRange(compiledFile);

            rawFile.data = header;
            rawFile.modified = true;

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

                var bitNumber6 = Utils.CopyBitsOfRange(bitField, 0, 6);
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

                colors.Add(new XRGB555(bytes.GetRange(i * 2, 2)));

            }

            offset += colorCount * 2;

        }

        void ParseTileGraphics() {

            var length = rawFile.data.Count() - offset;

            var bytes = rawFile.data.GetRange(offset, length);

            foreach (int i in Enumerable.Range(0, length / 2)) {
                var byteFiled = bytes.GetRange(i * 2, 2).ToArray();

                var bitField = new BitArray(byteFiled);

                tileGraphics.Add(new TileGraphics(
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 0, 8)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 8, 11)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 11, 13)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 13, 14)),
                    Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 14, 16))
                    ));
            }

        }

    }

    public abstract class FCopLevelLayoutParser {

        const int widthOffset = 16;
        const int heightOffset = 20;
        const int layoutOffset = 48;

        public static List<List<int>> Parse(IFFDataFile file) {

            int width = Utils.BytesToInt(file.data.ToArray(), widthOffset);
            int height = Utils.BytesToInt(file.data.ToArray(), heightOffset);

            var layout = new List<List<int>>();

            var offset = layoutOffset;

            foreach (int _ in Enumerable.Range(0, height)) {

                layout.Add(new List<int>());


                foreach (int i in Enumerable.Range(0, width)) {
                    layout.Last().Add(Utils.BytesToInt(file.data.ToArray(),offset) / 4);
                    offset += 4;
                }

            }

            return layout;

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

    public struct ThirdSectionBitfield {

        // 6 bit
        public int number1;
        // 10 bit
        public int number2;

        public ThirdSectionBitfield(int number1, int number2) {
            this.number1 = number1;
            this.number2 = number2;
        }

    }

    public struct TileBitfield {

        public int number1;
        public int number2;
        public int number3;
        public int number4;
        public int number5;
        public int number6;

        public TileBitfield(int number1, int number2, int number3, int number4, int number5, int number6) {
            this.number1 = number1;
            this.number2 = number2;
            this.number3 = number3;
            this.number4 = number4;
            this.number5 = number5;
            this.number6 = number6;
        }
    }

    public abstract class TextureCoordinate {

        static public float GetX(int offset, float width = 256f) {
            return (offset % width) / width;
        }

        static public float GetY(int offset, float width = 256f, float height = 2048f) {
            return (float)Math.Floor(offset / width) / height;
        }

    }

    public struct TileGraphics {

        public int number1;
        public int number2;
        public int number3;
        public int number4;
        public int number5;

        public TileGraphics(int number1, int number2, int number3, int number4, int number5) {
            this.number1 = number1;
            this.number2 = number2;
            this.number3 = number3;
            this.number4 = number4;
            this.number5 = number5;
        }

    }

}