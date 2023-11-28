

using System.Collections;
using System.Collections.Generic;

namespace FCopParser {

    public class FCopRPNS {

        public List<List<byte>> code = new();

        public List<byte> bytes = new();

        IFFDataFile rawFile;

        public FCopRPNS(IFFDataFile rawFile) {

            this.rawFile = rawFile;

            bytes = rawFile.data;

            var currentLine = new List<byte>();

            foreach (var b in rawFile.data) {

                currentLine.Add(b);

                if (b == 0) {

                    code.Add(new(currentLine));

                    currentLine.Clear();

                }

            }

        }

        public void Compile() {

            rawFile.data = bytes;

        }

    }


}