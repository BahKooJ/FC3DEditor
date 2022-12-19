
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace FCopParser {


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


        public IFFDataFile rawFile;

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