
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCopParser {

    public class FCopTexture : FCopAsset {

        public static List<byte> CCBFourCC = new List<byte>() { 32, 66, 67, 67 };
        public static List<byte> LkUpfourCC = new List<byte>() { 112, 85, 107, 76 };
        public static List<byte> PX16fourCC = new List<byte>() { 54, 49, 88, 80 };
        public static List<byte> PLUTfourCC = new List<byte>() { 84, 85, 76, 80 };

        //PS1
        public static List<byte> PDATfourCC = new List<byte>() { 84, 65, 68, 80 };


        const int imageSize = 131072;

        public List<ChunkHeader> offsets = new List<ChunkHeader>();

        public List<byte> bitmap;

        public List<XRGB555> colorPalette;
        public List<byte> colorPaletteData;
        public List<List<byte>> lookUpSections = new();
        public List<byte> ccbData;

        public bool isColorIndexed = false;

        public FCopTexture(IFFDataFile rawFile) : base(rawFile) {

            name = "bitmap " + DataID;

            FindChunks(rawFile.data.ToArray());

            ParseCCB();

            try {
                var px16 = offsets.First(chunkHeader => {
                    return chunkHeader.fourCCDeclaration == "PX16";
                });

                bitmap = rawFile.data.GetRange(px16.index + 8, px16.chunkSize - 8);

                ParseColorPalette();

                ParseLookUp();

            } 
            catch {

                var pdat = offsets.First(chunkHeader => {
                    return chunkHeader.fourCCDeclaration == "PDAT";
                });

                isColorIndexed = true;

                ParseColorPalette();

                var paletteIndexes = rawFile.data.GetRange(pdat.index + 8, pdat.chunkSize - 8);

                bitmap = new();

                foreach (var b in paletteIndexes) {
                    bitmap.AddRange(colorPalette[b].Compile(true));
                }

            }

        }

        void ReInit(bool withTexture) {

            offsets = new();
            colorPalette = new();
            colorPaletteData = new();
            lookUpSections = new();
            ccbData = new();

            FindChunks(rawFile.data.ToArray());

            ParseCCB();

            //try {

                if (withTexture) {

                    var px16 = offsets.First(chunkHeader => {
                        return chunkHeader.fourCCDeclaration == "PX16";
                    });

                    bitmap = rawFile.data.GetRange(px16.index + 8, px16.chunkSize - 8);

                }

                ParseColorPalette();

                ParseLookUp();

            //}
            //catch {

            //    var pdat = offsets.First(chunkHeader => {
            //        return chunkHeader.fourCCDeclaration == "PDAT";
            //    });

            //    ParseColorPalette();

            //    var paletteIndexes = rawFile.data.GetRange(pdat.index + 8, pdat.chunkSize - 8);

            //    bitmap = new();

            //    foreach (var b in paletteIndexes) {
            //        bitmap.AddRange(colorPalette[b].Compile(true));
            //    }

            //}

        }

        void ParseCCB() {

            var ccb = offsets.First(chunkHeader => {
                return chunkHeader.fourCCDeclaration == "CCB ";
            });

            ccbData = rawFile.data.GetRange(ccb.index + 60, 16);

        }

        void ParseLookUp() {

            var lkup = offsets.First(chunkHeader => {
                return chunkHeader.fourCCDeclaration == "LkUp";
            });

            var lkupData = rawFile.data.GetRange(lkup.index + 8, lkup.chunkSize - 8);

            foreach (var i in Enumerable.Range(0, 4)) {

                var data = lkupData.GetRange(i * 256, 256);

                lookUpSections.Add(data);

            }

        }

        void ParseColorPalette() {

            var plut = offsets.First(chunkHeader => {
                return chunkHeader.fourCCDeclaration == "PLUT";
            });

            colorPalette = new();

            colorPaletteData = rawFile.data.GetRange(plut.index + 20, plut.chunkSize - 20);

            foreach (var i in Enumerable.Range(0, colorPaletteData.Count / 2)) {

                var rgb = new XRGB555(new BitArray(colorPaletteData.GetRange(i * 2, 2).ToArray()));

                colorPalette.Add(rgb);

            }

        }

        public void ApplyColorPalette() {

            colorPaletteData.Clear();

            foreach (var rgb in colorPalette) {
                colorPaletteData.AddRange(rgb.Compile());
            }

        }

        public void ReinitColorPalette() {

            colorPalette.Clear();

            foreach (var i in Enumerable.Range(0, colorPaletteData.Count / 2)) {

                var rgb = new XRGB555(new BitArray(colorPaletteData.GetRange(i * 2, 2).ToArray()));

                colorPalette.Add(rgb);

            }

        }

        public List<byte> CreateLookupPaletteBMP() {

            var total = new List<byte>();

            var lkup = offsets.First(chunkHeader => {
                return chunkHeader.fourCCDeclaration == "LkUp";
            });

            var lkupData = rawFile.data.GetRange(lkup.index + 8, lkup.chunkSize - 8);

            foreach (var b in lkupData) {
                total.AddRange(colorPalette[b].Compile());
            }

            return total;

        }

        public List<byte> CreateLookupPaletteBMPSingle(int i) {

            var total = new List<byte>();

            foreach (var b in lookUpSections[i]) {
                total.AddRange(colorPalette[b].Compile());
            }

            return total;

        }

        public byte[] BitmapWithHeader() {

            var formattedBitmap = new List<byte>();

            formattedBitmap.AddRange(new List<byte>() { 66, 77 });
            formattedBitmap.AddRange(BitConverter.GetBytes(bitmap.Count + 54));
            formattedBitmap.AddRange(BitConverter.GetBytes(0));
            formattedBitmap.AddRange(BitConverter.GetBytes(54));
            formattedBitmap.AddRange(BitConverter.GetBytes(40));
            formattedBitmap.AddRange(BitConverter.GetBytes(256));
            formattedBitmap.AddRange(BitConverter.GetBytes(256));
            formattedBitmap.AddRange(BitConverter.GetBytes((short)1));
            formattedBitmap.AddRange(BitConverter.GetBytes((short)16));
            formattedBitmap.AddRange(BitConverter.GetBytes(0));
            formattedBitmap.AddRange(BitConverter.GetBytes(bitmap.Count));
            formattedBitmap.AddRange(BitConverter.GetBytes(0));
            formattedBitmap.AddRange(BitConverter.GetBytes(0));
            formattedBitmap.AddRange(BitConverter.GetBytes(0));
            formattedBitmap.AddRange(BitConverter.GetBytes(0));

            formattedBitmap.AddRange(bitmap);

            return formattedBitmap.ToArray();

        }

        public byte[] CbmpColorPaletteData() {

            var ccb = offsets.First(chunkHeader => {
                return chunkHeader.fourCCDeclaration == "CCB ";
            });

            var lkup = offsets.First(chunkHeader => {
                return chunkHeader.fourCCDeclaration == "LkUp";
            });

            var plut = offsets.First(chunkHeader => {
                return chunkHeader.fourCCDeclaration == "PLUT";
            });

            var total = new List<byte>();

            total.AddRange(rawFile.data.GetRange(ccb.index, ccb.chunkSize));
            total.AddRange(rawFile.data.GetRange(lkup.index, lkup.chunkSize));
            total.AddRange(rawFile.data.GetRange(plut.index, plut.chunkSize));

            return total.ToArray();

        }

        public byte[] ConvertToRGB565() {

            var total = new List<byte>();

            foreach (var i in Enumerable.Range(0, bitmap.Count / 2)) {

                var rgb = new XRGB555(new BitArray(bitmap.GetRange(i * 2, 2).ToArray()));
                total.AddRange(rgb.ToRGB565());

            }

            return total.ToArray();

        }

        public byte[] ConvertToARGB32() {

            var total = new List<byte>();

            foreach (var i in Enumerable.Range(0, bitmap.Count / 2)) {

                var rgb = new XRGB555(new BitArray(bitmap.GetRange(i * 2, 2).ToArray()));
                total.AddRange(rgb.ToARGB32());

            }

            return total.ToArray();

        }

        public byte[] ConvertToAlphaMap() {

            var total = new List<byte>();

            foreach (var i in Enumerable.Range(0, bitmap.Count / 2)) {

                var rgb = new XRGB555(new BitArray(bitmap.GetRange(i * 2, 2).ToArray()));
                total.AddRange(rgb.ToA32());

            }

            return total.ToArray();

        }

        public void ImportBMP(byte[] bytes) {

            var offset = BitConverter.ToInt32(bytes, 10);

            bitmap = new List<byte>(bytes).GetRange(offset, imageSize);

        }

        public void ImportCbmp(byte[] bytes) {
            rawFile.data = bytes.ToList();
            rawFile.modified = true;

            ReInit(true);

        }

        public void ImportColorPaletteData(byte[] bytes) {

            var cpOffsets = new List<ChunkHeader>();
            
            int offset = 0;

            while (offset < bytes.Length) {

                var fourCC = BytesToStringReversed(bytes, offset, 4);
                var size = Utils.BytesToInt(bytes, offset + 4);

                cpOffsets.Add(new ChunkHeader(offset, fourCC, size));

                offset += size;

            }

            var ccb = cpOffsets.First(chunkHeader => {
                return chunkHeader.fourCCDeclaration == "CCB ";
            });

            var lkup = cpOffsets.First(chunkHeader => {
                return chunkHeader.fourCCDeclaration == "LkUp";
            });

            var plut = cpOffsets.First(chunkHeader => {
                return chunkHeader.fourCCDeclaration == "PLUT";
            });

            var total = new List<byte>();

            total.AddRange(bytes.ToList().GetRange(ccb.index, ccb.chunkSize));
            total.AddRange(bytes.ToList().GetRange(lkup.index, lkup.chunkSize));

            total.AddRange(PX16fourCC);
            total.AddRange(BitConverter.GetBytes(imageSize + 8));

            total.AddRange(bitmap.GetRange(0, imageSize));

            total.AddRange(bytes.ToList().GetRange(plut.index, plut.chunkSize));

            rawFile.data = total;
            rawFile.modified = true;

            ReInit(false);

        }

        public (int, int) CreateColorPalette() {

            Dictionary<Tuple<byte, byte>, int> colorCount = new();

            foreach (var i in Enumerable.Range(0, bitmap.Count / 2)) {

                var pixel = bitmap.GetRange(i * 2, 2).ToArray();

                // The black pixel needs to be at the start
                if (pixel.SequenceEqual(new byte[] { 0, 0 })) {
                    continue;
                }

                var pair = new Tuple<byte, byte>(pixel[0], pixel[1]);

                if (colorCount.ContainsKey(pair)) {
                    colorCount[pair]++;
                }
                else {
                    colorCount[pair] = 1;
                }

            }

            var topColors = colorCount.OrderByDescending(pair => pair.Value).Take(255).Select(pair => pair.Key).ToList();

            var semiTransparentColors = topColors.Where(tuple => {
                return tuple.Item2 > 0x80;
            }).ToList();

            var opaqueColors = topColors.Where(tuple => {
                return tuple.Item2 < 0x80;
            }).ToList();

            colorPaletteData.Clear();

            colorPaletteData.AddRange(new byte[] { 0, 0 });

            foreach (var pixel in semiTransparentColors) {
                colorPaletteData.AddRange(new byte[] { pixel.Item1, pixel.Item2 });
            }

            foreach (var pixel in opaqueColors) {
                colorPaletteData.AddRange(new byte[] { pixel.Item1, pixel.Item2 });
            }

            while (colorPaletteData.Count < 512) {

                colorPaletteData.AddRange(new byte[] { 0, 0 });

            }

            ReinitColorPalette();

            return (semiTransparentColors.Count, opaqueColors.Count);

        }

        public void ClearLookUpData(int transparnetCount, int opaqueCount) {

            ccbData = new List<byte>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var section1 = new List<byte>();
            foreach (var i in Enumerable.Range(0, 256)) {
                section1.Add((byte)transparnetCount);
            }

            var section2 = new List<byte>();
            foreach (var i in Enumerable.Range(0, 256)) {
                section2.Add(0xFF);
            }

            var section3 = new List<byte>();
            foreach (var i in Enumerable.Range(0, 256)) {
                section3.Add(0);
            }

            var section4 = new List<byte>();
            foreach (var i in Enumerable.Range(0, 256)) {
                section4.Add((byte)(transparnetCount - 1));
            }

            lookUpSections[0] = section1;
            lookUpSections[1] = section2;
            lookUpSections[2] = section3;
            lookUpSections[3] = section4;

        }

        public IFFDataFile Compile() {

            //var counts = CreateColorPalette();

            //ClearLookUpData(counts.Item1, counts.Item2);

            // Not yet...
            if (isColorIndexed) {
                return null;
            }

            var total = new List<byte>();

            var ccb = offsets.First(chunkHeader => {
                return chunkHeader.fourCCDeclaration == "CCB ";
            });

            var plut = offsets.First(chunkHeader => {
                return chunkHeader.fourCCDeclaration == "PLUT";
            });

            total.AddRange(rawFile.data.GetRange(ccb.index, 60));

            total.AddRange(ccbData);

            total.AddRange(LkUpfourCC);
            total.AddRange(BitConverter.GetBytes(1032));

            foreach (var section in lookUpSections) {

                total.AddRange(section);

            }

            total.AddRange(PX16fourCC);
            total.AddRange(BitConverter.GetBytes(imageSize + 8));

            total.AddRange(bitmap.GetRange(0, imageSize));

            total.AddRange(rawFile.data.GetRange(plut.index, 20));

            total.AddRange(colorPaletteData);

            rawFile.data = total;
            rawFile.modified = true;

            return rawFile;

        }

        void FindChunks(byte[] bytes) {

            int offset = 0;

            while (offset < bytes.Length) {

                var fourCC = BytesToStringReversed(bytes, offset, 4);
                var size = Utils.BytesToInt(bytes, offset + 4);

                offsets.Add(new ChunkHeader(offset, fourCC, size));

                offset += size;

            }

        }

        string BytesToStringReversed(byte[] bytes, int offset, int length) {
            var s = Encoding.Default.GetString(bytes, offset, length);
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

    }

    public class XRGB555 {

        public bool x;

        public const float maxChannelValue = 31;

        public int r = 0;
        public int g = 0;
        public int b = 0;

        public XRGB555(bool x, int r, int g, int b) {
            this.x = x;
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public XRGB555(BitArray bits) {


            r = Utils.BitsToInt(Utils.CopyBitsOfRange(bits, 0, 5));
            g = Utils.BitsToInt(Utils.CopyBitsOfRange(bits, 5, 10));
            b = Utils.BitsToInt(Utils.CopyBitsOfRange(bits, 10, 15));
            x = bits[15];

        }

        public XRGB555(List<byte> bytes, bool isBGR = false) {

            if (isBGR) {

                BitArray bitsBGR = new BitArray(bytes.ToArray());

                b = Utils.BitsToInt(Utils.CopyBitsOfRange(bitsBGR, 0, 5));
                g = Utils.BitsToInt(Utils.CopyBitsOfRange(bitsBGR, 5, 10));
                r = Utils.BitsToInt(Utils.CopyBitsOfRange(bitsBGR, 10, 15));
                x = bitsBGR[15];

                return;

            }

            BitArray bits = new BitArray(bytes.ToArray());

            r = Utils.BitsToInt(Utils.CopyBitsOfRange(bits, 0, 5));
            g = Utils.BitsToInt(Utils.CopyBitsOfRange(bits, 5, 10));
            b = Utils.BitsToInt(Utils.CopyBitsOfRange(bits, 10, 15));
            x = bits[15];

        }

        public bool IsWhite() {

            return r == maxChannelValue && g == maxChannelValue && b == maxChannelValue;

        }

        public XRGB555 Clone() {
            return new XRGB555(x, r, g, b);
        }

        public void SetFromRGB32(int r, int g, int b) {

            double max8BitValue = 255;

            double redPercent = r / max8BitValue;
            double greenPercent = g / max8BitValue;
            double bluePercent = b / max8BitValue;

            var calculatedRed = (int)Math.Round(redPercent * maxChannelValue);
            var calculatedGreen = (int)Math.Round(greenPercent * maxChannelValue);
            var calculatedBlue = (int)Math.Round(bluePercent * maxChannelValue);

            this.r = calculatedRed;
            this.g = calculatedGreen;
            this.b = calculatedBlue;

        }

        public byte[] ToRGB565() {

            int max6bitValue = 63;

            double greenPercent = g / maxChannelValue;

            int calculatedGreen = (int)Math.Round(greenPercent * max6bitValue);

            var bitfield = new BitField(16, new List<BitNumber> {

                new BitNumber(5,r),
                new BitNumber(6,calculatedGreen),
                new BitNumber(5,b),

            });

            return Utils.BitArrayToByteArray(bitfield.Compile());

        }

        public byte[] ToARGB32() {

            int max8BitValue = 255;

            double redPercent = r / maxChannelValue;
            double greenPercent = g / maxChannelValue;
            double bluePercent = b / maxChannelValue;

            byte calculatedAlpha = 255;

            if (r + g + b == 0) {
                calculatedAlpha = 0;
            }
            else if (x) {
                calculatedAlpha = (byte)Math.Round(0.50 * max8BitValue); ;
            }

            byte calculatedRed = (byte)Math.Round(redPercent * max8BitValue);
            byte calculatedGreen = (byte)Math.Round(greenPercent * max8BitValue);
            byte calculatedBlue = (byte)Math.Round(bluePercent * max8BitValue);

            return new byte[] { calculatedAlpha, calculatedBlue, calculatedGreen, calculatedRed };

        }

        public byte[] ToA32() {

            byte calculatedAlpha = 255;
            if (r + g + b == 0) {
                calculatedAlpha = 0;
            }
            else if (x) {
                calculatedAlpha = (byte)Math.Round(0.50 * 255); ;
            }

            return new byte[] { calculatedAlpha, 255, 255, 255 };

        }

        public float[] ToColors() {

            float redPercent = r / maxChannelValue;
            float greenPercent = g / maxChannelValue;
            float bluePercent = b / maxChannelValue;

            return new float[] { redPercent, greenPercent, bluePercent };

        }

        public ushort ToUShort(bool isBGR = false) {

            if (isBGR) {

                var bitfieldbgr = new BitField(16, new List<BitNumber> {

                    new BitNumber(5,b),
                    new BitNumber(5,g),
                    new BitNumber(5,r),
                    new BitNumber(x)

                });

                return BitConverter.ToUInt16(Utils.BitArrayToByteArray(bitfieldbgr.Compile()));

            }

            var bitfield = new BitField(16, new List<BitNumber> {

                new BitNumber(5,r),
                new BitNumber(5,g),
                new BitNumber(5,b),
                new BitNumber(x)

            });

            return BitConverter.ToUInt16(Utils.BitArrayToByteArray(bitfield.Compile()));

        }

        public byte[] Compile(bool isBGR = false) {

            if (isBGR) {

                var bitfieldbgr = new BitField(16, new List<BitNumber> {

                    new BitNumber(5,b),
                    new BitNumber(5,g),
                    new BitNumber(5,r),
                    new BitNumber(x)

                });

                return Utils.BitArrayToByteArray(bitfieldbgr.Compile());

            }

            var bitfield = new BitField(16, new List<BitNumber> {

                new BitNumber(5,r),
                new BitNumber(5,g),
                new BitNumber(5,b),
                new BitNumber(x)

            });

            return Utils.BitArrayToByteArray(bitfield.Compile());

        }

    }

}