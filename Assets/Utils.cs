
namespace FCopParser {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    class Utils {

        public static Element[] CopyOfRange<Element>(Element[] array, int start, int end) {

            var length = end - start;

            var total = new Element[length];

            Array.Copy(array, start, total, 0, length);

            return total;

        }

        public static BitArray CopyBitsOfRange(BitArray bits, int start, int end) {

            var length = end - start;

            var total = new BitArray(length);

            foreach (int i in Enumerable.Range(start, length)) {
                total[i - start] = bits[i];
            }

            return total;

        }

        public static int BitsToInt(BitArray bits) {

            int[] array = new int[1];
            bits.CopyTo(array, 0);
            return array[0];

        }

        public static int BitsToSignedInt(BitArray bits, int bitCount) {

            int[] array = new int[1];
            bits.CopyTo(array, 0);
            var unsigned = array[0];

            var max = (int)Math.Pow(2, bitCount);

            if (unsigned > (max / 2) - 1) {
                return unsigned - max;
            }

            return unsigned;

        }

        public static byte[] BitArrayToByteArray(BitArray bits) {
            byte[] ret = new byte[bits.Length / 8];
            bits.CopyTo(ret, 0);
            return ret;
        }

        public static int BytesToInt(byte[] bytes, int offset) {
            return BitConverter.ToInt32(bytes, offset);
        }

        public static short BytesToShort(byte[] bytes, int offset) {
            return BitConverter.ToInt16(bytes, offset);
        }

        public static ushort BytesToUShort(byte[] bytes, int offset) {
            return BitConverter.ToUInt16(bytes, offset);
        }

        public static string RemovePathingFromFilePath(string filePath) {

            string Reverse(string s) {
                char[] charArray = s.ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
            }

            var total = "";

            foreach (var i in Enumerable.Range(0, filePath.Length)) {

                var c = filePath[(filePath.Length - 1) - i];

                if (c == '\\' || c == '/') {
                    return Reverse(total);
                }

                total += c;

            }

            return Reverse(total);

        }

        public static string RemoveExtensionFromFileName(string fileName) {

            var total = "";

            var periodCount = fileName.Where(c => {
                return c == '.';
            }).Count();

            if (periodCount == 0) {
                return fileName;
            }

            foreach (var c in fileName) {

                if (c == '.') {

                    if (periodCount == 1) {
                        return total;
                    } else {
                        periodCount--;
                    }

                }

                total += c;

            }

            return total;

        }

    }

    public class WavefrontParser {

        static List<char> numbers = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '.' };

        public struct Vertex {
            public float x;
            public float y;
            public float z;

            public Vertex(float x, float y, float z) {
                this.x = x;
                this.y = y;
                this.z = z;
            }

        }

        public struct UV {
            public float u; 
            public float v;

            public UV(float u, float v) {
                this.u = u;
                this.v = v;
            }
        }

        public struct FaceElement {
            public int vertIndex;
            public int textureIndex;
            public int normalIndex;

            public FaceElement(int vertIndex, int textureIndex, int normalIndex) {
                this.vertIndex = vertIndex;
                this.textureIndex = textureIndex;
                this.normalIndex = normalIndex;
            }

        }

        public List<Vertex> vertices = new();
        public List<Vertex> normals = new();
        public List<UV> uvs = new();
        public List<List<FaceElement>> faces = new();

