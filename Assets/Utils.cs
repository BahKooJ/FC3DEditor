
namespace FCopParser {
    using System;
    using System.Collections;
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