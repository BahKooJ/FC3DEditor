

using System.Collections;
using System.Collections.Generic;

namespace FCopParser {

    public class FCopRPNS {

        public List<FCopScript> code = new();

        public List<byte> bytes = new();

        IFFDataFile rawFile;

        public FCopRPNS(IFFDataFile rawFile) {

            this.rawFile = rawFile;

            bytes = rawFile.data;

            var currentLine = new List<byte>();

            var offset = 0;
            var i = 0;
            foreach (var b in rawFile.data) {

                currentLine.Add(b);

                i++;

                if (b == 0) {

                    code.Add(new FCopScript(offset, new List<byte>(currentLine)));

                    currentLine.Clear();

                    offset = i;
                }

            }

        }

        public void Compile() {

            rawFile.data = bytes;

        }

    }


}