using System.Collections.Generic;
using System.Linq;

namespace FCopParser {

    public class FCopRPNS {

        // Uses the script offset as it's ID or key.
        // The key is only updated on a new file load.
        // Even if the file is compiled the key stays the same.
        // The new offset is stored on FCopScript.
        public Dictionary<int, FCopScript> codeByOffset = new();
        public List<FCopScript> code = new();

        public List<byte> bytes = new();

        IFFDataFile rawFile;

        public FCopRPNS(IFFDataFile rawFile) {

            this.rawFile = rawFile;

            bytes = rawFile.data;

            var i = 0;
            while (i < bytes.Count) {

                var script = new FCopScript(i, bytes);
                codeByOffset[i] = script;
                code.Add(script);
                i = script.terminationOffset;

            }

        }

        public void AddScript() {

            var keys = codeByOffset.Keys.ToList();

            var nextKey = Utils.FindNextInt(keys);

            var script = new FCopScript(nextKey);
            codeByOffset[nextKey] = script;
            code.Insert(0, script);

        }

        public void RemoveScript(int offset) {
            codeByOffset.Remove(offset);
            code.RemoveAll(script => script.offset == offset);
        }

        public IFFDataFile Compile() {

            var total = new List<byte>();

            var offset = 0;
            foreach (var line in code) {

                var compiledLine = line.Compile(offset);

                total.AddRange(compiledLine);

                offset += compiledLine.Count;

            }

            rawFile.data = total;

            return rawFile;

        }

        public void ResetKeys() {

            var newOrder = new List<FCopScript>();
            foreach (var line in codeByOffset) {
                newOrder.Add(line.Value);
            }

            codeByOffset.Clear();

            foreach (var line in newOrder) {
                codeByOffset.Add(line.offset, line);
            }

        }

    }

}