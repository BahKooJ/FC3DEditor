
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FCopParser {

    public class FCopTexture {


        public List<ChunkHeader> offsets = new List<ChunkHeader>();

        public List<byte> bitmap;

        public FCopTexture(IFFDataFile rawFile) {

            FindChunks(rawFile.data.ToArray());

            var px16 = offsets.First(chunkHeader => {
                return chunkHeader.fourCCDeclaration == "PX16";
            });

            bitmap = rawFile.data.GetRange(px16.index + 8, px16.chunkSize - 8);

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

            return bitmap.ToArray();

        }

        public byte[] ConvertToRGB565() {

            var total = new List<byte>();

            foreach (var i in Enumerable.Range(0, bitmap.Count / 2)) {

                var rgb = new XRGB555(new BitArray(bitmap.GetRange(i * 2, 2).ToArray()));
                total.AddRange(rgb.ToRGB565());

            }

            return total.ToArray();

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

        public static string Reverse(string s) {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        string BytesToStringReversed(byte[] bytes, int offset, int length) {
            return Reverse(Encoding.Default.GetString(bytes, offset, length));
        }

    }

    public class XRGB555 {

        bool x;

        const double maxChannelValue = 31;

        int r;
        int g;
        int b;

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

        public XRGB555(List<byte> bytes) {

            BitArray bits = new BitArray(bytes.ToArray());

            r = Utils.BitsToInt(Utils.CopyBitsOfRange(bits, 0, 5));
            g = Utils.BitsToInt(Utils.CopyBitsOfRange(bits, 5, 10));
            b = Utils.BitsToInt(Utils.CopyBitsOfRange(bits, 10, 15));
            x = bits[15];

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

        public byte[] Compile() {

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