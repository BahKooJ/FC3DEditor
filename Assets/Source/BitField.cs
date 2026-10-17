
namespace FCopParser {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    class BitField {

        public int bitCount;
        public List<BitNumber> bitNumbers;
        public BitArray bits;

        public BitField(int bitCount, List<BitNumber> bitNumbers) {
            this.bitCount = bitCount;
            this.bitNumbers = bitNumbers;
            bits = new BitArray(bitCount);
        }

        public BitArray Compile() {

            var offset = 0;

            foreach (var bitNum in bitNumbers) {

                foreach (bool bit in bitNum.Get()) {

                    bits[offset] = bit;

                    offset++;

                }

            }

            return bits;

        }
    }

    class BitNumber {

        public int bitCount;
        public int number;

        public BitNumber(int bitCount, int number) {
            this.bitCount = bitCount;
            this.number = number;
        }

        public BitNumber(bool bit) {
            bitCount = 1;
            number = bit ? 1 : 0;
        }

        public BitArray Get() {

            if (number > (Math.Pow(2, bitCount)) - 1) {

                Console.WriteLine("Warning: Number is bigger than the amount of bits it holds");

            }

            var bits32 = new BitArray(new int[] { number });

            var bits = new BitArray(bitCount);

            foreach (int i in Enumerable.Range(0, bitCount)) {

                bits[i] = bits32[i];

            }

            return bits;

        }

    }

}