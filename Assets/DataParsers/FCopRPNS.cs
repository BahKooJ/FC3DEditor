using System.Collections.Generic;

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

            var i = 0;
            while (i < bytes.Count) {

                var script = new FCopScript(i, bytes);
                code[i] = script;
                i = script.terminationOffset;

            }

        }

        public IFFDataFile Compile() {

            var total = new List<byte>();

            var offset = 0;
            foreach (var line in code) {

                var compiledLine = line.Value.Compile(offset);

                total.AddRange(compiledLine);

                offset += compiledLine.Count;

            }

            rawFile.data = total;

            return rawFile;

        }

        public void ResetKeys() {

            var newOrder = new List<FCopScript>();
            foreach (var line in code) {
                newOrder.Add(line.Value);
            }

            code.Clear();

            foreach (var line in newOrder) {
                code.Add(line.offset, line);
            }

        }

    }


}