        public WavefrontParser(List<char> data) {

            List<List<char>> floatingValues = new();
            var expectedFloatingValueCount = 0;
            var originalKeyWord = "";

            void CreateVert() {

                var x = float.Parse(new string(floatingValues[0].ToArray()));
                var y = float.Parse(new string(floatingValues[1].ToArray()));
                var z = float.Parse(new string(floatingValues[2].ToArray()));

                vertices.Add(new Vertex(x, y, z));

            }

            void CreateNormal() {

                var x = float.Parse(new string(floatingValues[0].ToArray()));
                var y = float.Parse(new string(floatingValues[1].ToArray()));
                var z = float.Parse(new string(floatingValues[2].ToArray()));

                normals.Add(new Vertex(x, y, z));

            }

            void CreateUV() {

                var u = float.Parse(new string(floatingValues[0].ToArray()));
                var v = float.Parse(new string(floatingValues[1].ToArray()));

                uvs.Add(new UV(u, v));

            }

            void CreateFace() {

                faces.Add(new());

                foreach (var i in Enumerable.Range(0, floatingValues.Count / 3)) {

                    var vIndex = int.Parse(new string(floatingValues[i * 3].ToArray()));
                    var nIndex = int.Parse(new string(floatingValues[i * 3 + 1].ToArray()));
                    var uIndex = int.Parse(new string(floatingValues[i * 3 + 2].ToArray()));

                    faces.Last().Add(new FaceElement(vIndex, nIndex, uIndex));

                }

            }

            foreach (var c in data) {

                if (expectedFloatingValueCount != 0) {

                    if (numbers.Contains(c)) {

                        if (floatingValues.Count == 0) {
                            floatingValues.Add(new());
                        }

                        floatingValues.Last().Add(c);
                    }
                    else {

                        if (floatingValues.Last().Count != 0) {
                            floatingValues.Add(new());
                        }

                    }

                    // expectedFloatingValueCount being -1 means it has a varible value count and is going until a stop char is found
                    if (expectedFloatingValueCount == -1) {

                        if (c == '\n') {

                            if (originalKeyWord == "f") {
                                CreateFace();
                            }

                            expectedFloatingValueCount = 0;
                            floatingValues.Clear();
                            originalKeyWord = "";

                        }

                    }

                    // Just because the floating values equals the expected count,
                    // doesn't mean the value is done being added to.
                    // A new list being created indicats that the previous value is done.
                    if (floatingValues.Count == expectedFloatingValueCount + 1) {

                        switch (originalKeyWord) {

                            case "v":

                                CreateVert();

                                break;
                            case "vn":

                                CreateNormal();

                                break;
                            case "vt":

                                CreateUV();

                                break;
                        }

                        expectedFloatingValueCount = 0;
                        floatingValues.Clear();
                        originalKeyWord = "";

                    }

                    continue;
                }

                if (c != ' ' && c != '\n') {
                    originalKeyWord += c;
                }
                else {

                    switch (originalKeyWord) {
                        case "v":
                            expectedFloatingValueCount = 3;
                            floatingValues.Clear();
                            break;
                        case "vn":
                            expectedFloatingValueCount = 3;
                            floatingValues.Clear();
                            break;
                        case "vt":
                            expectedFloatingValueCount = 2;
                            floatingValues.Clear();
                            break;
                        case "f":
                            expectedFloatingValueCount = -1;
                            floatingValues.Clear();
                            break;
                        default:
                            originalKeyWord = "";
                            break;
                    }

                }

            }

        }

    }

    public class WaveParser {

        public int fileSize;
        public int audioFormat;
        public int channels;
        public int sampleRate;
        public int bytesPerSec;
        public int bytesPerBlock;
        public int bitsPerSample;
        public List<byte> sampleData;

        public List<byte> data;

        public WaveParser(List<byte> data) {

            this.data = data;

            var arrayData = data.ToArray();

            if (!data.GetRange(0, 4).SequenceEqual(new List<byte>() { 0x52, 0x49, 0x46, 0x46 })) {
                throw new InvalidFileException();
            }

            var offset = 4;

            fileSize = BitConverter.ToInt32(arrayData, offset);
            offset += 8;

            // JUNK
            // IDK why but sometimes a wave has this junk chunk because why not I guess
            if (data.GetRange(offset, 4).SequenceEqual(new List<byte>() { 0x4A, 0x55, 0x4E, 0x4B })) {
                offset += 4;
                offset += BitConverter.ToInt32(arrayData, offset) + 4;
            }

            // Move past fmt  and size
            offset += 8;

            audioFormat = BitConverter.ToInt16(arrayData, offset);
            offset += 2;
            channels = BitConverter.ToInt16(arrayData, offset);
            offset += 2;
            sampleRate = BitConverter.ToInt32(arrayData, offset);
            offset += 4;
            bytesPerSec = BitConverter.ToInt32(arrayData, offset);
            offset += 4;
            bytesPerBlock = BitConverter.ToInt16(arrayData, offset);
            offset += 2;
            bitsPerSample = BitConverter.ToInt16(arrayData, offset);
            offset += 2;

            // Move Past data fourCC
            offset += 4;
            sampleData = data.GetRange(offset + 4, BitConverter.ToInt32(arrayData, offset));

        }

    }

    public enum AssetType {
        WavSound,
        Texture,
        Object,
        NavMesh,
        SndsSound,
        Music,
        MiniAnimation,
        Mixed
    }

    public enum Axis {
        X = 0,
        Y = 1,
        Z = 2
    }

    public class IncorrectFileFormat : Exception {

        public AssetType assetType;

        public IncorrectFileFormat(AssetType assetType) {
            this.assetType = assetType;
        }

    }

}