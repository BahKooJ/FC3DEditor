﻿using System.Collections.Generic;

namespace FCopParser {

    public class FCopRPNS {

        // Uses the script offset as it's ID or key.
        // The key is only updated on a new file load.
        // Even if the file is compiled the key stays the same.
        // The new offset is stored on FCopScript.
        public Dictionary<int, FCopScript> code = new();

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

                    code.Add(offset, new FCopScript(offset, new List<byte>(currentLine)));

                    currentLine.Clear();

                    offset = i;
                }

            }

        }

        public void Compile() {

            var total = new List<byte>();

            var offset = 0;
            foreach (var line in code) {

                var compiledLine = line.Value.Compile(offset);

                total.AddRange(compiledLine);

                offset += compiledLine.Count;

            }

            rawFile.data = total;

        }

    }


}