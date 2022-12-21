
namespace FCopParser {
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;
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
    }

}